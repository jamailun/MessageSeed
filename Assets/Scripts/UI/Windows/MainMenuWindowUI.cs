using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuWindowUI : MonoBehaviour {

	private bool open = false;

	[SerializeField] private MainMenuUI.State _state;
	[SerializeField] private UnityEvent openEvent;
	[SerializeField] private UnityEvent closeEvent;
	public MainMenuUI.State TargetState => _state;

	public void Open() {
		if(open)
			return;
		//TODO ?
		openEvent?.Invoke();
		open = true;
	}
	public void Close() {
		if(!open)
			return;
		//TODO ?
		closeEvent?.Invoke();
		open = false;
	}

}