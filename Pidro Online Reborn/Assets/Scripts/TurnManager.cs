using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private Text playerNumberText;
    [SerializeField] private Text playerPassText;
    [SerializeField] private Button beginButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private Transform turnAcceptUI;
    [SerializeField] private Transform turnCardMoveUI;
    [SerializeField] private Transform turnPassUI;


    private int currentPlayer;
    public int playedCards;

    public bool hasAccepted;

    public bool canMove = false;

    private GameManager gameManager;

    private static TurnManager _instance;

    public static TurnManager Instance { get { return _instance; } }


    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this; 
        }
    }

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasAccepted && playedCards < gameManager.currentPlayer_field.Count)
        {
            EnableCardMoveUI();
        } else {
            DisableCardMoveUI();
        }
    }

    public void SetHasAccepted(bool value)
    {
        hasAccepted = value;
    }

    public void OnBeginClick()
    {
        canMove = true;
        Debug.Log("Player clicked begin button");
        playedCards = gameManager.currentPlayer_field.Count;
        DisableTurnAcceptUI();
        SetHasAccepted(true);
        switch(currentPlayer)
        {
            case 1:
            {
                gameManager.UpdateGameState(GameState.TurnPlayer1Start);
                break;
            }
            case 2:
            {
                gameManager.UpdateGameState(GameState.TurnPlayer2Start);
                break;
            }
            case 3:
            {
                gameManager.UpdateGameState(GameState.TurnPlayer3Start);
                break;
            }
            case 4:
            {
                gameManager.UpdateGameState(GameState.TurnPlayer4Start);
                break;
            }
        }
    }

    public void OnConfirmClick()
    {
        DisableTurnAcceptUI();
        canMove = false;
        playedCards++;

        foreach(string card in gameManager.currentPlayer_field)
        {
            GameObject.Find(card).GetComponent<Selectable>().played = true;
            if(!(gameManager.deckCardsPlayed.Contains(card)))
            {

                gameManager.PlayedCard(card);

                if(currentPlayer == 1 || currentPlayer == 3)
                {
                    gameManager.RedPlayedCard(card);
                } else if (currentPlayer == 2 || currentPlayer == 4)
                {
                    gameManager.BluePlayedCard(card);
                }


                switch(currentPlayer)
                {
                    case 1:
                        if(gameManager.GetCardRank(card) == 0)
                        {
                            gameManager.redTeamPoints++;
                            gameManager.RedPlayed2(true);
                        }
                        gameManager.player1_playedCardRank = gameManager.GetCardRank(card);
                        break;
                    case 2:
                        if(gameManager.GetCardRank(card) == 0)
                        {
                            gameManager.blueTeamPoints++;
                            gameManager.BluePlayed2(true);
                        }
                        gameManager.player2_playedCardRank = gameManager.GetCardRank(card);
                        break;
                    case 3:
                        if(gameManager.GetCardRank(card) == 0)
                        {
                            gameManager.redTeamPoints++;
                            gameManager.RedPlayed2(true);
                        }
                        gameManager.player3_playedCardRank = gameManager.GetCardRank(card);
                        break;
                    case 4:
                        if(gameManager.GetCardRank(card) == 0)
                        {
                            gameManager.blueTeamPoints++;
                            gameManager.BluePlayed2(true);
                        }
                        gameManager.player4_playedCardRank = gameManager.GetCardRank(card);
                        break;
                }
            }
        }


        switch(currentPlayer)
        {
            case 1:
            {
                gameManager.UpdateGameState(GameState.TurnPlayer1End);
                break;
            }
            case 2:
            {
                gameManager.UpdateGameState(GameState.TurnPlayer2End);
                break;
            }
            case 3:
            {
                gameManager.UpdateGameState(GameState.TurnPlayer3End);
                break;
            }
            case 4:
            {
                gameManager.UpdateGameState(GameState.TurnPlayer4End);
                break;
            }
        }
    }

    public void EnableTurnAcceptUI()
    {
        canMove = false;
        currentPlayer = gameManager.GetCurrentPlayer();
        playerNumberText.text = "Player " + currentPlayer + "'s turn!";
        turnAcceptUI.gameObject.SetActive(true);
    }

    public void DisableTurnAcceptUI()
    {
        turnAcceptUI.gameObject.SetActive(false);
    }

    public void EnableCardMoveUI()
    {
        turnCardMoveUI.gameObject.SetActive(true);
    }

    public void DisableCardMoveUI()
    {
        turnCardMoveUI.gameObject.SetActive(false);
    }

    public void EnableTurnPassUI()
    {
        canMove = false;
        currentPlayer = gameManager.GetCurrentPlayer();
        playerPassText.text = "Player " + currentPlayer + " ain't got no \ncards and has to pass!";
        turnPassUI.gameObject.SetActive(true);
    }

    public void DisableTurnPassUI()
    {
        canMove = false;
        turnPassUI.gameObject.SetActive(false);
    }
}