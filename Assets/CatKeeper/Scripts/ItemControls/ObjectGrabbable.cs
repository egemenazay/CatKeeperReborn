using Unity.Netcode;
using UnityEngine;

namespace CatKeeper.Scripts
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(Rigidbody))]
    public class ObjectGrabbable : NetworkBehaviour
    {
        private Rigidbody objectRigidbody;
        private Transform objectGrabPointTransform;

        [Header("Object Speed Settings")]
        [SerializeField] private float maxSpeed = 15f;
        [SerializeField] private float springStrength = 15f;
        [SerializeField] private float rotationSpeed = 15f;
        private void Awake()
        {
            objectRigidbody = GetComponent<Rigidbody>();
            objectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            objectRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        
        public void Grab(Transform grabPoint)
        {
            objectGrabPointTransform = grabPoint;
            objectRigidbody.useGravity = false;
            
            objectRigidbody.linearDamping = 5f; 
            objectRigidbody.angularDamping = 15f;
        }

        public bool IsGrabbed => objectGrabPointTransform != null;
        
        public void Drop()
        {
            objectGrabPointTransform = null;
            objectRigidbody.useGravity = true;
            
            objectRigidbody.linearDamping = 0f;
            objectRigidbody.angularDamping = 0.05f;
        }

        private void FixedUpdate()
        {
           HandeObjectPosition();
        }

        private void HandeObjectPosition()
        {
            if (objectGrabPointTransform == null) return;
            Vector3 displacement = objectGrabPointTransform.position - objectRigidbody.position;
            Vector3 springForce  = displacement * springStrength;

            Vector3 dampingForce = -objectRigidbody.linearVelocity;

            Vector3 totalForce = springForce + dampingForce;
            objectRigidbody.AddForce(totalForce, ForceMode.Force);

            if (objectRigidbody.linearVelocity.magnitude > maxSpeed)
                objectRigidbody.linearVelocity = objectRigidbody.linearVelocity.normalized * maxSpeed;

            float targetY = objectGrabPointTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);
            objectRigidbody.MoveRotation(
                Quaternion.Slerp(objectRigidbody.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed)
            );
        }
    }
}
