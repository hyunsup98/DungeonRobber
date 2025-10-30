
using System.Collections;
using UnityEngine;

public class SkeletonWarrior : Monster
{
    [SerializeField] WaitForSeconds detectDelay = new WaitForSeconds(1f);//감지 딜레이 
    Coroutine detectCoroutine; //감지 코루틴 변수

    private void OnEnable() //활성화 시점에서 초기화 
    {
        Init();
    }
    
    private void FixedUpdate()
    {
        StartCoroutine(WaitAnimationEnd("Spawn")); //스폰 애니메이션 끝날때까지 대기
       
        if (isDetective) //감지했을 때
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
        if (detectCoroutine != null)
            StopCoroutine(detectCoroutine);
    }

    protected override void Init()
    {
        maxHP = 50f;
        base.Init();
        attackRange = 2f;
        attackDamage = 10f;
        attackDelay = 1.5f;
        moveSpeed = 3f;
        detectedRangeRadius = 10f;
        
        detectCoroutine = StartCoroutine(nameof(DetectTarget));
        targetWaypoint = Waypoints[Random.Range(0, Waypoints.Length)];              
    }

    protected override void Attack()
    {
        //todo 공격 구현
        SetMoveBool(false); //이동 정지
        Debug.Log("Attack");
        monsterAnimator.SetTrigger("Attack");
        StartCoroutine(WaitAnimationEnd("Attack"));
    }

    /// <summary>
    /// 감지했을 때 취하는 행동 메서드
    /// </summary>
    protected override void DetectAction()
    {
        SetMoveBool(false);//이동 정지
        Vector3 tempVector = target.transform.position - transform.position;
        targetDistance = Vector3.SqrMagnitude(tempVector); // 단순 비교이므로 sqrMagnitude 사용

        if (targetDistance <= attackRange * attackRange) //공격 사거리안에 들어오면 공격 
        {
            if (Physics.Raycast(transform.position + (Vector3.up * 1.5f), transform.forward, out rayhit, attackRange, detectlayer) && rayhit.collider.CompareTag("Player")) //플레이어가 장애물 뒤에 숨어있지 않고 공격범위 내라면 
            {                                              
                Attack();               
            }
            else
            {
                SetMoveBool(true);//이동 가능
                agent.SetDestination(target.transform.position); //타겟의 위치로 이동
            }
        }
        else
        {     
            SetMoveBool(true);//이동 가능
            agent.SetDestination(target.transform.position); //타겟의 위치로 이동
        }
    }

    /// <summary>
    /// 데미지 피격 메서드 
    /// </summary>
    /// <param name="damage"> 해당 몬스터가 입을 피해 </param>
    protected override void GetDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// 감지하지 못했을 때 취하는 행동 메서드
    /// </summary>
    protected override void OverlookAction()
    {
        SetMoveBool(true);//??? ????
        agent.SetDestination(targetWaypoint.position);//목적지로 이동

        if (Vector3.SqrMagnitude(transform.position - targetWaypoint.position) < 0.1f * 0.1f) //a목적지에 도달했을 때
        {
            SetMoveBool(false);//??? ?????
            previousWaypoint = targetWaypoint; //이전 목적지에 현재 목적지 저장

            while (true)
            {
                monsterAnimator.SetTrigger("Arrive"); //도착 애니메이션 재생
                StartCoroutine(WaitAnimationEnd("LookAround")); //주변 살피기 애니메이션 재생 대기

                if (Waypoints.Length <= 1) //목적지가 하나밖에 없으면
                    break;

                targetWaypoint = Waypoints[Random.Range(0, Waypoints.Length)]; //새 목적지 설정

                if (targetWaypoint != previousWaypoint) //이전 목적지와 다르면
                    break;
            }
        }
    }

    // 벽 감지 시 회전 후 이동 
    // protected override void OverlookAction()
    // {
    //     Vector3 nextPos = transform.position + transform.forward * 2f;

    //     if (Physics.Raycast(transform.position + (Vector3.up * 1.5f), transform.forward, out rayhit, 5f, detectlayer))
    //     {           
    //         agent.isStopped = true;
    //         Debug.Log("뭔가 있음");

    //         agent.isStopped = false;
    //         nextPos = transform.position + transform.forward * 2f;
    //     }  
    //     agent.SetDestination(nextPos);//다음 위치로 전진                  
    // }

    /// <summary>
    /// 이동 관련 부울 변수 제어 메서드
    /// </summary>
    /// <param name="toSetBool">"움직일 때  true 움직이지 않을때 false"</param>
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
            colliders = Physics.OverlapSphere(transform.position, detectedRangeRadius, detectlayer);

            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Player")) //태그가 player이면
                    {
                        target = collider.gameObject; //타겟 설정
                        isDetective = true;
                        break;
                    }
                }
            }
            else
            {
                isDetective = false;
            }
            yield return detectDelay;
        }
    }
    
    /// <summary>
    /// 애니메이션 종료 대기 코루틴
    /// </summary>
    /// <param name="animName">에니메이션이름</param>
    /// <returns></returns>
    IEnumerator WaitAnimationEnd(string animName)
    {   
        while (!monsterAnimator.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            yield return null;
        }
        while(monsterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
    }
}
