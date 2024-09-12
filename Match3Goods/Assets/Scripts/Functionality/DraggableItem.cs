using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    [HideInInspector] public Transform parentAfterDrag; // ascuns in editor dar public pentru drop script
    public bool canDrag = true;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        animator.SetBool("isDropped", false);
        parentAfterDrag = transform.parent; // salvarea parent (pentru a reveni la el eventual)
        transform.SetParent(transform.root); // parintele obiectului care este dragged devine root-ul (ca sa nu fie constrans de ierarhie)
        transform.SetAsLastSibling(); // pentru evitare overlap cu celelalte elemente ui
        image.raycastTarget = false; // pentru ca sub cursor item-ul sa fie invisibil pentru a putea vedea slotul si asigna itemul la acel slot
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        transform.transform.position = Input.mousePosition; // positia obiectului care este dragged sa fie sub cursor
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        transform.SetParent(parentAfterDrag); // pune parintele inapoi
        image.raycastTarget = true; // revers invizibilitatea item-ului sub cursor pentru ca sa pot face drag and drop in continuare pe item
    }

}
