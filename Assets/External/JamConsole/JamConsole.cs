using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JamUtils2D.JamConsole {
    public class JamConsole : MonoBehaviour {

        [Header("Display config")]
        [SerializeField] private string timeFormat = "HH:mm:ss";
        [SerializeField] private JamConsoleColorSettings colorSettings = new();

        [Header("Scene config")]
        // output
        [SerializeField] private TMP_Text textPrefab;
        [SerializeField] private VerticalLayoutGroup container;
        [SerializeField] private ScrollRect scrollbar;
        // input
        [SerializeField] private SubmitWithButton inputField;
        // all
        [SerializeField] private Button visibilityButton;

        // Fields
        private static JamConsole Instance;

        private bool visible = true;

		private void Awake() {
			if(Instance) {
                Debug.LogWarning("Cannot have multiple instances of JamConsole. Destroying " + gameObject.name + ".");
                Destroy(gameObject);
                return;
			}
            Instance = this;
		}

		void OnEnable() {
            Application.logMessageReceived += MessageReceived;
            inputField.submitEvent += InputReceived;
            ApplyVisibility();
        }

        void OnDisable() {
            Application.logMessageReceived -= MessageReceived;
            inputField.submitEvent -= InputReceived;
        }

        private void InputReceived(string input) {
            Debug.Log("INPUT = (" + input + ")");
            inputField.InputField.ActivateInputField();
        }

        public void MessageReceived(string logString, string _stackTrace, LogType type) {
            AddLine(
                "[" + System.DateTime.Now.ToString(timeFormat) + "] " + logString,
                colorSettings.FromLogType(type)
            );
        }

        private void AddLine(string lineContent, Color color) {
            float scrollbarPosition = scrollbar.verticalNormalizedPosition;

            // Add the line
            var line = Instantiate(textPrefab, container.transform);
            line.text = lineContent;
            line.color = color;
            line.enabled = visible;

            // put scrollbar value to the old one (after recalculation).
            StartCoroutine(ApplyScrollPosition(scrollbar, scrollbarPosition));
        }

        public static void Print(string message, Color color) {
            if(Instance) {
                Instance.AddLine(message, color);
			}
        }

        private IEnumerator ApplyScrollPosition(ScrollRect sr, float verticalPos) {
            yield return new WaitForEndOfFrame();
            sr.verticalNormalizedPosition = verticalPos;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) sr.transform);
        }

        public void ToggleVisibility() {
            visible = !visible;
            ApplyVisibility();
        }

        private void ApplyVisibility() {
            foreach(var img in transform.GetComponentsInChildren<Image>())
                img.enabled = visible;
            foreach(var txt in transform.GetComponentsInChildren<TMP_Text>())
                txt.enabled = visible;

            visibilityButton.GetComponentInChildren<TMP_Text>().text = visible ? "Hide" : "Show";
            visibilityButton.GetComponent<Image>().enabled = true;
            visibilityButton.GetComponentInChildren<TMP_Text>().enabled = true;
        }

    }
}