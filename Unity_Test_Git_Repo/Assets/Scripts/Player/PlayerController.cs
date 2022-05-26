using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public Vector3 _velocity;


    //[SerializeField] private GameObject bulletSpawnPoint;
    //[SerializeField] private float waitTime;
    //[SerializeField] private GameObject bullet;
    Animator animator;
    // Start is called before the first frame update

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        //GatherInput();
        //Aim();
        //Look();
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity.normalized.magnitude * Time.fixedDeltaTime * _velocity.ToIso());
        Move(_velocity);
    }

    public void Move(Vector3 velocity)
    {
        //_rb.MovePosition(transform.position + (transform.forward * _input.magnitude) * _speed * Time.deltaTime);
        //_rb.MovePosition(transform.position + _input.ToIso() * _input.normalized.magnitude * _speed * Time.deltaTime);
        _velocity = velocity;
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectPoint);
    }

    #region Inainte
    //private void GatherInput()
    //{

    //    _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxis("Vertical"));
    //    if (_input == Vector3.zero)
    //        animator.SetBool("isWalking", false);
    //    else
    //        animator.SetBool("isWalking", true);

    //    if (Input.GetMouseButton(0))
    //    {
    //        //Shoot();
    //        //Invoke("Shoot", waitTime);
    //        gunController.Shoot();
    //    }
    //}

    //private void Aim()
    //{
    //    var (success, position) = GetMousePosition();
    //    if (success)
    //    {
    //        // Calculate the direction
    //        var direction = position - transform.position;

    //        // You might want to delete this line.
    //        // Ignore the height difference.
    //        direction.y = 0;

    //        // Make the transform look in the direction.
    //        transform.forward = direction;
    //    }
    //}

    //private (bool success, Vector3 position) GetMousePosition()
    //{
    //    var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

    //    if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
    //    {
    //        // The Raycast hit something, return with the position.
    //        return (success: true, position: hitInfo.point);
    //    }
    //    else
    //    {
    //        // The Raycast did not hit anything.
    //        return (success: false, position: Vector3.zero);
    //    }
    //}
    //private void Look()
    //{
    //    //if (_input == Vector3.zero) return;

    //    //var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
    //    //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);


    //    //Quaternion rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
    //    //_model.rotation = Quaternion.RotateTowards(_model.rotation, rot, _turnSpeed * Time.deltaTime);


    //}

    //private void Shoot()
    //{
    //    Instantiate(bullet.transform, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
    //}

    #endregion







}
