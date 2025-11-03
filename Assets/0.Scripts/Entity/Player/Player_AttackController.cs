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
    /// 실질적인 공격 메서드
    /// 공격 속도(AttackDelay) 만큼 IsAttack을 True로 바꿈
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoAttack()
    {
        AddPlayerBehaviorState(PlayerBehaviorState.IsAttack);

        SoundManager.Instance.PlaySoundEffect(attackClip);
        playerAnimator.SetTrigger("attack");

        Vector3 dir = transform.forward;
        Vector3 atkPos = attackPos.position + (transform.forward * (stats.GetStat(StatType.AttackRange) * 0.5f));

        RaycastHit[] hits = Physics.BoxCastAll(atkPos, new Vector3(0.5f, 0.4f, stats.GetStat(StatType.AttackRange) * 0.5f), dir, transform.rotation, 0f, attackMask);

        foreach (var hit in hits)
        {
            if(hit.collider.TryGetComponent<Monster>(out var enemy))
            {
                enemy.GetDamage(stats.GetStat(StatType.AttackDamage));
            }
        }

        yield return CoroutineManager.waitForSeconds(stats.GetStat(StatType.AttackDelay));
        RemovePlayerBehaviorState(PlayerBehaviorState.IsAttack);
    }

    /// <summary>
    /// 대미지 피격 메서드
    /// 플레이어의 체력을 깎아줌, 피격 관련 모션, 사운드 등을 입음
    /// </summary>
    /// <param name="damage"> 플레이어가 입을 대미지의 수치 </param>
    public override void GetDamage(float damage)
    {
        stats.ModifyStat(StatType.HP, -damage);
        SoundManager.Instance.PlaySoundEffect(hitClip);
        onPlayerStatChanged?.Invoke();

        if (stats.GetStat(StatType.HP) <= 0)
        {
            playerDeadAction?.Invoke();
        }
    }

    public void Revive()
    {
        if(CheckPlayerBehaviorState(PlayerBehaviorState.Dead))
        {
            playerAnimator.SetBool("alive", true);
            AddPlayerBehaviorState(PlayerBehaviorState.Alive);
            RemovePlayerBehaviorState(PlayerBehaviorState.Dead);
            stats.InitStat(StatType.HP);
            onPlayerStatChanged?.Invoke();
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
