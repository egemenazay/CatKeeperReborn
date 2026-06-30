using Unity.Netcode;
using UnityEngine;

namespace CatKeeper.Scripts
{
    public class HumanPlayerAnimation : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float locomotionBlendSpeed = 4f;
        public bool invertZ = false; 
        public float zOffset = 0f;
        
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform headBone;
        [SerializeField] private Transform cameraTransform;
        
        private PlayerLocomotionInput playerLocomotionInput;

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        
        private Vector3 currentBlendInput = Vector3.zero;
        private Vector2 lastSentMovementInput = Vector2.zero;
        private NetworkVariable<Vector2> networkMovementInput = new(
            Vector2.zero,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        private void Awake()
        {
            playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void LateUpdate()
        {
            HandleHeadMovement();
        }

        private void UpdateAnimationState()
        {
            Vector2 targetMovementInput = GetTargetMovementInput();

            currentBlendInput = Vector3.Lerp(currentBlendInput, targetMovementInput, locomotionBlendSpeed * Time.deltaTime);

            animator.SetFloat(inputXHash, currentBlendInput.x);
            animator.SetFloat(inputYHash, currentBlendInput.y);
        }

        private Vector2 GetTargetMovementInput()
        {
            if (!IsSpawned)
            {
                return playerLocomotionInput.MovementInput;
            }

            if (!IsOwner)
            {
                return networkMovementInput.Value;
            }

            Vector2 movementInput = playerLocomotionInput.MovementInput;
            SyncMovementInput(movementInput);
            return movementInput;
        }

        private void SyncMovementInput(Vector2 movementInput)
        {
            if ((movementInput - lastSentMovementInput).sqrMagnitude <= 0.0001f)
            {
                return;
            }

            lastSentMovementInput = movementInput;

            if (IsServer)
            {
                networkMovementInput.Value = movementInput;
                return;
            }

            UpdateMovementInputServerRpc(movementInput);
        }

        [ServerRpc]
        private void UpdateMovementInputServerRpc(Vector2 movementInput)
        {
            networkMovementInput.Value = movementInput;
        }

        private void HandleHeadMovement()
        {
            if (headBone == null || cameraTransform == null) return;
            
            float camX = cameraTransform.localEulerAngles.x;
            
            if (camX > 180f)
            {
                camX -= 360f;
            }
            
            camX = Mathf.Clamp(camX, -65, 30);
            
            if (invertZ)
            {
                camX = -camX; 
            }
            
            Vector3 newHeadRotation = new Vector3(headBone.localEulerAngles.x, headBone.localEulerAngles.y, camX + zOffset);
            headBone.localEulerAngles = newHeadRotation;
        }
    }
}
