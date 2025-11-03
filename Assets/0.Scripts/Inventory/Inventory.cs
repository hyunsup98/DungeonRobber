using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 인벤토리를 관리하는 메인 클래스
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 9;
    [Tooltip("슬롯 프리팹 (동적 생성 시 사용)")]
    [SerializeField] private GameObject slotPrefab;
    
    [Header("UI References")]
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private Transform slotParent;

    // 아이템 데이터
    public List<Item> items;

    // UI 슬롯들
    private InvenSlot[] slots;
    
    /// <summary>
    /// 인벤토리 크기 (읽기 전용)
    /// </summary>
    public int InvenSize
    {
        get => inventorySize;
        private set
        {
            inventorySize = Mathf.Max(1, value); // 최소 1개
        }
    }
    
    // 골드 관리 - GameManager.Gold를 사용 (호환성을 위한 프로퍼티)
    [System.Obsolete("GameManager.Gold를 직접 사용하세요. 이 프로퍼티는 GameManager.Gold를 반환합니다.")]
    public int Gold
    {
        get
        {
            if (GameManager.Instance != null)
                return GameManager.Instance.Gold;
            return 0;
        }
        set
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Gold = value;
                UpdateGoldText();
            }
        }
    }
    
    [Header("Gold UI")]
    [SerializeField] private TMPro.TextMeshProUGUI goldText;

    private void Awake()
    {
        // 인벤토리 크기만큼 null로 초기화
        items = new List<Item>();
        for (int i = 0; i < inventorySize; i++)
        {
            items.Add(null);
        }
        
        // 골드 텍스트 자동 찾기 (설정되지 않은 경우)
        if (goldText == null)
        {
            TMPro.TextMeshProUGUI[] allTexts = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            foreach (var text in allTexts)
            {
                if (text.CompareTag("GoldText"))
                {
                    goldText = text;
                    Debug.Log("Inventory: GoldText를 자동으로 찾았습니다.");
                    break;
                }
            }
        }
        
        InitializeSlots();
        HideInventory();

        GameManager.Instance.variationGoldAction += UpdateGoldText;

        // 초기 골드 UI 업데이트
        UpdateGoldText();
    }

    private void InitializeSlots()
    {
        if (slotParent == null)
        {
            Debug.LogWarning("Inventory: slotParent가 설정되지 않았습니다.");
            return;
        }

        // 기존 슬롯들 찾기
        InvenSlot[] existingSlots = slotParent.GetComponentsInChildren<InvenSlot>(true);
        
        // 필요한 슬롯 개수 확인
        int requiredSlots = inventorySize;
        int currentSlotCount = existingSlots != null ? existingSlots.Length : 0;
        
        // 슬롯이 부족하면 생성
        if (currentSlotCount < requiredSlots && slotPrefab != null)
        {
            for (int i = currentSlotCount; i < requiredSlots; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotParent);
                slotObj.name = $"InvenSlot_{i}";
            }
        }
        
        // 모든 슬롯 가져오기 (새로 생성된 것 포함)
        slots = slotParent.GetComponentsInChildren<InvenSlot>();
        
        // 슬롯 활성화/비활성화 처리
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < requiredSlots)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].ownerInventory = this;
                // items 리스트 범위 내에 있으면 아이템 할당
                if (i < items.Count)
                {
                    slots[i].Item = items[i];
                }
                else
                {
                    slots[i].Item = null;
                }
            }
            else
            {
                // 필요 없는 슬롯은 비활성화 (삭제하지 않고 보관)
                slots[i].gameObject.SetActive(false);
            }
        }
        
        // items 리스트 크기 조정
        ResizeItemsList(requiredSlots);
    }
    

    
    /// <summary>
    /// items 리스트 크기를 조정합니다.
    /// </summary>
    private void ResizeItemsList(int newSize)
    {
        if (items == null)
        {
            items = new List<Item>();
        }
        
        int currentSize = items.Count;
        
        if (newSize > currentSize)
        {
            // 리스트 확장 (null로 채움)
            for (int i = currentSize; i < newSize; i++)
            {
                items.Add(null);
            }
        }
        else if (newSize < currentSize)
        {
            // 리스트 축소 (뒤에서부터 제거, 아이템이 있는 슬롯은 보존)
            // 아이템이 있는 슬롯은 건드리지 않고, null 슬롯만 제거
            int nullCount = currentSize - newSize;
            for (int i = items.Count - 1; i >= 0 && nullCount > 0; i--)
            {
                if (items[i] == null)
                {
                    items.RemoveAt(i);
                    nullCount--;
                }
            }
            
            // 여전히 크기가 크면 강제로 축소 (뒤에서부터)
            while (items.Count > newSize)
            {
                items.RemoveAt(items.Count - 1);
            }
        }
    }

    /// <summary>
    /// 인벤토리 열림 여부를 반환합니다.
    /// </summary>
    public bool IsOpen => inventoryRoot != null && inventoryRoot.activeSelf;

    /// <summary>
    /// 인벤토리 열기/닫기를 토글합니다.
    /// </summary>
    public void ToggleInventory()
    {
        if (IsOpen)
            HideInventory();
        else
            ShowInventory();
    }

    /// <summary>
    /// 인벤토리를 표시합니다.
    /// </summary>
    public void ShowInventory()
    {
        if (inventoryRoot != null)
        {
            inventoryRoot.SetActive(true);
            
            // 인벤토리 크기를 확인하고 필요시 슬롯 재구성
            ValidateAndRebuildSlots();
            RefreshSlots();
        }
    }
    
    /// <summary>
    /// 인벤토리 크기를 확인하고 슬롯을 재구성합니다.
    /// </summary>
    private void ValidateAndRebuildSlots()
    {
        if (slotParent == null) return;
        
        // 현재 활성화된 슬롯 개수 확인
        InvenSlot[] existingSlots = slotParent.GetComponentsInChildren<InvenSlot>(true);
        int activeSlotCount = 0;
        
        if (existingSlots != null)
        {
            foreach (var slot in existingSlots)
            {
                if (slot.gameObject.activeSelf)
                    activeSlotCount++;
            }
        }
        
        // 인벤토리 크기와 맞지 않으면 재구성
        if (activeSlotCount != inventorySize)
        {
            Debug.Log($"Inventory: 슬롯 개수 불일치 감지 (활성: {activeSlotCount}, 필요: {inventorySize}). 슬롯을 재구성합니다.");
            InitializeSlots();
        }
    }

    /// <summary>
    /// 인벤토리를 숨깁니다.
    /// </summary>
    public void HideInventory()
    {
        // 컨텍스트 메뉴 숨기기
        InventoryContextMenu inventoryContextMenu = InventoryContextMenu.GetOrFind();
        if (inventoryContextMenu != null)
            inventoryContextMenu.Hide();

        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);
    }

    /// <summary>
    /// 인벤토리 슬롯을 새로고침합니다.
    /// </summary>
    public void RefreshSlots()
    {
        // 슬롯이 없거나 items가 없으면 초기화
        if (slots == null || items == null)
        {
            InitializeSlots();
            return;
        }
        
        // 슬롯 배열 재확인 (동적 변경 대응)
        InvenSlot[] currentSlots = slotParent.GetComponentsInChildren<InvenSlot>();
        if (currentSlots != null && currentSlots.Length != slots.Length)
        {
            slots = currentSlots;
        }

        // 활성화된 슬롯에만 아이템 할당
        int activeSlotCount = Mathf.Min(inventorySize, slots.Length);
        
        for (int i = 0; i < activeSlotCount; i++)
        {
            if (i < slots.Length && slots[i] != null)
            {
                bool wasActive = slots[i].gameObject.activeSelf;
                bool shouldBeActive = i < inventorySize;
                
                // 활성화 상태 변경이 필요한 경우에만
                if (wasActive != shouldBeActive)
                {
                    slots[i].gameObject.SetActive(shouldBeActive);
                }
                
                if (shouldBeActive)
                {
                    slots[i].ownerInventory = this;
                    // items 리스트 범위 내에 있으면 아이템 할당
                    Item newItem = (i < items.Count) ? items[i] : null;
                    if (slots[i].Item != newItem)
                    {
                        slots[i].Item = newItem;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 골드 텍스트를 업데이트합니다.
    /// </summary>
    public void UpdateGoldText()
    {
        if (goldText == null)
        {
            TMPro.TextMeshProUGUI[] allTexts = GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
            foreach (var text in allTexts)
            {
                if (text.CompareTag("GoldText"))
                {
                    goldText = text;
                    break;
                }
            }
        }
        
        if (goldText != null)
        {
            int currentGold = GameManager.Instance != null ? GameManager.Instance.Gold : 0;
            goldText.text = $"Gold: {currentGold:N0}";
        }
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.variationGoldAction -= UpdateGoldText;
        }
    }

    #region Item Management

    /// <summary>
    /// 아이템을 인벤토리에 추가합니다.
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    /// <returns>추가 성공 여부</returns>
    public bool AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Inventory: null 아이템은 추가할 수 없습니다.");
            return false;
        }

        if (slots == null || items == null)
        { 
            InitializeSlots();
        }

        // 같은 아이템이 이미 있으면 개수 증가
        if (items.Find(x => x != null && x.name == item.name) != null)
        {
            int itemIdx = items.FindIndex(x => x != null && x.name == item.name);
            Debug.Log($"인벤토리에 이미 '{item.itemName}' 아이템이 있습니다. 개수를 증가시킵니다.");
            slots[itemIdx].ItemQuantity++;
            RefreshSlots();
            return true;
        }

        // 빈 슬롯 찾기
        int idx = items.FindIndex(x => x == null);
        if (idx != -1)
        {
            items[idx] = item;
            slots[idx].ItemQuantity = 1;
            RefreshSlots();
            Debug.Log($"인벤토리에 '{item.itemName}' 아이템이 추가되었습니다.");
            return true;
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다. 아이템을 추가할 수 없습니다.");
            return false;
        }
    }

    /// <summary>
    /// 아이템을 인벤토리에서 제거합니다 (인덱스).
    /// </summary>
    /// <param name="idx">슬롯 인덱스</param>
    public void RemoveItem(int idx)
    {
        if (idx < 0 || idx >= items.Count)
        {
            Debug.LogWarning($"Inventory: 잘못된 인덱스 {idx}");
            return;
        }

            items[idx] = null;
        if (slots != null && idx < slots.Length)
            slots[idx].ItemQuantity = 0;
        
        RefreshSlots();
        Debug.Log($"인벤토리 {idx + 1}번째 아이템이 제거되었습니다.");
    }

    /// <summary>
    /// 아이템을 인벤토리에서 제거합니다 (아이템 오브젝트).
    /// </summary>
    /// <param name="item">제거할 아이템</param>
    public void RemoveItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Inventory: null 아이템은 제거할 수 없습니다.");
            return;
        }

        int idx = items.FindIndex(x => x == item);
        if (idx != -1)
        {
            items[idx] = null;
            if (slots != null && idx < slots.Length)
                slots[idx].ItemQuantity = 0;
            
            RefreshSlots();
            Debug.Log($"인벤토리에서 '{item.itemName}' 아이템이 제거되었습니다.");
        }
        else
        {
            Debug.LogWarning("RemoveItem: 해당 아이템이 인벤토리에 없습니다.");
        }
    }

    /// <summary>
    /// 두 슬롯의 아이템을 교환합니다.
    /// </summary>
    /// <param name="fromIdx">교환할 슬롯 인덱스 1</param>
    /// <param name="toIdx">교환할 슬롯 인덱스 2</param>
    public void SwapItems(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= items.Count || toIdx < 0 || toIdx >= items.Count)
        {
            Debug.LogWarning("Inventory: 잘못된 인덱스입니다.");
            return;
        }

        var tempItem = items[fromIdx];
        var tempQuantity = 0u;
        
        if (slots != null && fromIdx < slots.Length)
        {
            tempQuantity = slots[fromIdx].ItemQuantity;
        }

        items[fromIdx] = items[toIdx];
        items[toIdx] = tempItem;

        if (slots != null)
        {
            if (fromIdx < slots.Length)
                slots[fromIdx].ItemQuantity = toIdx < slots.Length ? slots[toIdx].ItemQuantity : 0;
            if (toIdx < slots.Length)
                slots[toIdx].ItemQuantity = tempQuantity;
        }

        RefreshSlots();
        Debug.Log($"슬롯 {fromIdx + 1}과 {toIdx + 1}의 아이템을 교환했습니다.");
    }

    /// <summary>
    /// 특정 인덱스의 아이템을 반환합니다.
    /// </summary>
    /// <param name="idx">슬롯 인덱스</param>
    /// <returns>아이템 또는 null</returns>
    public Item GetItem(int idx)
    {
        if (idx < 0 || idx >= items.Count)
            return null;
        
        return items[idx];
    }

    /// <summary>
    /// 인벤토리에 빈 공간이 있는지 확인합니다.
    /// </summary>
    /// <returns>빈 공간이 있으면 true</returns>
    public bool HasSpace()
    {
        return items.FindIndex(x => x == null) != -1;
    }

    /// <summary>
    /// 특정 아이템을 보유하고 있는지 확인합니다.
    /// </summary>
    /// <param name="item">확인할 아이템</param>
    /// <returns>보유하고 있으면 true</returns>
    public bool HasItem(Item item)
    {
        if (item == null) return false;
        return items.Contains(item);
    }

    /// <summary>
    /// 특정 아이템의 수량을 반환합니다.
    /// </summary>
    /// <param name="item">수량을 확인할 아이템</param>
    /// <returns>아이템 수량 (아이템이 없으면 0)</returns>
    public uint GetItemQuantity(Item item)
    {
        if (item == null) return 0;
        
        int idx = items.FindIndex(x => x == item);
        if (idx != -1 && slots != null && idx < slots.Length)
        {
            return slots[idx].ItemQuantity;
        }
        return 0;
    }

    /// <summary>
    /// 특정 아이템의 수량을 1 감소시킵니다. 수량이 0이 되면 아이템을 제거합니다.
    /// </summary>
    /// <param name="item">수량을 감소시킬 아이템</param>
    public void DecreaseItemQuantity(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Inventory: null 아이템의 수량을 감소시킬 수 없습니다.");
            return;
        }

        int idx = items.FindIndex(x => x == item);
        if (idx != -1)
        {
            bool itemRemoved = false;
            
            // 수량 감소
            if (slots != null && idx < slots.Length)
            {
                if (slots[idx].ItemQuantity > 0)
                {
                    uint oldQuantity = slots[idx].ItemQuantity;
                    slots[idx].ItemQuantity--;
                    Debug.Log($"인벤토리 '{item.itemName}' 수량 감소: {oldQuantity} -> {slots[idx].ItemQuantity}");
                    
                    // 수량이 0이 되면 아이템 제거
                    if (slots[idx].ItemQuantity == 0)
                    {
                        items[idx] = null;
                        itemRemoved = true;
                        Debug.Log($"인벤토리 '{item.itemName}' 수량이 0이 되어 제거되었습니다.");
                    }
                }
            }
            else
            {
                // 슬롯 정보가 없으면 직접 제거
                items[idx] = null;
                itemRemoved = true;
            }
            
            RefreshSlots();
            
            // 퀵슬롯도 갱신
            QuickSlot_Controller quickSlots = FindObjectOfType<QuickSlot_Controller>();
            if (quickSlots != null)
            {
                // 아이템이 제거된 경우 퀵슬롯에서도 제거
                if (itemRemoved)
                {
                    quickSlots.RemoveItem(item);
                }
                else
                {
                    // 수량만 변경된 경우 수량 동기화
                    quickSlots.RefreshSlots();
                }
            }
        }
        else
        {
            Debug.LogWarning($"DecreaseItemQuantity: '{item.itemName}' 아이템이 인벤토리에 없습니다.");
        }
    }

    #endregion

    #region Inventory Size Management

    /// <summary>
    /// 인벤토리 크기를 변경합니다.
    /// </summary>
    /// <param name="newSize">새로운 인벤토리 크기 (최소 1)</param>
    /// <returns>변경 성공 여부</returns>
    public bool SetInventorySize(int newSize)
    {
        if (newSize < 1)
        {
            Debug.LogWarning($"Inventory: 인벤토리 크기는 최소 1이어야 합니다. (요청: {newSize})");
            return false;
        }
        
        int oldSize = inventorySize;
        InvenSize = newSize;
        
        // 슬롯 재초기화
        InitializeSlots();
        
        // UI 새로고침
        RefreshSlots();
        
        Debug.Log($"Inventory: 인벤토리 크기가 {oldSize}에서 {inventorySize}로 변경되었습니다.");
        
        // 인벤토리 크기 변경 이벤트 호출
        OnInventorySizeChanged?.Invoke(oldSize, inventorySize);
        
        return true;
    }
    
    /// <summary>
    /// 인벤토리 크기를 증가시킵니다.
    /// </summary>
    /// <param name="amount">증가할 크기 (기본 1)</param>
    /// <returns>변경 성공 여부</returns>
    public bool IncreaseInventorySize(int amount = 1)
    {
        return SetInventorySize(inventorySize + amount);
    }
    
    /// <summary>
    /// 인벤토리 크기를 감소시킵니다.
    /// 주의: 아이템이 있는 슬롯은 최대한 보존합니다.
    /// </summary>
    /// <param name="amount">감소할 크기 (기본 1)</param>
    /// <returns>변경 성공 여부</returns>
    public bool DecreaseInventorySize(int amount = 1)
    {
        int newSize = inventorySize - amount;
        
        // 아이템이 있는 슬롯 개수 확인
        int occupiedSlots = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
                occupiedSlots++;
        }
        
        if (newSize < occupiedSlots)
        {
            Debug.LogWarning($"Inventory: 인벤토리 크기를 {newSize}로 줄일 수 없습니다. ({occupiedSlots}개의 슬롯에 아이템이 있음)");
            return false;
        }
        
        return SetInventorySize(newSize);
    }
    
    /// <summary>
    /// 인벤토리 크기 변경 이벤트
    /// </summary>
    public System.Action<int, int> OnInventorySizeChanged;

    #endregion

    #region Debug

    /// <summary>
    /// 디버그: 인벤토리 아이템 목록을 출력합니다.
    /// </summary>
    public void PrintItems()
    {
        Debug.Log("=== Inventory Items ===");
        string logs = "";
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            uint quantity = 0;
            if (slots != null && i < slots.Length)
                quantity = slots[i].ItemQuantity;

            if (item != null)
            {
                logs += $"Slot {i}: {item.itemName} (x{quantity})\n";
            }
            else
            {
                logs += $"Slot {i}: (empty)\n";
            }
        }
        Debug.Log(logs + "=======================");
    }

    #endregion
}
