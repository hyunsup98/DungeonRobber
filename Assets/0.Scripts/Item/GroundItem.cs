using UnityEngine;

/// <summary>
/// 맵에 떨어져 있는 아이템 오브젝트
/// </summary>
public class GroundItem : MonoBehaviour
{
    [Header("아이템 데이터")]
    public Item item;

    [Header("설정")]
    [SerializeField] private float pickupRange = 5f; // 줍기 가능 거리
    public float PickupRange => pickupRange; // 외부 접근을 위해 프로퍼티 추가
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private bool autoRotate = true;

    [Header("마우스 오버 감지")]
    [SerializeField] private LayerMask raycastLayerMask = -1; // 모든 레이어
    [SerializeField] private float raycastDistance = 100f;

    private Camera mainCamera;
    private Collider itemCollider;
    private bool isMouseOver = false;

    private void Start()
    {
        if (item == null)
        {
            Debug.LogWarning("GroundItem: 아이템 데이터가 설정되지 않았습니다.");
        }

        // 메인 카메라 찾기
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        // Collider 찾기
        itemCollider = GetComponent<Collider>();
        if (itemCollider == null)
        {
            itemCollider = GetComponentInChildren<Collider>();
        }
    }

    private void Update()
    {
        // 회전 애니메이션
        if (autoRotate)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

        // 마우스 오버 감지
        CheckMouseOver();
    }

    /// <summary>
    /// 마우스 오버를 감지하여 툴팁을 표시합니다.
    /// </summary>
    private void CheckMouseOver()
    {
        if (item == null || mainCamera == null || itemCollider == null)
            return;

        // 레이캐스트로 마우스가 이 아이템을 가리키는지 확인
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool currentlyMouseOver = false;

        if (Physics.Raycast(ray, out hit, raycastDistance, raycastLayerMask))
        {
            // 레이캐스트가 이 오브젝트나 자식 오브젝트를 맞췄는지 확인
            if (hit.collider == itemCollider || hit.collider.transform.IsChildOf(transform) || hit.collider.transform == transform)
            {
                currentlyMouseOver = true;
            }
        }

        // 상태가 변경되었을 때만 툴팁 표시/숨김
        if (currentlyMouseOver != isMouseOver)
        {
            isMouseOver = currentlyMouseOver;
            
            ItemTooltip tooltip = ItemTooltip.GetOrFind();
            if (tooltip != null)
            {
                if (isMouseOver)
                {
                    // 마우스 위치로 툴팁 표시
                    tooltip.Show(item, Input.mousePosition);
                }
                else
                {
                    tooltip.Hide();
                }
            }
        }
        
        // 마우스 오버 중일 때는 마우스 위치로 툴팁을 계속 업데이트
        if (isMouseOver)
        {
            ItemTooltip tooltip = ItemTooltip.GetOrFind();
            if (tooltip != null)
            {
                // 마우스 위치를 실시간으로 업데이트
                tooltip.UpdatePosition(Input.mousePosition);
            }
        }
    }

    /// <summary>
    /// 플레이어가 이 아이템을 줍을 수 있는지 확인
    /// </summary>
    public bool CanPickup(Vector3 playerPosition)
    {
        float distance = Vector3.Distance(transform.position, playerPosition);
        return distance <= pickupRange;
    }

    /// <summary>
    /// 아이템을 줍기
    /// </summary>
    public Item Pickup()
    {
        if (item == null)
            return null;

        Item pickedItem = item;
        
        // 아이템을 줍은 후 오브젝트 비활성화 또는 삭제
        gameObject.SetActive(false);
        
        return pickedItem;
    }

    // 디버그용 기즈모 그리기
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}

