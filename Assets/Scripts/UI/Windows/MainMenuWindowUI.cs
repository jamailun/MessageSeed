using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuWindowUI : MonoBehaviour {

	private bool open = false;

	[SerializeField] private UnityEvent openEvent;
	[SerializeField] private UnityEvent closeEvent;

	public void Open() {
		if(open)
			return;
		openEvent?.Invoke();
		open = true;
	}
	public void Close() {
		if(!open)
			return;
		closeEvent?.Invoke();
		open = false;
	}

}