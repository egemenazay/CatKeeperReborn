using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace CatKeeper.Scripts
{
    [RequireComponent(typeof(PlayerLocomotionInput))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerController : NetworkBehaviour
    {
        //THIS SCRIPTS ONLY CONTROLS PLAYER MOVEMENT, MAKE INHERITED SCRIPTS FOR SPECIFIC CHARACTER 
        protected PlayerLocomotionInput playerLocomotionInput;
        private Vector2 moveInput;
        private readonly float speedMultiplier = 5f;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 1f;
        [SerializeField] private float gravityMultiplier= 1f;
        
        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTrackTarget;
        [SerializeField] private float cameraXMultiplier;
        [SerializeField] private float cameraYMultiplier;
        [SerializeField] private GameObject visualMesh;
        [SerializeField] private GameObject playerHeadObject;
        
        [Header("Ground Check Settings")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private LayerMask groundLayer;
        
        protected CinemachineCamera cinemachineCam;
        private GameObject cameraObject;
        private bool isGrounded;
        private Rigidbody rb;
        private Vector3 moveDirection;
        private Vector3 slopeMoveDirection;


        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                SetLocalOwnerVisualsVisible(false);
                SetupCamera();
            }
            else
            {
                rb.isKinematic = true;
                
                if (playerLocomotionInput != null)
                {
                    playerLocomotionInput.enabled = false;
                }
            }
        }

        private void SetLocalOwnerVisualsVisible(bool isVisible)
        {
            if (visualMesh == null) return;

            Renderer[] renderers = visualMesh.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = isVisible;
            }
        }

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
            if (!IsOwner) return;
            
            HandleMovement();
            HandleLook();
        }
        
        protected virtual void Update()
        {
            if (!IsOwner) return;
            ReadInputs();
            isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
            slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        }
        
        private void ReadInputs()
        {
            moveInput = playerLocomotionInput.MovementInput;
        }
        
        private void HandleLook()
        {
            if (cinemachineCam == null) return;
            
            Vector3 camEuler = cinemachineCam.transform.rotation.eulerAngles;
            float targetY = camEuler.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);
            rb.MoveRotation(targetRotation);
        }
        
        private void HandleMovement()
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            if (!isGrounded)
            {
                rb.useGravity = true;
                Vector3 gravityForce = Vector3.up * (Physics.gravity.y * gravityMultiplier);
                rb.AddForce(gravityForce, ForceMode.Acceleration);
        
                moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
                const float airSpeed = 2f;
                rb.AddForce(moveDirection * (airSpeed * speedMultiplier), ForceMode.Acceleration); 
            } 
            else 
            {
                rb.useGravity = false;
        
                moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
                
                if (!OnSlope())
                {
                    rb.AddForce(moveDirection * (walkSpeed * speedMultiplier), ForceMode.Acceleration);   
                }
                else
                {
                    rb.AddForce(slopeMoveDirection.normalized * (walkSpeed * speedMultiplier), ForceMode.Acceleration);
                }
            }
        }
        
        protected virtual void SetupCamera()
        {
            cameraObject = GameObject.FindWithTag("MainCamera");
            if (cameraObject != null)
            {
                cinemachineCam = cameraObject.GetComponent<CinemachineCamera>();
                if (cinemachineCam != null)
                {
                    cinemachineCam.Follow = cameraTrackTarget;
                }
            }
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
    }
}
