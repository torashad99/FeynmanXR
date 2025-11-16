using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class InternetConnectionChecker : MonoBehaviour
{
    public TMP_Text resultText;

    private void Start()
    {
        SetBackgroundColor(Color.black);
        SetTextColor(Color.white);
        CheckInternetConnection();
    }

    private void CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            resultText.text = "Internet connection failed!";
        }
        else
        {
            resultText.text = "Internet connection successful!";
            StartCoroutine(GetDataFromAPI());
        }
    }

    public void MakeAPIRequest()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(GetDataFromAPI());
        }
    }

    private IEnumerator GetDataFromAPI()
    {
        string apiUrl = "https://www.boredapi.com/api/activity";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            ButtonController buttonController = FindObjectOfType<ButtonController>();
            buttonController.SetRequestStatus(false);

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                ActivityData activityData = JsonUtility.FromJson<ActivityData>(jsonResponse);
                resultText.text = activityData.activity;
            }
            else
            {
                resultText.text = "API request failed!";
            }
        }
    }

    private void SetBackgroundColor(Color color)
    {
        resultText.fontSharedMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, color);
        resultText.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0.5f);
        resultText.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.5f);
        resultText.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.5f);
    }

    private void SetTextColor(Color color)
    {
        resultText.color = color;
    }
}

[System.Serializable]
public class ActivityData
{
    public string activity;
}