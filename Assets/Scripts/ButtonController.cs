using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Oculus.Voice.Dictation;

public class ButtonController : MonoBehaviour
{
    public InternetConnectionChecker connectionChecker;
    public AppDictationExperience voiceExperience;
    private Button button;
    private bool isRequestInProgress = false;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        // if (!isRequestInProgress)
        // {
        //     StartCoroutine(ChangeButtonColor());
        //     connectionChecker.MakeAPIRequest();
        // }
        // voiceExperience.Toggle();
        voiceExperience.Activate(); //try this if it gives problems later on
    }

    private IEnumerator ChangeButtonColor()
    {
        isRequestInProgress = true;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.blue;
        button.colors = colors;

        yield return new WaitUntil(() => !isRequestInProgress);

        colors.normalColor = Color.white;
        button.colors = colors;
    }

    public void SetRequestStatus(bool status)
    {
        isRequestInProgress = status;
    }
}