// Scripts/ShelfSlot.cs
using System.Collections.Generic;
using UnityEngine;

public class ShelfSlot : MonoBehaviour
{
    public Transform[] itemPoints; // 3 vị trí trong kệ
    private List<Item> items = new List<Item>();
    private Collider2D areaCollider;

    void Awake()
    {
        areaCollider = GetComponent<Collider2D>();
    }

    public bool CanAcceptItem()
    {
        return items.Count < 3;
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        item.transform.SetParent(transform);
        item.transform.position = itemPoints[items.Count - 1].position;
        item.currentShelf = this;

        CheckForMatch();
    }

    public Item RemoveTopItem()
    {
        if (items.Count == 0) return null;
        Item top = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);
        return top;
    }

    public void CheckForMatch()
    {
        if (items.Count < 3) return;
        string type = items[0].itemType;
        if (items.TrueForAll(i => i.itemType == type))
        {
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();
            GameManager.Instance.AddScore(1);
        }
    }

    public bool IsWithinBounds(Vector3 position)
    {
        return areaCollider != null && areaCollider.OverlapPoint(position);
    }
       public List<string> GetItemTypesOnShelf()
    {
        List<string> types = new List<string>();
        foreach (Item itemInSlot in items) // 'items' là List<Item> của bạn
        {
            if (itemInSlot != null && !types.Contains(itemInSlot.itemType))
            {
                types.Add(itemInSlot.itemType);
            }
        }
        return types;
    }
}