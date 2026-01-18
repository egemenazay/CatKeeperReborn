using UnityEngine;
using UnityEngine.InputSystem;

namespace CatKeeper.Scripts
{
    public class FPSPlayerController : MonoBehaviour
    {
        private CharacterController controller;
        private PlayerControls inputActions;
        
        [Header("Movement Settings")]
        public float moveSpeed = 5f;

        [Header("Look Settings")]
        public float mouseSensitivity = 15f;
        public Transform cameraTransform;
        
        private Vector2 moveInput;
        private Vector2 lookInput;
        private float xRotation = 0f;
        
        private void Awake()
        {
            controller = GetComponent<CharacterController>();   
            inputActions = new PlayerControls();
        }
        
        private void OnEnable()
        {
            inputActions.PlayerLocomotionMap.Enable();
        }
        
        private void OnDisable()
        {
            inputActions.PlayerLocomotionMap.Disable();
        }

        private void Update()
        {
            ReadInputs();

            HandleMovement();
            HandleLook();
        }

        private void ReadInputs()
        {
            // Pulling input data from map  
            moveInput = inputActions.PlayerLocomotionMap.Movement.ReadValue<Vector2>();
            lookInput = inputActions.PlayerLocomotionMap.Look.ReadValue<Vector2>();
        }

        private void HandleMovement()
        {
            // WASD girdisi (x,y) -> 3D Dünya hareketine (x,z) çevrilmeli.
            // transform.right: Karakterin sağı, transform.forward: Karakterin önü
            // Bu sayede karakter nereye bakıyorsa "W" oraya götürür.
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

            // CharacterController'ı hareket ettir
            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        private void HandleLook()
        {
            // --- 1. Sağa/Sola Bakış (Gövde Döner) ---
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            // Tüm karakteri Y ekseninde döndür
            transform.Rotate(Vector3.up * mouseX);

            // --- 2. Yukarı/Aşağı Bakış (Sadece Kamera Döner) ---
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        
            xRotation -= mouseY; // Yukarı bakmak için eksi (ters mantık)
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Boynun 360 derece dönmesini engelle (Clamp)

            // Sadece kameranın açısını değiştiriyoruz
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}

