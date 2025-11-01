using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//�̴ϸ� ��Ʈ�ѷ�: ��Ŀ/�÷��̾������/�ȳ����� ����
public class MiniMapController : MonoBehaviour
{
    //�� �Ŵ��� ����
    [Header("References")]
    [SerializeField] private ZoneManager zoneManager;

    //�÷��̾� Ʈ������
    [SerializeField] private Transform playerTransform;

    //MiniMapCanvasHooks�� ���Ե� ĵ���� ������
    [Header("Canvas(Hooks Prefab)")]
    [SerializeField] private GameObject minimapCanvasPrefab;

    //���� XZ�� ��� Rect�� ��ȯ�ϱ� ���� ����
    [Header("World To Rect Mapping")]
    [SerializeField] private Vector2 worldMin = new Vector2(-50f, -50f);
    [SerializeField] private Vector2 worldMax = new Vector2(50f, 50f);

    //���� ��������Ʈ �� �������̵� UI
    [System.Serializable]
    public class ZoneSpriteSet
    {
        //�� �̸�(ZoneManager�� zoneName�� ��ġ)
        public string zoneName;

        //���� ��������Ʈ
        public Sprite normal;
        public Sprite warning;
        public Sprite locked;

        //�������̵� ��Ʈ
        public RectTransform overrideRoot;

        //�������̵� ���� ������
        public Image overrideStateIcon;

        //�������̵� ��
        public TMP_Text overrideLabel;
    }

    //���� ��������Ʈ ��Ʈ
    [Header("Per Zone Sprites")]
    [SerializeField] private List<ZoneSpriteSet> zoneSpriteSets = new List<ZoneSpriteSet>();

    //�÷��̾� ������ ���� �ӵ�
    [Header("Player Icon")]
    [SerializeField] private float playerFollowSpeed = 10f;

    //��� ���� ������ ����
    [Header("Warning Blink")]
    [SerializeField] private float blinkInterval = 0.5f;
    [SerializeField] private float warningDimAlpha = 0.35f;

    //��Ÿ�� ��
    private GameObject panelGO;
    private RectTransform backgroundRT;
    private Transform markersParent;
    private RectTransform playerIcon;
    private TMP_Text messageText;

    //�ȳ����� ���� �ڷ�ƾ
    private Coroutine hideMessageRoutine;

    //�̴ϸ� ���� ����
    private bool isOpen;

    //��Ŀ UI ���� ����ü
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

    //�� �̸��渶Ŀ UI ����
    private readonly Dictionary<string, MarkerUI> markers = new Dictionary<string, MarkerUI>();

    //Ȱ��ȭ �� �ʱ�ȭ
    private void OnEnable()
    {
        InstantiateCanvasIfNeeded();
        Subscribe(true);
        SyncMarkersToZones();
        RefreshAllStates();
        ToggleOpen(false);
    }

    //��Ȱ��ȭ �� ����
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

    //�Է� ó�� �� ������ ����
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

    //�̺�Ʈ ����/����
    private void Subscribe(bool onFlag)
    {
        if (zoneManager == null)
        {
            return;
        }

        if (onFlag == true)
        {
            zoneManager.OnZonesUpdated += OnZonesUpdated;
            zoneManager.OnMessageRequested += HandleMessage;
        }
        else
        {
            zoneManager.OnZonesUpdated -= OnZonesUpdated;
            zoneManager.OnMessageRequested -= HandleMessage;
        }
    }

    //�� ���� �̺�Ʈ
    private void OnZonesUpdated()
    {
        SyncMarkersToZones();
        RefreshAllStates();
    }

    //�ȳ������� �г� ������ �и�
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

