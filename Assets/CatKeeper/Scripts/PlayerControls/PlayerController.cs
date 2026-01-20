using UnityEngine;
using UnityEngine.InputSystem;

namespace CatKeeper.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController controller;
        private PlayerLocomotionInput playerLocomotionInput;
        private PlayerState playerState;

        // --- Ayarlar ---
        [Header("Movement Settings")] 
        public float moveSpeed = 5f;

        [Header("Look Settings")] 
        public float mouseSensitivity = 15f;
        
        public Transform cameraTransform; // Kamerayı buraya sürükleyeceğiz

        // --- Değişkenler ---
        private Vector2 moveInput;
        private Vector2 lookInput;
        private float xRotation = 0f;
        
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        }

        private void Update()
        {
            ReadInputs();

            HandleMovement();
            HandleLook();
        }
        
        private void ReadInputs()
        {
            moveInput = playerLocomotionInput.MovementInput;
            lookInput = playerLocomotionInput.LookInput;
        }

        private void HandleMovement()
        {
            Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized; //Unity'de X ekseni sağ ve sol; Z ekseni ileri geriyi temsil ediyor 

            controller.Move(move * moveSpeed * Time.deltaTime);
            
        }

        private void HandleLook()
        {
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX);

            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}