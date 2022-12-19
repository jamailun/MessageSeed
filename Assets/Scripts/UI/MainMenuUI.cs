using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	private State state = State.DEFAULT;

	[SerializeField] private MainMenuWindowUI windowSettings;
	[SerializeField] private MainMenuWindowUI windowNewMessage;
	[SerializeField] private MainMenuWindowUI windowProfile;
	[SerializeField] private MainMenuWindowUI windowMessages;
	[SerializeField] private MainMenuWindowUI windowFertilizer;
	[SerializeField] private MainMenuWindowUI windowFriends;

	public void TriggerOpenSettings(int x) {
		if (x == 0)
			TriggerHandleOpen(State.SHOW_SETTINGS, windowSettings);
		else if (x == 1)
			TriggerHandleOpen(State.SHOW_PROFILE, windowProfile);
	}

	public void TriggerOpenNewMessage() {
		TriggerHandleOpen(State.SHOW_NEW_MESSAGE, windowNewMessage);
	}

    public void TriggerOpenProfile() {
		TriggerHandleOpen(State.SHOW_PROFILE, windowProfile);
    }

	public void TriggerOpenMessages() {
		TriggerHandleOpen(State.SHOW_MESSAGES, windowMessages);
    }

	public void TriggerOpenFertilizer() {
		TriggerHandleOpen(State.SHOW_FERTILIZER, windowFertilizer);
    }

    public void TriggerOpenFriends() {
		TriggerHandleOpen(State.SHOW_FRIENDS, windowFriends);
    }

    public void CloseEverything() {
		windowSettings.Close();
		windowNewMessage.Close();
		windowProfile.Close();
		windowMessages.Close();
		windowFertilizer.Close();
		windowFriends.Close();
		state = State.DEFAULT;
	}

	private enum State {
		DEFAULT,
		SHOW_SETTINGS,
		SHOW_NEW_MESSAGE,
		SHOW_PROFILE,
		SHOW_MESSAGES,
		SHOW_FERTILIZER,
		SHOW_FRIENDS
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