using System;
using UnityEngine;

namespace CatKeeper.Scripts
{
    public class HumanPlayerAnimation : MonoBehaviour
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
        private PlayerState playerState;

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        
        private Vector3 currentBlendInput = Vector3.zero;

        private void Awake()
        {
            playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            playerState = GetComponent<PlayerState>();
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
            currentBlendInput = Vector3.Lerp(currentBlendInput, playerLocomotionInput.MovementInput, locomotionBlendSpeed * Time.deltaTime);

            animator.SetFloat(inputXHash, currentBlendInput.x);
            animator.SetFloat(inputYHash, currentBlendInput.y);
            
            playerState.SetPlayerMovementState(PlayerMovementState.Walking);
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
