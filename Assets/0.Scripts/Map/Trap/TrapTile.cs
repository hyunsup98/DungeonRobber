using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrapTile : MonoBehaviour
{
    // ���� ���� Ǯ
    [SerializeField] private TrapPoolRuntime pool;

    // Ʈ�� ������
    [SerializeField] private Trap trapPrefab;

    // ���� �θ�
    [SerializeField] private Transform spawnParent;

    // ��ٿ�
    [SerializeField] private float localCooldown = 0.8f;

    // 1ȸ�� ����
    [SerializeField] private bool oneShot = false;

    // �ߵ� ���� ����
    private bool canTrigger = true;

    // �ʱ� ���� �� ���� ���� Ǯ Ž��
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

    // �÷��̾ Ʈ���ſ� ������ ��
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
            Debug.LogWarning("[TrapTile] trapPrefab�� �������� �ʾҽ��ϴ�.");
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

    // ��ٿ� ó��
    IEnumerator CoCooldown()
    {
        canTrigger = false;
        yield return new WaitForSeconds(localCooldown);
        canTrigger = true;
    }
}
