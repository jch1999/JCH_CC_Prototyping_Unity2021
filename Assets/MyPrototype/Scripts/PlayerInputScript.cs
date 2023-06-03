using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
namespace MyPrototype
{
    public class PlayerInputScript : MonoBehaviour
    {
        [Header("CharacterInput Values")]
        [Tooltip("Character Move Input")]
        public Vector2 move;
        [Tooltip("Character Rotate Input")]
        public Vector2 look;
        [Tooltip("Character Rotate On/Off Input")]
        public bool rotatable=true;
        [Tooltip("Character Jump Input")]
        public bool jump;
        [Tooltip("Character Sprint Input")]
        public bool sprint;

        //analogMovement? 어디에 쓰이는 거람?
        [Header("Movement Setting")]
        public bool analogMovement;

        //PC로만 할 예정이니까 ios랑 android는 제거...
        //아 몰라 그냥 추가해!
#if !UNITY_IOS&&!UNITY_ANDROID
        [Header("Mouse Cursor Setting")]
        public bool cursorLocked=true;
        public bool cursurInputForLock=true;
#endif

        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if(rotatable)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnLookLock(InputValue value)
        {
            LookLockInput();
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
        
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
            //Debug.Log("MoveInput: "+newMoveDirection.ToString());
        } 

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
            //Debug.Log("LookInput: "+newLookDirection.ToString());
        }

        public void LookLockInput()
        {
            rotatable=!rotatable;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
            //Debug.Log("JumpInput: "+newJumpState.ToString());
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
            //Debug.Log("SprintInput: "+newSprintState.ToString());
        }
        
#if !UNITY_IOS&&!UNITY_ANDROID
        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState=newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
#endif

        public bool isExplain=false;
        public void OnExplain()
        {
            ExplainActive();
        }

        public void ExplainActive()
        {
            isExplain=true;
        }
    }
}