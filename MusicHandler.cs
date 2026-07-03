using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public static MusicHandler instance;

    private AudioSource musicSource;
    [SerializeField]private AudioClip musicClip;

    public bool isNightMode = false;

    private void Awake(){
        instance = this;

        musicSource = GetComponent<AudioSource>();
        musicSource.playOnAwake = true;
        musicSource.loop = true;
        musicSource.pitch = 0.8f;

    }//awake

    private void Start(){
        AudioHandler.instance.PlayAudio(musicSource, musicClip, AudioChannel.Music);
    }//start

    private void Update(){
        if(isNightMode)AdjustPitch(1f);
        else AdjustPitch(0.8f);
    }//update

    private void AdjustPitch(float newPitch){
        float pitching = musicSource.pitch;
        pitching = Mathf.Lerp(musicSource.pitch, newPitch, 5f * Time.deltaTime);
        musicSource.pitch = pitching;
    }//adjust pitch
}//class - music handler
