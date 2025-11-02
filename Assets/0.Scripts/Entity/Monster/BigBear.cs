using System.Collections;
using UnityEngine;


public class BigBear : Monster
{
    [SerializeField] private float detectDelay = 1f;//감지 딜레이 
    [SerializeField] private float animeDelay = 0.5f;//애니메이션 딜레이 
    Vector3 homePosition;
    bool isHome;//집인지 확인하는 변수
    int playerLayer;
    bool isAlive = true;
    Coroutine detectPlayerCoroutine; //감지 코루틴 변수
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
        attackDelaytime = stats.GetStat(StatType.AttackDelay);
    }
    
    private void FixedUpdate()
    {
        if (!isAlive) return;

        agent.speed = stats.GetStat(StatType.MoveSpeed);
        
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
        {
            StopCoroutine(detectPlayerCoroutine);            
        }
    }

    protected override void Init()
    {
        base.Init();
        //attackRange = 5f;
        //attackDelay = 3f;

        detectPlayerCoroutine = StartCoroutine(nameof(DetectTarget));
        //▼목적지 설정 플레이어 감지 전에는 집에서 안나가므로 현재 위치로 지정 
        homePosition = transform.position;

        isHome = true;
    }
    /// <summary>
    /// Awake시 초기화할 정보 담아놓은 메서드  
    /// </summary>
    protected override void awakeinit()
    {
        base.awakeinit();
        playerLayer = LayerMask.NameToLayer("Player");
    }

    protected override void Attack()
    {
        if(!isAttackCooltime)//공격 쿨타임이 아닐때 
        {
            SetMoveBool(false); //이동 멈춤
            monsterAnimator.SetTrigger("Attack");

            StartCoroutine(AttackDelay());              
        }
    }
    
    public void AttackPlayer()
    {
       if(Vector3.SqrMagnitude(target.transform.position - transform.position) <= stats.GetStat(StatType.AttackRange) * stats.GetStat(StatType.AttackRange))
        {
            target.GetComponentInParent<Entity>().GetDamage(stats.GetStat(StatType.AttackDamage)); 
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
        isHome = false;

        if (targetDistance <= stats.GetStat(StatType.AttackRange) * stats.GetStat(StatType.AttackRange)) //공격 사거리안에 들어오면 공격시작 
        {
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(0.5f, 0.5f, 0.5f), transform.forward, transform.rotation, stats.GetStat(StatType.AttackRange), obstacleLayerMask | targetLayer);
            //▼ 플레이어가 장애물 뒤에 숨어있지 않고 공격범위 내라면 
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
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
        monsterAnimator.SetTrigger("GetDamage");
        stats.ModifyStat(StatType.HP, -damage);

        if (stats.GetStat(StatType.HP) <= 0)
        {
            isAlive = false;
            monsterAnimator.SetBool("isDead", true);

        }
    }
     public void DestroyMonster()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 감지하지 못했을 때 취하는 행동 메서드
    /// </summary>
    protected override void OverlookAction()
    {
        if (!isHome)
        {
            SetMoveBool(true); //이동 상태로 전환
            agent.SetDestination(homePosition);//목적지로 이동  

            if (Vector3.SqrMagnitude(transform.position - homePosition) < 1f) //a목적지에 주변에 도달했을 때
            {
                SetMoveBool(false); //일단 멈춤 
                isHome = true;
            }
            
        }
        
    }

    /// <summary>
    /// 움직임 관련 부울 변수 일괄 설정 메서드
    /// </summary>
    /// <param name="toSetBool">"움직이는 경우 true 아닐 경우 false"</param>
    public void SetMoveBool(bool toSetBool)
    {
        agent.isStopped = !toSetBool;
        monsterAnimator.SetBool("isWalk", toSetBool);
        
    }
    public void StopMove() //애니메이션 이벤트용 이동정지 메서드
    {
        SetMoveBool(false);
    }
    public void StartMove() //애니메이션 이벤트용 이동 시작 메서드
    {
        SetMoveBool(true);
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

            foundTarget = false;
            waypoints.Clear();
            colliders = Physics.OverlapSphere(transform.position, detectDestinationRadius, detectLayer);

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
            yield return detectDelay;
        }
    }
    
    /// <summary>
    /// 공격 딜레이 넣어주는 메서드 
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackDelay()
    {
        isAttackCooltime = true;

        yield return CoroutineManager.waitForSeconds(attackDelaytime);

        isAttackCooltime = false;
    }
}
