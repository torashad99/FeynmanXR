using UnityEngine;
using UnityEngine.UI;

public class LRToggle : MonoBehaviour
{
    public RectTransform closeButton;
    public RectTransform speakButton;
    public RectTransform resetButton;
    public RectTransform modelButton;

    public Button toggleButton;

    public float buttonDisableDuration = 1f; // Duration to disable the button

    public float closeButtonLeftPosX;
    public float closeButtonRightPosX;
    public float closeButtonPosY;

    public float speakButtonLeftPosX;
    public float speakButtonRightPosX;
    public float speakButtonPosY;

    public float resetButtonLeftPosX;
    public float resetButtonRightPosX;
    public float resetButtonPosY;

    public float modelButtonLeftPosX;
    public float modelButtonRightPosX;
    public float modelButtonPosY;

    private bool isLeft = true;

    private void Start()
    {
        // Set initial positions to the left coordinates
        closeButton.anchoredPosition = new Vector2(closeButtonLeftPosX, closeButtonPosY);
        speakButton.anchoredPosition = new Vector2(speakButtonLeftPosX, speakButtonPosY);
        resetButton.anchoredPosition = new Vector2(resetButtonLeftPosX, resetButtonPosY);
        modelButton.anchoredPosition = new Vector2(modelButtonLeftPosX, modelButtonPosY);

        toggleButton.onClick.AddListener(ToggleButtonPositions);
    }

    public void ToggleButtonPositions()
    {

        toggleButton.interactable = false;

        if (isLeft)
        {
            // Move buttons to the right positions
            closeButton.anchoredPosition = new Vector2(closeButtonRightPosX, closeButtonPosY);
            speakButton.anchoredPosition = new Vector2(speakButtonRightPosX, speakButtonPosY);
            resetButton.anchoredPosition = new Vector2(resetButtonRightPosX, resetButtonPosY);
            modelButton.anchoredPosition = new Vector2(modelButtonRightPosX, modelButtonPosY);
        }
        else
        {
            // Move buttons to the left positions
            closeButton.anchoredPosition = new Vector2(closeButtonLeftPosX, closeButtonPosY);
            speakButton.anchoredPosition = new Vector2(speakButtonLeftPosX, speakButtonPosY);
            resetButton.anchoredPosition = new Vector2(resetButtonLeftPosX, resetButtonPosY);
            modelButton.anchoredPosition = new Vector2(modelButtonLeftPosX, modelButtonPosY);
        }

        // Toggle the flag
        isLeft = !isLeft;

        Invoke("EnableButton", buttonDisableDuration);
    }

    private void EnableButton()
    {
        // Enable the button's interactability
        toggleButton.interactable = true;
    }
}