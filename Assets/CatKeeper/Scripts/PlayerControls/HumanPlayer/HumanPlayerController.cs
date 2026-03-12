using System;
using UnityEngine;

namespace CatKeeper.Scripts
{
    public class HumanPlayerController : PlayerController
    {
        private HumanPlayerInteraction humanPlayerInteraction;

        protected override void Awake()
        {
            base.Awake();
            humanPlayerInteraction = GetComponent<HumanPlayerInteraction>();
        }

        protected override void Update()
        {
            base.Update();
            HandlePickUp();
        }
        
        private void HandlePickUp()
        {
            if (playerLocomotionInput.InteractTriggered) 
            {
                humanPlayerInteraction.TryPickUp();
                playerLocomotionInput.InteractTriggered = false;
            }
        }
    }
}
