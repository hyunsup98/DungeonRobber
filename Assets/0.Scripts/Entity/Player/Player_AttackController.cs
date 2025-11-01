using System.Collections;
using UnityEngine;

public sealed partial class Player_Controller
{
    protected override void Attack()
    {
        if (CheckPlayerBehaviorState(PlayerBehaviorState.IsAttack) || CheckPlayerBehaviorState(PlayerBehaviorState.IsDodge)) return;

        StartCoroutine(DoAttack());
    }

    /// <summary>
    /// �������� ���� �޼���
    /// ���� �ӵ�(AttackDelay) ��ŭ IsAttack�� True�� �ٲ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoAttack()
    {
        AddPlayerBehaviorState(PlayerBehaviorState.IsAttack);

        playerAnimator.SetTrigger("attack");

        Vector3 dir = transform.forward;
        Vector3 atkPos = attackPos.position + (transform.forward * (stats.GetStat(StatType.AttackRange) * 0.5f));

        RaycastHit[] hits = Physics.BoxCastAll(atkPos, new Vector3(0.5f, 0.4f, stats.GetStat(StatType.AttackRange) * 0.5f), dir, transform.rotation, 0f, attackMask);

        foreach (var hit in hits)
        {
            if(hit.collider.TryGetComponent<Monster>(out var enemy))
            {
                enemy.GetDamage(stats.GetStat(StatType.AttackRange));
            }
        }

        yield return CoroutineManager.waitForSeconds(stats.GetStat(StatType.AttackDelay));
        RemovePlayerBehaviorState(PlayerBehaviorState.IsAttack);
    }

    /// <summary>
    /// ����� �ǰ� �޼���
    /// �÷��̾��� ü���� �����, �ǰ� ���� ���, ���� ���� ����
    /// </summary>
    /// <param name="damage"> �÷��̾ ���� ������� ��ġ </param>
    public override void GetDamage(float damage)
    {
        stats.ModifyStat(StatType.HP, -damage);
        onPlayerStatChanged?.Invoke();

        if (stats.GetStat(StatType.HP) <= 0)
        {
            playerDeadAction?.Invoke();
        }
    }

    private void Dead()
    {
        playerAnimator.SetBool("alive", false);
        playerRigid.velocity = new Vector3(0, playerRigid.velocity.y, 0);
        RemovePlayerBehaviorState(PlayerBehaviorState.Alive);
        AddPlayerBehaviorState(PlayerBehaviorState.Dead);
    }
}
