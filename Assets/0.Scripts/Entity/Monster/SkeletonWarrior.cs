using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class SkeletonWarrior : Monster
{
    private void OnEnable()
    {
        Init();
    }

    protected override void Attack()
    {
        
    }

    protected override void DetectAction()
    {
        if(isDetective)
        {
            agent.SetDestination(target.transform.position);
        }        
    }

    protected override void GetDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Destroy(this);
        }
    }

    protected override void OverlookAction()
    {
        Direction rndDirection = (Direction)Random.Range(0, 4); //랜덤 방향 선언 

        switch(rndDirection)
        {
            case Direction.North:
                transform.LookAt(Vector3.forward);
                break;
            
            case Direction.South:
                transform.LookAt(Vector3.back);
                break;
            
            case Direction.East:
                transform.LookAt(Vector3.right);
                break;
            
            case Direction.West:
                transform.LookAt(Vector3.left);
                break;                 
        }

    }

    protected override IEnumerator DetectTarget()
    {
        WaitForSeconds detectDelay = new WaitForSeconds(0.1f);//감지 딜레이 

        while (true)
        {
            colliders = Physics.OverlapSphere(transform.position, detectedRangeRadius, detectlayer); 

            if (colliders.Length > 0) //collider가 하나라도 있다면 
            {
                isDetective = true;
                target = colliders[0].gameObject;
            }
            else
            {
                isDetective = false;
            }
            yield return detectDelay;
        }
    }
}
