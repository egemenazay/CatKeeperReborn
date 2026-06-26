using Unity.Netcode;
using UnityEngine;

namespace CatKeeper.Scripts
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(Rigidbody))]
    public class ObjectGrabbable : NetworkBehaviour
    {
        private Rigidbody objectRigidbody;
        private Collider[] objectColliders;
        private Transform objectGrabPointTransform;
        private NetworkObject holderNetworkObject;
        private Vector3 previousGrabPointPosition;
        private bool hasPreviousGrabPointPosition;

        [Header("Object Grabbed Speed Settings")]
        [SerializeField] private float maxSpeed = 15f;
        [SerializeField] private float springStrength = 15f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float objectAngularDamping = 15f;

        private void Awake()
        {
            objectRigidbody = GetComponent<Rigidbody>();
            objectColliders = GetComponentsInChildren<Collider>(true);
            objectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            objectRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public override void OnNetworkSpawn()
        {
            ConfigureNetworkPhysicsAuthority();
        }

        private void ConfigureNetworkPhysicsAuthority()
        {
            objectRigidbody.isKinematic = !IsServer;
            objectRigidbody.useGravity = IsServer && objectGrabPointTransform == null;
        }
        
        public void Grab(Transform grabPoint, NetworkObject holder)
        {
            if (!IsServer) return;

            objectGrabPointTransform = grabPoint;
            holderNetworkObject = holder;
            previousGrabPointPosition = grabPoint.position;
            hasPreviousGrabPointPosition = true;

            SetIgnoredHolderCollisions(holderNetworkObject, true);
            ApplyGrabbedRigidbodySettings();
            ApplyGrabbedRigidbodySettingsClientRpc(new NetworkObjectReference(holderNetworkObject));
        }

        public bool IsGrabbed => objectGrabPointTransform != null;
        
        public void Drop()
        {
            if (!IsServer) return;

            NetworkObject droppedByNetworkObject = holderNetworkObject;
            SetIgnoredHolderCollisions(droppedByNetworkObject, false);
            objectGrabPointTransform = null;
            holderNetworkObject = null;
            hasPreviousGrabPointPosition = false;

            ApplyDroppedRigidbodySettings();
            ApplyDroppedRigidbodySettingsClientRpc(new NetworkObjectReference(droppedByNetworkObject));
        }

        [ClientRpc]
        private void ApplyGrabbedRigidbodySettingsClientRpc(NetworkObjectReference holderReference)
        {
            if (IsServer) return;

            if (holderReference.TryGet(out NetworkObject holder))
            {
                SetIgnoredHolderCollisions(holder, true);
            }

            ApplyGrabbedRigidbodySettings();
        }

        [ClientRpc]
        private void ApplyDroppedRigidbodySettingsClientRpc(NetworkObjectReference holderReference)
        {
            if (IsServer) return;

            if (holderReference.TryGet(out NetworkObject holder))
            {
                SetIgnoredHolderCollisions(holder, false);
            }

            ApplyDroppedRigidbodySettings();
        }

        private void SetIgnoredHolderCollisions(NetworkObject holder, bool shouldIgnore)
        {
            if (holder == null) return;

            Collider[] holderColliders = holder.GetComponentsInChildren<Collider>(true);
            foreach (Collider objectCollider in objectColliders)
            {
                foreach (Collider holderCollider in holderColliders)
                {
                    if (objectCollider == null || holderCollider == null || objectCollider == holderCollider)
                        continue;

                    Physics.IgnoreCollision(objectCollider, holderCollider, shouldIgnore);
                }
            }
        }

        private void ApplyGrabbedRigidbodySettings()
        {
            objectRigidbody.isKinematic = !IsServer;
            objectRigidbody.useGravity = false;
            objectRigidbody.linearDamping = 0f;
            objectRigidbody.angularDamping = objectAngularDamping;
        }

        private void ApplyDroppedRigidbodySettings()
        {
            objectRigidbody.isKinematic = !IsServer;
            objectRigidbody.useGravity = IsServer;
            objectRigidbody.linearDamping = 0f;
            objectRigidbody.angularDamping = 0.05f;
        }

        private void FixedUpdate()
        {
            if (!IsServer) return;

            HandleObjectPosition();
        }

        private void HandleObjectPosition()
        {
            if (objectGrabPointTransform == null) return;

            float fixedDeltaTime = Time.fixedDeltaTime;
            Vector3 grabPointPosition = objectGrabPointTransform.position;
            Vector3 grabPointVelocity = hasPreviousGrabPointPosition
                ? (grabPointPosition - previousGrabPointPosition) / fixedDeltaTime
                : Vector3.zero;

            previousGrabPointPosition = grabPointPosition;
            hasPreviousGrabPointPosition = true;

            Vector3 displacement = objectGrabPointTransform.position - objectRigidbody.position;

            float criticalDamping = 2f * Mathf.Sqrt(springStrength * objectRigidbody.mass);
            Vector3 relativeVelocity = objectRigidbody.linearVelocity - grabPointVelocity;
            Vector3 springForce = displacement * springStrength;
            Vector3 dampingForce = -relativeVelocity * criticalDamping;

            objectRigidbody.AddForce(springForce + dampingForce, ForceMode.Force);

            if (objectRigidbody.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed)
                objectRigidbody.linearVelocity = objectRigidbody.linearVelocity.normalized * maxSpeed;

            float targetY = objectGrabPointTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);
            objectRigidbody.MoveRotation(
                Quaternion.Slerp(objectRigidbody.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed)
            );
        }
    }
}
