using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static UnityEditor.Progress;

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
    public GameObject redGemPrefab;
    public GameObject greenGemPrefab;
    public GameObject blueGemPrefab;
    public GameObject crownPrefab;
    public GameObject goldCoinPrefab;
    public GameObject bronzeCoinPrefab;
    public GameObject bronzeRingPrefab;
    public GameObject breadPrefab;
    public GameObject cloverPrefab;
    public GameObject pillsPrefab; 
    public GameObject hourglassPrefab;
    public GameObject bookPrefab;
    public GameObject bombPrefab;
    public GameObject swordPrefab;
    public GameObject featherPrefab;
    public GameObject applePrefab;

    private List<GameObject> _loadedSlots = new List<GameObject>();
    private List<GameObject> itemsLayer2 = new List<GameObject>();
    private List<GameObject> itemsLayer3 = new List<GameObject>();

    [Inject] private PopupManager popupManager;

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

                if (slotData.ItemHeldLayer3 != ItemType.None)
                {
                    GameObject itemPrefabLayer3 = GetItemPrefab(slotData.ItemHeldLayer3);
                    if (itemPrefabLayer3 != null)
                    {
                        GameObject itemInstanceLayer3 = Instantiate(itemPrefabLayer3, slotInstance.transform);
                        itemInstanceLayer3.SetActive(false); // Ascunde al treilea item
                        itemsLayer3.Add(itemInstanceLayer3);
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
            case ItemType.RedGem:
                return redGemPrefab;
            case ItemType.GreenGem:
                return greenGemPrefab;
            case ItemType.BlueGem:
                return blueGemPrefab;
            case ItemType.Crown:
                return crownPrefab;
            case ItemType.GoldCoin:
                return goldCoinPrefab;
            case ItemType.BronzeCoin:
                return bronzeCoinPrefab;
            case ItemType.BronzeRing:
                return bronzeRingPrefab;
            case ItemType.Bread:
                return breadPrefab;
            case ItemType.Clover:
                return cloverPrefab;
            case ItemType.Pills:
                return pillsPrefab;
            case ItemType.Hourglass:
                return hourglassPrefab;
            case ItemType.Book:
                return bookPrefab;
            case ItemType.Bomb:
                return bombPrefab;
            case ItemType.Sword:
                return swordPrefab;
            case ItemType.Feather:
                return featherPrefab;
            case ItemType.Apple:
                return applePrefab;
            default:
                return null;
        }
    }


    public bool CheckForHorizontalMatches()
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
                        return true;
                    }
                }
            }
        }

        return false;
    }


    private void HandleMatch(List<GameObject> matchingSlots)
    {
        foreach (var slot in matchingSlots)
        {
            GameObject item = slot.GetComponentInChildren<DraggableItem>().gameObject;

            Animator animator = item.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("isDropped", true);
                animator.SetTrigger("Match");
            }

            Destroy(item, 0.3f); // pentru sufiecient timp executare animatie inainte de distrugere
        }

        Debug.Log("Match 3 Detected!");
    }

    public bool CheckIfAllItemsCleared()
    {
        foreach (var slotInstance in _loadedSlots)
        {
            if (slotInstance.GetComponentInChildren<DraggableItem>() != null)
            {
                return false;
            }
        }

        _loadedSlots.Clear();
        popupManager.ShowWinPopup();

        return true;
    }


    public void ClearShelves()
    {
        List<GameObject> shelvesToRemove = new List<GameObject>();

        // itereaza prin fiecare copil si verifica daca era instantiat din shelf prefab
        foreach (Transform shelf in shelvesParent)
        {         
            if (shelf.gameObject.name.Contains(shelfPrefab.name))
            {
                shelvesToRemove.Add(shelf.gameObject);
            }
        }

        // distruge instantele
        foreach (var shelf in shelvesToRemove)
        {
            Destroy(shelf);
        }

        _loadedSlots.Clear();
    }

    // verifica daca sunt rafturi goale si activeaza urmatorul layer de iteme
    public void CheckShelvesAndActivateNextLayerItems()
    {
        // itereaza prin fiecare shelf
        foreach (Transform shelfTransform in shelvesParent)
        {
            bool allSlotsEmpty = true;
            bool hasLayer2Items = false;

            // itereaza prin fiecare slot in shelf
            foreach (Transform slotTransform in shelfTransform)
            {
                if (slotTransform.GetComponentInChildren<DraggableItem>() != null)
                {
                    allSlotsEmpty = false;
                    break;
                }
            }

            if (allSlotsEmpty)
            {
                // daca nu-s iteme in layer 1 activeaza layer 2
                hasLayer2Items = ActivateNextLayerItemsForShelf(shelfTransform, itemsLayer2);

                // daca nu-s iteme in layer 2 activeaza layer 3
                if (!hasLayer2Items)
                {
                    ActivateNextLayerItemsForShelf(shelfTransform, itemsLayer3);
                }
            }
        }
    }

    private bool ActivateNextLayerItemsForShelf(Transform shelfTransform, List<GameObject> itemLayer)
    {
        List<GameObject> itemsToRemove = new List<GameObject>();
        bool hasItemsLeft = false;

        // verifica iteme inactive
        foreach (var item in itemLayer)
        {
            // verifica daca partine shelfului corect
            if (item.transform.parent.parent == shelfTransform && !item.activeSelf)
            {
                item.SetActive(true);
                itemsToRemove.Add(item); // remove dupa activare
                hasItemsLeft = true;
            }
        }

        foreach (var item in itemsToRemove)
        {
            itemLayer.Remove(item);
        }

        return hasItemsLeft; // return true daca au fost activate iteme
    }


}
