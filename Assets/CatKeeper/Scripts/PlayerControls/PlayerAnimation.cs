using UnityEngine;

namespace CatKeeper.Scripts
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float locomotionBlendSpeed = 4f;
        
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

        private void UpdateAnimationState()
        {
            currentBlendInput = Vector3.Lerp(currentBlendInput, playerLocomotionInput.MovementInput, locomotionBlendSpeed * Time.deltaTime);

            animator.SetFloat(inputXHash, currentBlendInput.x);
            animator.SetFloat(inputYHash, currentBlendInput.y);
            
            playerState.SetPlayerMovementState(PlayerMovementState.Walking);
        }
    }
}
