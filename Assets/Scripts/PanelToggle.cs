using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Oculus.Interaction;
using TMPro;

public class PanelToggle : MonoBehaviour
{
    public GameObject panel; // Reference to the panel GameObject
    public Button toggleButton; // Reference to the button that toggles the panel
    public Button speakButton;
    public Button resetButton;
    public Button modelButton; //Special case, interactable not controlled by toggle

    public TMP_Text listeningText;

    public TMP_Text counterText;

    public SelectorUnityEventWrapper leftSpeakSelectorWrapper;
    public SelectorUnityEventWrapper rightSpeakSelectorWrapper;
    public SelectorUnityEventWrapper leftStopUpSelectorWrapper;
    public SelectorUnityEventWrapper rightStopUpSelectorWrapper;
    public SelectorUnityEventWrapper leftStopDownSelectorWrapper;
    public SelectorUnityEventWrapper rightStopDownSelectorWrapper;


    public TMP_Text toggleText;
    public float scaleDuration = 0.5f; // Duration of the scale animation
    public float buttonDisableDuration = 2.5f; // Duration to disable the button

    private Vector3 originalPanelScale; // Original scale of the panel
    private Vector3 originalSpeakButtonScale; // Original scale of the speak button
    private Vector3 originalResetButtonScale; // Original scale of the reset button
    private Vector3 originalModelButtonScale;
    private Vector3 listeningTextScale;
    private Vector3 counterTextScale;
    private bool isPanelVisible = true; // Track the visibility state of the panel

    private void Start()
    {
        // Store the original scale of the panel
        originalPanelScale = panel.transform.localScale;
        originalSpeakButtonScale = speakButton.transform.localScale;
        originalResetButtonScale = resetButton.transform.localScale;
        originalModelButtonScale = modelButton.transform.localScale;
        listeningTextScale = listeningText.transform.localScale;
        counterTextScale = counterText.transform.localScale;

        // Add a listener to the button's click event
        toggleButton.onClick.AddListener(TogglePanel);

        toggleText.text = isPanelVisible ? "Close" : "Open"; 
    }

    private void TogglePanel()
    {
        if (isPanelVisible)
        {
            HidePanel();
        }
        else
        {
            ShowPanel();
        }

        speakButton.interactable = false;
        resetButton.interactable = false;
        leftSpeakSelectorWrapper.enabled = false;
        rightSpeakSelectorWrapper.enabled = false;
        leftStopUpSelectorWrapper.enabled = false;
        rightStopUpSelectorWrapper.enabled = false;
        leftStopDownSelectorWrapper.enabled = false;
        rightStopDownSelectorWrapper.enabled = false;
        listeningText.enabled = false;
        counterText.enabled = false;

        isPanelVisible = !isPanelVisible; // Toggle the visibility state

        // Disable the button's interactability
        toggleButton.interactable = false;

        // Enable the button's interactability after the specified duration
        Invoke("EnableButton", buttonDisableDuration);
    }

    private void ShowPanel()
    {
        // Scale up the panel to its original dimensions
        StartCoroutine(ScalePanel(Vector3.zero, originalPanelScale));

        // Scale up the speak button to its original dimensions
        StartCoroutine(ScaleButton(speakButton, Vector3.zero, originalSpeakButtonScale));

        // Scale up the reset button to its original dimensions
        StartCoroutine(ScaleButton(resetButton, Vector3.zero, originalResetButtonScale));

        StartCoroutine(ScaleButton(modelButton, Vector3.zero, originalModelButtonScale));

        StartCoroutine(ScaleText(listeningText, Vector3.zero, listeningTextScale));

        StartCoroutine(ScaleText(counterText, Vector3.zero, counterTextScale));

    }

    private void HidePanel()
    {
        // Scale down the panel to zero
        StartCoroutine(ScalePanel(originalPanelScale, Vector3.zero));

        // Scale down the speak button to zero
        StartCoroutine(ScaleButton(speakButton, originalSpeakButtonScale, Vector3.zero));

        // Scale down the reset button to zero
        StartCoroutine(ScaleButton(resetButton, originalResetButtonScale, Vector3.zero));

        StartCoroutine(ScaleButton(modelButton, originalModelButtonScale, Vector3.zero));

        StartCoroutine(ScaleText(listeningText, listeningTextScale, Vector3.zero));

        StartCoroutine(ScaleText(counterText, counterTextScale, Vector3.zero));
    }

    private System.Collections.IEnumerator ScalePanel(Vector3 startScale, Vector3 endScale)
    {
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            panel.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel.transform.localScale = endScale;
    }

    private System.Collections.IEnumerator ScaleButton(Button button, Vector3 startScale, Vector3 endScale)
    {
        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            button.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        button.transform.localScale = endScale;
    }

    private System.Collections.IEnumerator ScaleText(TMP_Text text, Vector3 startScale, Vector3 endScale)
    {
        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            text.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.transform.localScale = endScale;
    }

    private void EnableButton()
    {
        // Enable the button's interactability
        toggleButton.interactable = true;
        toggleText.text = isPanelVisible ? "Close" : "Open";
        speakButton.interactable = isPanelVisible;
        resetButton.interactable = isPanelVisible; 
        leftSpeakSelectorWrapper.enabled = isPanelVisible;
        rightSpeakSelectorWrapper.enabled = isPanelVisible;
        leftStopUpSelectorWrapper.enabled = isPanelVisible;
        rightStopUpSelectorWrapper.enabled = isPanelVisible;
        leftStopDownSelectorWrapper.enabled = isPanelVisible;
        rightStopDownSelectorWrapper.enabled = isPanelVisible;
        listeningText.enabled = isPanelVisible;
        counterText.enabled = isPanelVisible;
    }
}