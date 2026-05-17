using UnityEngine;

namespace CatKeeper.Scripts
{
    public class CharacterSpawner : MonoBehaviour
    {
        public GameObject playerPrefab;
        public Transform spawnPoint;
        

        private GameObject spawnedPlayer;

        void Update()
        {
            // Press E to spawn
            if (Input.GetKeyDown(KeyCode.E))
            {
                TrySpawnPlayer();
            }
        }

        void TrySpawnPlayer()
        {
            // If already spawned → do nothing
            if (spawnedPlayer != null)
            {
                Debug.Log("Player already spawned!");
                return;
            }

            // Spawn player
            spawnedPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

            Debug.Log("Player spawned!");
        }

        private void ButtonPress()
        {
            
        }
    }
}
