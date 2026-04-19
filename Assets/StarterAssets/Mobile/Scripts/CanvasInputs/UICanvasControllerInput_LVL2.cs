using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput_LVL2 : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs_LVL2 starterAssetsInputs_LVL2;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs_LVL2.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs_LVL2.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs_LVL2.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs_LVL2.SprintInput(virtualSprintState);
        }
        
    }

}
