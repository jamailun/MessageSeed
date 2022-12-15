using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideShow : MonoBehaviour
{
    [SerializeField] private GameObject button;

    public void WhenButtonIsClicked() {

        if (button.activeInHierarchy == true) {
            button.SetActive(false);
        } else
            button.SetActive(true);
    }

        
}
