using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    [HideInInspector] public Transform parentAfterDrag; // ascuns in editor dar public pentru drop script
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag"); // TODO: pentru debug-ing (remove)
        parentAfterDrag = transform.parent; // salvarea parent (pentru a reveni la el eventual)
        transform.SetParent(transform.root); // parintele obiectului care este dragged devine root-ul (ca sa nu fie constrans de ierarhie)
        transform.SetAsLastSibling(); // pentru evitare overlap cu celelalte elemente ui
        image.raycastTarget = false; // pentru ca sub cursor item-ul sa fie invisibil pentru a putea vedea slotul si asigna itemul la acel slot
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging"); // TODO: pentru debug-ing (remove)
        transform.transform.position = Input.mousePosition; // positia obiectului care este dragged sa fie sub cursor
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag"); // TODO: pentru debug-ing (remove)
        transform.SetParent(parentAfterDrag); // pune parintele inapoi
        image.raycastTarget = true; // revers invizibilitatea item-ului sub cursor pentru ca sa pot face drag and drop in continuare pe item
    }
}
