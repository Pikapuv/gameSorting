using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
     public string itemType;
    public ShelfSlot currentShelf;

    private Vector3 offset;
    private Vector3 originalPosition;
    private bool isDragging = false;

    void OnMouseDown()
    {
        originalPosition = transform.position;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, 0);
    }

    void OnMouseUp()
    {
        isDragging = false;

        ShelfSlot[] allShelves = GameObject.FindObjectsOfType<ShelfSlot>();
        foreach (var shelf in allShelves)
        {
            if (shelf.IsWithinBounds(transform.position) && shelf.CanAcceptItem())
            {
                currentShelf.RemoveTopItem();
                shelf.AddItem(this);
                return;
            }
        }

        transform.position = originalPosition;
    }
}
