using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//미니맵 컨트롤러: 마커/플레이어아이콘/안내문구 제어
public class MiniMapController : MonoBehaviour
{
    //존 매니저 참조
    [Header("References")]
    [SerializeField] private ZoneManager _zoneManager;

    //플레이어 트랜스폼
    [SerializeField] private Transform _playerTransform;

    //MiniMapCanvasHooks가 포함된 캔버스 프리팹
    [Header("Canvas(Hooks Prefab)")]
    [SerializeField] private GameObject _minimapCanvasPrefab;

    //월드 XZ를 배경 Rect로 변환하기 위한 범위
    [Header("World To Rect Mapping")]
    [SerializeField] private Vector2 worldMin = new Vector2(-50f, -50f);
    [SerializeField] private Vector2 worldMax = new Vector2(50f, 50f);


    //존별 스프라이트 및 오버라이드 UI
    [System.Serializable]
    public class ZoneSpriteSet
    {
        //존 이름(ZoneManager의 zoneName과 일치)
        public string zoneName;

        //상태 스프라이트
        public Sprite normal;
        public Sprite warning;
        public Sprite locked;

        //오버라이드 루트
        public RectTransform overrideRoot;

        //오버라이드 상태 아이콘
        public Image overrideStateIcon;

        //오버라이드 라벨
        public TMP_Text overrideLabel;
    }


    //존별 스프라이트 세트
    [Header("Per Zone Sprites")]
    [SerializeField] private List<ZoneSpriteSet> zoneSpriteSets = new List<ZoneSpriteSet>();


    //플레이어 아이콘 보간 속도
    [Header("Player Icon")]
    [SerializeField] private float playerFollowSpeed = 10f;


    //경고 상태 깜빡임 설정
    [Header("Warning Blink")]
    [SerializeField] private float blinkInterval = 0.5f;
    [SerializeField] private float warningDimAlpha = 0.35f;


    //런타임 훅
    private GameObject panelGO;
    private RectTransform backgroundRT;
    private Transform markersParent;
    private RectTransform playerIcon;
    private TMP_Text messageText;


    //안내문구 숨김 코루틴
    private Coroutine hideMessageRoutine;


    //미니맵 열림 상태
    private bool isOpen;


    //마커 UI 보관 구조체
    private class MarkerUI
    {
        public Image img;
        public Coroutine blinker;
        public Sprite normal;
        public Sprite warning;
        public Sprite locked;
        public bool isOverride;
        public TMP_Text label;
    }
    

    //존 이름 > 마커 UI 매핑
    private readonly Dictionary<string, MarkerUI> markers = new Dictionary<string, MarkerUI>();


    //활성화 시 초기화
    private void OnEnable()
    {
        InstantiateCanvasIfNeeded();
        Subscribe(true);
        SyncMarkersToZones();
        RefreshAllStates();
        ToggleOpen(false);
        if(Player_Controller.Instance != null)
        {
            _playerTransform = Player_Controller.Instance.transform;
        }
    }


    //비활성화 시 정리
    private void OnDisable()
    {
        Subscribe(false);
        StopAllBlinkers();

        if (hideMessageRoutine != null)
        {
            StopCoroutine(hideMessageRoutine);
            hideMessageRoutine = null;
        }
    }


