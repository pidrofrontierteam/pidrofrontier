using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pidro : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;

    public GameObject playerArea;

    public static string[] suits = new string[] {"Spades", "Hearts", "Diamonds", "Clubs"};
    public static string[] values = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};

    

    private List<string> player1_hand = new List<string>();
    private List<string> player2_hand = new List<string>();
    private List<string> player3_hand = new List<string>();
    private List<string> player4_hand = new List<string>();

    private List<string> player1_field = new List<string>();
    private List<string> player2_field = new List<string>();
    private List<string> player3_field = new List<string>();
    private List<string> player4_field = new List<string>();



    public List<string> discard = new List<string>();
    
    public List<string> deck;

    public List<string> currentPlayer_hand;
    public List<string> currentPlayer_field;

    public void PlayCards()
    {
        DealCards(currentPlayer_hand);
    }

    // Start is called before the first frame update
    void Start()
    {
        cardFaces = Resources.LoadAll<Sprite>("English_pattern_playing_cards_deck");

        // PlayCards();
        deck = GenerateDeck();
        Shuffle(deck);

        // for DEBUG: sets current hand and field
        currentPlayer_hand = player1_hand;
        currentPlayer_field = player1_field;
    }

    // Update is called once per frame
    void Update()
    {

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

    void DealCards(List<string> hand)
    {

        for (int i = 0; i < 9; i++)
        {
            hand.Add(deck.Last<string>());
            deck.RemoveAt(deck.Count - 1);
        }

        foreach (string card in hand)
        {
            GameObject newCard = Instantiate(cardPrefab, playerArea.transform, true);
            newCard.transform.SetParent(playerArea.transform, false);
            newCard.name = card;
            newCard.GetComponent<Selectable>().faceUp = true;

            newCard.transform.position = playerArea.transform.position;
            newCard.transform.position = new Vector3(newCard.transform.position.x, newCard.transform.position.y, -1);
        }
    }


    public void MoveCardBetweenStringLists(string card, List<string> srcList, List<string> destList)
    {
        destList.Add(card);
        srcList.Remove(card);
    }
}
