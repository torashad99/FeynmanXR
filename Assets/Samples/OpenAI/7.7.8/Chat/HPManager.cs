using UnityEngine;
using System;
using TMPro;

public class HPManager : MonoBehaviour
{
    private const string HPKey = "HPKey";
    private const string LastDecrementTimeKey = "LastDecrementTimeKey";
    private const int InitialHP = 50;
    private const int ResetHours = 24;

    public TMP_Text counterText;

    private void Start()
    {
        // Check if HP exists in PlayerPrefs, if not, set it to the initial value
        if (!PlayerPrefs.HasKey(HPKey))
        {
            PlayerPrefs.SetInt(HPKey, InitialHP);
            PlayerPrefs.Save();
        }

        UpdateHPText();
    }

    public int GetHP()
    {
        CheckHPReset();
        return PlayerPrefs.GetInt(HPKey);
    }

    public void ResetHP()
    {
        PlayerPrefs.SetInt(HPKey, InitialHP);
        PlayerPrefs.Save();

        UpdateHPText();
    }

    public void DecrementHP()
    {
        CheckHPReset();

        int currentHP = PlayerPrefs.GetInt(HPKey);
        if (currentHP > 0)
        {
            currentHP--;
            PlayerPrefs.SetInt(HPKey, currentHP);
            PlayerPrefs.SetString(LastDecrementTimeKey, DateTime.Now.ToString());
            PlayerPrefs.Save();
        }

        UpdateHPText();
    }

    private void CheckHPReset()
    {
        if (PlayerPrefs.HasKey(LastDecrementTimeKey))
        {
            DateTime lastDecrementTime = DateTime.Parse(PlayerPrefs.GetString(LastDecrementTimeKey));
            TimeSpan timeSinceLastDecrement = DateTime.Now - lastDecrementTime;

            if (PlayerPrefs.GetInt(HPKey) == 0 && timeSinceLastDecrement.TotalHours >= ResetHours)
            {
                PlayerPrefs.SetInt(HPKey, InitialHP);
                PlayerPrefs.Save();

                UpdateHPText();
            }
        }
    }

    private void UpdateHPText()
    {
        if (counterText != null)
        {
            counterText.text = "HP: " + GetHP().ToString() + " /50";
        }
    }

    public string GetTimeRemaining()
    {
        if (PlayerPrefs.HasKey(LastDecrementTimeKey) && PlayerPrefs.GetInt(HPKey) == 0)
        {
            DateTime lastDecrementTime = DateTime.Parse(PlayerPrefs.GetString(LastDecrementTimeKey));
            DateTime resetTime = lastDecrementTime.AddHours(ResetHours);
            TimeSpan timeRemaining = resetTime - DateTime.Now;

            if (timeRemaining.TotalSeconds > 0)
            {
                return $"{timeRemaining.Hours:D2} hours, {timeRemaining.Minutes:D2} minutes, and {timeRemaining.Seconds:D2} seconds!";
            }
        }

        return "N/A";
    }
}