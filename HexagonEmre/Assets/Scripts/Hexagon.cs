using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public Color _color;
    public int _xValue;
    public int _yValue;

    private SpriteRenderer SpriteRenderer;
    private SpriteRenderer _selectedSprite;

    private Vector3 m_movePosition;

    private int _placeSpeed;

    private void Awake()
    {
        HideEdges();
    }

    private void HideEdges()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        _selectedSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _selectedSprite.enabled = false;
    }

    public void Start()
    {
        SpriteRenderer.color = _color;
        SpriteRenderer.enabled = true;
        _placeSpeed = 100;
    }

    private void Update()
    {
        PlaceHexagons();
    }

    //Placing hexagons in their places
    private void PlaceHexagons()
    {
        if (transform.position != m_movePosition)
        {
            float _xPos = Mathf.MoveTowards(transform.position.x, m_movePosition.x, _placeSpeed * Time.deltaTime);
            float _yPos = Mathf.MoveTowards(transform.position.y, m_movePosition.y, _placeSpeed * Time.deltaTime);
            transform.position = new Vector3(_xPos, _yPos, 0f);
        }
    }

    public void Selected(bool selected)
    {
        _selectedSprite.enabled = selected;

        if (selected)
        {
            SpriteRenderer.sortingOrder = 0;
            _selectedSprite.sortingOrder = -1;
        }
        else
        {
            SpriteRenderer.sortingOrder = -2;
            _selectedSprite.sortingOrder = -3;
        }
    }

    //Set the distance of hexes to each other
    public void SetGridPos(int xValue, int yValue, int speed = 5)
    {
        _xValue = xValue;
        _yValue = yValue;
        _placeSpeed = speed;

        float t_y = Mathf.Sqrt(3);
        m_movePosition = new Vector3(_xValue * 1.55f, _xValue % 2 == 0 ? _yValue * t_y : (_yValue * t_y) + t_y / 2, 0);
    }

    //Manage Hexagon Sprite
    public void SetSprite(bool enable)
    {
        SpriteRenderer.enabled = enable;
    }
}
