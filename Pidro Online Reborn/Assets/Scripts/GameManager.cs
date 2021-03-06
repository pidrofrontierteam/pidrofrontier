using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public enum GameState {
    MatchStart,
    FirstDraw,

    BetPlayer1Accept,
    BetPlayer1Start,
    BetPlayer1End,

    BetPlayer2Accept,
    BetPlayer2Start,
    BetPlayer2End,

    BetPlayer3Accept,
    BetPlayer3Start,
    BetPlayer3End,

    BetPlayer4Accept,
    BetPlayer4Start,
    BetPlayer4End,

    ChooseSuitAccept,
    ChooseSuit,
    FirstDiscard,
    SecondDraw,
    SecondDiscard,

    TurnPlayer1Accept,
    TurnPlayer1Start,
    TurnPlayer1Pass,
    TurnPlayer1End,

    TurnPlayer2Accept,
    TurnPlayer2Start,
    TurnPlayer2Pass,
    TurnPlayer2End,

    TurnPlayer3Accept,
    TurnPlayer3Start,
    TurnPlayer3Pass,
    TurnPlayer3End,

    TurnPlayer4Accept,
    TurnPlayer4Start,
    TurnPlayer4Pass,
    TurnPlayer4End,

    RoundEnd,
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
    public int turnIndex = 1;
    public int currentPlayer;
    public int firstPlayerIndex; // Increment by 1 after each round. If 4 set it to 1 rather than increment.
    public int dealer;
    public int redTeamPoints = 0;
    public int blueTeamPoints = 0;
    public bool isDeckOver = false;
    public bool redTeamHighestBet = false;
    public bool blueTeamHighestBet = false;
    public bool redPlay2 = false;
    public bool bluePlay2 = false;
    public int roundTotalPoints = 0;
    [Range(0.1f, 1f)]
    public float waitTime = 0.5f;

    [Space(20)] 

    private bool redWon = false;
    private bool blueWon = false;

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

    public List<string> player1_hand = new List<string>();
    public List<string> player2_hand = new List<string>();
    public List<string> player3_hand = new List<string>();
    public List<string> player4_hand = new List<string>();

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

    [Header("Player played cards' ranks")]
    public int player1_playedCardRank = 0;
    public int player2_playedCardRank = 0;
    public int player3_playedCardRank = 0;
    public int player4_playedCardRank = 0;
    [Space(20)]

    [Header("Highest 14")]
    public int highest14;
    [Space(20)]

    private List<string> hasSprite = new List<string>();

    private List<string> discard = new List<string>();
    
    private List<string> deck;
    private List<string> sortedDeck;

    public List<string> roundCardsPlayed;
    public List<string> roundCardsPlayedByRed;
    public List<string> roundCardsPlayedByBlue;
    public List<string> deckCardsPlayed;
    
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
    private TurnManager turnManager;

    // Singleton declaration
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    //UI references
    [SerializeField] private Text redTeamPointsText;
    [SerializeField] private Text blueTeamPointsText;
    [SerializeField] private Text teamBetText;
    [SerializeField] private Text betValueText;

    [SerializeField] private Transform winPanel;
    [SerializeField] private Text colorTeam;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button quitButton;


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
        turnManager = TurnManager.Instance;
    }

    // Start is called before the first frame update.
    void Start()
    {
        teamBetText.text = "";
        betValueText.text = "";
        
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
        // selectedSuit = "Hearts";
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
            case GameState.BetPlayer1Accept:
                HandleBetPlayer1Accept();
                break;
            case GameState.BetPlayer1Start:
                HandleBetPlayer1Start();
                break;
            case GameState.BetPlayer1End:
                HandleBetPlayer1End();
                break;
            case GameState.BetPlayer2Accept:
                HandleBetPlayer2Accept();
                break;
            case GameState.BetPlayer2Start:
                HandleBetPlayer2Start();
                break;
            case GameState.BetPlayer2End:
                HandleBetPlayer2End();
                break;
            case GameState.BetPlayer3Accept:
                HandleBetPlayer3Accept();
                break;
            case GameState.BetPlayer3Start:
                HandleBetPlayer3Start();
                break;
            case GameState.BetPlayer3End:
                HandleBetPlayer3End();
                break;
            case GameState.BetPlayer4Accept:
                HandleBetPlayer4Accept();
                break;
            case GameState.BetPlayer4Start:
                HandleBetPlayer4Start();
                break;
            case GameState.BetPlayer4End:
                HandleBetPlayer4End();
                break;
            case GameState.ChooseSuitAccept:
                HandleChooseSuitAccept();
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
            case GameState.TurnPlayer1Accept:
                HandleTurnPlayer1Accept();
                break;
            case GameState.TurnPlayer1Start:
                HandleTurnPlayer1Start();
                break;
            case GameState.TurnPlayer1Pass:
                StartCoroutine(HandleTurnPlayer1Pass());
                break;
            case GameState.TurnPlayer1End:
                HandleTurnPlayer1End();
                break;
            case GameState.TurnPlayer2Accept:
                HandleTurnPlayer2Accept();
                break;
            case GameState.TurnPlayer2Start:
                HandleTurnPlayer2Start();
                break;
            case GameState.TurnPlayer2Pass:
                StartCoroutine(HandleTurnPlayer2Pass());
                break;
            case GameState.TurnPlayer2End:
                HandleTurnPlayer2End();
                break;
            case GameState.TurnPlayer3Accept:
                HandleTurnPlayer3Accept();
                break;
            case GameState.TurnPlayer3Start:
                HandleTurnPlayer3Start();
                break;
            case GameState.TurnPlayer3Pass:
                StartCoroutine(HandleTurnPlayer3Pass());
                break;
            case GameState.TurnPlayer3End:
                HandleTurnPlayer3End();
                break;
            case GameState.TurnPlayer4Accept:
                HandleTurnPlayer4Accept();
                break;
            case GameState.TurnPlayer4Start:
                HandleTurnPlayer4Start();
                break;
            case GameState.TurnPlayer4Pass:
                StartCoroutine(HandleTurnPlayer4Pass());
                break;
            case GameState.TurnPlayer4End:
                HandleTurnPlayer4End();
                break;
            case GameState.RoundEnd:
                HandleRoundEnd();
                break;
            case GameState.MatchEnd:
                HandleMatchEnd();
                break;
        }

        // Invokes a new states only if there's been a change in gamestates
        OnGameStateChanged?.Invoke(newState);
    }

    #region GameStateHandling

    public void HandleMatchStart()
    {
        // Create the deck from which we draw cards
        deck = GenerateDeck();

        // Create another deck, the indeces of which are used for sorting
        sortedDeck = GenerateDeck();

        dealer = 1; //FIXME: DEBUG
        SetFirstPlayerBasedOnDealer(dealer);
        
        UpdateGameState(GameState.FirstDraw);
    }

    public void HandleFirstDraw()
    {
        Shuffle(deck);
        betIndex = 1;

        for (int i = 0; i < 3; i++)
            {
                DrawCards(3, player1_hand, player1_hand_area);
                ShowPlayer1Cards(false);

                DrawCards(3, player2_hand, player2_hand_area);
                ShowPlayer2Cards(false);

                DrawCards(3, player3_hand, player3_hand_area);
                ShowPlayer3Cards(false);

                DrawCards(3, player4_hand, player4_hand_area);
                ShowPlayer4Cards(false);
            }

        player1_hand = SortCardsList(player1_hand);
        SortCardsGameObjects(player1_hand, player1_hand_area);

        foreach(Transform child in player1_hand_area.transform)
        {
            UpdateRotationRelativeToParent(child);
        }

        player2_hand = SortCardsList(player2_hand);
        SortCardsGameObjects(player2_hand, player2_hand_area);

        foreach(Transform child in player2_hand_area.transform)
        {
            UpdateRotationRelativeToParent(child);
        }

        player3_hand = SortCardsList(player3_hand);
        SortCardsGameObjects(player3_hand, player3_hand_area);

        foreach(Transform child in player3_hand_area.transform)
        {
            UpdateRotationRelativeToParent(child);
        }

        player4_hand = SortCardsList(player4_hand);
        SortCardsGameObjects(player4_hand, player4_hand_area);

        foreach(Transform child in player4_hand_area.transform)
        {
            UpdateRotationRelativeToParent(child);
        }
        
        int order = DeterminePlayerOrder();

        // reset minimum bet between rounds
        betManager.ResetMinBet();
        
        switch (order) {
            case 1:
                UpdateGameState(GameState.BetPlayer1Accept);
                break;
            case 2:
                UpdateGameState(GameState.BetPlayer2Accept);
                break;
            case 3:
                UpdateGameState(GameState.BetPlayer3Accept);
                break;
            case 4:
                UpdateGameState(GameState.BetPlayer4Accept);
                break;
        }
    }

    public void HandleBetPlayer1Accept()
    {
        currentPlayer = 1;
        layoutManager.Player1Turn();
        betManager.EnableBetAcceptUI();
    }

    public void HandleBetPlayer1Start()
    {
        ShowPlayer1Cards(true);
        betManager.EnableBettingUI();
    }

    public void HandleBetPlayer1End()
    {
        ShowPlayer1Cards(false);
        player1_bet = currentPlayer_bet;
        if (betIndex < 4)
        {
            betIndex++;
            UpdateGameState(GameState.BetPlayer2Accept);
        } 
        else
        {
            UpdateGameState(GameState.ChooseSuitAccept);
        }
    }

    public void HandleBetPlayer2Accept()
    {
        currentPlayer = 2;
        layoutManager.Player2Turn();
        betManager.EnableBetAcceptUI();
    }

    public void HandleBetPlayer2Start()
    {
        ShowPlayer2Cards(true);
        betManager.EnableBettingUI();
    }

    public void HandleBetPlayer2End()
    {
        ShowPlayer2Cards(false);
        player2_bet = currentPlayer_bet;
        if (betIndex < 4)
        {
            betIndex++;
            UpdateGameState(GameState.BetPlayer3Accept);
        } 
        else
        {
            UpdateGameState(GameState.ChooseSuitAccept);
        }
    }

    public void HandleBetPlayer3Accept()
    {
        currentPlayer = 3;
        layoutManager.Player3Turn();
        betManager.EnableBetAcceptUI();
    }

    public void HandleBetPlayer3Start()
    {
        ShowPlayer3Cards(true);
        betManager.EnableBettingUI();
    }

    public void HandleBetPlayer3End()
    {
        ShowPlayer3Cards(false);
        player3_bet = currentPlayer_bet;
        if (betIndex < 4)
        {
            betIndex++;
            UpdateGameState(GameState.BetPlayer4Accept);
        } 
        else
        {
            UpdateGameState(GameState.ChooseSuitAccept);
        }
    }

    public void HandleBetPlayer4Accept()
    {
        currentPlayer = 4;
        layoutManager.Player4Turn();
        betManager.EnableBetAcceptUI();
    }

    public void HandleBetPlayer4Start()
    {
        ShowPlayer4Cards(true);
        betManager.EnableBettingUI();
    }

    public void HandleBetPlayer4End()
    {
        ShowPlayer4Cards(false);
        player4_bet = currentPlayer_bet;
        if (betIndex < 4)
        {
            betIndex++;
            UpdateGameState(GameState.BetPlayer1Accept);
        } 
        else
        {
            UpdateGameState(GameState.ChooseSuitAccept);
        }
    }

    public void HandleChooseSuitAccept()
    {
        if(player1_bet == 0 && player2_bet == 0 && player3_bet == 0 && player4_bet == 0)
        {
            switch(dealer)
            {
                case 1:
                    player1_bet = 6;
                    teamBetText.text = "Red Team";
                    betValueText.text = "bet 6";
                    redTeamHighestBet = true;
                    blueTeamHighestBet = false;
                    SetFirstPlayer(1);
                    break;
                case 2:
                    player2_bet = 6;
                    teamBetText.text = "Blue Team";
                    betValueText.text = "bet 6";
                    redTeamHighestBet = false;
                    blueTeamHighestBet = true;  
                    SetFirstPlayer(2);
                    break;
                case 3:
                    player3_bet = 6;
                    teamBetText.text = "Red Team";
                    betValueText.text = "bet 6";
                    redTeamHighestBet = true;
                    blueTeamHighestBet = false;
                    SetFirstPlayer(3);
                    break;
                case 4:
                    player4_bet = 6;
                    teamBetText.text = "Blue Team";
                    betValueText.text = "bet 6";
                    redTeamHighestBet = false;
                    blueTeamHighestBet = true;  
                    SetFirstPlayer(4);
                    break;
            }
        } else {
           int highest = GetHighestBet(); 

           if (player1_bet == highest || highest14 == 1)
           {
                Debug.Log("player 1 won bet");
                layoutManager.Player1Turn();
                currentPlayer = 1;
                SetFirstPlayer(1);
                redTeamHighestBet = true;
                blueTeamHighestBet = false;
                teamBetText.text = "Red Team";
                betValueText.text = "bet "+ player1_bet;
           } 
           else if (player2_bet == highest || highest14 == 2)
           {
                Debug.Log("player 2 won bet");
                layoutManager.Player2Turn();
                currentPlayer = 2;
                SetFirstPlayer(2);
                redTeamHighestBet = false;
                blueTeamHighestBet = true;                
                teamBetText.text = "Blue Team";
                betValueText.text = "bet "+ player2_bet;
           } 
           else if (player3_bet == highest || highest14 == 3)
           {
                Debug.Log("player 3 won bet");
                layoutManager.Player3Turn();
                currentPlayer = 3;
                SetFirstPlayer(3);
                redTeamHighestBet = true;
                blueTeamHighestBet = false;
                teamBetText.text = "Red Team";
                betValueText.text = "bet "+ player3_bet;
           } 
           else if (player4_bet == highest || highest14 == 4)
           {
                Debug.Log("player 4 won bet");
                layoutManager.Player4Turn();
                currentPlayer = 4;
                redTeamHighestBet = false;
                blueTeamHighestBet = true;
                teamBetText.text = "Blue Team";
                betValueText.text = "bet "+ player4_bet;
           } 
           else 
           {
               throw new Exception("Error finding highest bet");
           }
        }
        betManager.EnableSuitSelectionAcceptUI();
    }

    public void HandleChooseSuit()
    {
        betManager.EnableSuitSelectionUI();
    }

    public IEnumerator HandleFirstDiscard()
    {
        Debug.Log("Discarding p1");
        currentPlayer_hand = player1_hand;
        currentPlayer_hand_area = player1_hand_area;
        DiscardCards(player1_hand, player1_hand_area);
        yield return new WaitForSeconds(waitTime);

        Debug.Log("Discarding p2");
        currentPlayer_hand = player2_hand;
        currentPlayer_hand_area = player2_hand_area;
        DiscardCards(player2_hand, player2_hand_area);
        yield return new WaitForSeconds(waitTime);
        
        Debug.Log("Discarding p3");
        currentPlayer_hand = player3_hand;
        currentPlayer_hand_area = player3_hand_area;
        DiscardCards(player3_hand, player3_hand_area);
        yield return new WaitForSeconds(waitTime);
        
        Debug.Log("Discarding p4");
        currentPlayer_hand = player4_hand;
        currentPlayer_hand_area = player4_hand_area;
        DiscardCards(player4_hand, player4_hand_area);
        yield return new WaitForSeconds(waitTime);

        UpdateGameState(GameState.SecondDraw);
    }

    public IEnumerator HandleSecondDraw()
    {
        //TODO: Dealer gets all remaining cards.


        yield return new WaitForSeconds(waitTime);

        int failSafe = 0;
        while(player1_hand.Count() < 6)
        {
            if(failSafe == 50)
            {
                Debug.Log("ran out of cards");
                Application.Quit();
                break;
            }
            Debug.Log("Drew 1 card for p1");
            DrawCards(1, player1_hand, player1_hand_area);
            ShowPlayer1Cards(false);
            failSafe++;
        }

        failSafe = 0;
        yield return new WaitForSeconds(waitTime);
        while(player2_hand.Count() < 6)
        {
            if(failSafe == 50)
            {
                Debug.Log("ran out of cards");
                Application.Quit();
                break;
            }
            Debug.Log("Drew 1 card for p2");
            DrawCards(1, player2_hand, player2_hand_area);
            ShowPlayer2Cards(false);
            failSafe++;
        }

        failSafe = 0;
        yield return new WaitForSeconds(waitTime);
        while(player3_hand.Count() < 6)
        {
            if(failSafe == 50)
            {
                Debug.Log("ran out of cards");
                Application.Quit();
                break;
            }
            Debug.Log("Drew 1 card for p3");
            DrawCards(1, player3_hand, player3_hand_area);
            ShowPlayer3Cards(false);
            failSafe++;
            
        }

        failSafe = 0;
        yield return new WaitForSeconds(waitTime);
        while(player4_hand.Count() < 6)
        {
            if(failSafe == 50)
            {
                Debug.Log("ran out of cards");
                Application.Quit();
                break;
            }
            Debug.Log("Drew 1 card for p4");
            DrawCards(1, player4_hand, player4_hand_area);
            ShowPlayer4Cards(false);
            failSafe++;
        }

        player1_hand = SortCardsList(player1_hand);
        SortCardsGameObjects(player1_hand, player1_hand_area);
        foreach(Transform child in player1_hand_area.transform)
        {
            UpdateRotationRelativeToParent(child);
        }

        player2_hand = SortCardsList(player2_hand);
        SortCardsGameObjects(player2_hand, player2_hand_area);
        foreach(Transform child in player2_hand_area.transform)
        {
            UpdateRotationRelativeToParent(child);
        }

        player3_hand = SortCardsList(player3_hand);
        SortCardsGameObjects(player3_hand, player3_hand_area);
        foreach(Transform child in player3_hand_area.transform)
        {
            UpdateRotationRelativeToParent(child);
        }

        player4_hand = SortCardsList(player4_hand);
        SortCardsGameObjects(player4_hand, player4_hand_area);
        foreach(Transform child in player4_hand_area.transform)
        {
            UpdateRotationRelativeToParent(child);
        }

        yield return new WaitForSeconds(waitTime);
        UpdateGameState(GameState.SecondDiscard);
    }

    public IEnumerator HandleSecondDiscard()
    {
        //TODO: Dealer does not discard unsuited cards, and always keeps 6. Unsuited card values don't matter.

        int failSafe = 0;

        Debug.Log("Discarding unsuited p1");
        DiscardCards(player1_hand, player1_hand_area);
        Debug.Log("Discarding suited p1");
        while(player1_hand.Count() > 6)
        {
            if(failSafe == 50)
            {
                break;
            }
            DiscardLowestSuited(player1_hand, player1_hand_area);
            failSafe++;
        }
        yield return new WaitForSeconds(waitTime);

        failSafe = 0;
        Debug.Log("Discarding unsuited p2");
        DiscardCards(player2_hand, player2_hand_area);
        Debug.Log("Discarding suited p2");
        while(player2_hand.Count() > 6)
        {
            if(failSafe == 50)
            {
                break;
            }
            DiscardLowestSuited(player2_hand, player2_hand_area);
            failSafe++;
        }
        yield return new WaitForSeconds(waitTime);
        
        failSafe = 0;
        Debug.Log("Discarding unsuited p3");
        DiscardCards(player3_hand, player3_hand_area);
        Debug.Log("Discarding suited p3");
        while(player3_hand.Count() > 6)
        {
            if(failSafe == 50)
            {
                break;
            }
            DiscardLowestSuited(player3_hand, player3_hand_area);
            failSafe++;
        }
        yield return new WaitForSeconds(waitTime);

        failSafe = 0;
        Debug.Log("Discarding unsuited p4");
        DiscardCards(player4_hand, player4_hand_area);
        Debug.Log("Discarding suited p4");
        while(player4_hand.Count() > 6)
        {
            if(failSafe == 50)
            {
                break;
            }
            DiscardLowestSuited(player4_hand, player4_hand_area);
            failSafe++;
        }
        yield return new WaitForSeconds(waitTime);

        int order = DeterminePlayerOrder();

        switch (order) {
            case 1:
                UpdateGameState(GameState.TurnPlayer1Accept);
                break;
            case 2:
                UpdateGameState(GameState.TurnPlayer2Accept);
                break;
            case 3:
                UpdateGameState(GameState.TurnPlayer3Accept);
                break;
            case 4:
                UpdateGameState(GameState.TurnPlayer4Accept);
                break;
        }
    }

    public void HandleTurnPlayer1Accept()
    {
        currentPlayer_field = player1_field;
        currentPlayer_field_area = player1_field_area;
        currentPlayer_hand = player1_hand;
        currentPlayer_hand_area = player1_hand_area;

        currentPlayer = 1;
        layoutManager.Player1Turn();
        turnManager.EnableTurnAcceptUI();
    }

    public IEnumerator HandleTurnPlayer1Pass()
    {
        turnManager.EnableTurnPassUI();
        yield return new WaitForSeconds(2.5f);
        turnManager.DisableTurnPassUI();
        UpdateGameState(GameState.TurnPlayer1End);
    }

    public void HandleTurnPlayer1Start()
    {
        if(player1_hand.Count() > 0)
        {
            ShowPlayer1Cards(true);
        } 
        else 
        {
            Debug.Log("player 1 is out of cards");
            UpdateGameState(GameState.TurnPlayer1Pass);
        }
    }

    public void HandleTurnPlayer1End()
    {
        turnManager.SetHasAccepted(false);
        ShowPlayer1Cards(false);
        // NEXT TURN
        redTeamPointsText.text = redTeamPoints + " points";
        blueTeamPointsText.text = blueTeamPoints + " points";
        if (turnIndex < 4)
        {
            turnIndex++;
            UpdateGameState(GameState.TurnPlayer2Accept);
        } 
        else if (turnIndex >= 4)
        {
            //NEW ROUND
            UpdateGameState(GameState.RoundEnd);
        }
    }

    public void HandleTurnPlayer2Accept()
    {
        currentPlayer_field = player2_field;
        currentPlayer_field_area = player2_field_area;
        currentPlayer_hand = player2_hand;
        currentPlayer_hand_area = player2_hand_area;

        currentPlayer = 2;
        layoutManager.Player2Turn();
        turnManager.EnableTurnAcceptUI();
    }

    public IEnumerator HandleTurnPlayer2Pass()
    {
        turnManager.EnableTurnPassUI();
        yield return new WaitForSeconds(2.5f);
        turnManager.DisableTurnPassUI();
        UpdateGameState(GameState.TurnPlayer2End);
    }

    public void HandleTurnPlayer2Start()
    {
        if(player2_hand.Count() > 0)
        {
            ShowPlayer2Cards(true);
        } 
        else 
        {
            Debug.Log("player 2 is out of cards");
            UpdateGameState(GameState.TurnPlayer2Pass);
        }
    }

    public void HandleTurnPlayer2End()
    {
        turnManager.SetHasAccepted(false);
        ShowPlayer2Cards(false);
        // NEXT TURN
        redTeamPointsText.text = redTeamPoints + " points";
        blueTeamPointsText.text = blueTeamPoints + " points";
        if (turnIndex < 4)
        {
            turnIndex++;
            UpdateGameState(GameState.TurnPlayer3Accept);
        } 
        else if (turnIndex >= 4)
        {
            //NEW ROUND
            UpdateGameState(GameState.RoundEnd);
        }
    }

    public void HandleTurnPlayer3Accept()
    {
        currentPlayer_field = player3_field;
        currentPlayer_field_area = player3_field_area;
        currentPlayer_hand = player3_hand;
        currentPlayer_hand_area = player3_hand_area;

        currentPlayer = 3;
        layoutManager.Player3Turn();
        turnManager.EnableTurnAcceptUI();
    }

    public IEnumerator HandleTurnPlayer3Pass()
    {
        turnManager.EnableTurnPassUI();
        yield return new WaitForSeconds(2.5f);
        turnManager.DisableTurnPassUI();
        UpdateGameState(GameState.TurnPlayer3End);
    }

    public void HandleTurnPlayer3Start()
    {
        if(player3_hand.Count() > 0)
        {
            ShowPlayer3Cards(true);
        } 
        else 
        {
            Debug.Log("player 3 is out of cards");
            UpdateGameState(GameState.TurnPlayer3Pass);
        }
    }

    public void HandleTurnPlayer3End()
    {
        turnManager.SetHasAccepted(false);
        ShowPlayer3Cards(false);
        // NEXT TURN
        redTeamPointsText.text = redTeamPoints + " points";
        blueTeamPointsText.text = blueTeamPoints + " points";
        if (turnIndex < 4)
        {
            turnIndex++;
            UpdateGameState(GameState.TurnPlayer4Accept);
        } 
        else if (turnIndex >= 4)
        {
            //NEW ROUND
            UpdateGameState(GameState.RoundEnd);
        }
    }

    public void HandleTurnPlayer4Accept()
    {
        currentPlayer_field = player4_field;
        currentPlayer_field_area = player4_field_area;
        currentPlayer_hand = player4_hand;
        currentPlayer_hand_area = player4_hand_area;

        currentPlayer = 4;
        layoutManager.Player4Turn();
        turnManager.EnableTurnAcceptUI();
    }

    public IEnumerator HandleTurnPlayer4Pass()
    {
        turnManager.EnableTurnPassUI();
        yield return new WaitForSeconds(2.5f);
        turnManager.DisableTurnPassUI();
        UpdateGameState(GameState.TurnPlayer4End);
    }

    public void HandleTurnPlayer4Start()
    {
        if(player4_hand.Count() > 0)
        {
            ShowPlayer4Cards(true);
        } 
        else 
        {
            Debug.Log("player 4 is out of cards");
            UpdateGameState(GameState.TurnPlayer4Pass);
        }
    }

    public void HandleTurnPlayer4End()
    {
        turnManager.SetHasAccepted(false);
        ShowPlayer4Cards(false);
        // NEXT TURN
        redTeamPointsText.text = redTeamPoints + " points";
        blueTeamPointsText.text = blueTeamPoints + " points";
        if (turnIndex < 4)
        {
            turnIndex++;
            UpdateGameState(GameState.TurnPlayer1Accept);
        } 
        else if (turnIndex >= 4)
        {
            //NEW ROUND
            UpdateGameState(GameState.RoundEnd);
        }
    }


    public void HandleRoundEnd()
    {

        // calculate points
        int pointsToAward = 0;
        int pointsForCheck = 0;
        foreach(string card in roundCardsPlayed)
        {
            // add points of all cards that aren't a 2
            if(GetCardRank(card) != 0)
            {
                pointsToAward += GetCardPoints(card);
            }

        
        if(redTeamHighestBet)
        {
            foreach(string redcard in roundCardsPlayedByRed)            
            {
                if(GetCardRank(redcard) != 0 && !deckCardsPlayed.Contains(redcard))
                {
                    pointsForCheck += GetCardPoints(redcard);
                }
            }
        } else if(blueTeamHighestBet)
        {
            foreach(string bluecard in roundCardsPlayedByBlue)
            {
                if(GetCardRank(bluecard) != 0 && !deckCardsPlayed.Contains(bluecard))
                {
                    pointsForCheck += GetCardPoints(bluecard);
                }
            }
        }

            // add points of all cards, including the 2, for checking if points >= round's bet


        }
        Debug.Log(pointsToAward + " to award this round");

        // Checks whether round is over or not
        if(player1_hand.Count == 0 && player2_hand.Count == 0 && player3_hand.Count == 0 && player4_hand.Count == 0)
        {
            isDeckOver = true;
        } else {
            isDeckOver = false;
        }

        int highestCard = GetHighestPlayedCard();
        int roundBet = GetHighestBet();
        Debug.Log("highest");

        //FIXME:
        //FIXME:
        //FIXME:
        //FIXME:
        //FIXME:
        //FIXME:
        //FIXME:
        //FIXME:
        //FIXME:
        //FIXME:
        //FIXME:
        //FIXME:
        // PenaltyCheck is only done after a DECK and the TOTAL points of a team for one DECK is tallied and used for the penalty comparison
        // i.e. players lose points ONLY if they don't earn them in a DECK


        // MATCH
            // DECK n
                // ROUND n
                    // TURN 1
                    // TURN 2
                    // TURN 3
                    // TURN 4
                // ROUND n+1
                    // TURN 1
                    // TURN 2
                    // TURN 3
                    // TURN 4
                //<---- PenaltyCheck
            // DECK n+1
                // ROUND n
                    // TURN 1
                    // TURN 2
                    // TURN 3
                    // TURN 4
                // ROUND n+1
                    // TURN 1
                    // TURN 2
                    // TURN 3
                    // TURN4 
                //<---- PenaltyCheck
            // DECK n+2
            //....


        turnIndex = 1;
        if(highestCard == player1_playedCardRank)
        {
            Debug.Log("player 1 win");
            redTeamPoints += pointsToAward;
            CheckForWinner();
            ResetPlayedCardRanks();
            if(isDeckOver)
            {

                // new Deck, dealer++, new bets
                foreach(string card in deckCardsPlayed)
                {
                    pointsToAward += GetCardPoints(card);
                }
                PenaltyCheckRed(pointsForCheck, pointsToAward, roundBet, redPlay2, bluePlay2);
                redTeamPointsText.text = redTeamPoints + " points";
                blueTeamPointsText.text = blueTeamPoints + " points";
                IncrementDealer();
                Reset();
                UpdateGameState(GameState.FirstDraw);

            }
            else 
            {
                ResetPlayedCards();
                redTeamPointsText.text = redTeamPoints + " points";
                blueTeamPointsText.text = blueTeamPoints + " points";
                UpdateGameState(GameState.TurnPlayer1Accept);
                
            }
        }
        else if(highestCard == player2_playedCardRank)
        {
            Debug.Log("player 2 win");
            blueTeamPoints += pointsToAward;
            CheckForWinner();
            ResetPlayedCardRanks();
            if(isDeckOver)
            {
                // new Deck, dealer++, new bets
                PenaltyCheckBlue(pointsForCheck, pointsToAward, roundBet, redPlay2, bluePlay2);
                redTeamPointsText.text = redTeamPoints + " points";
                blueTeamPointsText.text = blueTeamPoints + " points";
                IncrementDealer();
                Reset();
                UpdateGameState(GameState.FirstDraw);
            }
            else 
            {
                ResetPlayedCards();
                redTeamPointsText.text = redTeamPoints + " points";
                blueTeamPointsText.text = blueTeamPoints + " points";
                UpdateGameState(GameState.TurnPlayer2Accept);
            }
        }
        else if(highestCard == player3_playedCardRank)
        {
            Debug.Log("player 3 win");
            redTeamPoints += pointsToAward;
            CheckForWinner();
            ResetPlayedCardRanks();
            if(isDeckOver)
            {
                // new Deck, dealer++, new bets
                PenaltyCheckRed(pointsForCheck, pointsToAward, roundBet, redPlay2, bluePlay2);
                IncrementDealer();
                Reset();
                UpdateGameState(GameState.FirstDraw);
                redTeamPointsText.text = redTeamPoints + " points";
                blueTeamPointsText.text = blueTeamPoints + " points";
            }
            else 
            {
                ResetPlayedCards();
                redTeamPointsText.text = redTeamPoints + " points";
                blueTeamPointsText.text = blueTeamPoints + " points";
                UpdateGameState(GameState.TurnPlayer3Accept);
            }
        }
        else if(highestCard == player4_playedCardRank)
        {
            Debug.Log("player 4 win");
            blueTeamPoints += pointsToAward;
            CheckForWinner();
            ResetPlayedCardRanks();
            if(isDeckOver)
            {
                // new Deck, dealer++, new bets
                PenaltyCheckBlue(pointsForCheck, pointsToAward, roundBet, redPlay2, bluePlay2);
                redTeamPointsText.text = redTeamPoints + " points";
                blueTeamPointsText.text = blueTeamPoints + " points";
                IncrementDealer();
                Reset();
                UpdateGameState(GameState.FirstDraw);
            }
            else 
            {
                ResetPlayedCards();
                redTeamPointsText.text = redTeamPoints + " points";
                blueTeamPointsText.text = blueTeamPoints + " points";
                UpdateGameState(GameState.TurnPlayer4Accept);
            }
        }
    }

    public void HandleMatchEnd()
    {
        if(redWon)
        {
            SceneManager.LoadScene("RedWinGame");
        } else if (blueWon)
        {
            SceneManager.LoadScene("BlueWinGame");
        }
    }

    #endregion GameStateHandling

    private void IncrementDealer()
    {
        if(dealer == 4)
            dealer = 1;
        else
            dealer++;

        layoutManager.ChangeDealer(dealer);
        Debug.Log("changed dealer to " + dealer);
    }

    public void PenaltyCheckRed(int pointsCheck, int pointsAward, int bet, bool red2, bool blue2)
    {
        Debug.Log("Penalty check for red: " + "| Red bet highest: " + redTeamHighestBet + "|" + pointsCheck + " pointsCheck |" + pointsAward + " pointsAward |" + bet + " bet |" + red2 + " redBool |" + blue2 + " bluebool |");
        if(pointsCheck < bet)
        {
            Debug.Log("reference points less than bet");
            if(redTeamHighestBet)
            {
                Debug.Log("red team bet highest");
                redTeamPoints -= bet;
                if(red2)
                {
                    Debug.Log("red played a 2");
                    redTeamPoints--;
                    redPlay2 = false;
                }
            } 
            else if(blueTeamHighestBet)
            {
                Debug.Log("blue team bet highest");
                blueTeamPoints -= bet;
                if(blue2)
                {
                    Debug.Log("blue played a 2");
                    blueTeamPoints--;
                    bluePlay2 = false;
                }
            }
        }
    }

    public void PenaltyCheckBlue(int pointsCheck, int pointsAward, int bet, bool red2, bool blue2)
    {
        Debug.Log("Penalty check for blue: " + "| Blue bet highest: " + blueTeamHighestBet + "|" + pointsCheck + " pointsCheck |" + pointsAward + " pointsAward |" + bet + " bet |" + red2 + " redBool |" + blue2 + " bluebool |");
        if(pointsCheck < bet)
        {
            Debug.Log("reference points less than bet");
            if(redTeamHighestBet)
            {
                Debug.Log("red team bet highest");
                redTeamPoints -= bet;

                if(red2)
                {
                    Debug.Log("red played a 2");
                    redTeamPoints--;
                    redPlay2 = false;
                }
            } 
            else if(blueTeamHighestBet)
            {
                Debug.Log("blue team bet highest");
                blueTeamPoints -= bet;
                if(blue2)
                {
                    Debug.Log("blue played a 2");
                    blueTeamPoints--;
                    bluePlay2 = false;
                }
            }
        }
    }

                // if points >= roundBet
                // did redTeam place bet?
                    // redTeam gets points
                // did blueTeam place bet?
                    // redTeam gets points
            // if points < roundBet
                // did redTeam place bet?
                    // redTeam gets penalized
                // did blueTeam place bet?
                    // redTeam gets points

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
    public void DiscardCards(List<string> currPlayer_hand, GameObject currPlayer_hand_area)
    {
        // Adds each non-matching card in currentPlayer_hand list to discard list. (Does not remove, because can't modify while enumerating)
        foreach (string card in currPlayer_hand)
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
        for (int i = 0; i < currPlayer_hand.Count; )
        {
            if (discard.Contains(currPlayer_hand[i]))
            {
                Destroy(currPlayer_hand_area.transform.Find(currPlayer_hand[i]).gameObject); //Destroy gameobject in hand area with a name that matches i

                currPlayer_hand.RemoveAt(i);
            } else {
                i++;
            }
        }
    }

    public void DiscardLowestSuited(List<string> currPlayer_hand, GameObject currPlayer_hand_area)
    {
        foreach(string card in currPlayer_hand)
        {
            if(!(card.Substring(0,card.Length).Contains("2") || card.Substring(0,card.Length).Contains("5") || card.Substring(0,card.Length).Contains("10") || card.Substring(0,card.Length).Contains("J") || card.Substring(0,card.Length).Contains("A")))
            {
                discard.Add(card);
                break;
            } 
        }
        // Removes each card in discard list from currentPlayer_hand list.
        for (int i = 0; i < currPlayer_hand.Count; )
        {
            if (discard.Contains(currPlayer_hand[i]))
            {
                Destroy(currPlayer_hand_area.transform.Find(currPlayer_hand[i]).gameObject); //Destroy gameobject in hand area with a name that matches i
                currPlayer_hand.RemoveAt(i);
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
    
    public void ShowPlayer1Cards(bool value)
    {
        foreach(Transform card in player1_hand_area.transform)
        {
            if(value)
            {
                if(card.GetComponent<Selectable>() != null)
                {
                    card.GetComponent<Selectable>().faceUp = true;
                }
            }
            else
            {
                if(card.GetComponent<Selectable>() != null)
                {
                    card.GetComponent<Selectable>().faceUp = false;
                }
            }
        }
    }

    public void ShowPlayer2Cards(bool value)
    {
        foreach(Transform card in player2_hand_area.transform)
        {
            if(value)
            {
                if(card.GetComponent<Selectable>() != null)
                {
                    card.GetComponent<Selectable>().faceUp = true;
                }
            }
            else
            {
                if(card.GetComponent<Selectable>() != null)
                {
                    card.GetComponent<Selectable>().faceUp = false;
                }
            }
        }
    }

    public void ShowPlayer3Cards(bool value)
    {
        foreach(Transform card in player3_hand_area.transform)
        {
            if(value)
            {
                if(card.GetComponent<Selectable>() != null)
                {
                    card.GetComponent<Selectable>().faceUp = true;
                }
            }
            else
            {
                if(card.GetComponent<Selectable>() != null)
                {
                    card.GetComponent<Selectable>().faceUp = false;
                }
            }
        }
    }

    public void ShowPlayer4Cards(bool value)
    {
        foreach(Transform card in player4_hand_area.transform)
        {
            if(value)
            {
                if(card.GetComponent<Selectable>() != null)
                {
                    card.GetComponent<Selectable>().faceUp = true;
                }
            }
            else
            {
                if(card.GetComponent<Selectable>() != null)
                {
                    card.GetComponent<Selectable>().faceUp = false;
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

    public void UpdateRotationRelativeToParent(Transform card)
    {
        card.localRotation = Quaternion.Euler(0,0,0);
    }

    public void RedPlayed2(bool value)
    {
        redPlay2 = true;
    }

    public void BluePlayed2(bool value)
    {
        bluePlay2 = true;
    }

    public void CheckForWinner()
    {
        // if red team == 62 points and blue != 62 points
        if(CheckGameOver(redTeamPoints) && (!(CheckGameOver(blueTeamPoints))))
        {
            redWon = true;
            blueWon = false;
        } 
        // if blue team == 62 points and red != 62 points
        else if ((!(CheckGameOver(redTeamPoints)) && (CheckGameOver(blueTeamPoints))))
        {
            blueWon = true;
            redWon = false;
        } 
        //if red team AND blue team == 62 points
        else if(CheckGameOver(redTeamPoints) && (CheckGameOver(blueTeamPoints)))
        {
            if(redTeamHighestBet)
            {
                blueWon = false;
                redWon = true;
            }
            else if (blueTeamHighestBet)
            {
                blueWon = true;
                redWon = false;
            }
        }
        UpdateGameState(GameState.MatchEnd);
    }

    public void Reset()
    {
        minBet = 6;
        betManager.UpdateMinBet(minBet);
        deck = GenerateDeck();
        sortedDeck = GenerateDeck();
        SetFirstPlayerBasedOnDealer(dealer);
        discard.Clear();
        hasSprite.Clear();
        roundCardsPlayed.Clear();
        roundCardsPlayedByBlue.Clear();
        roundCardsPlayedByRed.Clear();
        deckCardsPlayed.Clear();
        

        player1_field.Clear();
        player1_hand.Clear();
        EmptyArea(player1_field_area);
        EmptyArea(player1_hand_area);

        player2_field.Clear();
        player2_hand.Clear();
        EmptyArea(player2_field_area);
        EmptyArea(player2_hand_area);

        player3_field.Clear();
        player3_hand.Clear();
        EmptyArea(player3_field_area);
        EmptyArea(player3_hand_area);

        player4_field.Clear();
        player4_hand.Clear();
        EmptyArea(player4_field_area);
        EmptyArea(player4_hand_area);

        teamBetText.text = "";
        betValueText.text = "";
    }

    public void EmptyArea(GameObject area)
    {
        foreach(Transform card in area.gameObject.transform)
        {
            if(card.gameObject.tag != "deck")
                Destroy(card.gameObject);
            else
                continue;
        }
    }

    public void UpdateCurrentPlayerLists(int current)
    {
        switch(current)
        {
            case 1:
                player1_hand = currentPlayer_hand;
                player1_field = currentPlayer_field;
                break;
            case 2:
                player2_hand = currentPlayer_hand;
                player2_field = currentPlayer_field;
                break;
            case 3:
                player3_hand = currentPlayer_hand;
                player3_field = currentPlayer_field;
                break;
            case 4:
                player4_hand = currentPlayer_hand;
                player4_field = currentPlayer_field;
                break;
        }
    }

    public void PlayedCard(string card)
    {
        roundCardsPlayed.Add(card);
    }

    public void RedPlayedCard(string card)
    {
        roundCardsPlayedByRed.Add(card);
    }

    public void BluePlayedCard(string card)
    {
        roundCardsPlayedByBlue.Add(card);
    }

    public void ResetPlayedCards()
    {
        foreach(string card in roundCardsPlayed)
        {
            deckCardsPlayed.Add(card);
        }
        roundCardsPlayed.Clear();
    }

    public bool CheckGameOver(int points)
    {
        if(points >= 62)
        {
            return true;
        } else {
            return false;
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

    public int GetHighestPlayedCard()
    {
        int max = player1_playedCardRank;

        if (player2_playedCardRank > max)
            max = player2_playedCardRank;
        if (player3_playedCardRank > max)
            max = player3_playedCardRank;
        if (player4_playedCardRank > max)
            max = player4_playedCardRank;
        return max;
    }

    private void ResetPlayedCardRanks()
    {
        player1_playedCardRank = 0;
        player2_playedCardRank = 0;
        player3_playedCardRank = 0;
        player4_playedCardRank = 0;
    }

    public int GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public int GetCardRank(string card)
    {
        // IF 2
        if(card.Substring(0,card.Length).Contains("2"))
        {
            return 0;
        } 
        // IF 3
        else if(card.Substring(0,card.Length).Contains("3"))
        {
            return 1;
        } 
        // IF 4
        else if(card.Substring(0,card.Length).Contains("4"))
        {
            return 2;
        } 
        // IF 5
        else if(card.Substring(0,card.Length).Contains("5"))
        {
            // IF WRONG 5
            if(card.Substring(0,card.Length).Contains(sameColorSuit))
            {
                return 3;
            }
            // IF WRONG 5
            else if(card.Substring(0,card.Length).Contains(selectedSuit))
            {
                return 4;
            }
        } 
        // IF 6
        else if(card.Substring(0,card.Length).Contains("6"))
        {
            return 5;
        } 
        // IF 7
        else if(card.Substring(0,card.Length).Contains("7"))
        {
            return 6;
        } 
        // IF 8
        else if(card.Substring(0,card.Length).Contains("8"))
        {
            return 7;
        } 
        // IF 9
        else if(card.Substring(0,card.Length).Contains("9"))
        {
            return 8;
        } 
        // IF 10
        else if(card.Substring(0,card.Length).Contains("10"))
        {
            return 9;
        } 
        // IF J
        else if(card.Substring(0,card.Length).Contains("J"))
        {
            return 10;
        } 
        // IF Q
        else if(card.Substring(0,card.Length).Contains("Q"))
        {
            return 11;
        } 
        // IF K
        else if(card.Substring(0,card.Length).Contains("K"))
        {
            return 12;
        } 
        // IF A
        else if(card.Substring(0,card.Length).Contains("A"))
        {
            return 13;
        } 
        else 
        {
            throw new Exception("Invalid card rank found");
        }
        return 0;
    }

    public int GetCardPoints(string card)
    {
         // 2, 10, J & A are worth 1 point
        if(card.Substring(0,card.Length).Contains("2") || card.Substring(0,card.Length).Contains("10") || card.Substring(0,card.Length).Contains("J") || card.Substring(0,card.Length).Contains("A"))
        {
            return 1;
        } 
        // 5 is worth 5 points
        else if(card.Substring(0,card.Length).Contains("5"))
        {
            return 5;
        } 
        // Other cards are worthless
        else 
        {
            return 0;
        }
    }


    public bool GetRedBetHighest()
    {
        return redTeamHighestBet;
    }

    public bool GetBlueBetHighest()
    {
        return blueTeamHighestBet;
    }    

    public int DeterminePlayerOrder()
    {
        Debug.Log("firstPlayerIndex: " + firstPlayerIndex);
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

    // public void DetermineNextRoundOrder()
    // {
    //     int highest = GetHighestPlayedCard(); 

    //     if (player1_playedCardValue == highest)
    //     {
    //         layoutManager.Player1Turn();
    //         UpdateGameState(GameState.TurnPlayer1Accept);
    //     } 
    //     else if (player2_playedCardValue == highest)
    //     {
    //         layoutManager.Player2Turn();
    //         UpdateGameState(GameState.TurnPlayer2Accept);
    //     } 
    //     else if (player3_playedCardValue == highest)
    //     {
    //         layoutManager.Player3Turn();
    //         UpdateGameState(GameState.TurnPlayer3Accept);
    //     } 
    //     else if (player4_playedCardValue == highest)
    //     {
    //         layoutManager.Player4Turn();
    //         UpdateGameState(GameState.TurnPlayer4Accept);
    //     } 
    //     else 
    //     {
    //         throw new Exception("Error finding highest bet");
    //     }
    //     turnIndex = 1;
    // }

    public void SetFirstPlayer(int value)
    {
        firstPlayerIndex = value;
    }

    public void SetFirstPlayerBasedOnDealer(int value)
    {
        if(value == 4)
        {
            firstPlayerIndex = 1;
        } else {
            firstPlayerIndex = (value+1);
        }
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

    public void OnPlayAgainClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }


}