using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [Header("Adjust Settings")]
    public int _gridWidth;
    public int _gridHeight;
    public int _colorCount;
    public int _bombAppearScore;
    public int _bombTimer;
    public int _bestScore;

    public static SettingsManager _instance;


    //Singleton
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        LoadData();
    }

    private void LoadData()
    {
        if (!PlayerPrefs.HasKey("bestScore"))
        {
            SetDefaultValues();
        }
        LoadValues();
    }

    private void LoadValues()
    {
        _gridWidth = PlayerPrefs.GetInt("gridWidth");
        _gridHeight = PlayerPrefs.GetInt("gridHeight");

        _colorCount = PlayerPrefs.GetInt("colorCount");

        _bombAppearScore = PlayerPrefs.GetInt("bombScore");
        _bombTimer = PlayerPrefs.GetInt("bombSecond");

        _bestScore = PlayerPrefs.GetInt("bestScore");

    }

    //Set Base Settings
    private void SetDefaultValues()
    {
        PlayerPrefs.SetInt("bestScore", 0);

        PlayerPrefs.SetInt("gridWidth", 8);
        PlayerPrefs.SetInt("gridHeight", 9);

        PlayerPrefs.SetInt("colorCount", 5);

        PlayerPrefs.SetInt("bombScore", 1000);
        PlayerPrefs.SetInt("bombSecond", 5);

        PlayerPrefs.SetInt("sound", 0);

        PlayerPrefs.SetInt("music", 0);
    }

    //Get values frome GameManagement and Set all values
    public void SetBestScore(int value) { PlayerPrefs.SetInt("bestScore", value); }

    public void SetGridWidth(int value) { PlayerPrefs.SetInt("gridWidth", value); }
    public void SetGridHeight(int value) { PlayerPrefs.SetInt("gridHeight", value); }

    public void SetColorCount(int value) { PlayerPrefs.SetInt("colorCount", value); }

    public void SetBombScore(int value) { PlayerPrefs.SetInt("bombScore", value); }
    public void SetBombSecond(int value) { PlayerPrefs.SetInt("bombSecond", value); }
}
