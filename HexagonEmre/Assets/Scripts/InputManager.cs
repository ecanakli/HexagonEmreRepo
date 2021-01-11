using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Hexagon _selectedHexagon;
    private int _selectingAngle;

    private bool _isDragging;
    public void OnDrag(PointerEventData eventData)
    {
        _isDragging = true;

        if (eventData.delta.x > 0)
        {
            GridManager._instance.RotateHexagons(120);
        }
        else
        {
            GridManager._instance.RotateHexagons(-120);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isDragging)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (hit)
        {
            Vector3 _relative = hit.transform.InverseTransformPoint(hit.point);
            int _angle = (int)(Mathf.Atan2(_relative.x, _relative.y) * Mathf.Rad2Deg);

            _angle = (_angle + 360) % 360;

            _selectedHexagon = hit.transform.GetComponent<Hexagon>();
            _selectingAngle = _angle;

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
        GridManager._instance.SelectHexagons(_selectedHexagon, _selectingAngle);
    }
}
