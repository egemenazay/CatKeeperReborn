using System;
using UnityEngine;

namespace CatKeeper.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController controller;
        protected PlayerLocomotionInput playerLocomotionInput;
        
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        
        [Header("Movement Settings")] 
        private Vector2 moveInput;
        private Vector2 lookInput;
        private float gravity = -10f;
        private float velocity;
        [SerializeField] private float gravityMultiplier = 2.5f;
        [SerializeField] private float moveSpeed = 5f;
        
        [Header("Look Settings")] 
        [SerializeField]private float mouseSensitivity = 15f;
        

        
        private float xRotation = 0f;
        
        
        protected virtual void Awake()
        {
            controller = GetComponent<CharacterController>();
            playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        }

        protected virtual void Update()
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
            if (controller.isGrounded && velocity < 0)
            {
                // Tam sıfır yerine küçük bir negatif değer (-2f gibi) 
                // vermek yerle teması (grounding) daha stabil tutar.
                velocity = -2f; 
            }

            Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
            
            velocity += gravity * gravityMultiplier * Time.deltaTime;
            Vector3 finalMove = move * moveSpeed;
            finalMove.y = velocity;

            controller.Move(finalMove * Time.deltaTime);
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