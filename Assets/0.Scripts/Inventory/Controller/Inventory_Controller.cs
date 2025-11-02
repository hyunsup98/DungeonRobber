using UnityEngine;

/// <summary>
/// 인벤토리 비즈니스 로직 컨트롤러
/// Inventory.cs와 UIController 사이의 중개 역할
/// </summary>
public class Inventory_Controller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private QuickSlot_Controller quickSlots;

    private void Awake()
    {
        // 참조가 없으면 자동으로 찾기
        if (inventory == null)
            inventory = FindObjectOfType<Inventory>();
        
        if (quickSlots == null)
            quickSlots = FindObjectOfType<QuickSlot_Controller>();

        // QuickSlots에 Inventory 참조 설정
        if (quickSlots != null)
            quickSlots.inventory = inventory;
    }

    /// <summary>
    /// 외부에서 인벤토리 접근
    /// </summary>
    public Inventory GetInventory()
    {
        return inventory;
    }

    /// <summary>
    /// 외부에서 퀵슬롯 접근
    /// </summary>
    public QuickSlot_Controller GetQuickSlots()
    {
        return quickSlots;
    }

    /// <summary>
    /// 아이템을 인벤토리에 추가
    /// </summary>
    public bool AddItemToInventory(Item item)
    {
        if (inventory != null)
            return inventory.AddItem(item);
        
        return false;
    }

    /// <summary>
    /// 아이템을 퀵슬롯에 추가
    /// </summary>
    public void AddItemToQuickSlots(Item item)
    {
        quickSlots?.AddItem(item);
    }

    /// <summary>
    /// 인벤토리 토글
    /// </summary>
    public void ToggleInventoryUI()
    {
        inventory?.ToggleInventory();
    }
}
