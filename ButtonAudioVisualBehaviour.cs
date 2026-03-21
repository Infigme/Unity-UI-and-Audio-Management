using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAudioVisualBehaviour : MonoBehaviour
{
    public static ButtonAudioVisualBehaviour Instance;
    private new AudioHandler audio;
    private EventSystem eSystem;
    private AudioSource source;
    [HideInInspector]public Button[] buttons;

    [SerializeField]private AudioClip select, submit;

    private bool m_hasSubmitted = false;

    private void Awake(){
        Instance = this;

        audio = FindObjectOfType<AudioHandler>();
        eSystem = FindObjectOfType<EventSystem>();
        source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = false;
    }//awake

    private void Start(){
        //on select, on pointer enter (cursor highlight) and on click events for buttons
        foreach(Button _button in buttons){
            Button _btn = _button;
            EventTrigger eTrigger = _btn.gameObject.AddComponent<EventTrigger>();

            //for keyboard and controller navigation
            EventTrigger.Entry _selectEntry = new EventTrigger.Entry{eventID = EventTriggerType.Select};
            _selectEntry.callback.AddListener((data) => {AudioFXOnSelect(_btn);});
            eTrigger.triggers.Add(_selectEntry);

            //for mouse pointer naviagtion
            EventTrigger.Entry _pointerEntry = new EventTrigger.Entry{eventID = EventTriggerType.PointerEnter};
            _pointerEntry.callback.AddListener((data) => {AudioFXOnSelect(_btn); _btn.Select(); });
            eTrigger.triggers.Add(_pointerEntry);

            _btn.onClick.AddListener(() => AudioFXOnSubmit());
        }
    }//start

    private void AudioFXOnSelect(Button _thisButton){
        if(_thisButton.interactable)audio.PlayAudio(source, select, AudioHandler.sfxVolume);
    }//audio fx on select

    private void AudioFXOnSubmit(){
        if(!m_hasSubmitted){
            //play submit fx
            audio.PlayAudio(source, submit, AudioHandler.sfxVolume);
            m_hasSubmitted = true;
        }
    }//audio fx on submit
}//class
