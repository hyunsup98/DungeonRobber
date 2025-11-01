using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPool : ObjectPool<Trap> { }


public interface IDamageable
{
    void ApplyDamage(int amount);
}


public class Trap : MonoBehaviour
{

    [SerializeField] TrapPool pool;
    void Start()
    {
        if (!pool)
        {
            pool = FindAnyObjectByType<TrapPool>();
        }
        
        if (!pool)
        {
            Debug.LogWarning("TrapPool이 없습니다.");
            return;
        }

    }


    [Header("Trap Settings")]
    [SerializeField] int damage = 10;
    [SerializeField] float activeDuration = 0.6f;
    [SerializeField] float cooldown = 1.0f;
    [SerializeField] Collider hitbox;            // isTrigger = true
    [SerializeField] bool damageOncePerActivation = false;
    [SerializeField] float damageTick = 0.2f;

    bool isActive;
    Coroutine _routine;

    readonly Dictionary<Collider, float> _lastHitTime = new();
    readonly HashSet<Collider> _hitThisActivation = new();

    public void Activate(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(CoRun());
    }

    IEnumerator CoRun()
    {
        _hitThisActivation.Clear();
        _lastHitTime.Clear();

        gameObject.SetActive(true);
        if (hitbox) hitbox.enabled = true;
        isActive = true;

        yield return new WaitForSeconds(activeDuration);

        isActive = false;
        if (hitbox) hitbox.enabled = false;

        yield return new WaitForSeconds(cooldown);

        // 풀로 반환
        if (TrapPool.Instance != null)
        {
            transform.SetParent(TrapPool.Instance.transform, false);
            TrapPool.Instance.TakeObjects(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = null;
        isActive = false;
        if (hitbox) hitbox.enabled = false;
        _hitThisActivation.Clear();
        _lastHitTime.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;

        // Player 태그만 판정
        if (!other.CompareTag("Player")) return;

        if (damageOncePerActivation)
        {
            if (_hitThisActivation.Contains(other)) return;
            DealDamage(other);
            _hitThisActivation.Add(other);
            return;
        }

        float t = Time.time;
        if (_lastHitTime.TryGetValue(other, out float last) && (t - last) < damageTick)
            return;

        DealDamage(other);
        _lastHitTime[other] = t;
    }

    void DealDamage(Collider other)
    {
        if (other.TryGetComponent(out IDamageable dmg))
        {
            dmg.ApplyDamage(damage);
        }
    }
}


