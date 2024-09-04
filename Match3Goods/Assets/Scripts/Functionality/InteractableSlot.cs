using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) // daca nu sunt alti "copii" in slot se executa
        {
            Debug.Log("Dropped"); // TODO: remove
            GameObject dropped = eventData.pointerDrag; // obiectul care va fi dropped
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform; // seteaza noul slot ca parinte
        }
    }
}
