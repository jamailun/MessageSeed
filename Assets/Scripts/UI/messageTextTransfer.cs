using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class messageTextTransfer : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField textfield;
    [SerializeField] private TMPro.TMP_InputField header;


    public void ReadInput() {

        Debug.Log(header.text);
        Debug.Log(textfield.text);   
    }
}
