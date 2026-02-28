using System;
using UnityEngine;

namespace CatKeeper.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController controller;
        private PlayerLocomotionInput playerLocomotionInput;
        private PlayerInteraction playerInteraction;
        
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
        
        
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            playerInteraction = GetComponent<PlayerInteraction>();
        }

        private void Update()
        {
            ReadInputs();
            
            HandlePickUp();
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
            Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
            velocity += gravity * gravityMultiplier * Time.deltaTime;
            move.y = velocity;
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

        private void HandlePickUp()
        {
            if (playerLocomotionInput.InteractTriggered) 
            {
                playerInteraction.TryPickUp();
                playerLocomotionInput.InteractTriggered = false;
            }
        }
    }
}