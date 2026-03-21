using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;
    private new AudioHandler audio;

    private AudioSource source;
    [SerializeField]private AudioClip[] soundtracks;

    private void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
            source = gameObject.AddComponent<AudioSource>();
            audio = FindObjectOfType<AudioHandler>();
        }else Destroy(gameObject);

    }//awake

    private void Start(){
        AudioHandler.masterVolume = StorageHandler.Instance.LoadVolume();
        source.loop = true;
        source.playOnAwake = true;
        AudioClip _track = soundtracks[Random.Range(0, soundtracks.Length)];
        audio.PlayAudio(source, _track, AudioHandler.musicVolume);
    }//start

    private void Update(){
        source.volume = AudioHandler.musicVolume * AudioHandler.masterVolume;
    }//update
    
}//class