    //입력 처리 및 프레임 갱신
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) == true)
        {
            bool next = true;
            if (isOpen == true)
            {
                next = false;
            }
            ToggleOpen(next);
        }

        if (isOpen == true)
        {
            UpdatePlayerIconPosition();
        }

        UpdateAllMarkerPositions();
    }



    //이벤트 구독/해제
    private void Subscribe(bool onFlag)
    {
        if (_zoneManager == null)
        {
            return;
        }

        if (onFlag == true)
        {
            _zoneManager.OnZonesUpdated += OnZonesUpdated;
            _zoneManager.OnMessageRequested += HandleMessage;
        }
        else
        {
            _zoneManager.OnZonesUpdated -= OnZonesUpdated;
            _zoneManager.OnMessageRequested -= HandleMessage;
        }
    }



    //존 갱신 이벤트
    private void OnZonesUpdated()
    {
        SyncMarkersToZones();
        RefreshAllStates();
    }



    //안내문구를 패널 밖으로 분리
    private void EnsureMessageDetached()
    {
        if (panelGO == null)
        {
            return;
        }
        if (messageText == null)
        {
            return;
        }

        Transform panelParent = panelGO.transform.parent;
        if (panelParent != null)
        {
            if (messageText.transform.IsChildOf(panelGO.transform) == true)
            {
                messageText.transform.SetParent(panelParent, false);
            }
        }
    }



    //캔버스 인스턴스 생성 또는 씬에서 찾기
    private void InstantiateCanvasIfNeeded()
    {
        if (panelGO != null)
        {
            return;
        }

        MiniMapCanvasHooks hooks = Object.FindObjectOfType<MiniMapCanvasHooks>(true);

        if (hooks == null)
        {
            if (_minimapCanvasPrefab == null)
            {
                Debug.LogError("//MiniMap _minimapCanvasPrefab is not set");
                return;
            }

            GameObject instance = Instantiate(_minimapCanvasPrefab);
            hooks = instance.GetComponentInChildren<MiniMapCanvasHooks>(true);

            if (hooks == null)
            {
                Debug.LogError("//MiniMap MiniMapCanvasHooks not found in prefab");
                return;
            }
        }

        panelGO = hooks._miniMapPanel;
        backgroundRT = hooks._miniMapBackground;
        markersParent = hooks._miniMapMarkersParent;
        playerIcon = hooks._miniMapPlayerIcon;
        messageText = hooks._miniMapMessageText;

        if (panelGO != null)
        {
            panelGO.SetActive(false);
        }
        if (playerIcon != null)
        {
            playerIcon.gameObject.SetActive(false);
        }
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        EnsureMessageDetached();
    }



    //존 목록과 마커 동기화
    private void SyncMarkersToZones()
    {
        if (_zoneManager == null)
        {
            return;
        }

        HashSet<string> shouldExist = new HashSet<string>();
        IReadOnlyList<ZoneManager.Zone> zones = _zoneManager.Zones;

        for (int i = 0; i < zones.Count; i++)
        {
            ZoneManager.Zone z = zones[i];
            if (z == null)
            {
                continue;
            }

            shouldExist.Add(z.zoneName);

            if (markers.ContainsKey(z.zoneName) == true)
            {
                continue;
            }

            ZoneSpriteSet set = FindSpriteSet(z.zoneName);
            if (ValidateSet(set) == false)
            {
                Debug.LogError("//MiniMap ZoneSpriteSet missing or invalid: " + z.zoneName);
            }
            else
            {
                Image img = null;
                bool useOverride = false;
                TMP_Text label = null;

                if (set.overrideRoot != null)
                {
                    useOverride = true;

                    if (set.overrideStateIcon != null)
                    {
                        img = set.overrideStateIcon;
                    }
                    else
                    {
                        img = set.overrideRoot.GetComponentInChildren<Image>(true);
                    }

                    if (set.overrideLabel != null)
                    {
                        label = set.overrideLabel;
                    }
                    else
                    {
                        label = set.overrideRoot.GetComponentInChildren<TMP_Text>(true);
                    }

                    if (label != null)
                    {
                        if (string.IsNullOrEmpty(label.text) == true)
                        {
                            label.text = set.zoneName;
                        }
                    }
                }
                else
                {
                    img = CreateMarkerImage("Marker_" + z.zoneName);
                }

                if (img != null)
                {
                    MarkerUI ui = new MarkerUI();
                    ui.img = img;
                    ui.isOverride = useOverride;
                    ui.normal = set.normal;
                    ui.warning = set.warning;
                    ui.locked = set.locked;
                    ui.label = label;

                    if (ui.img.sprite == null)
                    {
                        ui.img.sprite = ui.normal;
                    }

                    ui.img.color = Color.white;
                    markers.Add(z.zoneName, ui);

                    if (useOverride == false)
                    {
                        RectTransform rt = ui.img.rectTransform;
                        if (rt != null)
                        {
                            rt.anchoredPosition = WorldToMini(GetZoneWorldPos(z));
                        }
                    }
                }
            }
        }

        List<string> toRemove = new List<string>();
        foreach (KeyValuePair<string, MarkerUI> kv in markers)
        {
            string key = kv.Key;
            if (shouldExist.Contains(key) == false)
            {
                toRemove.Add(key);
            }
        }

        for (int j = 0; j < toRemove.Count; j++)
        {
            string name = toRemove[j];
            MarkerUI ui = markers[name];

            if (ui != null)
            {
                if (ui.blinker != null)
                {
                    StopCoroutine(ui.blinker);
                }

                if (ui.isOverride == false)
                {
                    if (ui.img != null)
                    {
                        GameObject go = ui.img.gameObject;
                        Destroy(go);
                    }
                }
            }

            markers.Remove(name);
        }
    }



    //스프라이트 세트 유효성 검사
    private bool ValidateSet(ZoneSpriteSet s)
    {
        if (s == null)
        {
            return false;
        }
        if (s.normal == null)
        {
            return false;
        }
        if (s.warning == null)
        {
            return false;
        }
        if (s.locked == null)
        {
            return false;
        }
        return true;
    }



    //동적 마커 이미지 생성
    private Image CreateMarkerImage(string goName)
    {
        if (markersParent == null)
        {
            Debug.LogError("//MiniMap _miniMapMarkersParent is invalid");
            return null;
        }

        GameObject go = new GameObject(goName, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(markersParent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(24f, 24f);

        Image img = go.GetComponent<Image>();
        img.raycastTarget = false;
        img.preserveAspect = true;

        return img;
    }



    //모든 마커 위치 갱신
    private void UpdateAllMarkerPositions()
    {
        if (_zoneManager == null)
        {
            return;
        }

        IReadOnlyList<ZoneManager.Zone> zones = _zoneManager.Zones;

        for (int i = 0; i < zones.Count; i++)
        {
            ZoneManager.Zone z = zones[i];
            if (z == null)
            {
                continue;
            }

            MarkerUI ui;
            bool has = markers.TryGetValue(z.zoneName, out ui);
            if (has == true)
            {
                if (ui != null)
                {
                    if (ui.img != null)
                    {
                        if (ui.isOverride == false)
                        {
                            RectTransform rt = ui.img.rectTransform;
                            if (rt != null)
                            {
                                rt.anchoredPosition = WorldToMini(GetZoneWorldPos(z));
                            }
                        }
                    }
                }
            }
        }
    }



    //모든 마커 상태 갱신
    private void RefreshAllStates()
    {
        if (_zoneManager == null)
        {
            return;
        }

        IReadOnlyList<ZoneManager.Zone> zones = _zoneManager.Zones;

        for (int i = 0; i < zones.Count; i++)
        {
            ZoneManager.Zone z = zones[i];
            if (z == null)
            {
                continue;
            }

            MarkerUI ui;
            bool has = markers.TryGetValue(z.zoneName, out ui);
            if (has == true)
            {
                ApplyState(ui, z.state);
            }
        }
    }



    //상태 적용 및 깜빡임 처리
    private void ApplyState(MarkerUI ui, ZoneManager.ZoneState state)
    {
        if (ui == null)
        {
            return;
        }
        if (ui.img == null)
        {
            return;
        }

        if (state == ZoneManager.ZoneState.Normal)
        {
            ui.img.sprite = ui.normal;
            StopBlink(ui);
            ui.img.color = Color.white;
            return;
        }

        if (state == ZoneManager.ZoneState.Warning)
        {
            ui.img.sprite = ui.warning;
            StartBlink(ui);
            ui.img.color = Color.white;
            return;
        }

        if (state == ZoneManager.ZoneState.Locked)
        {
            ui.img.sprite = ui.locked;
            StopBlink(ui);
            ui.img.color = Color.white;
            return;
        }
    }



    //깜빡임 시작
    private void StartBlink(MarkerUI ui)
    {
        if (ui.blinker == null)
        {
            ui.blinker = StartCoroutine(BlinkRoutine(ui.img));
        }
    }



    //깜빡임 정지
    private void StopBlink(MarkerUI ui)
    {
        if (ui.blinker == null)
        {
            return;
        }
        StopCoroutine(ui.blinker);
        ui.blinker = null;
    }



    //알파 깜빡임 루틴
    private IEnumerator BlinkRoutine(Image img)
    {
        bool bright = true;
        WaitForSeconds wait = new WaitForSeconds(blinkInterval);

        while (true)
        {
            if (bright == true)
            {
                bright = false;
            }
            else
            {
                bright = true;
            }

            if (img != null)
            {
                Color c = img.color;
                if (bright == true)
                {
                    c.a = 1f;
                }
                else
                {
                    c.a = warningDimAlpha;
                }
                img.color = c;
            }

            yield return wait;
        }
    }



    //모든 깜빡임 정지
    private void StopAllBlinkers()
    {
        foreach (KeyValuePair<string, MarkerUI> kv in markers)
        {
            MarkerUI m = kv.Value;
            if (m != null)
            {
                if (m.blinker != null)
                {
                    StopCoroutine(m.blinker);
                }
            }
        }
    }



    //미니맵 열기/닫기(안내문구는 유지)
    private void ToggleOpen(bool openFlag)
    {
        isOpen = openFlag;

        if (panelGO != null)
        {
            panelGO.SetActive(isOpen);
        }
        if (playerIcon != null)
        {
            playerIcon.gameObject.SetActive(isOpen);
        }
    }



    //플레이어 아이콘 위치 보간
    private void UpdatePlayerIconPosition()
    {
        if (_playerTransform == null || playerIcon == null)
        {
            return;
        }

        //위치 보간 이동
        Vector2 target = WorldToMini(_playerTransform.position);
        playerIcon.anchoredPosition = Vector2.Lerp(
            playerIcon.anchoredPosition,
            target,
            Time.deltaTime * playerFollowSpeed
        );

        //회전 적용 (플레이어 Y 회전값 기준)
        float mInimapIconRotation = _playerTransform.eulerAngles.y;
        playerIcon.localRotation = Quaternion.Euler(0f, 0f, -mInimapIconRotation);
    }



    //월드 좌표를 배경 로컬 좌표로 변환
    private Vector2 WorldToMini(Vector3 world)
    {
        if (backgroundRT == null)
        {
            return Vector2.zero;
        }

        Rect rect = backgroundRT.rect;

        //비율 계산
        float normalizedX = (world.x - worldMin.x) / (worldMax.x - worldMin.x);
        float normalizedZ = (world.z - worldMin.y) / (worldMax.y - worldMin.y);

        // 중심 기준 오프셋 계산
        float localX = (normalizedX - 0.5f) * rect.width;
        float localZ = (normalizedZ - 0.5f) * rect.height;

        return new Vector2(localX, localZ);
    }



    //안내문구 처리
    private void HandleMessage(string msg, Color color, float seconds)
    {
        if (messageText == null)
        {
            return;
        }

        messageText.text = msg;
        messageText.color = color;

        if (messageText.gameObject.activeSelf == false)
        {
            messageText.gameObject.SetActive(true);
        }

        if (hideMessageRoutine != null)
        {
            StopCoroutine(hideMessageRoutine);
            hideMessageRoutine = null;
        }

        hideMessageRoutine = StartCoroutine(HideAfter(seconds));
    }



    //지연 후 안내문구 숨김
    private IEnumerator HideAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }



    //존 이름으로 스프라이트 세트 찾기
    private ZoneSpriteSet FindSpriteSet(string zoneName)
    {
        for (int i = 0; i < zoneSpriteSets.Count; i++)
        {
            ZoneSpriteSet s = zoneSpriteSets[i];
            if (s != null)
            {
                if (string.Equals(s.zoneName, zoneName) == true)
                {
                    return s;
                }
            }
        }
        return null;
    }



    //존 월드 위치 구하기
    private Vector3 GetZoneWorldPos(ZoneManager.Zone z)
    {
        if (z == null)
        {
            return Vector3.zero;
        }
        if (z.areaRootObject != null)
        {
            return z.areaRootObject.position;
        }
        return Vector3.zero;
    }
}
