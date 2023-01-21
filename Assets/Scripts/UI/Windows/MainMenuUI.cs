using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	private MainMenuWindowUI currentWindow;
	private MainMenuWindowUI subPanelOpen;
	private MainMenuWindowUI subSubPanelOpen;

	private readonly List<MainMenuWindowUI> windows = new();

	[SerializeField] private LogoutUI logoutUI;
	[SerializeField] private DisconnectedUI disconnectedUI;

	public bool IsSomethingOpen() {
		return currentWindow != null || logoutUI.isActiveAndEnabled || disconnectedUI.isActiveAndEnabled;
	}

	private void Start() {
		// add all windows
		windows.AddRange(GetComponentsInChildren<MainMenuWindowUI>());
	}

	// Called by events
	public void TriggerOpen(MainMenuWindowUI newWindow) {
		if(currentWindow == newWindow) {
			// close
			currentWindow.Close();
			currentWindow = null;
		} else if(currentWindow == null) {
			// open
			newWindow.Open();
			currentWindow = newWindow;
		}
	}

	public void TryOpen(MainMenuWindowUI target) {
		if(currentWindow == null) {
			target.Open();
			currentWindow = target;
		}
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
		currentWindow = null;
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

}