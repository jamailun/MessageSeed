using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour {

    [SerializeField] private Sprite spriteIcon;
    [SerializeField] private Sprite spriteClosed;

    [SerializeField] private Image targetButton;

    private bool displayIcon = true;

    public void ChangeSprite() {
        // invert the 'display icon' boolean
        displayIcon = !displayIcon;

        // Change the sprite according to the boolean value
        targetButton.sprite = displayIcon ? spriteIcon : spriteClosed;
    }

}
