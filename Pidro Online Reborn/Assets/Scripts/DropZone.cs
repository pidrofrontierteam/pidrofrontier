using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    [SerializeField]private List<GameObject> cards = new List<GameObject>();

    /*
    The dropzone has a list which grows and shrinks as gameobjects are added to it. 
    Whenever the list updates, the sorting order does too. The first element in the list is lower in the sorting order.
    */

    public void AddCardToList(GameObject gameObject)
    {
        cards.Add(gameObject);
        // UpdateListSortingOrder();
    }

    public void RemoveCardFromList(GameObject gameObject)
    {
        cards.Remove(gameObject);
        // UpdateListSortingOrder();
    }

    private void UpdateListSortingOrder()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<SpriteRenderer>().sortingOrder = i;
        }
    }
}
