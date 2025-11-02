using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof(NavMeshAgent))]
public abstract class Monster : Entity
{
    [Tooltip("플레이어 감지 거리")]
    [SerializeField] protected float playerDetectedRange; // 플레이어 감지 거리 반지름
    [Tooltip("목적지 감지 거리 ")] 
    [SerializeField] protected float detectDestinationRadius; //목적지 감지 반지름
    [Tooltip("장애물 레이어")]
    [SerializeField] protected LayerMask obstacleLayer; //감지할 물체 레이어
    [Tooltip("목적지 레이어")]
    [SerializeField] protected LayerMask destinationLayer; //목적지 레이어   
    [Tooltip("공격 또는 행동할 타겟 레이어")]
    [SerializeField] protected LayerMask targetLayer; //타겟 레이어
    [SerializeField] protected NavMeshAgent agent; //ai navigation 사용을 위한 변수
    [SerializeField] protected Animator monsterAnimator; //에니메이션 관리   

    [Header("테스트용 변수 플레이시 자동으로 변경됨")]
    [Space(15f)]
    [SerializeField] protected GameObject target; //감지할 타겟   
    [SerializeField] protected float targetDistance; // 타겟 간의 거리      
    [SerializeField] protected Transform previousWaypoint; //이전 목적지 위치 
    [SerializeField] protected Transform targetWaypoint; // 현재 목적지 위치 
    [SerializeField] protected Collider[] colliders; // 감지된 물체 넣어놓는 배열
    [SerializeField] protected List<Transform> waypoints; //순찰 지점들 저장용 리스트

    [Header("이 아래는 하위 몬스터가 가지고 있는 로컬 변수")]
    

    protected RaycastHit rayhit; //레이캐스트 히트 정보 저장용 변수   
    protected bool isDetectTarget = false; //감지했는가를 나타내는 변수 
    protected bool isAttackCooltime = false; //공격 쿨타임인지를 나타내는 변수
    protected float attackDelaytime; //공격 딜레이 시간 
    protected LayerMask detectLayer;
    

    protected abstract override void Attack();
    public abstract override void GetDamage(float damage);
    protected abstract IEnumerator DetectTarget();// 적 감지를 위한 코루틴 함수
    protected abstract void DetectAction(); //감지했을 때 취하는 행동 메서드
    protected abstract void OverlookAction(); //감지하지 못했을 때 취하는 행동 메서드

    protected virtual void awakeinit() 
    {  
        detectLayer |= obstacleLayer; //detectLayer에 obstacleTargetLayer속 레이어 추가
        detectLayer |= destinationLayer; //detectLayer에 destinationLayer속 레이어 추가
        detectLayer |= targetLayer;
        waypoints = new List<Transform>();
        
    }

}
