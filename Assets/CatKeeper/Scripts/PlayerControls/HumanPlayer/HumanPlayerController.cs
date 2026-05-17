using Unity.VisualScripting;
using UnityEngine;

namespace CatKeeper.Scripts
{
    [RequireComponent(typeof(PlayerState))]
    public class HumanPlayerController : PlayerController
    {
        [Header("Pick Up Settings")]
        [SerializeField] private float pickUpRange = 3f;
        [Header("Pick Up References")]
        [SerializeField] private Transform objectGrabPointTransform;
        [SerializeField] private LayerMask pickUpLayerMask;
        
        private ObjectGrabbable objectGrabbable;
        private PlayerState playerState;
        
        protected override void Awake()
        {
            base.Awake();
            playerState = GetComponent<PlayerState>();
        }
        
        protected override void Update()
        {
            base.Update();
            HandlePickUp();
        }
        
        private void HandlePickUp()
        {
            if (playerLocomotionInput.InteractTriggered) 
            {
                TryPickUp();
                playerLocomotionInput.InteractTriggered = false;
            }
        }

        private void TryPickUp()
        {
            if (playerState.CurrentPlayerHandState == PlayerHandState.Empty)
            { 
                PickUp();
            }
            else if(playerState.CurrentPlayerHandState == PlayerHandState.Holding)
            {
                Drop();
            }
        }
        
        private void PickUp()
        {
            if (Physics.Raycast(cinemachineCam.transform.position, cinemachineCam.transform.forward, out RaycastHit raycastHit, pickUpRange, pickUpLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                {
                    objectGrabbable.Grab(objectGrabPointTransform);
                    playerState.SetPlayerHandState(PlayerHandState.Holding);
                }
            }
        }
        private void Drop()
        {
            objectGrabbable.Drop();
            objectGrabbable = null;
            playerState.SetPlayerHandState(PlayerHandState.Empty);
        }

        protected override void SetupCamera()
        {
            base.SetupCamera();
            objectGrabPointTransform.SetParent(cinemachineCam.transform, false);
        }
    }
}
