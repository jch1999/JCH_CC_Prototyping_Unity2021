using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class MYOnTriggerEvent : MonoBehaviour
{
    public LayerMask targetLayer;
    //for my prototype,there need to be emission  or emission outline effects for item when player enter the item's detection bound.
    [Header("Trigger Enter Event Section")]
    public UnityEvent onTriggerEnterEvent;
    
    //Item's effects should be turned off, when player exit from it's detection bound.
    [Space(10)]
    [Header("Trigger Exit Event Section")]
    public UnityEvent onTriggerExitEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer!=targetLayer)
            return;
    }
    
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer!=targetLayer)
            return;
        
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
