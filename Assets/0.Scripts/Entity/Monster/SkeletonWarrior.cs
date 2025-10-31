using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class SkeletonWarrior : Monster
{
    [SerializeField] WaitForSeconds detectDelay = new WaitForSeconds(1f);//감지 딜레이 
    [SerializeField] WaitForSeconds animeDelay = new WaitForSeconds(0.5f);//애니메이션 딜레이 
    int playerLayer;
    int WaypointLayer;   
    Coroutine detectPlayerCoroutine; //감지 코루틴  변수

    private void Awake()
    {
        awakeinit();
    }

    private void OnEnable() //활성화 시점에서 초기화 
    {
        Init();
        StartCoroutine(WaitAnimationEnd("Spawn")); //스폰 애니메이션 종료까지 대기     
    }
    private void FixedUpdate()
    {       
        
        if (isDetectTarget) //감지했을 때
        {
            DetectAction();
        }
        else
        {
            OverlookAction();
        }
    }

    private void OnDisable() //비활성화시 코루틴 정지 
    {
        if (detectPlayerCoroutine != null)
            StopCoroutine(detectPlayerCoroutine);
    }

    protected override void Init()
    {
        base.Init();
        //attackRange = 2f;
        //attackDelay = 1.5f;
        playerDetectedRange = 30f;

        detectPlayerCoroutine = StartCoroutine(nameof(DetectTarget));

        if (isDetectTarget == false) //주변에 타겟이 없으면 
        {
            //▼목적지 설정 
            targetWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        }
        agent.speed = stats.GetStat(StatType.MoveSpeed);
    }
    /// <summary>
    /// Awake시 초기화할 정보 담아놓은 메서드  
    /// </summary>
    protected override void awakeinit()
    {
        base.awakeinit();
        playerLayer = LayerMask.NameToLayer("Player");
        WaypointLayer = LayerMask.NameToLayer("Waypoint");
    }

    protected override void Attack()
    {
        //todo 공격 구현
        SetMoveBool(false); //이동 멈춤
        Debug.Log("Attack");
        monsterAnimator.SetTrigger("Attack");
        
       
    }

    /// <summary>
    /// 감지했을 때 취하는 행동 메서드
    /// </summary>
    protected override void DetectAction()
    {
        SetMoveBool(false); //일단 멈춤
        Vector3 tempVector = target.transform.position - transform.position;
        targetDistance = Vector3.SqrMagnitude(tempVector); // 단순 비교이므로 sqrMagnitude 사용
        transform.LookAt(target.transform.position); //타겟 바라보기

        if (targetDistance <= stats.GetStat(StatType.AttackRange) * stats.GetStat(StatType.AttackRange)) //공격 사거리안에 들어오면 공격 
        {            
            //▼ 플레이어가 장애물 뒤에 숨어있지 않고 공격범위 내라면 
            if (Physics.Raycast(transform.position + (Vector3.up * 1.5f), transform.forward, out rayhit, stats.GetStat(StatType.AttackRange), obstacleTargetLayer) && rayhit.collider.CompareTag("Player")) 
            {                                               
                Attack();               
            }
            else
            {
                SetMoveBool(true); //이동 상태로 전환 
                agent.SetDestination(target.transform.position); //타겟의 위치로 이동
            }
        }
        else
        {
            SetMoveBool(true); //이동 상태로 전환   
            agent.SetDestination(target.transform.position); //타겟의 위치로 이동
        }
    }

    /// <summary>
    /// 데미지 피격 메서드 
    /// </summary>
    /// <param name="damage"> 해당 몬스터가 입을 피해 </param>
    public override void GetDamage(float damage)
    {
        monsterAnimator.SetTrigger("GetDamage");
        stats.ModifyStat(StatType.HP, -damage);

        if (stats.GetStat(StatType.HP) <= 0)
        {
            monsterAnimator.SetTrigger("Die");
            Destroy(this);
        }
    }

    /// <summary>
    /// 감지하지 못했을 때 취하는 행동 메서드
    /// </summary>
    protected override void OverlookAction()
    {
        SetMoveBool(true); //이동 상태로 전환
        agent.SetDestination(targetWaypoint.position);//목적지로 이동  
        
        if (Vector3.SqrMagnitude(transform.position - targetWaypoint.position) < 10f) //a목적지에 주변에 도달했을 때
        {
            SetMoveBool(false); //일단 멈춤
            previousWaypoint = targetWaypoint; //이전 목적지에 현재 목적지 저장                        

            while (true && isDetectTarget == false)
            {
                Debug.Log(waypoints.Count);            
                if (waypoints.Count <= 1) //목적지가 하나밖에 없으면
                    break;

                targetWaypoint = waypoints[Random.Range(0, waypoints.Count)];  //새 목적지 설정

                if (targetWaypoint != previousWaypoint) //이전 목적지와 다르면
                    break;
            }
           
        }
    }

    /// <summary>
    /// 움직임 관련 부울 변수 일괄 설정 메서드
    /// </summary>
    /// <param name="toSetBool">"움직이는 경우 true 아닐 경우 false"</param>
    void SetMoveBool(bool toSetBool)
    {
        agent.isStopped = !toSetBool;
        monsterAnimator.SetBool("isWalk", toSetBool);
    }


    /// <summary>
    /// 감지를 위한 코루틴 함수
    /// 감지 범위 내에서 감지 레이어에 해당하는 오브젝트가 있으면 colliders 배열에 저장
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator DetectTarget()
    {
        while (true)
        {
            waypoints.Clear();
            colliders = Physics.OverlapSphere(transform.position, detectDestinationRadius, detectLayer);

            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    //▼ 레이어가 player고 플레이어 감지거리 내에 있다면 
                    if (collider.gameObject.layer == playerLayer && Vector3.SqrMagnitude(collider.transform.position - transform.position) < playerDetectedRange * playerDetectedRange ) 
                    {
                        target = collider.gameObject; //타겟 설정
                        isDetectTarget = true;
                        break;
                    }
                    else if (collider.gameObject.layer == WaypointLayer)//레이어가 웨이포인트라면
                    {
                        waypoints.Add(collider.transform);
                    }
                }               
            }
            else
            {
                isDetectTarget = false;
            }
            yield return detectDelay;
        }
    }
    
    /// <summary>
    /// 애니메이션 종료 대기 코루틴
    /// </summary>
    /// <param name="animName">애니메이션 이름 </param>
    /// <returns></returns>
    private IEnumerator WaitAnimationEnd(string animName)
    {
        while (!monsterAnimator.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            yield return null;
        }
        while (monsterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            agent.isStopped = true; //애니메이션 재생되는 동안 멈춤
            yield return null;
        }
        agent.isStopped = false; //애니메이션 재생 끝나면 다시 이동 가능
        yield return animeDelay;
    }          
}
