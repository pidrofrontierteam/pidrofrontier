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
    public string sameColorSuit;
    public bool firstDeal = true;  
    public int minBet;
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;
    public int betIndex;
    public int currentPlayer;
    public int firstPlayerIndex; // Increment by 1 after each round. If 4 set it to 1 rather than increment.
    public int dealer;
    
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



    // Singleton references
    private LayoutManager layoutManager;
    private BetManager betManager;


    // Singleton declaration
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }


    private bool isBetting = false;


    // Awake is called when the script instance is being loaded.
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

    // Start is called before the first frame update.
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

    // Update is called once per frame.
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
                StartCoroutine(HandleFirstDiscard());
                break;
            case GameState.SecondDraw:
                StartCoroutine(HandleSecondDraw());
                break;
            case GameState.SecondDiscard:
                StartCoroutine(HandleSecondDiscard());
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

        // Invokes a new states only if there's been a change in gamestates
        OnGameStateChanged?.Invoke(newState);
    }

    #region GameStateHandling

    public void HandleMatchStart()
    {
        Debug.Log("===HANDLE MATCH START===");
        // Create the deck from which we draw cards
        deck = GenerateDeck();

        // Create another deck, the indeces of which are used for sorting
        sortedDeck = GenerateDeck();

        dealer = 1; //FIXME: DEBUG
        SetFirstPlayer(dealer);
        
        UpdateGameState(GameState.FirstDraw);
    }

    public void HandleFirstDraw()
    {
        Debug.Log("===HANDLE FIRST DRAW===");
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
        Debug.Log("===HANDLE BET PLAYER 1===");
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
        Debug.Log("===HANDLE BET PLAYER 2===");
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
        Debug.Log("===HANDLE BET PLAYER 3===");
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
        Debug.Log("===HANDLE BET PLAYER 4===");
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
        Debug.Log("===HANDLE CHOOSE SUIT===");
        if(player1_bet == 0 && player2_bet == 0 && player3_bet == 0 && player4_bet == 0)
        {
            switch(dealer)
            {
                case 1:
                    player1_bet = 6;
                    break;
                case 2:
                    player2_bet = 6;
                    break;
                case 3:
                    player3_bet = 6;
                    break;
                case 4:
                    player4_bet = 6;
                    break;
            }
        } else {
           int highest = GetHighestBet(); 

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
        }
        betManager.EnableSuitSelectionUI();
    }

    public IEnumerator HandleFirstDiscard()
    {
        Debug.Log("===HANDLE FIRST DISCARD===");
        Debug.Log("Discarding p1");
        DiscardCards(player1_hand, player1_hand_area);
        yield return new WaitForSeconds(1);

        Debug.Log("Discarding p2");
        DiscardCards(player2_hand, player2_hand_area);
        yield return new WaitForSeconds(1);
        
        Debug.Log("Discarding p3");
        DiscardCards(player3_hand, player3_hand_area);
        yield return new WaitForSeconds(1);
        
        Debug.Log("Discarding p4");
        DiscardCards(player4_hand, player4_hand_area);
        yield return new WaitForSeconds(1);

        UpdateGameState(GameState.SecondDraw);
    }

    public IEnumerator HandleSecondDraw()
    {
        Debug.Log("===HANDLE SECOND DRAW===");

        //TODO: Dealer gets all remaining cards.


        yield return new WaitForSeconds(1);
        while(player1_hand.Count() < 6)
        {
            Debug.Log("Drew 1 card for p1");
            DrawCards(1, player1_hand, player1_hand_area);
        }

        yield return new WaitForSeconds(1);
        while(player2_hand.Count() < 6)
        {
            Debug.Log("Drew 1 card for p2");
            DrawCards(1, player2_hand, player2_hand_area);
        }

        yield return new WaitForSeconds(1);
        while(player3_hand.Count() < 6)
        {
            Debug.Log("Drew 1 card for p3");
            DrawCards(1, player3_hand, player3_hand_area);
        }

        yield return new WaitForSeconds(1);
        while(player4_hand.Count() < 6)
        {
            Debug.Log("Drew 1 card for p4");
            DrawCards(1, player4_hand, player4_hand_area);
        }

        player1_hand = SortCardsList(player1_hand);
        SortCardsGameObjects(player1_hand, player1_hand_area);

        player2_hand = SortCardsList(player2_hand);
        SortCardsGameObjects(player2_hand, player2_hand_area);

        player3_hand = SortCardsList(player3_hand);
        SortCardsGameObjects(player3_hand, player3_hand_area);

        player4_hand = SortCardsList(player4_hand);
        SortCardsGameObjects(player4_hand, player4_hand_area);

        yield return new WaitForSeconds(1);
        UpdateGameState(GameState.SecondDiscard);
    }

    public IEnumerator HandleSecondDiscard()
    {
        Debug.Log("===HANDLE SECOND DISCARD===");
        //TODO: Dealer does not discard unsuited cards, and always keeps 6. Unsuited card values don't matter.

        Debug.Log("Discarding unsuited p1");
        DiscardCards(player1_hand, player1_hand_area);
        Debug.Log("Discarding suited p1");
        while(player1_hand.Count() > 6)
        {
            DiscardLowestSuited(player1_hand, player1_hand_area);
        }
        yield return new WaitForSeconds(1);

        Debug.Log("Discarding unsuited p2");
        DiscardCards(player2_hand, player2_hand_area);
        Debug.Log("Discarding suited p2");
        while(player2_hand.Count() > 6)
        {
            DiscardLowestSuited(player2_hand, player2_hand_area);
        }
        yield return new WaitForSeconds(1);
        
        Debug.Log("Discarding unsuited p3");
        DiscardCards(player3_hand, player3_hand_area);
        Debug.Log("Discarding suited p3");
        while(player3_hand.Count() > 6)
        {
            DiscardLowestSuited(player3_hand, player3_hand_area);
        }
        yield return new WaitForSeconds(1);
        
        Debug.Log("Discarding unsuited p4");
        DiscardCards(player4_hand, player4_hand_area);
        Debug.Log("Discarding suited p4");
        while(player4_hand.Count() > 6)
        {
            DiscardLowestSuited(player4_hand, player4_hand_area);
        }
        yield return new WaitForSeconds(1);

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

    #endregion GameStateHandling

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

    void DrawCards(int amount, List<string> hand, GameObject area)
    {
        try
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
        catch (System.Exception)
        {
            Debug.Log("Exception caught, deck is probably empty. Congratulations on the one in a ???-illion error.");
            //FIXME: Show congratulatory error message and restart round.
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
            if (card.Substring(0,card.Length).Contains(sameColorSuit) && (card.Substring(0,card.Length).Contains("5")))
            {
                //If wrong color pidro is found, do not add it to the discard list.
                Debug.Log("Wrong color pidro found");
            }
            else if (!card.Substring(0,card.Length).Contains(selectedSuit))
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

    public void DiscardLowestSuited(List<string> currentPlayer_hand, GameObject currentPlayer_hand_area)
    {
        foreach(string card in currentPlayer_hand)
        {
            if(!(card.Substring(0,card.Length).Contains("2") || card.Substring(0,card.Length).Contains("5") || card.Substring(0,card.Length).Contains("10") || card.Substring(0,card.Length).Contains("J") || card.Substring(0,card.Length).Contains("A")))
            {
                discard.Add(card);
                break;
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

    ///<summary>
    /// Sets selected suit to parameter value and also stores the same colored suit, which is used for determining "wrong color pidro"
    ///</summary>
    public void SetSuit(string suit)
    {
        selectedSuit = suit;

        switch(suit)
        {
            case "Spades":
                sameColorSuit = "Clubs";
                break;
            case "Hearts":
                sameColorSuit = "Diamonds";
                break;
            case "Clubs":
                sameColorSuit = "Spades";
                break;
            case "Diamonds":
                sameColorSuit = "Hearts";
                break;
        }
        
    }

    public void SetFirstPlayer(int value)
    {
        if(value == 4)
        {
            firstPlayerIndex = 1;
        } else {
            firstPlayerIndex = (value+1);
        }
        
    }
}