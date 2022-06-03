using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    [SerializeField] float _speed = 10;
    [SerializeField] float _damage = 1;
    [SerializeField] float _lifeTime = 1.5f;
    [SerializeField] float _hitBox = .001f;
    [SerializeField] Color trailColor;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
        //Collider[] initialCollisions = Physics.OverlapSphere(transform.position, _hitBox, collisionMask);
        //if (initialCollisions.Length > 0)
        //{
        //    OnHitObject(initialCollisions[0], transform.position);
        //}
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float moveDistance = _speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    } 


    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(_damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
