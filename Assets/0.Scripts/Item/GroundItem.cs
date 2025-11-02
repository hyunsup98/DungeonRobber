using UnityEngine;

/// <summary>
/// 맵에 떨어져 있는 아이템 오브젝트
/// </summary>
public class GroundItem : MonoBehaviour
{
    [Header("아이템 데이터")]
    public Item item;

    [Header("설정")]
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private bool autoRotate = true;

    private void Start()
    {
        if (item == null)
        {
            Debug.LogWarning("GroundItem: 아이템 데이터가 설정되지 않았습니다.");
        }
    }

    private void Update()
    {
        // 회전 애니메이션
        if (autoRotate)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
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

