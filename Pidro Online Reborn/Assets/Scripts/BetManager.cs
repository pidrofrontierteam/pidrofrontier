using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetManager : MonoBehaviour
{
    [Header("Bet Accept")]
    [SerializeField] private Transform acceptBetUI;
    [SerializeField] private Button beginBetButton;
    [SerializeField] private Text playerNumberBetAccept;
    

    [Header("Betting")]
    [SerializeField] private Transform bettingUI;
    [SerializeField] private Button betButton;
    [SerializeField] private Button passButton;
    [SerializeField] private Slider betSlider;
    [SerializeField] private Text betValue;
    [SerializeField] private Text playerNumberBet;
    public int minBet = 6;
    public int maxBet = 14;
    public bool isBetting = false;
    
    [Header("Suit Selection Accept")]
    [SerializeField] private Transform acceptSuitSelUI;
    [SerializeField] private Button beginSuitSelButton;
    [SerializeField] private Text playerNumberSuitSelAccept;

    [Header("Suit Selection")]
    [SerializeField] private Transform suitSelUI;
    [SerializeField] private Text playerNumberSuit;
    public string suit;
    


    private int currentPlayer;

    
    
    
    
    private int sliderValue = 6;

    private GameManager gameManager;

    private static BetManager _instance;

    public static BetManager Instance { get { return _instance; } }


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

    void Update()
    {
        UpdateSlider();
    }
    public void EnableBetAcceptUI()
    {
        currentPlayer = gameManager.GetCurrentPlayer();
        playerNumberBetAccept.text = "Player " + currentPlayer + "'s turn to bet!";
        acceptBetUI.gameObject.SetActive(true);
    }

    public void DisableBetAcceptUI()
    {
        acceptBetUI.gameObject.SetActive(false);
    }
    public void EnableSuitSelectionAcceptUI()
    {
        currentPlayer = gameManager.GetCurrentPlayer();
        playerNumberSuitSelAccept.text = "Player " + currentPlayer + " won the bet!";
        acceptSuitSelUI.gameObject.SetActive(true);
    }

    public void DisableSuitSelectionAcceptUI()
    {
        acceptSuitSelUI.gameObject.SetActive(false);
    }

    public void OnBeginSuitSelClick()
    {
        switch(currentPlayer)
        {
            case 1:
            {
                gameManager.ShowPlayer1Cards(true);
                break;
            }
            case 2:
            {
                gameManager.ShowPlayer2Cards(true);
                break;
            }
            case 3:
            {
                gameManager.ShowPlayer3Cards(true);
                break;
            }
            case 4:
            {
                gameManager.ShowPlayer4Cards(true);
                break;
            }
        }
        DisableSuitSelectionAcceptUI();
        gameManager.UpdateGameState(GameState.ChooseSuit);
    }

    public void OnBeginBetClick()
    {
        switch(currentPlayer)
        {
            case 1:
            {
                gameManager.UpdateGameState(GameState.BetPlayer1Start);
                break;
            }
            case 2:
            {
                gameManager.UpdateGameState(GameState.BetPlayer2Start);
                break;
            }
            case 3:
            {
                gameManager.UpdateGameState(GameState.BetPlayer3Start);
                break;
            }
            case 4:
            {
                gameManager.UpdateGameState(GameState.BetPlayer4Start);
                break;
            }
        }
        DisableBetAcceptUI();
    }


    public void OnBetClick()
    {
        if(sliderValue == 14)
        {
            gameManager.SetHighest14(currentPlayer);
        }
        gameManager.SetBet(sliderValue);
        UpdateMinBet(sliderValue);
        UpdateSliderMin();

        Debug.Log("Player " + currentPlayer.ToString() + " bet " + sliderValue, playerNumberBet);
        EndBetTurn();
    }

    public void OnPassClick()
    {   
        gameManager.SetBet(0);
        Debug.Log("Player " + currentPlayer.ToString() + " passed!");
        EndBetTurn();
    }

    public void OnSpadesClick()
    {
        Debug.Log("Spades button pressed!");
        suit = "Spades";
        EndSuitSelection();
    }

    public void OnHeartsClick()
    {
        Debug.Log("Hearts button pressed!");
        suit = "Hearts";
        EndSuitSelection();
    }

    public void OnClubsClick()
    {
        Debug.Log("Clubs button pressed!");
        suit = "Clubs";
        EndSuitSelection();
    }

    public void OnDiamondsClick()
    {
        Debug.Log("Diamonds button pressed!");
        suit = "Diamonds";
        EndSuitSelection();
    }

    public string GetSuit()
    {
        return suit;
    }

    public void EnableBettingUI()
    {
        bettingUI.gameObject.SetActive(true);
        isBetting = true;
        currentPlayer = gameManager.GetCurrentPlayer();
        playerNumberBet.text = "Player " + currentPlayer.ToString();
    }

    public void DisableBettingUI()
    {
        bettingUI.gameObject.SetActive(false);
        isBetting = false;
    }


    public void EnableSuitSelectionUI()
    {
        suitSelUI.gameObject.SetActive(true);
        currentPlayer = gameManager.GetCurrentPlayer();
        playerNumberSuit.text = "Player " + currentPlayer.ToString();
    }

    public void DisableSuitSelectionUI()
    {
        suitSelUI.gameObject.SetActive(false);
    }


    private void UpdateSlider()
    {
        sliderValue = (int)betSlider.value;
        betValue.text = sliderValue.ToString();
    }

    public void UpdateSliderMin()
    {
        if(minBet != 14)
            betSlider.minValue = minBet + 1;
        else
            betSlider.minValue = minBet;
    }

    public void ResetMinBet()
    {
         betSlider.minValue = betSlider.value = sliderValue = minBet = 6;
        //  UpdateSlider();
    }

    public void UpdateMinBet(int value)
    {
        minBet = value;
    }

    public void EndBetTurn()
    {
        DisableBettingUI();
        switch (gameManager.GetCurrentPlayer())
        {
            case 1:
                gameManager.UpdateGameState(GameState.BetPlayer1End);
                break;
            case 2:
                gameManager.UpdateGameState(GameState.BetPlayer2End);
                break;
            case 3:
                gameManager.UpdateGameState(GameState.BetPlayer3End);
                break;
            case 4:
                gameManager.UpdateGameState(GameState.BetPlayer4End);
                break;
            default:
                break;
        }
    }
    public void EndSuitSelection()
    {
        DisableSuitSelectionUI();
        gameManager.SetSuit(suit);
        gameManager.SetFirstPlayer(currentPlayer);
        gameManager.UpdateGameState(GameState.FirstDiscard);
    }
}
