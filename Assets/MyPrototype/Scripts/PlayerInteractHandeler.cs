using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerInteractHandeler : MonoBehaviour
{
    [FormerlySerializedAs("IteractableIcon")]
    public Sprite InteractablePointer;
    public Sprite NormalPointer;

    [SerializeField]
    private Image m_PointerImage;

    [SerializeField]
    private Vector3 m_OriginalPointerSize;

    // Start is called before the first frame update
    void Start()
    {
        //목표 프렘이레이트 설정
        Application.targetFrameRate=60;

        Camera mainCam=Camera.main;
        CinemachineBrain cinemachineBrain=mainCam.GetComponent<CinemachineBrain>();
        if(cinemachineBrain==null)
            mainCam.gameObject.AddComponent<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        OnInteractItem[] targetInteract=null;
        //카메라 시야로부터 레이를 내보낸다.
        Ray ray=Camera.main.ViewportPointToRay(Vector3.one*0.5f);
        RaycastHit hit;

        bool displayInteractable=false;
        //ray를 최대 2.0f만큼 내보내어 충돌된 결과를 hit에 저장해라
        //길이 수정 2.0f -> 4.0f
        //충돌결과가 존재한다면
        if(Physics.Raycast(ray,out hit,4.0f))
        {
            //배열로 반환하는 구나? 여태까지 List 인줄 알았는데
            OnInteractItem[] interacts=hit.collider.gameObject.GetComponentsInChildren<OnInteractItem>();

            if(interacts.Length>0)
            {
                displayInteractable=true;
                targetInteract=interacts;
                m_PointerImage.color=Color.gray;

                for(int i=0;i<targetInteract.Length;i++)
                {
                    if(!targetInteract[i].isActiveAndEnabled)
                    {
                        m_PointerImage.color=Color.white;
                        break;
                    }
                }
            }
        }

        //감지된 타겟이 존재하고 입력이 이번 프레임에 들어왔을 경우
        if(targetInteract!=null&&
            (Mouse.current.leftButton.wasPressedThisFrame||Keyboard.current.eKey.wasPressedThisFrame))
        {
            for(int i=0;i<targetInteract.Length;i++)
            {
                //활성화된 상호작용 객체와 상호작용 실행.
                if(targetInteract[i].isActiveAndEnabled)
                    targetInteract[i].Interact();
            }
        }

        if(displayInteractable)
        {
            m_PointerImage.sprite=InteractablePointer;
            m_PointerImage.transform.localScale=m_OriginalPointerSize*2.0f;
        }
        else
        {
            m_PointerImage.sprite=NormalPointer;
            m_PointerImage.transform.localScale=m_OriginalPointerSize;
        }
    }
}
