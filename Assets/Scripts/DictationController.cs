using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Oculus.Voice.Dictation;
using OpenAI.Samples.Chat;

public class DictationController : MonoBehaviour
{

    public AppDictationExperience dictationExperience;

    public TMP_Text listeningText;
    public TMP_InputField inputField;
    public Button speakButton;
    public Button closeButton;
    public Button resetButton;

    public ChatBehaviour chatBehaviour;

    // Cache the latest partial so we can commit on stop
    private string _latestPartial = "";
    private string _latestFull = "";
    private bool activated = false;

    // Cache and display each partial result
    public void OnPartialTranscription(string text)
    {
        _latestPartial = text;
    }

    public void OnFullTranscription(string text)
    {
        if(_latestFull == "" || _latestFull == " "){
            _latestFull = text;
        }
        else
        {
            _latestFull = _latestFull + ". " + text;
        }
    }

    // Called by your Speak button
    public void start()
    {
        _latestFull = "";
        // _latestPartial = "";
        dictationExperience.ActivateImmediately();
        activated = true;
    }

    // Called by your Close (Stop) button
    public void stopThumbsUp()
    {
        if(activated)
        {
            // Commit whatever full we have so far
            inputField.text = _latestFull;
            chatBehaviour.SubmitChat();

            listeningText.text = "Click Button / Wave Hand to Speak";
            speakButton.interactable = true;
            closeButton.interactable = true;
            resetButton.interactable = true;

            dictationExperience.Deactivate();
            inputField.text = "";
            _latestFull = "";
            // _latestPartial = "";
            activated = false;
        }
    }

    public void stopThumbsDown()
    {
        if(activated)
        {
            listeningText.text = "Click Button / Wave Hand to Speak";
            speakButton.interactable = true;
            closeButton.interactable = true;
            resetButton.interactable = true;

            dictationExperience.Deactivate();
            inputField.text = "";
            _latestFull = "";
            // _latestPartial = "";
            activated = false;
        }
    }
}
