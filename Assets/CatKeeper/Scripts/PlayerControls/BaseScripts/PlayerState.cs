using UnityEngine;

namespace CatKeeper.Scripts
{
    public enum PlayerMovementState
    {
        Idle = 0,
        Walking = 1,
    }
    public enum PlayerHandState 
    {
        Empty = 0,
        Holding = 1,
    }
    
    public class PlayerState : MonoBehaviour
    {
        [field:SerializeField] public PlayerMovementState CurrentPlayerMovementState {get; private set;} =  PlayerMovementState.Idle;
        [field:SerializeField] public PlayerHandState CurrentPlayerHandState {get; private set;} =  PlayerHandState.Empty;
        
        public void SetPlayerMovementState(PlayerMovementState playerMovementState)
        {
            CurrentPlayerMovementState = playerMovementState;
        }
        public void SetPlayerHandState(PlayerHandState playerHandState)
        {
            CurrentPlayerHandState = playerHandState;
        }
        
    }
}
