using UnityEngine;

public class HideShow : MonoBehaviour {
    public void TriggerHideShow() {
        gameObject.SetActive( ! gameObject.activeInHierarchy);
    }  
}
