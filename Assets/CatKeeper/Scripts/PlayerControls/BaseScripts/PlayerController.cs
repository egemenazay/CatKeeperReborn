using UnityEngine;

namespace CatKeeper.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerLocomotionInput playerLocomotionInput;
        private HumanPlayerInteraction humanPlayerInteraction;
        private Vector2 moveInput;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;

        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTransform;
        
        private Rigidbody rb;
        
        private void Awake()
        {
            playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            rb = GetComponent<Rigidbody>();
            humanPlayerInteraction = GetComponent<HumanPlayerInteraction>();
            
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }
        private void Update()
        {
            ReadInputs();
            HandlePickUp();
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
        
        //BOTTOM FUNCTIONS GONA BE ON INDIVIDUAL CHRACTERS SCRPITS FOR NOW ITS ONLY TESTING
        private void HandlePickUp()
        {
            
            if (playerLocomotionInput.InteractTriggered) 
            {
                humanPlayerInteraction.TryPickUp();
                playerLocomotionInput.InteractTriggered = false;
            }
        }
    }
}
