using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuHandler : MonoBehaviour
{
    private EventSystem eSystem;
    [SerializeField]private Button[] buttons; //0 - play, 1 - settings, 2 - credits, 3 - trophies, 4 - quit
    [SerializeField]private Text description;

    private bool m_hasSubmitted = false;

    private void Awake(){
        eSystem = FindObjectOfType<EventSystem>();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }//awake

    private void Start(){
        ButtonAudioVisualBehaviour.Instance.buttons = buttons;

        //on select, on pointer enter (cursor highlight) and on click events for buttons
        foreach(Button _button in buttons){
            Button _btn = _button;
            EventTrigger eTrigger = _btn.gameObject.AddComponent<EventTrigger>();

            //for keyboard and controller navigation
            EventTrigger.Entry _selectEntry = new EventTrigger.Entry{eventID = EventTriggerType.Select};
            _selectEntry.callback.AddListener((data) => {BehaviourOnSelect(_btn);});
            eTrigger.triggers.Add(_selectEntry);

            //for mouse pointer naviagtion
            EventTrigger.Entry _pointerEntry = new EventTrigger.Entry{eventID = EventTriggerType.PointerEnter};
            _pointerEntry.callback.AddListener((data) => {BehaviourOnSelect(_btn); _btn.Select(); });
            eTrigger.triggers.Add(_pointerEntry);

            string _action = _button.name;
            _btn.onClick.AddListener(() => BehaviourOnSubmit(_action));
        }
    }//start

    private void BehaviourOnSelect(Button _this){
        if(_this.interactable && !m_hasSubmitted){
            description.text = _this.name;
        }
    }//switch audio

    private void BehaviourOnSubmit(string _action){
        if(!m_hasSubmitted){
            //disable other buttons 
            foreach(Button _btn in buttons)_btn.interactable = (_btn.name == _action);

            //switch through to preform appropriate action
            switch(_action){
                case "play": StartCoroutine(SceneHandler.Instance.CallScene(4)); break;
                case "settings": StartCoroutine(SceneHandler.Instance.CallScene(2)); break;
                case "credits": StartCoroutine(SceneHandler.Instance.CallScene(3)); break;
                case "quit": StartCoroutine(QuitSequence()); break;
            }
            m_hasSubmitted = true;
        }
    }//Submit

    private IEnumerator QuitSequence(){
        yield return new WaitForSeconds(0.5f);
        Fader.Instance.fadeIn = false; //because we fade out first
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }//quit
}//class
