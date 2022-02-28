using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetHandler : MonoBehaviour
{
    [SerializeField] private Button betButton;
    [SerializeField] private Button passButton;
    [SerializeField] private Slider betSlider;

    [SerializeField] private Transform BettingUI;
    [SerializeField] private Transform SuitSelUI;

    public int minBet = 6;
    public int maxBet = 14;

    public string suit;

    [SerializeField] private Text betValue;
    private int sliderValue = 6;

    private Pidro pidro;

    void Awake()
    {
    }

    void Start()
    {
        pidro = Pidro.Instance;
        //minBet = pidro.GetMinBet();
    }

    void Update()
    {
        UpdateSliderValue();
        
    }

    public void OnBetClick()
    {
        pidro.SetBet(sliderValue);
        Debug.Log("Bet " + sliderValue);
        BettingUI.gameObject.SetActive(false);
    }

    public void OnPassClick()
    {
        Debug.Log("Pass button pressed!");
        BettingUI.gameObject.SetActive(false);
    }

    public void OnSpadesClick()
    {
        Debug.Log("Spades button pressed!");
        suit = "Spades";
        SuitSelUI.gameObject.SetActive(false);
    }

    public void OnHeartsClick()
    {
        Debug.Log("Hearts button pressed!");
        suit = "Hearts";
        SuitSelUI.gameObject.SetActive(false);
    }

    public void OnClubsClick()
    {
        Debug.Log("Clubs button pressed!");
        suit = "Clubs";
        SuitSelUI.gameObject.SetActive(false);
    }

    public void OnDiamondsClick()
    {
        Debug.Log("Diamonds button pressed!");
        suit = "Diamonds";
        SuitSelUI.gameObject.SetActive(false);
    }

    public string GetSuit()
    {
        return suit;
    }

    public void EnableSuitSelectionUI()
    {
        SuitSelUI.gameObject.SetActive(true);
    }


    private void UpdateSliderValue()
    {
        betSlider.minValue = minBet;
        sliderValue = (int)betSlider.value;
        betValue.text = sliderValue.ToString();
    }

    public void UpdateMinBet(int value)
    {
        minBet = value;
    }
}
