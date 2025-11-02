using System.Collections;
using UnityEngine;

//플레이어가 특정 구역에 들어오면 탈출 카운트다운을 실행하고,
//완료 시 GameState.Base 상태로 변경하는 트리거 스크립트
public class ReturnToBaseOnTouch : MonoBehaviour
{
    [Header("감지 설정")]
    [SerializeField] private string _reachPlayerTag = "Player";   //감지할 태그
    [SerializeField] private float _reachWaitSeconds = 5f;        //카운트다운 시간
    [SerializeField] private bool _cancelIfPlayerExit = true;     //플레이어가 나가면 취소 여부

    [Header("UI 설정")]
    [SerializeField] private EscapeCountDownUI countDownUI;       //카운트다운 UI 참조
    [SerializeField] private string _escapeMessage = "5초 후 지역을 탈출합니다"; //안내 문구

    private Coroutine running; //현재 실행 중인 코루틴 저장

    private void Awake()
    {
        //인스펙터에 지정되지 않았으면 자동 검색
        if (countDownUI == null)
        {
            countDownUI = FindObjectOfType<EscapeCountDownUI>(true);
        }
    }

    //플레이어가 트리거 영역에 진입 시 호출
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[EscapeArea] OnTriggerEnter: {other.name}, tag={other.tag}");
        //감지할 태그가 아니면 무시
        if (other.CompareTag(_reachPlayerTag) == false)
        {
            return;
        }

        //이미 카운트다운 진행 중이면 중복 실행 방지
        if (running != null)
        {
            Debug.Log("[EscapeArea] 이미 카운트다운 중입니다. 무시했습니다.");
            return;
        }

        Debug.Log("카운트 다운 시작");

        //카운트다운 시작
        running = StartCoroutine(CountDownAndReturn());
    }

    //플레이어가 영역을 벗어났을 때 호출
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[EscapeArea] OnTriggerExit: {other.name}, tag={other.tag}");

        if (other.CompareTag(_reachPlayerTag) == false)
        {
            return;
        }

        //설정에 따라 취소 여부 결정
        if (_cancelIfPlayerExit == false)
        {
            return;
        }

        //카운트다운 중단
        if (running != null)
        {
            Debug.Log("[EscapeArea] 카운트다운 중단");
            StopCoroutine(running);
            running = null;
        }

        //UI 숨김
        if (countDownUI != null)
        {
            countDownUI.EscapeCountDownHide();
        }
    }

    //탈출 카운트다운 진행 코루틴
    private IEnumerator CountDownAndReturn()
    {
        float remain = Mathf.Max(0f, _reachWaitSeconds);

        //UI 활성화 및 초기화
        if (countDownUI != null)
        {
            countDownUI.EscapeCountDownShow(_escapeMessage, remain);
        }

        //남은 시간을 매 프레임 갱신
        while (remain > 0f)
        {
            if (countDownUI != null)
            {
                countDownUI.EscapeCountDownSetTime(remain);
            }

            yield return null;
            remain -= Time.deltaTime;
        }

        //완료 후 UI 숨김
        if (countDownUI != null)
        {
            countDownUI.EscapeCountDownHide();
        }

        //상태를 Base로 전환 > 자동으로 SceneOnGameState가 씬 이동 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentGameState = GameState.Base;
        }

        running = null;
    }
}
