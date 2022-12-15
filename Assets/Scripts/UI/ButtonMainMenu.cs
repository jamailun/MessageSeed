using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMainMenu : MonoBehaviour {

    [SerializeField] private TMPro.TMP_Text thetext;
    [SerializeField] private TMPro.TMP_Text toappears;
    [SerializeField] private GameObject Image;

    private bool isShowing = false;

    public void ButtonPress() {
        Debug.Log("it works tm");
        
        isShowing = !isShowing;
        if(isShowing == true) {
            thetext.text = "true";
        }
        else {
            thetext.text = "isShowing is false";
        }
        // toappears.gameObject.SetActive(isShowing);
        // center of gravity of mesages : if plant is close to others it will need more water:
        // if its far away from everything, few likes will be nedded.
    }

}
