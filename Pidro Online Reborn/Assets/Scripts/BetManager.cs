using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetManager : MonoBehaviour
{
    [SerializeField] private Button betButton;
    [SerializeField] private Button passButton;
    [SerializeField] private Slider betSlider;

    [SerializeField] private Transform bettingUI;
    [SerializeField] private Transform suitSelUI;

    public int minBet = 6;
    public int maxBet = 14;

    public string suit;

    private int currentPlayer;

    public bool isBetting = false;

    [SerializeField] private Text betValue;
    [SerializeField] private Text playerNumberBet;
    [SerializeField] private Text playerNumberSuit;
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
        //minBet = pidro.GetMinBet();
    }

    void Update()
    {
        UpdateSlider();
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
        // betSlider.minValue = minBet;
        sliderValue = (int)betSlider.value;
        betValue.text = sliderValue.ToString();
    }

    private void UpdateSliderMin()
    {
        if(minBet != 14)
            betSlider.minValue = minBet + 1;
        else
            betSlider.minValue = minBet;
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
