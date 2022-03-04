using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameState {
    MatchStart,
    FirstDraw,
    BetPlayer1Start,
    BetPlayer1End,
    BetPlayer2Start,
    BetPlayer2End,
    BetPlayer3Start,
    BetPlayer3End,
    BetPlayer4Start,
    BetPlayer4End,
    ChooseSuit,
    FirstDiscard,
    SecondDraw,
    SecondDiscard,
    TurnPlayer1Start,
    TurnPlayer1End,
    TurnPlayer2Start,
    TurnPlayer2End,
    TurnPlayer3Start,
    TurnPlayer3End,
    TurnPlayer4Start,
    TurnPlayer4End,
    CalculateWinner,
    MatchEnd
}

public class GameManager : MonoBehaviour
{
    [Header("Visible for debugging")]
    public string selectedSuit;
    public bool firstDeal = true;  
    public int minBet;
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;
    public int betIndex;
    private int currentPlayer;
    [Space(20)] 

    [Header("Card GameObject")]
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    [Space(20)]

    [Header("Players' Hand Gameobjects")]
    public GameObject player1_hand_area;
    public GameObject player2_hand_area;
    public GameObject player3_hand_area;
    public GameObject player4_hand_area;
    [Space(20)]

    [Header("Players' Field Gameobjects")]
    public GameObject player1_field_area;
    public GameObject player2_field_area;
    public GameObject player3_field_area;
    public GameObject player4_field_area;
    [Space(20)]

