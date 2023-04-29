using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class OnInteractItem : MonoBehaviour
{
    public bool isOneShot;
    [FormerlySerializedAs("onTriggerEnterEvent")]
    public UnityEvent onInteractEvent;

    bool m_hasBeenTriggered;

    public void Interact()
    {
        if(isOneShot&&m_hasBeenTriggered)
            return;
        
        //중앙 포인터 비활성화
        GameObject.Find("CenterPoint").SetActive(false);
        //플레이어 입력 정지
        GameObject.Find("Player").GetComponentInChildren<MyPrototype.FirstPersonController>().DisablePlayerInput();
        //상호 작용 이벤트 실행
        onInteractEvent.Invoke();
        m_hasBeenTriggered=true;
        gameObject.SetActive(false);
    }
}
