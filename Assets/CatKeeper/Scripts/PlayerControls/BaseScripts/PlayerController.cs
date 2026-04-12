using UnityEngine;

namespace CatKeeper.Scripts
{
    [RequireComponent(typeof(PlayerLocomotionInput))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerController : MonoBehaviour
    {
        //THIS SCRIPTS ONLY CONTROLS PLAYER MOVEMENT, MAKE INHERITED SCRIPTS FOR SPECIFIC CHARACTERS
        
        protected PlayerLocomotionInput playerLocomotionInput;
        private Vector2 moveInput;
        
        [Header("Movement Settings")]
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float deceleration = 5f;

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
            RaycastHit hit;
            Vector3 groundNormal = Vector3.up;
            
            if (cameraForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                rb.MoveRotation(targetRotation);
            }
            
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
            {
                groundNormal = hit.normal;
            }
            if (moveInput != Vector2.zero)
            {
                Vector3 moveDirection = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
                moveDirection.y = 0f;
                moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
                rb.AddForce(moveDirection * acceleration, ForceMode.VelocityChange);
            }
        }
    }
}
