using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	private State state = State.DEFAULT;

	[SerializeField] private MainMenuWindowUI windowSettings;
	[SerializeField] private MainMenuWindowUI windowNewMessage;

	public void TriggerOpenSettings() {
		TriggerHandleOpen(State.SHOW_SETTINGS, windowSettings);
	}

	public void TriggerOpenNewMessage() {
		TriggerHandleOpen(State.SHOW_NEW_MESSAGE, windowNewMessage);
	}

	public void CloseEverything() {
		windowSettings.Close();
		windowNewMessage.Close();
		state = State.DEFAULT;
	}

	private enum State {
		DEFAULT,
		SHOW_SETTINGS,
		SHOW_NEW_MESSAGE
	}

	private void TriggerHandleOpen(State newState, MainMenuWindowUI window) {
		if(state == newState) {
			// close
			window.Close();
			state = State.DEFAULT;
		} else if(state == State.DEFAULT) {
			// open
			window.Open();
			state = newState;
		}
	}

}