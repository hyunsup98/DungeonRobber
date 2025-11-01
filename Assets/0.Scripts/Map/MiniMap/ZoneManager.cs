using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�� ���¸� �����ϴ� �Ŵ���
public class ZoneManager : MonoBehaviour
{
    //�� ����
    public enum ZoneState
    {
        Normal,
        Warning,
        Locked
    }

    //���� �� ������
    [Serializable]
    public class Zone
    {
        //�� �̸�
        public string zoneName;

        //���� ��ġ�� Ʈ������
        public Transform areaRootObject;

        //���� �ڵ� ũ�� ���
        public bool autoSizeFromBounds = true;

        //�� ũ��(XZ)
        public Vector2 areaSizeXZ = new Vector2(10f, 10f);

        //����Ʈ ũ��(XZ)
        public Vector2 effectSizeXZ = new Vector2(10f, 10f);

        //����Ʈ ���� ������
        public float effectHeightOffset = 0f;

        //����Ʈ �β�(Y)
        [Min(0.01f)]
        public float effectThicknessY = 1.0f;

        //���¿� ����Ʈ �ν��Ͻ�
        [HideInInspector] public ZoneState state = ZoneState.Normal;
        [HideInInspector] public GameObject lockedEffectInstance;
    }

    //�� ����Ʈ
    [Header("Zone Settings")]
    [SerializeField] private List<Zone> zones = new List<Zone>();

    //�ð� ����
    [Header("Timing")]
    [SerializeField] private float timeBetweenLocks = 30f;
    [SerializeField] private float warningDuration = 60f;

    //��� ����Ʈ ������
    [Header("Locked Effect")]
    [SerializeField] private GameObject lockedEffectPrefab;

    //�ٴ� ������ ����ũ
    [Header("Floor Raycast")]
    [SerializeField] private LayerMask floorMask = ~0;

    //�޽��� ����
    [Header("Message Colors")]
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color lockedColor = new Color(1f, 0.35f, 0.35f, 1f);
    [SerializeField] private Color infoColor = Color.white;

    //�̺�Ʈ
    public event Action OnZonesUpdated;
    public event Action<string, Color, float> OnMessageRequested;

    //��Ÿ�� ����
    private Coroutine warningCoroutine;
    private Zone currentWarningTarget;

    //�б� ���� ����Ʈ
    public IReadOnlyList<Zone> Zones
    {
        get { return zones; }
    }

    //�ʱ�ȭ
    private void Awake()
    {
        for (int i = 0; i < zones.Count; i++)
        {
            Zone z = zones[i];
            if (z == null)
            {
                continue;
            }

            if (z.areaRootObject == null)
            {
                Debug.LogWarning("//ZoneManager areaRootObject�� ��� ����: " + z.zoneName);
                continue;
            }

            if (z.autoSizeFromBounds == true)
            {
                Vector2 size = GetXZSizeFromObject(z.areaRootObject.gameObject);
                if (size.x > 0.001f || size.y > 0.001f)
                {
                    z.areaSizeXZ = size;
                }
            }

            z.state = ZoneState.Normal;
        }
    }

    //���� �� ������ ����
    private void Start()
    {
        StartCoroutine(LockSchedulerLoop());
        NotifyZonesUpdated();
    }

