//FirstPersonController 참조
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using TMPro;
#endif

namespace MyPrototype
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM&&STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif

    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character")]
        public float MoveSpeed=4.0f;
        [Tooltip("Sprint speed of the character")]
        public float SprintSpeed=6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed=1.0f;
        [Tooltip("Acceleration or deacceleration")]
        public float SpeedChangeRate=10.0f;

        //높이 뛰고 빨리 떨어진다?
        [Space(10)]
        [Tooltip("Jump height of the character")]
        public float JumpHeight=1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("JumpCoolTime, Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout=0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout=0.15f;

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

        //cinemachine
        private float _chinemachineTargetPitch;
        private float _chinemachineTargetYaw;

        //player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity=53.0f;

        //timeout deltaTime
        private float _jumpTimeOutDelta;
        private float _fallTimeOutDelta;

        private CharacterController _controller;
        private PlayerInputScript _input;
        private GameObject _mainCamera;

        private float _threshold=0.01f;

        //player move sound
        private AudioSource audioSource;
        public AudioClip walkSound;
        public AudioClip jumpSound;

        //item count
        [SerializeField]
        private int MaxItemCount;
        [SerializeField]
        private int NowItemCount;

        public TextMeshProUGUI itemGetCountText;
        public TextMeshProUGUI explainGetCountText;

        void Awake()
        {
            if(_mainCamera==null)
                _mainCamera=GameObject.FindGameObjectWithTag("MainCamera");
            Cursor.visible=false;
        }
        // Start is called before the first frame update
        void Start()
        {
            _controller=GetComponent<CharacterController>();
            _input=GetComponent<PlayerInputScript>();

            audioSource=GetComponent<AudioSource>();
            audioSource.clip=walkSound;
            audioSource.Play();

            _jumpTimeOutDelta= JumpTimeout;
            _fallTimeOutDelta= FallTimeout;
        }

        // Update is called once per frame
        void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        void LateUpdate()
        {
            CameraRotation();
        }

        //땅과의 충돌여부를 확인
        private void GroundedCheck()
        {
            //GroundedOffset만큼 현재 위치에서 y 값을 감소시킨 지점을 원점으로 구를 생성해서
            //GroundedLayers에 속하는 물체와의 충돌여부 검사결과를 Grounded에 대입
            Vector3 spherePos=new Vector3(transform.position.x,transform.position.y-GroundedOffset,transform.position.z);
            Grounded=Physics.CheckSphere(spherePos,GroundedRadius,GroundLayers,QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            //if rotaion is disabled
            if(!_input.rotatable)
                return;

            // if there is an input
            if(_input.look.sqrMagnitude>=_threshold)
            {
                //좌우 회전 제한을 추가하려고 했는데 직접 해보니 영..
                //제자리에서 한 바퀴를 못 도니 많이 불편하네
                //카메라랑 케릭터랑 좀 떨어져 있어서 벽 뚫는게 불편한데 어쩔 수 없나?
                _chinemachineTargetPitch+=_input.look.y*RotationSpeed*Time.deltaTime;
                _rotationVelocity=_input.look.x*RotationSpeed*Time.deltaTime;
                //_chinemachineTargetYaw+=_input.look.x*RotationSpeed*Time.deltaTime;

                // clamp out pitch rotation
                _chinemachineTargetPitch=ClampAngle(_chinemachineTargetPitch,BottomClamp,TopClamp);

                //clamp out yaw rotation
                //_chinemachineTargetYaw=ClampAngle(_chinemachineTargetYaw,BottomClamp/2,TopClamp/2);

                //Update Cinemachine camera target pitch
                //CinemachineCameraTarget.transform.localRotation=Quaternion.Euler(_chinemachineTargetPitch,_chinemachineTargetYaw,0.0f);
                CinemachineCameraTarget.transform.localRotation=Quaternion.Euler(_chinemachineTargetPitch,0.0f,0.0f);

                //rotate the player left and right
                transform.Rotate(Vector3.up*_rotationVelocity);
            }
        }

        private void Move()
        {
            //set target speed - default MoveSpeed,  SprintSpeed if sprint is pressed
            float targetSpeed=_input.sprint?SprintSpeed:MoveSpeed;

            //if move isn't pressed, set targetSpeed 0
            if(_input.move==Vector2.zero)
                targetSpeed=0.0f;
            
            // a reference to the players current horizonatl velocity
            //수평에 해당하는 x,z 축 속도 값의 크기를 가져온다?
            float CurrentHorizontalSpeed=new Vector3(_controller.velocity.x,0.0f,_controller.velocity.z).magnitude;
            
            float speedOffset=0.1f;
            float inputMagnitude=_input.analogMovement?_input.move.magnitude:1.0f;

            // accelerate or deaccelerate to target speed
            if(CurrentHorizontalSpeed<targetSpeed-speedOffset||CurrentHorizontalSpeed>targetSpeed+speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // 유기적인 선형 속도변환를 제공하는 대신 곡선형 속도 변화 결과를 만들어 낸다
				// note T in Lerp is clamped, so we don't need to clamp our speed
                // Lerp에 T는 고정되어 있기 때문에 속도를 고정할 필요 x ?
                _speed=Mathf.Lerp(CurrentHorizontalSpeed,targetSpeed*inputMagnitude,Time.deltaTime*SpeedChangeRate);

                //소수점 3자리 수까지만 남기기.
                _speed=Mathf.Round(_speed*1000f)/1000f;
            }
            else
            {
                _speed=targetSpeed;                
            }

            // normalize input direction
            Vector3 inputDirection=new Vector3(_input.move.x,0.0f,_input.move.y).normalized;


            if(_input.move!=Vector2.zero)
            {
                inputDirection=transform.right*_input.move.x+transform.forward*_input.move.y;
            }

            // move the player -x, z 축 이동량 + y 축 이동량
            _controller.Move(inputDirection.normalized*(_speed*Time.deltaTime)+new Vector3(0.0f,_verticalVelocity,0.0f)*Time.deltaTime);

            //걷는 소리 재생 속도 조절
            audioSource.pitch=_input.sprint?SprintSpeed/MoveSpeed:1.0f;
            if(targetSpeed!=0.0f&&!audioSource.isPlaying)
            {
                if(Grounded)
                {
                    audioSource.Play();
                    Debug.Log("Play!");
                }
            }
        }

        private void JumpAndGravity()
        {
            if(Grounded)
            {
                // reset the fallTimeOut Timer
                _fallTimeOutDelta=FallTimeout;
                
                // stop our velocity dropping infinitely when grounded
                // 땅에 닿아있는 상태일 경우 속도가 무한히 감소하는 것을 중단한다.
                if(_verticalVelocity<0.0f)
                {
                    _verticalVelocity=-2.0f;
                }
                
                // Jump
                if(_input.jump&&_jumpTimeOutDelta<=0.0f)
                {
                    //점프 높이*-2f*Gravity의 제곱근 많큼 위로 상승 - Gravitiy의 기본값이 -15f
                    _verticalVelocity=Mathf.Sqrt(JumpHeight*-2f*Gravity);
                    
                    //점프 소리 재생
                    audioSource.PlayOneShot(jumpSound);
                }

                // jump timeOut
                if(_jumpTimeOutDelta>=0.0f)
                {
                    _jumpTimeOutDelta-=Time.deltaTime;
                }
            }
            else
            {
                //reset the jump timeout timer
                _jumpTimeOutDelta=JumpTimeout;

                // fall timeOut
                if(_fallTimeOutDelta>=0.0f)
                {
                    _fallTimeOutDelta-=Time.deltaTime;
                }

                //if we are not grounded, do not jump
                _input.jump=false;
            }

            // apply gravity twice if velocity under terminal (multifly by delta time twice to linerly speed up over time)
            // 터미널 속도 아래로 수직속도가 감소하면 감소속도 선형 증가
            if(_verticalVelocity<_terminalVelocity)
            {
                _verticalVelocity+=Gravity*Time.deltaTime;
            }
        }

        // 0~360이내로 각도 값을 수정한 뒤 주어진 최소~최대 구간 내의 값으로 변환
        private static float ClampAngle(float lfAngle,float lfMin,float lfMax)
        {
            if(lfAngle<-360.0f)lfAngle+=360.0f;
            if(lfAngle>360.0f)lfAngle-=360.0f;
            return Mathf.Clamp(lfAngle,lfMin,lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen=new Color(0.0f,1.0f,0.0f,0.35f);
            Color transparentRed=new Color(1.0f,0.0f,0.0f,0.35f);

            if(Grounded)
            {
                Gizmos.color=transparentGreen;
            }
            else
            {
                Gizmos.color=transparentRed;
            }

            //when selected, draw a gizmo in the position of, and matching radius of, the grounded color
            Gizmos.DrawSphere(new Vector3(transform.position.x,transform.position.y-GroundedOffset,transform.position.z),GroundedRadius);
        }
        
        // 플레이어 입력 활성, UI 입력 정지, 커서 비활성화(비가시화, 고정)
        public void EnablePlayerInput()
        {
            PlayerInput input=GetComponent<PlayerInput>();
            input.actions.FindActionMap("Player").Enable();
            input.actions.FindActionMap("UI").Disable();
#if UNITY_EDITOR||UNITY_EDITOR_WIN
            Cursor.visible=false;
            Mouse.current.WarpCursorPosition(new Vector3(0.0f,0.0f,0.0f));
            Cursor.lockState=CursorLockMode.Locked;
#endif
        }
        
        // 플레이어 입력 정지, UI 입력 활성, 커서 활성화(가시화, 이동)
        public void DisablePlayerInput()
        {
            PlayerInput input=GetComponent<PlayerInput>();
            input.actions.FindActionMap("Player").Disable();
            input.actions.FindActionMap("UI").Enable();
#if UNITY_EDITOR||UNITY_EDITOR_WIN
            Cursor.visible=true;
            Cursor.lockState=CursorLockMode.Confined;
#endif
        }

        public void ItemCountTextUpdate()
        {
            NowItemCount++;
            itemGetCountText.text=$"{NowItemCount}/{MaxItemCount}";
            explainGetCountText.text=$"{NowItemCount}/{MaxItemCount}";
        }
    }
}