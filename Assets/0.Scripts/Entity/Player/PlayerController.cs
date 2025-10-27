using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어에 관련된 기능들을 담당하는 클래스
/// 이동, 공격, 애니메이션 등의 요소를 담당
/// </summary>
public sealed class PlayerController : Entity
{
    [SerializeField] private Rigidbody playerRigid;

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Rigidbody의 velocity를 이용한 이동 메서드
    /// W, A, S, D 키를 이용해 이동 구현
    /// </summary>
    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveVelocity = new Vector3(horizontal, 0, vertical).normalized;
        moveVelocity *= moveSpeed;
        playerRigid.velocity = moveVelocity;

    }

    protected override void Init()
    {

    }

    protected override void Attack()
    {

    }

    protected override void GetDamage(float damage)
    {

    }
}
