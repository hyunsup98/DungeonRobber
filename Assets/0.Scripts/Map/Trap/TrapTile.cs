using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrapTile : MonoBehaviour
{
    // 같은 씬의 풀
    [SerializeField] private TrapPoolRuntime pool;

    // 트랩 프리팹
    [SerializeField] private Trap trapPrefab;

    // 생성 부모
    [SerializeField] private Transform spawnParent;

    // 쿨다운
    [SerializeField] private float localCooldown = 0.8f;

    // 1회용 여부
    [SerializeField] private bool oneShot = false;

    // 발동 가능 여부
    private bool canTrigger = true;

    // 초기 설정 및 같은 씬의 풀 탐색
    void Awake()
    {
        if (pool == null)
        {
            TrapPoolRuntime[] pools = FindObjectsOfType<TrapPoolRuntime>(true);
            foreach (var p in pools)
            {
                if (p.gameObject.scene == gameObject.scene)
                {
                    pool = p;
                    break;
                }
            }
        }

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    // 플레이어가 트리거에 들어왔을 때
    void OnTriggerEnter(Collider other)
    {
        if (canTrigger == false)
        {
            return;
        }

        if (pool == null)
        {
            return;
        }

        if (other.CompareTag("Player") == false)
        {
            return;
        }

        if (trapPrefab == null)
        {
            Debug.LogWarning("[TrapTile] trapPrefab이 지정되지 않았습니다.");
            return;
        }

        Transform parent = spawnParent != null ? spawnParent : pool.transform;

        Trap trap = pool.GetObjects(trapPrefab, parent);
        trap.transform.SetParent(parent, false);
        trap.Activate(transform.position, transform.rotation);

        if (oneShot == true)
        {
            canTrigger = false;
            enabled = false;
        }
        else
        {
            StartCoroutine(CoCooldown());
        }
    }

    // 쿨다운 처리
    IEnumerator CoCooldown()
    {
        canTrigger = false;
        yield return new WaitForSeconds(localCooldown);
        canTrigger = true;
    }
}
