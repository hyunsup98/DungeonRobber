
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonWarrior : Monster
{
    [SerializeField] WaitForSeconds detectDelay = new WaitForSeconds(4f);//감지 딜레이 
    [SerializeField] WaitForSeconds changeDirectionDelay = new WaitForSeconds(2f);//방향 전환 딜레이 

    Direction direction; //몬스터 방향 저장용 변수
    Coroutine detectCoroutine; //감지 코루틴  변수
    Coroutine changeDirection; //랜덤 방향 저장하는 코루틴 변수
    
    private void OnEnable() //활성화 되면 초기화 
    {
        Init();  
    }

    private void Update()
    {
        if (isDetective) //감지됐을 때
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

        if (changeDirection != null)
            StopCoroutine(changeDirection);
    }

    protected override void Init()
    {
        base.Init();
        detectCoroutine = StartCoroutine(nameof(DetectTarget));
        changeDirection = StartCoroutine(nameof(ChangeDirection));
    }

    protected override void Attack()
    {
        //todo 공격 구현
        Debug.Log("Attack");
    }

    protected override void DetectAction()
    {
        monsterrib.velocity = Vector3.zero;

        Vector3 tempVector = target.transform.position - transform.position;
        targetDistance = Vector3.SqrMagnitude(tempVector); // 단순 비교이므로 sqrMagnitude 사용

        if (targetDistance <= attackRange * attackRange) //공격 사거리안에 들어오면 공격 
        {
            if (Physics.Raycast(transform.position, transform.forward, out rayhit, attackRange, detectlayer) && rayhit.collider.CompareTag("Player")) //공격 범위 내 첫 충돌이 플레이어면 
            {
                Attack();
            }
            else
            {
                agent.SetDestination(target.transform.position); //타겟의 위치로 이동
            }
        }
        else
        {
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

    protected override void OverlookAction()
    {
        agent.SetDestination(transform.position);//감지 되지 않았을때 현재 위치에 정지

        Debug.Log("Move Forward");

        if (Physics.Raycast(transform.position, transform.forward, out rayhit, 1f, detectlayer))
        {
            agent.SetDestination(transform.position);
        }

        switch (direction)
        {
            case Direction.North:
                transform.LookAt(Vector3.forward);
                break;

            case Direction.South:
                transform.LookAt(Vector3.back);
                break;

            case Direction.East:
                transform.LookAt(Vector3.right);
                break;

            case Direction.West:
                transform.LookAt(Vector3.left);
                break;
        }          
        agent.Move(transform.forward * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 감지를 위한 코루틴 함수
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator DetectTarget()
    {
        while (true)
        {
            colliders = Physics.OverlapSphere(transform.position, detectedRangeRadius, detectlayer);

            if (colliders.Length > 0) //player가 감지되어 colliders배열에 들어왔는지 확인  
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Player"))
                    {
                        target = collider.gameObject;
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

    protected IEnumerator ChangeDirection()
    {
        while (true)
        {
            direction = (Direction)Random.Range(0, 4); //랜덤 방향 지정 
            yield return changeDirectionDelay;
        }
    }

}
