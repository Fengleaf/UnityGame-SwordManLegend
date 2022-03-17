using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour
{
    float dragOffsetX;
    float dragOffsetY;

    private void Start()
    {
        dragOffsetX = 0;
        dragOffsetY = 0;
    }

    private void OnMouseDown()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragOffsetX = transform.position.x - mouse.x;
        dragOffsetY = transform.position.y - mouse.y;
    }

    private void OnMouseDrag()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(mouse.x + dragOffsetX, mouse.y + dragOffsetY);
    }
}
