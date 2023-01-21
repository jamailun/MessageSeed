using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JamUtils2D.JamConsole {
    public class JamConsole : MonoBehaviour {

        [Header("Display config")]
        [SerializeField] private string timeFormat = "HH:mm:ss";
        [SerializeField] private JamConsoleColorSettings colorSettings = new();
        [SerializeField] private int maxLines = 50;

        [Header("Scene config")]
        // output
        [SerializeField] private JamConsoleLine linePrefab;//TMP_Text textPrefab;
        [SerializeField] private VerticalLayoutGroup container;
        [SerializeField] private ScrollRect scrollbar;
        // input
        [SerializeField] private SubmitWithButton inputField;
        // all
        [SerializeField] private RectTransform consoleView;

        [Header("Persistence config")]
        [SerializeField] private bool dontDestroyOnLoad = true;
        [SerializeField] private bool startHidden = true;

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

		private void Start() {
            if(dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
	    	}
            if(startHidden) {
                visible = false;
                ApplyVisibility();
            }
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
            //inputField.InputField.ActivateInputField();
            if(input == "logs") {
                visible = true;
                ApplyVisibility();
            } else if(input == "hide") {
                visible = false;
                ApplyVisibility();
            } else if(input == "clear") {
                container.transform.DestroyChildren();
            }
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
            var line = Instantiate(linePrefab, container.transform);
            line.SetText(lineContent, color, visible);

            // remove the max amount
            if(container.transform.childCount > maxLines)
                Destroy(container.transform.GetChild(0).gameObject);

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
            foreach(var img in consoleView.GetComponentsInChildren<Image>())
                img.enabled = visible;
            foreach(var txt in consoleView.GetComponentsInChildren<TMP_Text>())
                txt.enabled = visible;
        }

    }
}