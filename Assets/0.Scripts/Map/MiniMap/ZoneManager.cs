using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//존 상태를 관리하는 매니저
public class ZoneManager : MonoBehaviour
{
    //존 상태
    public enum ZoneState
    {
        Normal,
        Warning,
        Locked
    }

    //개별 존 데이터
    [Serializable]
    public class Zone
    {
        //존 이름
        public string zoneName;

        //씬에 배치된 트랜스폼
        public Transform areaRootObject;

        //경계로 자동 크기 계산
        public bool autoSizeFromBounds = true;

        //존 크기(XZ)
        public Vector2 areaSizeXZ = new Vector2(10f, 10f);

        //이펙트 크기(XZ)
        public Vector2 effectSizeXZ = new Vector2(10f, 10f);

        //이펙트 높이 오프셋
        public float effectHeightOffset = 0f;

        //이펙트 두께(Y)
        [Min(0.01f)]
        public float effectThicknessY = 1.0f;

        //상태와 이펙트 인스턴스
        [HideInInspector] public ZoneState state = ZoneState.Normal;
        [HideInInspector] public GameObject lockedEffectInstance;
    }

    //존 리스트
    [Header("Zone Settings")]
    [SerializeField] private List<Zone> zones = new List<Zone>();

    //시간 관련
    [Header("Timing")]
    [SerializeField] private float timeBetweenLocks = 30f;
    [SerializeField] private float warningDuration = 60f;

    //잠금 이펙트 프리팹
    [Header("Locked Effect")]
    [SerializeField] private GameObject lockedEffectPrefab;

    //바닥 감지용 마스크
    [Header("Floor Raycast")]
    [SerializeField] private LayerMask floorMask = ~0;

    //메시지 색상
    [Header("Message Colors")]
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color lockedColor = new Color(1f, 0.35f, 0.35f, 1f);
    [SerializeField] private Color infoColor = Color.white;

    //이벤트
    public event Action OnZonesUpdated;
    public event Action<string, Color, float> OnMessageRequested;

    //런타임 변수
    private Coroutine warningCoroutine;
    private Zone currentWarningTarget;

    //읽기 전용 리스트
    public IReadOnlyList<Zone> Zones
    {
        get { return zones; }
    }

    //초기화
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
                Debug.LogWarning("//ZoneManager areaRootObject가 비어 있음: " + z.zoneName);
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

    //시작 시 스케줄 시작
    private void Start()
    {
        StartCoroutine(LockSchedulerLoop());
        NotifyZonesUpdated();
    }

    //비활성화 시 코루틴 정지
    private void OnDisable()
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }
    }

    //일정 시간마다 잠금 스케줄 실행
    private IEnumerator LockSchedulerLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(timeBetweenLocks);

        while (true)
        {
            if (AllLocked() == true)
            {
                RequestMessage("모든 존이 잠겼습니다.", infoColor, 3f);
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
            RequestMessage(target.zoneName + " 존이 곧 잠깁니다.", warningColor, 10f);

            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
            }

            warningCoroutine = StartCoroutine(WarningThenLock(target));
        }
    }

    //경고 후 잠금
    private IEnumerator WarningThenLock(Zone z)
    {
        yield return new WaitForSeconds(warningDuration);

        if (z.state == ZoneState.Warning)
        {
            SetZoneState(z, ZoneState.Locked);
            SpawnLockedEffect(z);
            RequestMessage(z.zoneName + " 존이 잠겼습니다.", lockedColor, 10f);

            currentWarningTarget = null;
            warningCoroutine = null;
        }
    }

    //잠금 이펙트 생성
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

    //이펙트 크기 제한
    private Vector2 ClampSizeToRect(Vector2 desired, Vector2 rectSize)
    {
        float w = Mathf.Clamp(desired.x, 0.1f, rectSize.x);
        float h = Mathf.Clamp(desired.y, 0.1f, rectSize.y);
        return new Vector2(w, h);
    }

    //이펙트 중심 제한
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

    //씬 오브젝트 크기 계산
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

    //상태 변경 및 갱신 이벤트
    private void SetZoneState(Zone z, ZoneState next)
    {
        if (z.state == next)
        {
            return;
        }

        z.state = next;
        NotifyZonesUpdated();
    }

    //모두 잠금 상태인지 확인
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

    //랜덤 노멀 존 선택
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

    //메시지 요청
    private void RequestMessage(string msg, Color color, float time)
    {
        if (OnMessageRequested != null)
        {
            OnMessageRequested(msg, color, time);
        }
    }

    //존 갱신 알림
    private void NotifyZonesUpdated()
    {
        if (OnZonesUpdated != null)
        {
            OnZonesUpdated();
        }
    }
}
