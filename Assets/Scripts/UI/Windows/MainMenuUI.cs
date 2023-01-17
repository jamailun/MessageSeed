using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	private State state = State.DEFAULT;
	private MainMenuWindowUI subPanelOpen;
	private MainMenuWindowUI subSubPanelOpen;

	private readonly List<MainMenuWindowUI> windows = new();

	[SerializeField] private LogoutUI logoutUI;
	[SerializeField] private DisconnectedUI disconnectedUI;

	public bool IsSomethingOpen() {
		return state != State.DEFAULT || logoutUI.isActiveAndEnabled || disconnectedUI.isActiveAndEnabled;
	}

	private void Start() {
		// add all windows
		windows.AddRange(GetComponentsInChildren<MainMenuWindowUI>());
	}

	// Called by events
	public void TriggerOpen(MainMenuWindowUI target) {
		State newState = (target == null) ? State.DEFAULT : target.TargetState;
		TriggerHandleOpen(newState, target);
	}

	// c'est IMMONDE mais j'ai pas le temps de faire mieux désolé
	public void OpenSubSubPanel(MainMenuWindowUI panel) {
		if(subSubPanelOpen) {
			subSubPanelOpen.Close();
		}
		subSubPanelOpen = panel;
		if(panel)
			panel.Open();
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

	public void CloseOnTop() {
		if(subSubPanelOpen) {
			subSubPanelOpen.Close();
			subSubPanelOpen = null;
		} else if(subPanelOpen) {
			subPanelOpen.Close();
			subPanelOpen = null;
		} else {
			CloseEverything();
		}
	}

	public void CloseEverything() {
		foreach(var w in windows)
			w.Close();
		state = State.DEFAULT;
		subPanelOpen = null;
		subSubPanelOpen = null;
	}

	public void CloseSubPanel() {
		if(subSubPanelOpen) {
			subSubPanelOpen.Close();
			subSubPanelOpen = null;
		} else if(subPanelOpen) {
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
		SHOW_PLAYER_CUSTOMIZATION,
		SHOW_PLAYER_SETTINGS,
		SHOW_CREDITS,
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
	}

}