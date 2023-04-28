using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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

        onInteractEvent.Invoke();
        m_hasBeenTriggered=true;
    }
}
