using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float _startingHealth;
    public float _health { get; protected set; }
    protected bool _dead;

    protected Animator animator;
    private Collider _collider;

    public event System.Action OnDeath;
    bool hasHitAnimation = false;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _health = _startingHealth;

        //hasHitAnimation = ContainsParam(animator, "Hit");
    }
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)// folositoare pt a cunoaste unde a lovit 
    {
        TakeDamage(damage);
    }
    public virtual void TakeDamage(float damage)
    {
        //if (hasHitAnimation)
        //{
        //    animator.SetTrigger("Hit");
        //}
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
        _collider.enabled = false;
        if (OnDeath != null)
        {
            OnDeath();
        }
        animator.SetTrigger("Die");
        var destroyTime = 3f;
        GameObject.Destroy(gameObject, destroyTime);
    }


    public bool ContainsParam(Animator _Anim, string _ParamName)
    {
        foreach (AnimatorControllerParameter param in _Anim.parameters)
        {
            if (param.name == _ParamName) return true;
        }
        return false;
    }

}
