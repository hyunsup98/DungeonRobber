using UnityEngine;

public sealed partial class Player_Controller
{
    protected override void Attack()
    {
        Vector3 dir = transform.forward;
        Vector3 atkPos = attackPos.position + (transform.forward * (attackRange * 0.5f));

        RaycastHit[] hits = Physics.BoxCastAll(atkPos, new Vector3(0.5f, 0.4f, attackRange * 0.5f), dir, transform.rotation, 0f, attackMask);

        foreach(var hit in hits)
        {
            //todo 공격 로직 넣기
            Debug.Log("공격!");
        }
    }

    /// <summary>
    /// 대미지 피격 메서드
    /// 플레이어의 체력을 깎아줌, 피격 관련 모션, 사운드 등을 입음
    /// </summary>
    /// <param name="damage"> 플레이어가 입을 대미지의 수치 </param>
    protected override void GetDamage(float damage)
    {
        CurrentHP -= damage;
    }

    private void OnDrawGizmos()
    {
        Vector3 dir = transform.forward;
        Vector3 atkPos = attackPos.position + (transform.forward * (attackRange * 0.5f));

        Gizmos.color = Color.green;
        Gizmos.DrawCube(atkPos, new Vector3(1f, 0.8f, attackRange));
    }
}
