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
        private Collider[] holderColliders;
        private Transform objectGrabPointTransform;
        private readonly NetworkVariable<bool> isGrabbed = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        [Header("Object Speed Settings")]
        [SerializeField] private float maxSpeed = 35f;
        [SerializeField] private float springStrength = 30f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float snapDistance = 2f;
        [SerializeField] private float heldLinearDamping = 1f;
        [SerializeField] private float heldAngularDamping = 5f;

        private void Awake()
        {
            objectRigidbody = GetComponent<Rigidbody>();
            objectColliders = GetComponentsInChildren<Collider>();
            objectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            objectRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        
        public void Grab(Transform grabPoint)
        {
            if (!IsServer || IsGrabbed) return;
            
            objectGrabPointTransform = grabPoint;
            isGrabbed.Value = true;
            objectRigidbody.useGravity = false;
            
            objectRigidbody.linearDamping = heldLinearDamping; 
            objectRigidbody.angularDamping = heldAngularDamping;
            
            SetHolderCollisionsIgnored(true);
        }

        public bool IsGrabbed => isGrabbed.Value;
        
        public void Drop()
        {
            if (!IsServer || !IsGrabbed) return;
            
            objectGrabPointTransform = null;
            isGrabbed.Value = false;
            objectRigidbody.useGravity = true;
            objectRigidbody.linearVelocity = Vector3.zero;
            
            objectRigidbody.linearDamping = 0f;
            objectRigidbody.angularDamping = 0.05f;
            
            SetHolderCollisionsIgnored(false);
        }

        private void FixedUpdate()
        {
            if (!IsServer) return;
            
            HandleObjectPosition();
        }

        private void HandleObjectPosition()
        {
            if (objectGrabPointTransform == null) return;
            Vector3 displacement = objectGrabPointTransform.position - objectRigidbody.position;
            float distance = displacement.magnitude;
            
            if (distance > snapDistance)
            {
                objectRigidbody.position = objectGrabPointTransform.position;
                objectRigidbody.linearVelocity = Vector3.zero;
                return;
            }
            
            Vector3 targetVelocity = displacement * springStrength;
            objectRigidbody.linearVelocity = Vector3.ClampMagnitude(targetVelocity, maxSpeed);

            float targetY = objectGrabPointTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);
            objectRigidbody.MoveRotation(
                Quaternion.Slerp(objectRigidbody.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed)
            );
        }
        
        private void SetHolderCollisionsIgnored(bool ignore)
        {
            if (ignore)
            {
                holderColliders = objectGrabPointTransform.GetComponentsInParent<Collider>();
            }
            
            if (objectColliders == null || holderColliders == null) return;
            
            foreach (Collider objectCollider in objectColliders)
            {
                foreach (Collider holderCollider in holderColliders)
                {
                    if (objectCollider == null || holderCollider == null) continue;
                    
                    Physics.IgnoreCollision(objectCollider, holderCollider, ignore);
                }
            }
            
            if (!ignore)
            {
                holderColliders = null;
            }
        }
    }
}
