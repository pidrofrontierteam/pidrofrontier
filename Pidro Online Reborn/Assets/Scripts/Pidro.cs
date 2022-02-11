using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pidro : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;

    public GameObject player1_hand_area;
    public GameObject player2_hand_area;
    public GameObject player3_hand_area;
    public GameObject player4_hand_area;

    public GameObject player1_field_area;
    public GameObject player2_field_area;
    public GameObject player3_field_area;
    public GameObject player4_field_area;

    public static string[] suits = new string[] {"Spades", "Hearts", "Clubs", "Diamonds"};
    public static string[] values = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};

    private bool firstDeal = true;    

    private List<string> player1_hand = new List<string>();
    private List<string> player2_hand = new List<string>();
    private List<string> player3_hand = new List<string>();
    private List<string> player4_hand = new List<string>();

    private List<string> player1_field = new List<string>();
    private List<string> player2_field = new List<string>();
    private List<string> player3_field = new List<string>();
    private List<string> player4_field = new List<string>();

    public List<string> hasSprite = new List<string>();
    public List<string> discard = new List<string>();
    
    public List<string> deck;
    public List<string> sortedDeck;

    public List<string> currentPlayer_hand;
    public List<string> currentPlayer_field;
    public GameObject currentPlayer_hand_area;
    public GameObject currentPlayer_field_area;

    private static Pidro _instance;

    public static Pidro Instance { get { return _instance; } }


    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this; 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cardFaces = Resources.LoadAll<Sprite>("English_pattern_playing_cards_deck");

        // Create the deck from which we draw cards
        deck = GenerateDeck();
        Shuffle(deck);

        // Create another deck, the indeces of which are used for sorting
        sortedDeck = GenerateDeck();

        //DEBUG: sets current hand and field
        currentPlayer_hand = player1_hand;
        currentPlayer_field = player1_field;
        currentPlayer_hand_area = player1_hand_area;
        currentPlayer_field_area = player1_field_area;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCards()
    {

        if (firstDeal)
        {
            for (int i = 0; i < 3; i++)
            {
                //Debug.Log("Dealt 3 cards to player 1");
                currentPlayer_hand = player1_hand;
                currentPlayer_hand_area = player1_hand_area;
                DealCards(3, currentPlayer_hand, currentPlayer_hand_area);

                //Debug.Log("Dealt 3 cards to player 2");
                currentPlayer_hand = player2_hand;
                currentPlayer_hand_area = player2_hand_area;
                DealCards(3, currentPlayer_hand, currentPlayer_hand_area);

                //Debug.Log("Dealt 3 cards to player 3");
                currentPlayer_hand = player3_hand;
                currentPlayer_hand_area = player3_hand_area;
                DealCards(3, currentPlayer_hand, currentPlayer_hand_area);

                //Debug.Log("Dealt 3 cards to player 4");
                currentPlayer_hand = player4_hand;
                currentPlayer_hand_area = player4_hand_area;
                DealCards(3, currentPlayer_hand, currentPlayer_hand_area);
            }
        }
        player1_hand = SortCardsList(player1_hand);
        SortCardsGameObjects(player1_hand, player1_hand_area);

        player2_hand = SortCardsList(player2_hand);
        SortCardsGameObjects(player2_hand, player2_hand_area);

        player3_hand = SortCardsList(player3_hand);
        SortCardsGameObjects(player3_hand, player3_hand_area);

        player4_hand = SortCardsList(player4_hand);
        SortCardsGameObjects(player4_hand, player4_hand_area);


        //DEBUG:
        currentPlayer_hand = player1_hand;
        currentPlayer_field = player1_field;
        currentPlayer_hand_area = player1_hand_area;
        currentPlayer_field_area = player1_field_area;
    }

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newDeck.Add(v + " of " + s);
            }
        }
        return newDeck;
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    // Deprecated but used for reference
    void PidroDeal()
    {
        float yOffset = 0;
        float zOffset = 0.03f;
        foreach (string card in deck)
        {
            GameObject newCard = Instantiate(cardPrefab, new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z - zOffset), Quaternion.identity);
            newCard.name = card;
            newCard.GetComponent<Selectable>().faceUp = true;

            yOffset += 1f;
            zOffset += 0.03f;
        }
    }

    void DealCards(int amount, List<string> hand, GameObject area)
    {

        for (int i = 0; i < amount; i++)
        {
            hand.Add(deck.Last<string>());
            deck.RemoveAt(deck.Count - 1);
        }

        foreach (string card in hand)
        {
            if (!(hasSprite.Contains(card))) 
            {
                GameObject newCard = Instantiate(cardPrefab, area.transform, true);
                newCard.transform.SetParent(area.transform, false);
                newCard.name = card;
                newCard.GetComponent<Selectable>().faceUp = true;

                newCard.transform.position = area.transform.position;
                newCard.transform.position = new Vector3(newCard.transform.position.x, newCard.transform.position.y, -((sortedDeck.FindIndex(z => z.Equals(newCard.name))))/1000f);

                if(area.name == "Player2_area")
                    newCard.transform.Rotate(0,0,90);

                if(area.name == "Player3_area")
                    newCard.transform.Rotate(0,0,180);

                if(area.name == "Player4_area")
                    newCard.transform.Rotate(0,0,-90);


                hasSprite.Add(card);
            }
        }
    }

    public Sprite GetCardFaceAtIndex(int i)
    {
        return cardFaces[i];
    }


    /// <summary>
    /// Moves a card (string) between a source string-list and a destination string-list
    /// </summary>
    public void MoveCardBetweenLists(string card, List<string> srcList, List<string> destList)
    {
        destList.Add(card);
        srcList.Remove(card);
    }

    /// <summary>
    /// Sorts list of cards (strings) based on a pre-sorted "master list"
    /// </summary>
    public List<string> SortCardsList(List<string> list)
    {
        list = list.OrderBy(d => sortedDeck.IndexOf(d)).ToList();
        Debug.Log("Sorted list");
        return list;
    }


    /// <summary>
    /// Sorts children (gameobjects) of parent gameobject based on provided list of cards (strings)
    /// </summary>
    public void SortCardsGameObjects(List<string> list, GameObject parent)
    {

        for (int i = 0; i < list.Count; i++)
        {
            foreach (Transform child in parent.transform)
            {
                if (list[i].Equals(child.name))
                {
                    child.transform.SetSiblingIndex(i);
                }
            }
        }
    }
}
