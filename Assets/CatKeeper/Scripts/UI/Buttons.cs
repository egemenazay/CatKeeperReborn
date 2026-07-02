using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace CatKeeper.Scripts
{
    public class Buttons : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;
        [SerializeField] private Button playButton;
        
        [Header("Screens")]
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject playScreen;
        [SerializeField] private GameObject lobbyScreen;
        
        [Header("Menus")]
        [SerializeField] private GameObject startMenu;

        private void Start()
        {
            hostButton.onClick.AddListener(OnHostButtonPressed);
            clientButton.onClick.AddListener(OnClientButtonPressed);
            playButton.onClick.AddListener(OnPlayButtonPressed);
        }

        private void OnHostButtonPressed()
        {
            NetworkManager.Singleton.StartHost();
            CloseStartMenu();
        }

        private void OnClientButtonPressed()
        {
            NetworkManager.Singleton.StartClient();
            CloseStartMenu();
        }

        private void OnPlayButtonPressed()
        {
            lobbyScreen.SetActive(true);
            mainMenuScreen.SetActive(false);
        }

        private void CloseStartMenu()
        {
            startMenu.SetActive(false);
        }
    }
}
