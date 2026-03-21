using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopUpTextEffect : MonoBehaviour
{
    private Text textOperator;
    private Image imageOperator;

    [SerializeField]private bool m_isText = false;

    private void Awake(){
        if(m_isText)textOperator = GetComponent<Text>();
        else imageOperator = GetComponent<Image>();
    }//awake

    private void Start(){
        StartCoroutine(MotionAndEffect());
    }//start

    public void TextOperation(string _message){
        textOperator.text = _message;
    }//operation

    public void ImageOperation(Sprite _icon){
        imageOperator.sprite = _icon;
    }//image operation

    private IEnumerator MotionAndEffect(){
        float _t = 0;
        Color _col = Color.black;
        if(m_isText)_col = textOperator.color;
        else _col = imageOperator.color;

        while(_t < 3f){
            _t += Time.deltaTime;
            
            if(m_isText)textOperator.color = new Color(_col.r, _col.g, _col.b, Mathf.Lerp(1, 0, _t / 1.5f));
            else imageOperator.color = new Color(_col.r, _col.g, _col.b, Mathf.Lerp(1, 0, _t / 1.5f));

            transform.Translate(Vector3.up * 50f * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }//motion and effect
    
}//class
