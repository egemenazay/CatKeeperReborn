using UnityEngine;
using static CatKeeper.Scripts.GameTags;

namespace CatKeeper.Scripts
{
    public static class GameTags
    {
        public const string Floor = "Floor";
        public const string PlaceZone = "PlaceZone";
        public const string Pickable = "Pickable";
        public const string Player = "Player";
    }
    public class ItemMessState : MonoBehaviour
    {
        [Header("Mess Point Settings")]
        public float messPenalty = 10f;
        
        [Header("Rest Point Settings")]
        [SerializeField] private float restVelocityThreshold = 0.5f;
        
        private bool isMessy = false;
        private ObjectGrabbable objectGrabbable;
        private Rigidbody objectRigidbody;

        private void Awake()
        {
            objectGrabbable = GetComponent<ObjectGrabbable>();
            objectRigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(Floor) && !isMessy)
            {
                isMessy = true;
                MessManager.Instance.AddMess(messPenalty);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag(PlaceZone)) return;
            if (!isMessy) return;
            if (objectGrabbable.IsGrabbed) return;
            if (objectRigidbody.linearVelocity.sqrMagnitude > restVelocityThreshold * restVelocityThreshold) return;

            isMessy = false;
            MessManager.Instance.AddMess(-messPenalty);
        }
    }
}