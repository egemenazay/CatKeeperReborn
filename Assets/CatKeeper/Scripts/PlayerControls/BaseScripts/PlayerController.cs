using UnityEngine;

namespace CatKeeper.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        //THIS SCRIPTS ONLY CONTROLS PLAYER MOVEMENT, MAKE INHERITED SCRIPTS FOR SPECIFIC CHARACTER 
        protected PlayerLocomotionInput playerLocomotionInput;
        private Vector2 moveInput;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;

        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTransform;
        
        private Rigidbody rb;
        
        protected virtual void Awake()
        {
            playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            rb = GetComponent<Rigidbody>();
            
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }
        protected virtual void Update()
        {
            ReadInputs();
        }
        
        private void ReadInputs()
        {
            moveInput = playerLocomotionInput.MovementInput;
        }
        private void HandleMovement()
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            
            if (cameraForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                rb.MoveRotation(targetRotation);
            }
            
            Vector3 moveDirection = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
            moveDirection.y = 0f;
            rb.AddForce(moveDirection * walkSpeed, ForceMode.VelocityChange);
        }
    }
}
