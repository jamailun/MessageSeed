using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class clickbuttonTutorial1 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private Image image;
    [SerializeField] private Sprite dropdown, close;

    public void OnPointerDown(PointerEventData eventData) {
        
        image.sprite = close;
    }

    public void OnPointerUp(PointerEventData eventData) {

        image.sprite = dropdown;
    }

    public void Iwasclicked() {
        Debug.Log("Clicked");
    }

    
}
