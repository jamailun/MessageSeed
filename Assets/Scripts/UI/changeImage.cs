using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class changeImage : MonoBehaviour
{
    [SerializeField] private Sprite[] buttonsprite;
    [SerializeField] private Image targetbutton;

    public void ChangeSprite() {

        if(targetbutton.sprite == buttonsprite[0]) {

            targetbutton.sprite = buttonsprite[1];
            return;
        }

        targetbutton.sprite = buttonsprite[0];
    }

}
