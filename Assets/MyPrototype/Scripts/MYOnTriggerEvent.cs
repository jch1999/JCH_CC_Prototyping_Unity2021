using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class MYOnTriggerEvent : MonoBehaviour
{
    //for my prototype,there need to be emission  or emission outline effects for item when player enter the item's detection bound.
    [Header("Trigger Enter Event Section")]
    public UnityEvent onTriggerEnterEvent;
    
    //Item's effects should be turned off, when player exit from it's detection bound.
    [Space(10)]
    [Header("Trigger Exit Event Section")]
    public UnityEvent onTriggerExitEvent;
    
    [Space(10)]
    public bool m_EnterHasBeenTriggered;
    public bool m_ExitHasBeenTriggered;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        onTriggerEnterEvent.Invoke();
        m_EnterHasBeenTriggered=true;
        m_ExitHasBeenTriggered=false;
    }
    
    void OnTriggerExit(Collider other)
    {
        onTriggerExitEvent.Invoke();
        m_ExitHasBeenTriggered=true;
        m_EnterHasBeenTriggered=false;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(new Vector3(transform.position.x,transform.position.y-1f,transform.position.z),2.5f);
    }
}
