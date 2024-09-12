using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InteractableSlot : MonoBehaviour, IDropHandler
{
    [Inject] private SlotManager slotManager;
    [Inject] private PopupManager popupManager;
    [Inject] private GameStateManager stateManager;

    private DraggableItem currentDraggableItem;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.ChildCountActive() == 0) // daca nu sunt alti "copii" activi in slot se executa
        {
            GameObject dropped = eventData.pointerDrag; // obiectul care va fi dropped
            currentDraggableItem = dropped.GetComponent<DraggableItem>();
            currentDraggableItem.parentAfterDrag = transform; // seteaza noul slot ca parinte

            StartCoroutine(DelayedCheckForMatches()); // verificare daca sunt 3 match-uri
            StartCoroutine(DelayedCheckForLayers()); // verificare iteme pe layere
            StartCoroutine(DelayedCheckForCompletion()); // verificare clear nivel
        }
    }


    private IEnumerator DelayedCheckForMatches()
    {
        yield return new WaitForSeconds(0.2f);
        if (!slotManager.CheckForHorizontalMatches())
        {
            Animator animator = currentDraggableItem.GetComponent<Animator>();
            animator.SetBool("isDropped", true);
        };
    }

    private IEnumerator DelayedCheckForLayers()
    {
        yield return new WaitForSeconds(0.6f);
            slotManager.CheckShelvesAndActivateNextLayerItems();
    }
    private IEnumerator DelayedCheckForCompletion()
    {
        yield return new WaitForSeconds(0.7f);
        if (slotManager.CheckIfAllItemsCleared())
        {
            stateManager.ChangeState(GameState.Win);
            stateManager.ChangeState(GameState.LevelTransition);
        };
    }
}


// Extension metod pentru Transform (static)
// numara numarul de iteme active in slot
// este necesar pentru object snapping
// transformCount == 0 nu merge deoarece numara si itemele (children) inactive
public static class TransformExtensions
{
    public static int ChildCountActive(this Transform t)
    {
        int count = 0;
        foreach (Transform child in t)
        {
            if (child.gameObject.activeSelf)
                count++;
        }
        return count;
    }
}
