using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JamUtils2D.JamConsole {
    public class JamConsoleLine : MonoBehaviour {

        [SerializeField] private TMP_Text textField;

        public void SetText(string text, Color color, bool visible) {
            textField.text = text;
            textField.color = color;
            textField.enabled = visible;
        }

    }
}