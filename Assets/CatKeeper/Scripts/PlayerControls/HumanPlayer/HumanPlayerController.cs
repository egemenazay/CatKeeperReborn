using System;
using Unity.Netcode;
using UnityEngine;

namespace CatKeeper.Scripts
{
    [RequireComponent(typeof(PlayerState))]
    public class HumanPlayerController : PlayerController
    {
        [Header("Pick Up Settings")]
        [SerializeField] private float pickUpRange = 3f;
        [SerializeField] private float grabPointDistance = 2f;
        [SerializeField] private float verticalOffset = 0f;
        [SerializeField] private float smoothSpeed = 3f;
        [Header("Pick Up References")]
        [SerializeField] private Transform objectGrabPointTransform;
        [SerializeField] private LayerMask pickUpLayerMask;
        
        private ObjectGrabbable heldObject;
        private PlayerState playerState;
        
        protected override void Awake()
        {
            base.Awake();
            playerState = GetComponent<PlayerState>();
            playerLocomotionInput.InteractPressed += HandleInteractPressed;
        }

        public override void OnDestroy()
        {
            if (playerLocomotionInput != null)
            {
                playerLocomotionInput.InteractPressed -= HandleInteractPressed;
            }
            
            base.OnDestroy();
        }

        private void LateUpdate()
        {
            HandlePickUpPosition();
        }

        private void HandleInteractPressed()
        {
            if (!IsOwner) return;

            if (playerState.CurrentPlayerHandState == PlayerHandState.Holding)
            {
                DropServerRpc();
                return;
            }

            if (TryFindPickupTarget(out ObjectGrabbable target))
            {
                PickUpServerRpc(
                    new NetworkObjectReference(target.NetworkObject),
                    objectGrabPointTransform.localPosition,
                    objectGrabPointTransform.localRotation
                );
            }
        }
        
        private void HandlePickUpPosition()
        {
            if (!IsOwner)
                return;

            if (objectGrabPointTransform == null || cinemachineCam == null)
                return;

            Vector3 targetPosition =
                cinemachineCam.transform.position +
                cinemachineCam.transform.forward * grabPointDistance +
                Vector3.up * verticalOffset; 
            
            objectGrabPointTransform.position = targetPosition;
            // objectGrabPointTransform.position = Vector3.Lerp(
            //     objectGrabPointTransform.position,
            //     targetPosition,
            //     Time.deltaTime * smoothSpeed
            // );

            objectGrabPointTransform.rotation = Quaternion.LookRotation(cinemachineCam.transform.forward, Vector3.up);

            if (!IsServer && IsSpawned && playerState.CurrentPlayerHandState == PlayerHandState.Holding)
            {
                UpdateGrabPointServerRpc(
                    objectGrabPointTransform.localPosition,
                    objectGrabPointTransform.localRotation
                );
            }
        }
        
        private bool TryFindPickupTarget(out ObjectGrabbable target)
        {
            target = null;
            
            if (cinemachineCam == null) return false;

            if (!Physics.Raycast(cinemachineCam.transform.position, cinemachineCam.transform.forward, out RaycastHit raycastHit, pickUpRange, pickUpLayerMask))
            {
                return false;
            }

            target = raycastHit.transform.GetComponentInParent<ObjectGrabbable>();
            return target != null && target.NetworkObject != null;
        }

        [ServerRpc]
        private void PickUpServerRpc(
            NetworkObjectReference targetReference,
            Vector3 grabPointLocalPosition,
            Quaternion grabPointLocalRotation
        )
        {
            if (heldObject != null) return;
            if (!targetReference.TryGet(out NetworkObject targetNetworkObject)) return;
            if (!targetNetworkObject.TryGetComponent(out ObjectGrabbable target)) return;
            if (target.IsGrabbed) return;

            ApplyGrabPointPose(grabPointLocalPosition, grabPointLocalRotation);
            if (!IsTargetInPickupRange(target.transform)) return;

            heldObject = target;
            heldObject.Grab(objectGrabPointTransform);
            SetHandStateClientRpc(PlayerHandState.Holding);
        }

        [ServerRpc]
        private void DropServerRpc()
        {
            if (heldObject == null) return;

            heldObject.Drop();
            heldObject = null;
            SetHandStateClientRpc(PlayerHandState.Empty);
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void UpdateGrabPointServerRpc(Vector3 localPosition, Quaternion localRotation)
        {
            if (heldObject == null) return;

            ApplyGrabPointPose(localPosition, localRotation);
        }

        private void ApplyGrabPointPose(Vector3 localPosition, Quaternion localRotation)
        {
            if (objectGrabPointTransform == null) return;

            float maxLocalDistance = pickUpRange + 1f;
            objectGrabPointTransform.localPosition = Vector3.ClampMagnitude(localPosition, maxLocalDistance);
            objectGrabPointTransform.localRotation = localRotation.normalized;
        }

        [ClientRpc]
        private void SetHandStateClientRpc(PlayerHandState handState)
        {
            playerState.SetPlayerHandState(handState);
        }

        private bool IsTargetInPickupRange(Transform target)
        {
            float maxPickupDistance = pickUpRange + 0.5f;
            Vector3 pickupOrigin = objectGrabPointTransform != null ? objectGrabPointTransform.position : transform.position;
            
            if (Vector3.Distance(pickupOrigin, target.position) <= maxPickupDistance)
            {
                return true;
            }

            return Vector3.Distance(transform.position, target.position) <= maxPickupDistance;
        }
    }
}
