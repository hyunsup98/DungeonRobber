using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class RushTurtle : Monster
{
    [SerializeField] float detectDelay = 1f;//감지 딜레이 
    [SerializeField] float animeDelay = 0.5f;//애니메이션 딜레이 
    [SerializeField] BaseBuff stunned; //스턴  
    [SerializeField] float selfAttackDamage = 10f; //attack 후 반동 딜 
    [SerializeField] float addedSpeed = 10f; //돌진시 추가되는 이동속도 
    [SerializeField] float beforeAttackDelay = 1f; //동진 전 대기시간
    [SerializeField] float rushColliderRadius = 0.05f; //돌진 충돌 감지 거리
    int playerLayer;
    int enemyLayer;
    int WaypointLayer;
    int obstacleLayer;
    int WallLayer;
    bool isAttacking = false; 
    bool isAlive = true;
    Coroutine detectPlayerCoroutine; //감지 코루틴  변수
    Coroutine RushAttackCoroutine; //동진 공격 코루틴 변수

    private void Awake()
    {
        awakeinit();
    }

    private void OnEnable() //활성화 시점에서 초기화 
    {
        Init();   
    }
    private void Start()
    {
        attackDelaytime = stats.GetBaseStat(StatType.AttackDelay);
    }
    private void FixedUpdate()
    {
        agent.speed = stats.GetStat(StatType.MoveSpeed);

        if (isDetectTarget) //감지했을 때
        {
            DetectAction();
        }
        else
        {
            OverlookAction();
        }


        Debug.Log($"isAttacking: {isAttacking}");
        Debug.Log($"isDetectTarget: {isDetectTarget}");
        Debug.Log($"moveSpeed: {stats.GetBaseStat(StatType.MoveSpeed)}");
        Debug.Log($"agentSpeed: {agent.speed}");
        
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

        detectPlayerCoroutine = StartCoroutine(nameof(DetectTarget));
        //▼목적지 설정 
        targetWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        
        
    }
    /// <summary>
    /// Awake시 초기화할 정보 담아놓은 메서드  
    /// </summary>
    protected override void awakeinit()
    {
        base.awakeinit();
        playerLayer = LayerMask.NameToLayer("Player");
        WaypointLayer = LayerMask.NameToLayer("Waypoint");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
        WallLayer = LayerMask.NameToLayer("Wall");
    }

    protected override void Attack()
    {
        if (RushAttackCoroutine == null)
        {
            RushAttackCoroutine = StartCoroutine(nameof(RushAttack));
        }
        
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
        bool isPlayerinhit = false;
        
       
          if (targetDistance <= stats.GetStat(StatType.AttackRange) * stats.GetStat(StatType.AttackRange)) //공격 사거리안에 들어오면 공격시작 
        {
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(0.5f, 0.5f, 0.5f), transform.forward, transform.rotation, stats.GetStat(StatType.AttackRange), obstacleLayer | targetLayer);
            //▼ 플레이어가 장애물 뒤에 숨어있지 않고 공격범위 내라면 
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    SetMoveBool(true);
                    Attack();
                    isPlayerinhit = true;
                    break;
                }
            }
            if(!isPlayerinhit)
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
        stats.ModifyStat(StatType.HP, -damage);
        Debug.Log(stats.GetStat(StatType.HP));
        
        if (stats.GetStat(StatType.HP) <= 0)
        {
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

            while (isDetectTarget == false)
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
        Debug.Log($"toSetBool : {toSetBool}");
    }


    /// <summary>
    /// 감지를 위한 코루틴 함수
    /// 감지 범위 내에서 감지 레이어에 해당하는 오브젝트가 있으면 colliders 배열에 저장
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator DetectTarget()
    {
        bool foundTarget;
        while (true)
        {
            waypoints.Clear();
            colliders = Physics.OverlapSphere(transform.position, detectDestinationRadius, detectLayer);
            foundTarget = false;

            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    //▼ 레이어가 player고 플레이어 감지거리 내에 있다면 
                    if (collider.gameObject.layer == playerLayer && Vector3.SqrMagnitude(collider.transform.position - transform.position) < playerDetectedRange * playerDetectedRange)
                    {
                        target = collider.gameObject; //타겟 설정
                        foundTarget = true;
                        isDetectTarget = true;
                        break;
                    }
                    else if (collider.gameObject.layer == WaypointLayer)//레이어가 웨이포인트라면
                    {
                        waypoints.Add(collider.transform);
                    }
                }              
                if (!foundTarget)
                {
                    isDetectTarget = false;
                }
            }
            else
            {
                isDetectTarget = false;
            }
            yield return CoroutineManager.waitForSeconds(detectDelay);
        }
    }

    
    private IEnumerator AttackDelay()
    {
        isAttackCooltime = true;

        yield return CoroutineManager.waitForSeconds(attackDelaytime);

        SetMoveBool(true);
        isAttackCooltime = false;
    }

    private IEnumerator RushAttack()
    {
        Debug.Log("공격 시작");
        SetMoveBool(false);

        agent.ResetPath(); //멈추기
        transform.LookAt(target.transform.position); //타겟 바라보기
        yield return CoroutineManager.waitForSeconds(beforeAttackDelay);//잠깐 대기하여 바라보기 
        SetMoveBool(true);

        Vector3 rushDirection = transform.forward.normalized;

        Collider[] rushColliders;
        stats.ModifyBaseStat(StatType.MoveSpeed, addedSpeed);
        while (true)
        {
            rushColliders = Physics.OverlapSphere(transform.position + transform.forward * rushColliderRadius, rushColliderRadius, detectLayer);

            if (rushColliders.Length > 0)
            {
                break;
            }
            agent.Move(rushDirection * stats.GetBaseStat(StatType.MoveSpeed) * Time.deltaTime);
            yield return null;
        }

        if (rushColliders[0].gameObject.layer == enemyLayer || rushColliders[0].gameObject.layer == playerLayer)
        {
            rushColliders[0].GetComponentInParent<Entity>()?.GetDamage(stats.GetBaseStat(StatType.AttackDamage));
            GetDamage(selfAttackDamage);
            agent.SetDestination(transform.position);
            yield return CoroutineManager.waitForSeconds(5f);
        }
        else if (rushColliders[0].gameObject.layer == obstacleLayer || rushColliders[0].gameObject.layer == WallLayer)
        {
            GetDamage(selfAttackDamage);
            agent.SetDestination(transform.position);
            yield return CoroutineManager.waitForSeconds(5f);
        }
        //등껍질 애니메이션 하다가 플레이어가 범위 밖으로 나가면 다시 시작
        stats.ModifyBaseStat(StatType.MoveSpeed, -addedSpeed);
        isAttacking = false;
        Debug.Log("공격끝");
        RushAttackCoroutine = null;

    }

}




