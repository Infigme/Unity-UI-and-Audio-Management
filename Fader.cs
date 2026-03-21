using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader Instance;
    public bool fadeIn = true;

    private Image fader;

    private void Awake(){
        Instance = this;
        fader = GetComponent<Image>();
        fader.color = Color.white;
    }//awake

    private void Update(){
        if(fadeIn)fader.color = Color.Lerp(fader.color, new Color(fader.color.r, fader.color.g, fader.color.b, 0), 5f * Time.deltaTime);
        else fader.color = Color.Lerp(fader.color, new Color(fader.color.r, fader.color.g, fader.color.b, 1), 5f * Time.deltaTime);
    }//update
}//class
