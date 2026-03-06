using System;
using UnityEngine;

namespace CatKeeper.Scripts
{
    public class HumanPlayerInteraction : MonoBehaviour
    {
        [Header("Settings")]
        public float pickUpRange = 3f;
        
        [Header("References")]
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
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpRange, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                    {
                        objectGrabbable.Grab(objectGrabPointTransform);
                        playerState.SetPlayerHandState(PlayerHandState.Holding);
                    }
                }
            }
            else if(playerState.CurrentPlayerHandState == PlayerHandState.Holding)
            {
                objectGrabbable.Drop();
                objectGrabbable = null;
                playerState.SetPlayerHandState(PlayerHandState.Empty);
            }
        }
        
    }
}
