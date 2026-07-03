using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the available audio categories across the game.
/// Removes the requirement to pass raw float values across systems.
/// </summary>
public enum AudioChannel{
    Master,
    SFX,
    UI,
    Music,
    Ambient
}//audio channel - enum

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler instance { get; private set; }

    [Header("Volume Sliders (Linear 0 to 1)")]
    [Range(0f, 1f)] [SerializeField] private float master = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float sfx = 0.6f;
    [Range(0f, 1f)] [SerializeField] private float music = 0.1f;
    [Range(0f, 1f)] [SerializeField] private float ui = 0.3f;
    [Range(0f, 1f)] [SerializeField] private float ambient = 0.1f;

    // Tracks actively playing audio sources. 
    // Swapped to a List of a concrete class to allow direct mutation of elements without structural copying bottlenecks.
    private readonly List<ActiveTrack> activeTracks = new List<ActiveTrack>();

    /// <summary>
    /// Internal tracking class representing a live, sounding AudioSource.
    /// Changed from a struct to a class so tracking properties can be modified instantly in memory.
    /// </summary>
    private class ActiveTrack{
        public AudioSource Source;
        public AudioChannel Channel;
        public float LocalModifier;
    }//active track - class

    private void Awake(){
        // Enforce thread-safe persistent Singleton pattern
        if (instance == null){
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }else Destroy(gameObject);
    }//awake

    /// <summary>
    /// Assigns and plays an audio clip on a designated source component.
    /// </summary>
    /// <param name="source">Target Unity AudioSource component.</param>
    /// <param name="clip">The sound asset clip to play.</param>
    /// <param name="channel">The audio channel bus to map this sound execution to.</param>
    /// <param name="localModifier">Individual balance adjustment for naturally loud or quiet assets.</param>
    public void PlayAudio(AudioSource source, AudioClip clip, AudioChannel channel, float localModifier = 1f){
        if (source == null || clip == null) return;

        // Clean up tracking references to prevent memory leaks if an AudioSource is being recycled
        CleanDeadTracks();

        source.clip = clip;
        // Resolved Math: Now utilizes pure linear evaluation, preventing the volume from dropping off a cliff.
        source.volume = CalculateVolume(channel, localModifier);
        source.Play();

        // Register the track for dynamic runtime mixing only if it's not already tracked
        if (!IsTracking(source)){
            activeTracks.Add(new ActiveTrack 
            { 
                Source = source, 
                Channel = channel, 
                LocalModifier = localModifier 
            });
        }
    }//play audio

    /// <summary>
    /// Instantly halts an AudioSource playback sequence and removes it from mixing update trees.
    /// </summary>
    public void StopAudio(AudioSource source){
        if (source == null) return;
        
        source.Stop();
        activeTracks.RemoveAll(t => t.Source == source);
    }//stop audio

    /// <summary>
    /// High-performance Event-Driven interface to change volume settings safely from UI Sliders.
    /// This eliminates the need for a performance-heavy frame Update loop.
    /// </summary>
    public void UpdateVolumeSetting(AudioChannel channel, float value){
        float clampedValue = Mathf.Clamp01(value);

        // Update the backing field variable
        switch (channel){
            case AudioChannel.Master: master = clampedValue; break;
            case AudioChannel.SFX: sfx = clampedValue; break;
            case AudioChannel.Music: music = clampedValue; break;
            case AudioChannel.UI: ui = clampedValue; break;
            case AudioChannel.Ambient: ambient = clampedValue; break;
        }

        // Push volume mutations to active elements instantly when the setting scales
        SyncActiveTrackVolumes();
    }//update volume settings

    /// <summary>
    /// Calculates the native linear volume configuration required by the AudioSource API.
    /// </summary>
    private float CalculateVolume(AudioChannel channel, float localModifier){
        // Linear multiplication scales naturally across matching sub-buses without signal distortion.
        // SFX (0.25) * Master (0.5) now equals 0.125, maintaining crisp, audible sound resolution.
        return GetChannelVolume(channel) * master * Mathf.Clamp01(localModifier);
    }//calculate volume

    private float GetChannelVolume(AudioChannel channel){
        return channel switch
        {
            AudioChannel.Master => 1f, // Master acts as global scalar multiplication
            AudioChannel.SFX => sfx,
            AudioChannel.UI => ui,
            AudioChannel.Music => music,
            AudioChannel.Ambient => ambient,
            _ => 1f
        };
    }// get channel volume

    /// <summary>
    /// Iterates across active playing elements to synchronize execution levels during a settings change.
    /// </summary>
    private void SyncActiveTrackVolumes(){
        for (int i = activeTracks.Count - 1; i >= 0; i--){
            ActiveTrack track = activeTracks[i];

            // If the source was destroyed natively or stopped playing, strip it from execution lists
            if (track.Source == null || !track.Source.isPlaying){
                activeTracks.RemoveAt(i);
                continue;
            }

            // Direct updating occurs ONLY when volume parameters are explicitly adjusted by the user
            track.Source.volume = CalculateVolume(track.Channel, track.LocalModifier);
        }
    }//sync active track volumes

    /// <summary>
    /// Housekeeping function to prune dead tracks and maintain small array search overhead profiles.
    /// </summary>
    private void CleanDeadTracks(){
        activeTracks.RemoveAll(track => track.Source == null || !track.Source.isPlaying);
    }//clean dead tracks

    private bool IsTracking(AudioSource source){
        for (int i = 0; i < activeTracks.Count; i++){
            if (activeTracks[i].Source == source) return true;
        }
        return false;
    }//isTracking
}//class