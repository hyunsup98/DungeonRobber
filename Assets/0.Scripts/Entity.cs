using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    float attackDamage;
    float maxHP;
    float currentHP;
    float moveSpeed;
    float attackRange;
    float attackDelay;

    protected abstract void Init(); //엔티티 속성 초기화 
    protected void Attack(Entity target) 
    {
        target.getDamaged(attackDamage);
    } 

    protected virtual void getDamaged(float damage) 
    {
        currentHP -= damage;     
    }
}
