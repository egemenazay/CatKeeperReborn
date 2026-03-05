using System;
using UnityEngine;

namespace CatKeeper.Scripts
{
    public class ObjectGrabbable : MonoBehaviour
    {
        private Rigidbody objectRigidbody;
        private Transform objectGrabPointTransform;
        [SerializeField] private float followSpeed = 15f;
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
            
            objectRigidbody.linearDamping = 10f; 
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
            if (objectGrabPointTransform != null)
            {
                // ---------------- POZİSYON HESAPLAMASI (HIZ İLE) ----------------
                // Hedefe olan yönü ve mesafeyi bul
                Vector3 direction = objectGrabPointTransform.position - transform.position;
                float distance = direction.magnitude;

                // Objeyi hedefe doğru fırlatır gibi hız veriyoruz.
                // Mesafe ne kadar fazlaysa o kadar hızlı çeker (Yay etkisi)
                objectRigidbody.linearVelocity = direction * followSpeed * distance; // 15f çarpanıyla oynayabilirsin

                // ---------------- ROTASYON HESAPLAMASI (DİK DURMA) ----------------
                
                // Kameranın (HoldPoint) sadece Y eksenindeki açısını alıyoruz.
                // X ve Z'yi 0 yaparak "Dik Durma"yı garantiliyoruz.
                float targetY = objectGrabPointTransform.eulerAngles.y;
                
                // Hedef rotasyon: X=0 (Eğilme yok), Y=Kamera Yönü, Z=0 (Yatma yok)
                Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);

                // Şu anki açıdan hedef açıya yumuşak bir geçiş yap (Slerp)
                Quaternion smoothedRotation = Quaternion.Slerp(objectRigidbody.rotation, targetRotation, Time.fixedDeltaTime * 10f);
                
                // Fiziği bozmadan döndürmek için MoveRotation kullanıyoruz
                objectRigidbody.MoveRotation(smoothedRotation);
            }
        }
    }
}
