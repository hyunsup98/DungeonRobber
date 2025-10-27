using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum Direction {East, South, West, North }

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public abstract class Monster : Entity
{
    [SerializeField] protected GameObject target; //감지할 타겟
    [SerializeField] protected float targetDistance; // 타겟 간의 거리
    [SerializeField] protected float detectedRangeRadius; // 감지 거리 반지름 
    [SerializeField] protected Collider[] colliders; // 감지된 물체 넣어놓는 배열
    [SerializeField] protected LayerMask detectlayer; //물체를 감지할 레이어
    [SerializeField] protected NavMeshAgent agent; //ai navigation 사용을 위한 변수

    protected bool isDetective = false; //감지했는가를 나타내는 변수      
    protected abstract override void Attack();
    protected abstract override void GetDamage(float damage);
    protected abstract IEnumerator DetectTarget();// 적 감지를 위한 코루틴 함수
    protected abstract void DetectAction(); //감지했을 때 취하는 행동 메서드
    protected abstract void OverlookAction(); //감지하지 못했을 때 취하는 행동 메서드
                       
}
