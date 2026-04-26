using System;
using UnityEngine;

namespace CatKeeper.Scripts
{
    public class ObjectGrabbable : MonoBehaviour
    {
        private Rigidbody objectRigidbody;
        private Transform objectGrabPointTransform;

        [Header("Object Speed Settings")]
        [SerializeField] private float maxSpeed = 15f;
        [SerializeField] private float springStrenght = 15f;
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
            objectRigidbody.angularDamping = 5f;
        }

        public bool IsGrabbed => objectGrabPointTransform != null;
        
        public void Drop()
        {
            objectGrabPointTransform = null;
            objectRigidbody.useGravity = true;
            
            objectRigidbody.linearDamping = 0f;
            objectRigidbody.angularDamping = 0.05f;
        }

        [Obsolete("Obsolete")]
        private void FixedUpdate()
        {
           HandeObjectPosition();
        }

        private void HandeObjectPosition()
        {
            if (objectGrabPointTransform == null) return;
            Vector3 displacement = objectGrabPointTransform.position - objectRigidbody.position;
            Vector3 springForce  = displacement * springStrenght;

            Vector3 dampingForce = -objectRigidbody.linearVelocity * 15f;

            Vector3 gravityCounter = -Physics.gravity * objectRigidbody.mass;

            Vector3 totalForce = springForce + dampingForce + gravityCounter;
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
