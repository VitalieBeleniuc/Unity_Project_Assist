using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SlotManager : MonoBehaviour
{

    public GameObject shelfPrefab;
    public GameObject slotPrefab; // slot-ul
    public Transform shelvesParent;
    public Transform slotsParent; // canvas-ul

    public GameObject redPotionPrefab;
    public GameObject bluePotionPrefab;
    public GameObject greenPotionPrefab;
    public GameObject purplePotionPrefab;

    private List<GameObject> _loadedSlots = new List<GameObject>();
    private List<GameObject> itemsLayer2 = new List<GameObject>();

    // Zenject factory
    [Inject] private InteractableSlotFactory _slotFactory;
    public void LoadSlots(List<Level.Shelf> shelvesData) // Incarca sloturile din JSON
    {
        foreach (Level.Shelf shelfData in shelvesData)
        {
            shelvesParent.position = Vector3.zero; // pentru mentinerea spatiului local
            GameObject shelfInstance = Instantiate(shelfPrefab, shelvesParent);

            // Pozitioneaza rafturile dupa JSON
            shelfInstance.transform.position = new Vector3(shelfData.ShelfPositionX, shelfData.ShelfPositionY, 0);
            Transform shelfTransform = shelfInstance.transform;

            foreach (Level.Shelf.Slot slotData in shelfData.Slots)
            {
                InteractableSlot slotInstance = _slotFactory.Create();
                slotInstance.transform.SetParent(shelfTransform, false);

                // pozitiile conform datelor din JSON
                slotInstance.transform.position = new Vector3(slotData.SlotPositionX, slotData.SlotPositionY, 0);

                _loadedSlots.Add(slotInstance.gameObject); // necesar pentru match3 logica

                // item-ele conform datelor din JSON (primul layer)
                if (slotData.ItemHeld != ItemType.None)
                {
                    GameObject itemPrefab = GetItemPrefab(slotData.ItemHeld);
                    if (itemPrefab != null)
                    {
                        GameObject itemInstance = Instantiate(itemPrefab, slotInstance.transform);
                    }
                }

                // item-ele conform datelor din JSON (al doilea layer)
                if (slotData.ItemHeldLayer2 != ItemType.None)
                {
                    GameObject itemPrefabLayer2 = GetItemPrefab(slotData.ItemHeldLayer2);
                    if (itemPrefabLayer2 != null)
                    {
                        GameObject itemInstanceLayer2 = Instantiate(itemPrefabLayer2, slotInstance.transform);
                        itemInstanceLayer2.SetActive(false); // Ascunde al doilea item
                        itemsLayer2.Add(itemInstanceLayer2);
                    }
                }
            }
        }
    }


    private GameObject GetItemPrefab(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.RedPotion:
                return redPotionPrefab;
            case ItemType.GreenPotion:
                return greenPotionPrefab;
            case ItemType.BluePotion:
                return bluePotionPrefab;
            case ItemType.PurplePotion:
                return purplePotionPrefab;
            default:
                return null;
        }
    }


    public void CheckForHorizontalMatches()
    {
        // dictionar pentru stocarea slot-urilor dupa raftul pe care se afla
        Dictionary<GameObject, List<GameObject>> slotsByShelf = new Dictionary<GameObject, List<GameObject>>();

        // organizarea slot-urilor
        foreach (var slotInstance in _loadedSlots)
        {
            var shelf = slotInstance.transform.parent.gameObject;

            if (!slotsByShelf.ContainsKey(shelf))
            {
                slotsByShelf[shelf] = new List<GameObject>();
            }

            slotsByShelf[shelf].Add(slotInstance);
        }

        // itereaza prin fiecare raft si verifica match 3 dupa pozitia y
        foreach (var shelfEntry in slotsByShelf)
        {
            var slotsInShelf = shelfEntry.Value;

            // inca un dictionar pentru slot-uri dupa pozitia lor y
            Dictionary<float, List<GameObject>> slotsByRow = new Dictionary<float, List<GameObject>>();

            // organizarea slot-urilor
            foreach (var slotInstance in slotsInShelf)
            {
                var slotPosition = slotInstance.transform.position.y;

                if (!slotsByRow.ContainsKey(slotPosition))
                {
                    slotsByRow[slotPosition] = new List<GameObject>();
                }

                slotsByRow[slotPosition].Add(slotInstance);
            }

            // itereaza prin fiecare slot si verifica match 3 dupa pozitia y
            foreach (var row in slotsByRow)
            {
                var slotsInRow = row.Value;
                slotsInRow.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

                List<GameObject> matchingSlots = new List<GameObject>();

                for (int i = 0; i < slotsInRow.Count; i++)
                {
                    var currentSlot = slotsInRow[i];
                    var currentItem = currentSlot.GetComponentInChildren<DraggableItem>();

                    if (currentItem == null)
                    {
                        matchingSlots.Clear(); // Reset daca nu-s iteme
                        continue;
                    }

                    // verifica daca itemul curent e de acelasi tip ca si ultimul item din matchingSlots
                    // daca da il adauga
                    if (matchingSlots.Count == 0 || currentItem.name == matchingSlots[matchingSlots.Count - 1].GetComponentInChildren<DraggableItem>().name)
                    {
                        matchingSlots.Add(currentSlot);
                    }
                    else
                    {
                        matchingSlots.Clear();
                        matchingSlots.Add(currentSlot);
                    }

                    // daca s-a gasit un match3 
                    if (matchingSlots.Count == 3)
                    {
                        HandleMatch(matchingSlots);
                        matchingSlots.Clear(); // clear pentru prevenire overlap
                    }
                }
            }
        }
    }


    private void HandleMatch(List<GameObject> matchingSlots)
    {
        foreach (var slot in matchingSlots)
        {
            Destroy(slot.GetComponentInChildren<DraggableItem>().gameObject);
            // TODO: posibil de adaugat scor
        }

        Debug.Log("Match 3 Detected!");
    }

    public void CheckIfAllItemsCleared()
    {
        foreach (var slotInstance in _loadedSlots)
        {
            if (slotInstance.GetComponentInChildren<DraggableItem>() != null)
            {
                Debug.Log(slotInstance.GetComponentInChildren<DraggableItem>());
                return;
            }
        }

        Debug.Log("Congratulations! You cleared the level!");
    }





    // verifica daca sunt rafturi goale si activeaza urmatorul layer de iteme
    public void CheckShelvesAndActivateNextLayerItems()
    {
        // iterare prin fiecare raft
        foreach (Transform shelfTransform in shelvesParent)
        {
            bool allSlotsEmpty = true;

            // iterare prin fiecare slot din raft
            foreach (Transform slotTransform in shelfTransform)
            {
                if (slotTransform.GetComponentInChildren<DraggableItem>() != null)
                {
                    // Debug.Log("Not clear"); // TODO: pentru debug, remove
                    allSlotsEmpty = false;
                    break;
                }
            }

            if (allSlotsEmpty)
            {
                // Debug.Log("All clear"); // TODO: pentru debug, remove
                ActivateNextLayerItemsForShelf(shelfTransform);
            }
        }
    }

    private void ActivateNextLayerItemsForShelf(Transform shelfTransform)
    {
        List<GameObject> itemsToRemove = new List<GameObject>();

        // cauta itemele inactive si le activeaza
        foreach (var itemLayer2 in itemsLayer2)
        {
            // verifica daca itemul apartine raftului corect
            if (itemLayer2.transform.parent.parent == shelfTransform && !itemLayer2.activeSelf)
            {
                itemLayer2.SetActive(true);
                itemsToRemove.Add(itemLayer2); // removal
            }
        }

        foreach (var item in itemsToRemove)
        {
            itemsLayer2.Remove(item);
        }
    }


}