    //ĵ���� �ν��Ͻ� ���� �Ǵ� ������ ã��
    private void InstantiateCanvasIfNeeded()
    {
        if (panelGO != null)
        {
            return;
        }

        MiniMapCanvasHooks hooks = Object.FindObjectOfType<MiniMapCanvasHooks>(true);

        if (hooks == null)
        {
            if (minimapCanvasPrefab == null)
            {
                Debug.LogError("//MiniMap minimapCanvasPrefab is not set");
                return;
            }

            GameObject instance = Instantiate(minimapCanvasPrefab);
            hooks = instance.GetComponentInChildren<MiniMapCanvasHooks>(true);

            if (hooks == null)
            {
                Debug.LogError("//MiniMap MiniMapCanvasHooks not found in prefab");
                return;
            }
        }

        panelGO = hooks.panel;
        backgroundRT = hooks.background;
        markersParent = hooks.markersParent;
        playerIcon = hooks.playerIcon;
        messageText = hooks.messageText;

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

    //�� ��ϰ� ��Ŀ ����ȭ
    private void SyncMarkersToZones()
    {
        if (zoneManager == null)
        {
            return;
        }

        HashSet<string> shouldExist = new HashSet<string>();
        IReadOnlyList<ZoneManager.Zone> zones = zoneManager.Zones;

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

    //��������Ʈ ��Ʈ ��ȿ�� �˻�
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

    //���� ��Ŀ �̹��� ����
    private Image CreateMarkerImage(string goName)
    {
        if (markersParent == null)
        {
            Debug.LogError("//MiniMap markersParent is invalid");
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

    //��� ��Ŀ ��ġ ����
    private void UpdateAllMarkerPositions()
    {
        if (zoneManager == null)
        {
            return;
        }

        IReadOnlyList<ZoneManager.Zone> zones = zoneManager.Zones;

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

    //��� ��Ŀ ���� ����
    private void RefreshAllStates()
    {
        if (zoneManager == null)
        {
            return;
        }

        IReadOnlyList<ZoneManager.Zone> zones = zoneManager.Zones;

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

    //���� ���� �� ������ ó��
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

    //������ ����
    private void StartBlink(MarkerUI ui)
    {
        if (ui.blinker == null)
        {
            ui.blinker = StartCoroutine(BlinkRoutine(ui.img));
        }
    }

    //������ ����
    private void StopBlink(MarkerUI ui)
    {
        if (ui.blinker == null)
        {
            return;
        }
        StopCoroutine(ui.blinker);
        ui.blinker = null;
    }

    //���� ������ ��ƾ
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

    //��� ������ ����
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

    //�̴ϸ� ����/�ݱ�(�ȳ������� ����)
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

    //�÷��̾� ������ ��ġ ����
    private void UpdatePlayerIconPosition()
    {
        if (playerTransform == null)
        {
            return;
        }
        if (playerIcon == null)
        {
            return;
        }

        Vector2 target = WorldToMini(playerTransform.position);

        playerIcon.anchoredPosition = Vector2.Lerp(
            playerIcon.anchoredPosition,
            target,
            Time.deltaTime * playerFollowSpeed
        );
    }

    //���� ��ǥ�� ��� ���� ��ǥ�� ��ȯ
    private Vector2 WorldToMini(Vector3 world)
    {
        float tx = 0.5f;
        float tz = 0.5f;

        float dx = Mathf.Abs(worldMax.x - worldMin.x);
        if (dx > 0.00001f)
        {
            tx = Mathf.InverseLerp(worldMin.x, worldMax.x, world.x);
        }

        float dz = Mathf.Abs(worldMax.y - worldMin.y);
        if (dz > 0.00001f)
        {
            tz = Mathf.InverseLerp(worldMin.y, worldMax.y, world.z);
        }

        if (backgroundRT == null)
        {
            return Vector2.zero;
        }

        Rect r = backgroundRT.rect;
        float px = (tx - 0.5f) * r.width;
        float pz = (tz - 0.5f) * r.height;

        return new Vector2(px, pz);
    }

    //�ȳ����� ó��
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

    //���� �� �ȳ����� ����
    private IEnumerator HideAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    //�� �̸����� ��������Ʈ ��Ʈ ã��
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

    //�� ���� ��ġ ���ϱ�
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
