using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetHandler : MonoBehaviour
{
    [SerializeField] private Button betButton;
    [SerializeField] private Button passButton;
    [SerializeField] private Slider betSlider;

    public int minBet = 6;
    public int maxBet = 14;

    [SerializeField] private Text betValue;
    private int sliderValue = 6;

    public Pidro pidro;

    void Awake()
    {
    }

    void Start()
    {
        pidro = Pidro.Instance;
        minBet = pidro.GetMinBet();
    }

    void Update()
    {
        UpdateSliderValue();
    }

    public void OnBetClick()
    {
        pidro.SetBet(sliderValue);
        Debug.Log("Bet " + sliderValue);
    }

    public void OnPassClick()
    {
        Debug.Log("Pass button pressed!");
    }

    public void UpdateSliderValue()
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
