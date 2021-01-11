using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BombHexagon : MonoBehaviour
{
    public TMP_Text _timerText;
    private int _timer;
    private SpriteRenderer SpriteRenderer;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    //Set bomb color same as the hexagon color
    public void SetColor(Color color)
    {
        _timer = SettingsManager._instance._bombTimer + 1;
        SpriteRenderer.color = color;
    }

    //Reduce time each move
    public void Countdown()
    {
        _timer--;
        _timerText.text = _timer.ToString();

        //If Bomb Not Destroyed in Time
        if (_timer <= 0)
        {
            GridManager._instance.BombExploded();
        }
    }
}
