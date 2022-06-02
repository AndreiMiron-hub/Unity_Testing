using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    [SerializeField] public float _moveSpeed = 1f;
    [SerializeField]  LayerMask groundMask;
    Camera viewCamera;

    private PlayerController controller;
    private GunController gunController;

    [SerializeField] private Transform crossHair;
    public Animator animatorPlayer;


    Vector3 camForward;
    Vector3 move;
    Vector3 moveInput;

    float forwardAmount;
    float turnAmount;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>(); 
        gunController = GetComponent<GunController>();
        animatorPlayer = GetComponent<Animator>();

        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        

        // Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            controller.LookAt(point);
            crossHair.position = point;
            
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }
        }

        // Weapon input
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }
    }

    private void FixedUpdate()
    {
        // Movement Input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * _moveSpeed;

        if (viewCamera.transform != null)
        {
            camForward = Vector3.Scale(viewCamera.transform.up, new Vector3(1, 0, 1)).normalized;
            move = moveInput.z * camForward + moveInput.x * viewCamera.transform.right;
        }
        else
        {
            move = moveInput.z * Vector3.forward + moveInput.x * Vector3.right;
        }

        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        Move(move);

        controller.Move(moveVelocity);
    }

    private void Move(Vector3 move)
    {
        if (move.magnitude > 1)
        {
            move.Normalize();
        }
        this.moveInput = move;
        ConvertMoveInput();
        UpdateAnimator();

    }

    private void ConvertMoveInput()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveInput);
        turnAmount = localMove.x;
        forwardAmount = localMove.z;
    }

    private void UpdateAnimator()
    {
        animatorPlayer.SetFloat("forward", forwardAmount, 0.1f, Time.deltaTime);
        animatorPlayer.SetFloat("turn", turnAmount, 0.1f, Time.deltaTime);
    }

}
