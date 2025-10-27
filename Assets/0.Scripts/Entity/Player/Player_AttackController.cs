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
            //todo ���� ���� �ֱ�
            Debug.Log("����!");
        }
    }

    /// <summary>
    /// ����� �ǰ� �޼���
    /// �÷��̾��� ü���� �����, �ǰ� ���� ���, ���� ���� ����
    /// </summary>
    /// <param name="damage"> �÷��̾ ���� ������� ��ġ </param>
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
