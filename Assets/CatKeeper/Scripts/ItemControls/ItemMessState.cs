using UnityEngine;
using static CatKeeper.Scripts.GameTags;

namespace CatKeeper.Scripts
{
    public class ItemMessState : MonoBehaviour
    {
        [Header("Dağınıklık Ayarları")]
        public float messPenalty = 10f;
        
        private bool isMessy = false; 
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(GameTags.Floor) && !isMessy)
            {
                isMessy = true;
                MessManager.Instance.AddMess(messPenalty);
            }
        }

        // Görünmez bir alanın (Zone) içine girdiğinde çalışır
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlaceZone") && isMessy)
            {
                isMessy = false;
                Debug.Log("Eşya yerine kondu! Dağınıklık azalıyor.");
                MessManager.Instance.AddMess(-messPenalty);
            }
        }
    }
}