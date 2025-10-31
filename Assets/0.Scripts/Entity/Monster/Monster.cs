using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public abstract class Monster : Entity
{
    [SerializeField] protected GameObject target; //감지할 타겟
    [SerializeField] protected float targetDistance; // 타겟 간의 거리
    [SerializeField] protected float detectedRangeRadius; // 감지 거리 반지름 
    [SerializeField] protected LayerMask detectlayer; //감지할 물체 레이어    
    [SerializeField] protected NavMeshAgent agent; //ai navigation 사용을 위한 변수
    [SerializeField] protected Rigidbody monsterrib; //몬스터 리지드바디 변수   
    [SerializeField] protected Transform previousWaypoint; //이전 목적지 위치 
    [SerializeField] protected Transform targetWaypoint; // 현재 목적지 위치 
    [SerializeField] protected Animator monsterAnimator; //에니메이션 관리 
    [SerializeField] protected Collider[] colliders; // 감지된 물체 넣어놓는 배열
    [SerializeField] protected Transform[] Waypoints; //순찰 지점들 저장용 배열

    protected RaycastHit rayhit; //레이캐스트 히트 정보 저장용 변수   
    public bool isDetective = false; //감지했는가를 나타내는 변수 

    protected abstract override void Attack();
    public abstract override void GetDamage(float damage);
    protected abstract IEnumerator DetectTarget();// 적 감지를 위한 코루틴 함수
    protected abstract void DetectAction(); //감지했을 때 취하는 행동 메서드
    protected abstract void OverlookAction(); //감지하지 못했을 때 취하는 행동 메서드
                       
}
