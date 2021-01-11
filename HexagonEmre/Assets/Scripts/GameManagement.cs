using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    [Header("MainPanels")]
    [SerializeField] GameObject _endPanel;
    [SerializeField] GameObject _settingsPanel;
    [SerializeField] GameObject _menuPanel;

    [Header("SettingsPanel")]
    [SerializeField] Text _gridWidthText;
    [SerializeField] Text _gridHeightText;
    [SerializeField] Text _colorCountText;
    [SerializeField] Slider _gridWidthSlider;
    [SerializeField] Slider _gridHeightSlider;
    [SerializeField] Slider _colorCountSlider;

    [Header("GamePanel")]
    [SerializeField] Text _scoreText;
    [SerializeField] Text _bestScoreText;
    [SerializeField] Text _movesText;

    [Header("FinishPanel")]
    [SerializeField] Text _endScoreText;
    [SerializeField] Text _endBestScoreText;

    public static GameManagement _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void PlayGame()
    {
        _bestScoreText.text = "BEST: " + SettingsManager._instance._bestScore;
        _scoreText.text = "0";
        _movesText.text = "0";

        ColorManager._instance.CreateGridColors();
        GridManager._instance.CreateGrid();
        GridManager._instance.SetCameraPos();
    }

    public void Settings()
    {
        _gridWidthSlider.value = SettingsManager._instance._gridWidth;
        _gridHeightSlider.value = SettingsManager._instance._gridHeight;

        _colorCountSlider.value = SettingsManager._instance._colorCount;

        _gridWidthText.text = _gridWidthSlider.value.ToString();
        _gridHeightText.text = _gridHeightSlider.value.ToString();
        _colorCountText.text = _colorCountSlider.value.ToString();

    }

    public void GridWidthSlider()
    {
        _gridWidthText.text = _gridWidthSlider.value.ToString();
        SettingsManager._instance.SetGridWidth((int)_gridWidthSlider.value);
    }

    public void GridHeightSlider()
    {
        _gridHeightText.text = _gridHeightSlider.value.ToString();
        SettingsManager._instance.SetGridHeight((int)_gridHeightSlider.value);
    }

    public void ColorCountSlider()
    {
        _colorCountText.text = _colorCountSlider.value.ToString();
        SettingsManager._instance.SetColorCount((int)_colorCountSlider.value);
    }

    public void SetScore(int score)
    {
        _scoreText.text = score.ToString();
    }

    public void SetMove(int move)
    {
        _movesText.text = move.ToString();
    }

    public void FinishGame()
    {
        FinishPanel();
    }

    private void FinishPanel()
    {
        _menuPanel.SetActive(false);
        _endPanel.SetActive(true);
    }

    public void BackButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void HomeButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void SetFinishScore(int score)
    {
        _endScoreText.text = score.ToString();
        if (SettingsManager._instance._bestScore < score)
        {
            SettingsManager._instance.SetBestScore(score);
            _endBestScoreText.text = "NEW BEST SCORE";
        }
        else
        {
            _endBestScoreText.text = "BEST: " + SettingsManager._instance._bestScore;
        }
    }
}
