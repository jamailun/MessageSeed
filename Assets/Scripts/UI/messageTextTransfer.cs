using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class messageTextTransfer : MonoBehaviour
{
    private string input;

    public void ReadInput(string message) {

        input = message;
        Debug.Log(input);
    }
}
