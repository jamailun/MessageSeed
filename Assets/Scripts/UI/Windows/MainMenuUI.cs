using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	private State state = State.DEFAULT;
	private MainMenuWindowUI subPanelOpen;

	private readonly List<MainMenuWindowUI> windows = new();

	private void Start() {
		// add all windows
		windows.AddRange(GetComponentsInChildren<MainMenuWindowUI>());
	}

	// Called by events
	public void TriggerOpen(MainMenuWindowUI target) {
		State newState = (target == null) ? State.DEFAULT : target.TargetState;
		TriggerHandleOpen(newState, target);
	}

	// Called by events
	public void OpenSubPanel(MainMenuWindowUI panel) {
		if(subPanelOpen) {
			subPanelOpen.Close();
		}
		subPanelOpen = panel;
		if(panel)
			panel.Open();
	}

	public void CloseEverything() {
		foreach(var w in windows)
			w.Close();
		state = State.DEFAULT;
	}

	public void CloseSubPanel() {
		if(subPanelOpen) {
			subPanelOpen.Close();
			subPanelOpen = null;
		}
	}

	[System.Serializable]
	public enum State {
		DEFAULT = 0,
		// message
		SHOW_SETTINGS = 10,
		SHOW_PROFILE,
		SHOW_FRIENDS,
		SHOW_FERTILIZER,
		SHOW_SHOP,
		// message
		SHOW_MESSAGE_NEW = 20,
		SHOW_READ_MESSAGE,
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
		//TODO close subpanel here
	}

}