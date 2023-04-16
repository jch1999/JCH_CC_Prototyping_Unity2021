//FirstPersonController 참조
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace MyPrototype
{
    [RequireComponent(typeof(CharacterController))]
    #if ENABLE_INPUT_SYSTEM&&STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
    #endif

    public class CharacterController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character")]
        public float MoveSpeed=4.0f;
        [Tooltip("Sprint speed of the character")]
        public float SprintSpeed=6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RoatationSpeed=6.0f;
        [Tooltip("Acceleration or deacceleration")]
        public float SpeedChangeRate=10.0f;

        //높이 뛰고 빨리 떨어진다?
        [Space(10)]
        [Tooltip("Jump height of the character")]
        public float JumpHeight=1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("JumpCoolTime")]
        public float jumpCool=0.1f;
        [Tooltip("Time required to pass before entering the fallState")]
        public float fallTimeout=0.15f;

        [Header("Player Grounded")]
        [Tooltip("Is Character on the ground?,If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded=true;
        [Tooltip("Useful for teh rough ground.")]//지면과의 거리의 오차값?
        public float GroundedOffset=-0.14f;
        [Tooltip("The radius of the grounded check. Should match with radius of ChracterController.")]
        public float GroundedRadius=0.5f;
        [Tooltip("What layer character uses as Ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        void Awake()
        {
            
        }
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void LateUpdate()
        {
            
        }

        //땅과의 충돌여부를 확인
        private void GroundedCheck()
        {
            //GroundedOffset만큼 현재 위치에서 y 값을 감소시킨 지점을 원점으로 구를 생성해서
            //GroundedLayers에 속하는 물체와의 충돌여부 검사결과를 Grounded에 대입
            Vector3 spherePos=new Vector3(transform.position.x,transform.position.y-GroundedOffset,transform.position.z);
            Grounded=Physics.CheckSphere(spherePos,GroundedRadius,GroundLayers,QueryTriggerInteraction.Ignore);
        }

        public void Move()
        {

        }

        public void JumpAndGravity()
        {

        }
    }
}