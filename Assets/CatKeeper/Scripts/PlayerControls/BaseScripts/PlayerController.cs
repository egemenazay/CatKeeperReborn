using System;
using UnityEngine;

namespace CatKeeper.Scripts
{
    [RequireComponent(typeof(PlayerLocomotionInput))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerController : MonoBehaviour
    {
        //THIS SCRIPTS ONLY CONTROLS PLAYER MOVEMENT, MAKE INHERITED SCRIPTS FOR SPECIFIC CHARACTER 
        protected PlayerLocomotionInput playerLocomotionInput;
        private Vector2 moveInput;
        private Vector2 lookInput;
        private float speedMultiplier = 5f;
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 1f;
        [SerializeField] private float gravityMultiplier= 1f;
        
        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTransform;
        
        [Header("Ground Check Settings")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private LayerMask groundLayer;
        
        private bool isGrounded;
        private Rigidbody rb;
        private Vector3 moveDirection;
        private Vector3 slopeMoveDirection;
        
        protected virtual void Awake()
        {
            playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            rb = GetComponent<Rigidbody>();
            
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void Start()
        {
            cameraTransform.transform.SetParent(null);
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }
        protected virtual void Update()
        {
            ReadInputs();
            isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
            slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        }

        RaycastHit slopeHit;
        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.5f))
            {
                if (slopeHit.normal != Vector3.up)
                {
                    return true;
                }
            }
            return false;
        }
        
        private void ReadInputs()
        {
            moveInput = playerLocomotionInput.MovementInput;
            lookInput = playerLocomotionInput.LookInput;
        }
        private void HandleMovement()
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
    
            if (cameraForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward.normalized);
                rb.MoveRotation(targetRotation);
            }

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            if (!isGrounded)
            {
                rb.useGravity = true;
                Vector3 gravityForce = Vector3.up * Physics.gravity.y * gravityMultiplier;
                rb.AddForce(gravityForce, ForceMode.Acceleration);
                moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
                const float airSpeed = 2f;
                rb.AddForce(moveDirection.normalized * airSpeed * speedMultiplier, ForceMode.Acceleration); 
            } 
            else if (isGrounded)
            {
                rb.useGravity = false;
                moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
                
                if (!OnSlope())
                {
                    rb.AddForce(moveDirection.normalized * walkSpeed * speedMultiplier, ForceMode.Acceleration);   
                }
                else
                {
                    rb.AddForce(slopeMoveDirection.normalized * walkSpeed * speedMultiplier, ForceMode.Acceleration);
                }
            }
        }
    }
}
