using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking};
    State currentState;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;

    // This enemy particle
    public ParticleSystem deathEffect;
    public ParticleSystem hitEffect;

    float attackDistanceThreshold = .5f;
    float timeBetweenAtacks = 1;
    float nextAtackTime;
    float damage = 1;

    float attackerCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            attackerCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //animator = GetComponent<Animator>();

        if (hasTarget)
        {
            currentState = State.Chasing;
            Chasing();
            targetEntity.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }
        else
            Idle();

    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget && !_dead)
        {
            if (Time.time > nextAtackTime && target != null)
            {
                float sqrtDistanceToTarget = (target.position - transform.position).sqrMagnitude;

                if (sqrtDistanceToTarget < Mathf.Pow(attackDistanceThreshold + attackerCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAtackTime = Time.time + timeBetweenAtacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
        
    }

    
    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth)
    {
        pathfinder.speed = moveSpeed;

        if (hasTarget && !_dead)
        {
            damage = Mathf.Ceil(targetEntity._startingHealth / hitsToKillPlayer);
        }
        _startingHealth = enemyHealth;
        // mai pot face cod
    }
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        
        if (!_dead)
        {
            if (damage >= _health)
            {
                AudioManager.instance.PlaySound("Enemy Death", transform.position);
                Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
            }
            else
            {
                Destroy(Instantiate(hitEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
            }
        }
        AudioManager.instance.PlaySound("Impact", transform.position);
        base.TakeHit(damage, hitPoint, hitDirection);
    }
    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - directionToTarget * (attackerCollisionRadius + targetCollisionRadius);
        Atack();

        float percent = 0; // value 0 to 1
        float attackSpeed = 3;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;

            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        currentState = State.Chasing;
        pathfinder.enabled = true;

    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while(hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (attackerCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!_dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
                else if (_dead)
                {
                    pathfinder.isStopped = true;
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }


    #region Animations

    private void Idle()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private void Chasing()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", 0.5f);
        }
        
    }

    private void Atack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    #endregion
}