    private static string[] suits = new string[] {"Spades", "Hearts", "Clubs", "Diamonds"};
    private static string[] values = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};

    private List<string> player1_hand = new List<string>();
    private List<string> player2_hand = new List<string>();
    private List<string> player3_hand = new List<string>();
    private List<string> player4_hand = new List<string>();

    private List<string> player1_field = new List<string>();
    private List<string> player2_field = new List<string>();
    private List<string> player3_field = new List<string>();
    private List<string> player4_field = new List<string>();

    [Header("Player bets")]
    public int player1_bet = 0;
    public int player2_bet = 0;
    public int player3_bet = 0;
    public int player4_bet = 0;
    [Space(20)]

    [Header("Highest 14")]
    public int highest14;
    [Space(20)]

    private List<string> hasSprite = new List<string>();


    private List<string> discard = new List<string>();
    
    private List<string> deck;
    private List<string> sortedDeck;
    
    [Header("Current Player's Information")]
    public List<string> currentPlayer_hand;
    public List<string> currentPlayer_field;
    public GameObject currentPlayer_hand_area;
    public GameObject currentPlayer_field_area;
    public int currentPlayer_bet = 0;
    [Space(20)]

    private int firstPlayerIndex; // Increment by 1 after each round. If 4 set it to 1 rather than increment.

    // Singleton references
    private LayoutManager layoutManager;
    private BetManager betManager;


    // Singleton declaration
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }


    private bool isBetting = false;


    void Awake()
    {
        // Disallow duplicate GameManager instances as it is a singleton
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this; 
        }

        // Set singleton references to their respective instances
        layoutManager = LayoutManager.Instance;
        betManager = BetManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        cardFaces = Resources.LoadAll<Sprite>("English_pattern_playing_cards_deck");

        UpdateGameState(GameState.MatchStart);

        //TODO: Remove these when states verified working.
        // Create the deck from which we draw cards
        // deck = GenerateDeck();
        // Shuffle(deck);

        // Create another deck, the indeces of which are used for sorting
        // sortedDeck = GenerateDeck();

        //DEBUG: sets current hand and field
        currentPlayer_hand = player1_hand;
        currentPlayer_field = player1_field;
        currentPlayer_hand_area = player1_hand_area;
        currentPlayer_field_area = player1_field_area;

        selectedSuit = "Hearts";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateGameState(GameState newState) 
    {
        state = newState;

        switch (newState)
        {
            case GameState.MatchStart:
                HandleMatchStart();
                break;
            case GameState.FirstDraw:
                HandleFirstDraw();
                break;
            case GameState.BetPlayer1Start:
                HandleBetPlayer1Start();
                break;
            case GameState.BetPlayer1End:
                HandleBetPlayer1End();
                break;
            case GameState.BetPlayer2Start:
                HandleBetPlayer2Start();
                break;
            case GameState.BetPlayer2End:
                HandleBetPlayer2End();
                break;
            case GameState.BetPlayer3Start:
                HandleBetPlayer3Start();
                break;
            case GameState.BetPlayer3End:
                HandleBetPlayer3End();
                break;
            case GameState.BetPlayer4Start:
                HandleBetPlayer4Start();
                break;
            case GameState.BetPlayer4End:
                HandleBetPlayer4End();
                break;
            case GameState.ChooseSuit:
                HandleChooseSuit();
                break;
            case GameState.FirstDiscard:
                HandleFirstDiscard();
                break;
            case GameState.SecondDraw:
                HandleSecondDraw();
                break;
            case GameState.SecondDiscard:
                HandleSecondDiscard();
                break;
            case GameState.TurnPlayer1Start:
                break;
            case GameState.TurnPlayer1End:
                break;
            case GameState.TurnPlayer2Start:
                break;
            case GameState.TurnPlayer2End:
                break;
            case GameState.TurnPlayer3Start:
                break;
            case GameState.TurnPlayer3End:
                break;
            case GameState.TurnPlayer4Start:
                break;
            case GameState.TurnPlayer4End:
                break;
            case GameState.CalculateWinner:
                break;
            case GameState.MatchEnd:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void HandleMatchStart()
    {
        // Create the deck from which we draw cards
        deck = GenerateDeck();

        // Create another deck, the indeces of which are used for sorting
        sortedDeck = GenerateDeck();

        // For the first round, Player 1 always starts.
        firstPlayerIndex = 1;
        UpdateGameState(GameState.FirstDraw);
    }

    public void HandleFirstDraw()
    {
        Shuffle(deck);
        betIndex = 1;

        for (int i = 0; i < 3; i++)
            {
                //Debug.Log("Dealt 3 cards to player 1");
                DrawCards(3, player1_hand, player1_hand_area);

                //Debug.Log("Dealt 3 cards to player 2");
                DrawCards(3, player2_hand, player2_hand_area);

                //Debug.Log("Dealt 3 cards to player 3");
                DrawCards(3, player3_hand, player3_hand_area);

                //Debug.Log("Dealt 3 cards to player 4");
                DrawCards(3, player4_hand, player4_hand_area);
            }

        player1_hand = SortCardsList(player1_hand);
        SortCardsGameObjects(player1_hand, player1_hand_area);

        player2_hand = SortCardsList(player2_hand);
        SortCardsGameObjects(player2_hand, player2_hand_area);

        player3_hand = SortCardsList(player3_hand);
        SortCardsGameObjects(player3_hand, player3_hand_area);

        player4_hand = SortCardsList(player4_hand);
        SortCardsGameObjects(player4_hand, player4_hand_area);
        
        int order = DeterminePlayerOrder();

        switch (order) {
            case 1:
                UpdateGameState(GameState.BetPlayer1Start);
                break;
            case 2:
                UpdateGameState(GameState.BetPlayer2Start);
                break;
            case 3:
                UpdateGameState(GameState.BetPlayer3Start);
                break;
            case 4:
                UpdateGameState(GameState.BetPlayer4Start);
                break;
        }
    }

    public void HandleBetPlayer1Start()
    {
        currentPlayer = 1;
        layoutManager.Player1Turn();
        betManager.EnableBettingUI();
    }

    public void HandleBetPlayer1End()
    {
        player1_bet = currentPlayer_bet;
        if (betIndex < 4)
        {
            betIndex++;
            UpdateGameState(GameState.BetPlayer2Start);
        } 
        else
        {
            UpdateGameState(GameState.ChooseSuit);
        }
    }

    public void HandleBetPlayer2Start()
    {
        currentPlayer = 2;
        layoutManager.Player2Turn();
        betManager.EnableBettingUI();
    }

    public void HandleBetPlayer2End()
    {
        player2_bet = currentPlayer_bet;
        if (betIndex < 4)
        {
            betIndex++;
            UpdateGameState(GameState.BetPlayer3Start);
        } 
        else
        {
            UpdateGameState(GameState.ChooseSuit);
        }
    }

    public void HandleBetPlayer3Start()
    {
        currentPlayer = 3;
        layoutManager.Player3Turn();
        betManager.EnableBettingUI();
    }

    public void HandleBetPlayer3End()
    {
        player3_bet = currentPlayer_bet;
        if (betIndex < 4)
        {
            betIndex++;
            UpdateGameState(GameState.BetPlayer4Start);
        } 
        else
        {
            UpdateGameState(GameState.ChooseSuit);
        }
    }

    public void HandleBetPlayer4Start()
    {
        currentPlayer = 4;
        layoutManager.Player4Turn();
        betManager.EnableBettingUI();
    }

    public void HandleBetPlayer4End()
    {
        player4_bet = currentPlayer_bet;
        if (betIndex < 4)
        {
            betIndex++;
            UpdateGameState(GameState.BetPlayer1Start);
        } 
        else
        {
            UpdateGameState(GameState.ChooseSuit);
        }
    }

    public void HandleChooseSuit()
    {
        if(player1_bet == 0 && player2_bet == 0 && player3_bet == 0 && player4_bet == 0)
        {
            player1_bet = 6; // should be dealer
        } else {
           int highest = GetHighestBet(); 

           //FIXME: Make sure that in the case where multiple people bid 14, the last to do so wins
           if (player1_bet == highest || highest14 == 1)
           {
               Debug.Log("player 1 won bet");
               layoutManager.Player1Turn();
               currentPlayer = 1;
           } 
           else if (player2_bet == highest || highest14 == 2)
           {
               Debug.Log("player 2 won bet");
               layoutManager.Player2Turn();
               currentPlayer = 2;
           } 
           else if (player3_bet == highest || highest14 == 3)
           {
               Debug.Log("player 3 won bet");
               layoutManager.Player3Turn();
               currentPlayer = 3;
           } 
           else if (player4_bet == highest || highest14 == 4)
           {
               Debug.Log("player 4 won bet");
               layoutManager.Player4Turn();
               currentPlayer = 4;
           } 
           else 
           {
               throw new Exception("Error finding highest bet");
           }

           betManager.EnableSuitSelectionUI();
        }
    }

    public void HandleFirstDiscard()
    {
        Debug.Log("Discarding p1");
        DiscardCards(player1_hand, player1_hand_area);

        Debug.Log("Discarding p2");
        DiscardCards(player2_hand, player2_hand_area);
        
        Debug.Log("Discarding p3");
        DiscardCards(player3_hand, player3_hand_area);
        
        Debug.Log("Discarding p4");
        DiscardCards(player4_hand, player4_hand_area);

        // Discard non-suited cards so that 6 remain, prioritized by value or whether pidro
        // Move to second draw
    }

    public void HandleSecondDraw()
    {
        // Draw new cards unless.. reasons
        // Move to second discard
    }

    public void HandleSecondDiscard()
    {
        // Discard non-suited cards so that 6 remain, prioritized by value or whether pidro
        // Move to first round of the game

        int order = DeterminePlayerOrder();

        switch (order) {
            case 1:
                UpdateGameState(GameState.TurnPlayer1Start);
                break;
            case 2:
                UpdateGameState(GameState.TurnPlayer2Start);
                break;
            case 3:
                UpdateGameState(GameState.TurnPlayer3Start);
                break;
            case 4:
                UpdateGameState(GameState.TurnPlayer4Start);
                break;
        }
    }

    public int GetHighestBet()
    {
        int max = player1_bet;

        if (player2_bet > max)
            max = player2_bet;
        if (player3_bet > max)
            max = player3_bet;
        if (player4_bet > max)
            max = player4_bet;

        return max;
    }

    //TODO: Remove when states verified working.
    // public void PlayCards()
    // {

    //     if (firstDeal)
    //     {
    //         for (int i = 0; i < 3; i++)
    //         {
    //             //Debug.Log("Dealt 3 cards to player 1");
    //             currentPlayer_hand = player1_hand;
    //             currentPlayer_hand_area = player1_hand_area;
    //             DealCards(3, currentPlayer_hand, currentPlayer_hand_area);

    //             //Debug.Log("Dealt 3 cards to player 2");
    //             currentPlayer_hand = player2_hand;
    //             currentPlayer_hand_area = player2_hand_area;
    //             DealCards(3, currentPlayer_hand, currentPlayer_hand_area);

    //             //Debug.Log("Dealt 3 cards to player 3");
    //             currentPlayer_hand = player3_hand;
    //             currentPlayer_hand_area = player3_hand_area;
    //             DealCards(3, currentPlayer_hand, currentPlayer_hand_area);

    //             //Debug.Log("Dealt 3 cards to player 4");
    //             currentPlayer_hand = player4_hand;
    //             currentPlayer_hand_area = player4_hand_area;
    //             DealCards(3, currentPlayer_hand, currentPlayer_hand_area);

    //         }
    //         firstDeal = false;
    //     } else 
    //     {
    //         Debug.Log("Second deal");
    //     }
    //     player1_hand = SortCardsList(player1_hand);
    //     SortCardsGameObjects(player1_hand, player1_hand_area);

    //     player2_hand = SortCardsList(player2_hand);
    //     SortCardsGameObjects(player2_hand, player2_hand_area);

    //     player3_hand = SortCardsList(player3_hand);
    //     SortCardsGameObjects(player3_hand, player3_hand_area);

    //     player4_hand = SortCardsList(player4_hand);
    //     SortCardsGameObjects(player4_hand, player4_hand_area);


    //     //DEBUG:
    //     currentPlayer_hand = player1_hand;
    //     currentPlayer_field = player1_field;
    //     currentPlayer_hand_area = player1_hand_area;
    //     currentPlayer_field_area = player1_field_area;
    // }

    public int DeterminePlayerOrder()
    {
        switch (firstPlayerIndex)
        {
            case 1: // Player 1 is first, Player 2 second, Player 3 third, Player 4 fourth.
                return 1;
            case 2: // Player 2 is first, Player 3 second, Player 4 third, Player 1 fourth.
                return 2;
            case 3: // Player 3 is first, Player 4 second, Player 1 third, Player 2 fourth.
                return 3;
            case 4: // Player 4 is first, Player 1 second, Player 2 third, Player 3 fourth.
                return 4;
            default:
                throw new Exception("firstPlayerIndex out of bounds");
        }
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

    // TODO: Remove when states verified working.
    // Deprecated but used for reference
    // void PidroDeal()
    // {
    //     float yOffset = 0;
    //     float zOffset = 0.03f;
    //     foreach (string card in deck)
    //     {
    //         GameObject newCard = Instantiate(cardPrefab, new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z - zOffset), Quaternion.identity);
    //         newCard.name = card;
    //         newCard.GetComponent<Selectable>().faceUp = true;

    //         yOffset += 1f;
    //         zOffset += 0.03f;
    //     }
    // }

    void DrawCards(int amount, List<string> hand, GameObject area)
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

    /// <summary>
    /// Removes cards that do not belong to the selected suite from currentPlayer_hand and adds it to a discard list and removes any matching GameObjects from the scene.
    /// </summary>
    public void DiscardCards(List<string> currentPlayer_hand, GameObject currentPlayer_hand_area)
    {
        // Adds each non-matching card in currentPlayer_hand list to discard list. (Does not remove, because can't modify while enumerating)
        foreach (string card in currentPlayer_hand)
        {
            if (!card.Substring(0,card.Length).Contains(selectedSuit))
            {
                discard.Add(card);
            }
        }

        // Removes each card in discard list from currentPlayer_hand list.
        for (int i = 0; i < currentPlayer_hand.Count; )
        {
            if (discard.Contains(currentPlayer_hand[i]))
            {
                Destroy(currentPlayer_hand_area.transform.Find(currentPlayer_hand[i]).gameObject); //Destroy gameobject in hand area with a name that matches i
                currentPlayer_hand.RemoveAt(i);
            } else {
                i++;
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

    public void SetHighest14(int playerNumber)
    {
        switch(playerNumber)
        {
            case 1:
                highest14 = 1;
                break;
            case 2:
                highest14 = 2;
                break;
            case 3:
                highest14 = 3;
                break;
            case 4:
                highest14 = 4;
                break;
        }
    }

    /// <summary>
    /// Sets the current player's bet value to parameter value
    /// </summary>
    public void SetBet(int newBetValue)
    {
        currentPlayer_bet = newBetValue;
    }

    public int GetMinBet()
    {
        return minBet;
    }

    public void DebugPrint()
    {
        Debug.Log("all my homies hate putin");
    }

    public int GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void SetSuit(string suit)
    {
        selectedSuit = suit;
    }

    public void SetFirstPlayer(int player)
    {
        firstPlayerIndex = player;
    }
}