using UnityEngine;
using TMPro;

/// <summary>
/// Submits an InputField with the specified button.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_InputField))]
public class SubmitWithButton : MonoBehaviour {

    [SerializeField] private string submitKey = "Submit";
    [SerializeField] private bool trimWhitespace = true;
    [SerializeField] private bool clearAfterSubmit = true;
    private TMP_InputField _inputField;
    public TMP_InputField InputField => _inputField;

    public SubmitEvent submitEvent;

    private void Start() {
        _inputField = GetComponent<TMP_InputField>();
        _inputField.onEndEdit.AddListener(fieldValue => {
            if(trimWhitespace)
                _inputField.text = fieldValue = fieldValue.Trim();
            if(Input.GetButton(submitKey))
                ValidateAndSubmit(fieldValue);
        });
    }

    private bool IsInvalid(string fieldValue) {
        return string.IsNullOrEmpty(fieldValue);
    }

    public void ValidateAndSubmit(string fieldValue) {
        if(IsInvalid(fieldValue))
            return;
        // call delegate
        submitEvent(fieldValue);
        // clear content
        if(clearAfterSubmit)
            _inputField.text = "";
    }

    // to be called from a submit button onClick event
    public void ValidateAndSubmit() {
        ValidateAndSubmit(_inputField.text);
    }

    public delegate void SubmitEvent(string value);
}