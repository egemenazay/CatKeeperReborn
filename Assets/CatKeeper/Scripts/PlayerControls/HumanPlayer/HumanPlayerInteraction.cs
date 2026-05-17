using System;
using UnityEngine;

namespace CatKeeper.Scripts
{
    public class HumanPlayerInteraction : MonoBehaviour
    {
        [Header("Pick Up Settings")]
        [SerializeField] private float pickUpRange = 3f;
        [Header("Pick Up References")]
        [SerializeField] private Transform objectGrabPointTransform;
        [SerializeField] private Transform playerCameraTransform;
        [SerializeField] private LayerMask pickUpLayerMask;
        
        private ObjectGrabbable objectGrabbable;
        private PlayerState playerState;

        private void Awake()
        {
            playerState = GetComponent<PlayerState>();
        }

        public void TryPickUp()
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
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpRange, pickUpLayerMask))
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
        
    }
}
