// Scripts/ItemSpawner.cs (Phiên bản cho cả 2 điều kiện)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    public List<GameObject> itemPrefabs;

    [Header("Spawn Settings")]
    [Range(0, 3)]
    public int itemsToSpawnPerShelf = 3; // Cố gắng lấp đầy kệ
    public int maxGlobalCountPerItemType = 3; // Điều kiện 1: Tối đa 3 instance của mỗi loại item TRÊN TOÀN BỘ KỆ

    private ShelfSlot[] allShelfSlots;
    private Dictionary<string, int> spawnedGlobalItemCounts; // Theo dõi số lượng đã spawn toàn cục của mỗi itemType

    void Start()
    {
        allShelfSlots = FindObjectsOfType<ShelfSlot>();
        spawnedGlobalItemCounts = new Dictionary<string, int>();

        if (itemPrefabs == null || itemPrefabs.Count == 0)
        {
            Debug.LogError("ItemSpawner: Chưa gán Item Prefabs!");
            return;
        }

        foreach (var prefab in itemPrefabs)
        {
            Item itemComponent = prefab.GetComponent<Item>();
            if (itemComponent != null && !string.IsNullOrEmpty(itemComponent.itemType))
            {
                if (!spawnedGlobalItemCounts.ContainsKey(itemComponent.itemType))
                {
                    spawnedGlobalItemCounts.Add(itemComponent.itemType, 0);
                }
            }
            else
            {
                Debug.LogError($"Prefab {prefab.name} không có component Item hoặc itemType rỗng!");
            }
        }

        if (allShelfSlots == null || allShelfSlots.Length == 0)
        {
            Debug.LogWarning("ItemSpawner: Không tìm thấy ShelfSlot nào trong màn chơi.");
            return;
        }

        SpawnInitialItems();
    }

    void SpawnInitialItems()
    {
        foreach (ShelfSlot shelf in allShelfSlots)
        {
            // Lấy danh sách các loại item đã có trên kệ này
            List<string> typesOnThisShelf = shelf.GetItemTypesOnShelf();

            for (int i = 0; i < itemsToSpawnPerShelf; i++)
            {
                if (shelf.CanAcceptItem())
                {
                    // Lấy danh sách các prefab có thể spawn (đáp ứng điều kiện 1)
                    List<ItemPrefabInfo> globallyAvailablePrefabs = GetGloballyAvailablePrefabs();

                    // Lọc thêm: loại bỏ những type đã có trên kệ này (đáp ứng điều kiện 2)
                    List<ItemPrefabInfo> locallyAvailablePrefabs = globallyAvailablePrefabs
                        .Where(info => !typesOnThisShelf.Contains(info.itemType))
                        .ToList();

                    if (locallyAvailablePrefabs.Count == 0)
                    {
                        // Debug.Log($"Kệ {shelf.gameObject.name}: Không còn loại item nào phù hợp để spawn (đã có đủ loại hoặc các loại còn lại đã đạt giới hạn toàn cục).");
                        break; // Không còn gì phù hợp để spawn cho kệ này
                    }

                    // Chọn một prefab ngẫu nhiên từ danh sách đã được lọc kỹ
                    int randomIndex = Random.Range(0, locallyAvailablePrefabs.Count);
                    ItemPrefabInfo randomItemInfo = locallyAvailablePrefabs[randomIndex];

                    GameObject newItemGO = Instantiate(randomItemInfo.prefab);
                    Item newItem = newItemGO.GetComponent<Item>();

                    if (newItem != null)
                    {
                        shelf.AddItem(newItem);
                        spawnedGlobalItemCounts[newItem.itemType]++; // Cập nhật số đếm toàn cục
                        typesOnThisShelf.Add(newItem.itemType);    // Cập nhật các loại trên kệ này
                        // Debug.Log($"Đã spawn {newItem.itemType} (Toàn cục: {spawnedGlobalItemCounts[newItem.itemType]}) vào kệ {shelf.gameObject.name}");
                    }
                    else
                    {
                        Debug.LogError($"Prefab {randomItemInfo.prefab.name} không có component Item!");
                        Destroy(newItemGO);
                    }
                }
                else
                {
                    break; // Kệ đầy
                }
            }
        }
    }

    private class ItemPrefabInfo
    {
        public GameObject prefab;
        public string itemType;
    }

    private List<ItemPrefabInfo> GetGloballyAvailablePrefabs()
    {
        List<ItemPrefabInfo> available = new List<ItemPrefabInfo>();
        foreach (var prefab in itemPrefabs)
        {
            Item itemComponent = prefab.GetComponent<Item>();
            if (itemComponent != null && !string.IsNullOrEmpty(itemComponent.itemType))
            {
                if (spawnedGlobalItemCounts.ContainsKey(itemComponent.itemType) &&
                    spawnedGlobalItemCounts[itemComponent.itemType] < maxGlobalCountPerItemType)
                {
                    available.Add(new ItemPrefabInfo { prefab = prefab, itemType = itemComponent.itemType });
                }
            }
        }
        return available;
    }
}