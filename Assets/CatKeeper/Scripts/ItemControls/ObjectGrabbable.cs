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
        private Vector3 previousGrabPointPosition;
        private bool hasPreviousGrabPointPosition;
        private bool isSteady;

        [Header("Object Speed Settings")]
        [SerializeField] private float maxSpeed = 15f;
        [SerializeField] private float springStrength = 15f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float steadyDistance = 0.05f;
        [SerializeField] private float objectAngularDamping = 15f;
        private void Awake()
        {
            objectRigidbody = GetComponent<Rigidbody>();
            objectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            objectRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        
        public void Grab(Transform grabPoint)
        {
            objectGrabPointTransform = grabPoint;
            previousGrabPointPosition = grabPoint.position;
            hasPreviousGrabPointPosition = true;
            isSteady = false;
            objectRigidbody.useGravity = false;

            objectRigidbody.linearDamping = 0f;
            objectRigidbody.angularDamping = objectAngularDamping;
        }

        public bool IsGrabbed => objectGrabPointTransform != null;
        
        public void Drop()
        {
            objectGrabPointTransform = null;
            hasPreviousGrabPointPosition = false;
            isSteady = false;
            objectRigidbody.useGravity = true;
            
            objectRigidbody.linearDamping = 0f;
            objectRigidbody.angularDamping = 0.05f;
        }

        private void FixedUpdate()
        {
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
            float distance = displacement.magnitude;

            if (isSteady)
            {
                isSteady = distance <= steadyDistance * 2f;
            }
            else
            {
                isSteady = distance <= steadyDistance;
            }

            if (isSteady)
            {
                Vector3 correctionVelocity = displacement / fixedDeltaTime;
                objectRigidbody.linearVelocity = Vector3.ClampMagnitude(
                    grabPointVelocity + correctionVelocity,
                    maxSpeed
                );
            }
            else
            {
                float criticalDamping = 2f * Mathf.Sqrt(springStrength * objectRigidbody.mass);
                Vector3 relativeVelocity = objectRigidbody.linearVelocity - grabPointVelocity;
                Vector3 springForce = displacement * springStrength;
                Vector3 dampingForce = -relativeVelocity * criticalDamping;

                objectRigidbody.AddForce(springForce + dampingForce, ForceMode.Force);
            }

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
