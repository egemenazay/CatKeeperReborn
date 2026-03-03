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
        [Header("Dağınıklık Ayarları")]
        public float messPenalty = 10f;
        
        private bool isMessy = false; 
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(Floor) && !isMessy)
            {
                isMessy = true;
                MessManager.Instance.AddMess(messPenalty);
            }
        }

        // Görünmez bir alanın (Zone) içine girdiğinde çalışır
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlaceZone) && isMessy)
            {
                isMessy = false;
                MessManager.Instance.AddMess(-messPenalty);
            }
        }
    }
}