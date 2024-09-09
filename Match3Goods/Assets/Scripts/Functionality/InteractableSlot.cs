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

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.ChildCountActive() == 0) // daca nu sunt alti "copii" activi in slot se executa
        {
            GameObject dropped = eventData.pointerDrag; // obiectul care va fi dropped
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform; // seteaza noul slot ca parinte

            StartCoroutine(DelayedCheckForMatches()); // verificare daca sunt 3 match-uri
            StartCoroutine(DelayedCheckForLayers()); // verificare iteme pe layere
            StartCoroutine(DelayedCheckForCompletion()); // verificare clear nivel
        }
    }


    // TODO: posibil sa fie necesara o ajustare de delay-uri pentru animatii si alte chestii
    private IEnumerator DelayedCheckForMatches()
    {
        yield return new WaitForSeconds(0.1f);
            slotManager.CheckForHorizontalMatches();
    }
    private IEnumerator DelayedCheckForLayers()
    {
        yield return new WaitForSeconds(0.2f);
            slotManager.CheckShelvesAndActivateNextLayerItems();
    }
    private IEnumerator DelayedCheckForCompletion()
    {
        yield return new WaitForSeconds(0.3f);
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