    //��Ȱ��ȭ �� �ڷ�ƾ ����
    private void OnDisable()
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }
    }

    //���� �ð����� ��� ������ ����
    private IEnumerator LockSchedulerLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(timeBetweenLocks);

        while (true)
        {
            if (AllLocked() == true)
            {
                RequestMessage("��� ���� �����ϴ�.", infoColor, 3f);
                yield break;
            }

            yield return wait;

            Zone target = GetRandomNormalZone();
            if (target == null)
            {
                continue;
            }

            currentWarningTarget = target;
            SetZoneState(target, ZoneState.Warning);
            RequestMessage(target.zoneName + " ���� �� ���ϴ�.", warningColor, 10f);

            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
            }

            warningCoroutine = StartCoroutine(WarningThenLock(target));
        }
    }

    //��� �� ���
    private IEnumerator WarningThenLock(Zone z)
    {
        yield return new WaitForSeconds(warningDuration);

        if (z.state == ZoneState.Warning)
        {
            SetZoneState(z, ZoneState.Locked);
            SpawnLockedEffect(z);
            RequestMessage(z.zoneName + " ���� �����ϴ�.", lockedColor, 10f);

            currentWarningTarget = null;
            warningCoroutine = null;
        }
    }

    //��� ����Ʈ ����
    private void SpawnLockedEffect(Zone z)
    {
        if (lockedEffectPrefab == null)
        {
            return;
        }

        if (z.lockedEffectInstance != null)
        {
            Destroy(z.lockedEffectInstance);
        }

        Vector3 center = z.areaRootObject.position;
        Vector3 spawnBase = center;

        RaycastHit hit;
        bool hitFloor = Physics.Raycast(center + Vector3.up * 100f, Vector3.down, out hit, 1000f, floorMask, QueryTriggerInteraction.Ignore);
        if (hitFloor == true)
        {
            spawnBase = hit.point;
        }

        Vector2 areaSize = z.areaSizeXZ;
        Vector2 desired = z.effectSizeXZ;
        Vector2 clampedSize = ClampSizeToRect(desired, areaSize);
        Vector3 clampedPos = ClampCenterToRect(spawnBase, center, clampedSize, areaSize);

        GameObject go = Instantiate(lockedEffectPrefab, clampedPos + Vector3.up * z.effectHeightOffset, Quaternion.identity);

        go.transform.localScale = new Vector3(clampedSize.x, z.effectThicknessY, clampedSize.y);

        z.lockedEffectInstance = go;
    }

    //����Ʈ ũ�� ����
    private Vector2 ClampSizeToRect(Vector2 desired, Vector2 rectSize)
    {
        float w = Mathf.Clamp(desired.x, 0.1f, rectSize.x);
        float h = Mathf.Clamp(desired.y, 0.1f, rectSize.y);
        return new Vector2(w, h);
    }

    //����Ʈ �߽� ����
    private Vector3 ClampCenterToRect(Vector3 spawnBase, Vector3 rectCenter, Vector2 effSize, Vector2 rectSize)
    {
        float halfRectX = rectSize.x * 0.5f;
        float halfRectZ = rectSize.y * 0.5f;
        float halfEffX = effSize.x * 0.5f;
        float halfEffZ = effSize.y * 0.5f;

        float minX = rectCenter.x - (halfRectX - halfEffX);
        float maxX = rectCenter.x + (halfRectX - halfEffX);
        float minZ = rectCenter.z - (halfRectZ - halfEffZ);
        float maxZ = rectCenter.z + (halfRectZ - halfEffZ);

        float cx = Mathf.Clamp(spawnBase.x, minX, maxX);
        float cz = Mathf.Clamp(spawnBase.z, minZ, maxZ);

        return new Vector3(cx, spawnBase.y, cz);
    }

    //�� ������Ʈ ũ�� ���
    private Vector2 GetXZSizeFromObject(GameObject go)
    {
        if (go == null)
        {
            return Vector2.zero;
        }

        Collider col = go.GetComponentInChildren<Collider>();
        if (col != null)
        {
            Bounds b = col.bounds;
            return new Vector2(b.size.x, b.size.z);
        }

        Renderer r = go.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            Bounds b = r.bounds;
            return new Vector2(b.size.x, b.size.z);
        }

        return Vector2.zero;
    }

    //���� ���� �� ���� �̺�Ʈ
    private void SetZoneState(Zone z, ZoneState next)
    {
        if (z.state == next)
        {
            return;
        }

        z.state = next;
        NotifyZonesUpdated();
    }

    //��� ��� �������� Ȯ��
    private bool AllLocked()
    {
        for (int i = 0; i < zones.Count; i++)
        {
            Zone z = zones[i];
            if (z != null)
            {
                if (z.state != ZoneState.Locked)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //���� ��� �� ����
    private Zone GetRandomNormalZone()
    {
        List<Zone> list = new List<Zone>();

        for (int i = 0; i < zones.Count; i++)
        {
            Zone z = zones[i];
            if (z != null)
            {
                if (z.state == ZoneState.Normal)
                {
                    list.Add(z);
                }
            }
        }

        if (list.Count == 0)
        {
            return null;
        }

        int idx = UnityEngine.Random.Range(0, list.Count);
        return list[idx];
    }

    //�޽��� ��û
    private void RequestMessage(string msg, Color color, float time)
    {
        if (OnMessageRequested != null)
        {
            OnMessageRequested(msg, color, time);
        }
    }

    //�� ���� �˸�
    private void NotifyZonesUpdated()
    {
        if (OnZonesUpdated != null)
        {
            OnZonesUpdated();
        }
    }
}
