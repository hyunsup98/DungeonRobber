using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ ���õ� ��ɵ��� ����ϴ� Ŭ����
/// �̵�, ����, �ִϸ��̼� ���� ��Ҹ� ���
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
    /// Rigidbody�� velocity�� �̿��� �̵� �޼���
    /// W, A, S, D Ű�� �̿��� �̵� ����
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
