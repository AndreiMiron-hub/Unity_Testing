using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float _startingHealth;
    protected float _health;
    protected bool _dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        _health = _startingHealth;
    }
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)// folositoare pt a cunoaste unde a lovit 
    {
        TakeDamage(damage);
    }
    public virtual void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0 && !_dead)
        {
            Die();
        }
    }

    [ContextMenu("Suicide")]
    protected void Die()
    {
        _dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }

    
}
