using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suit 
{
    Hearts = 1,
    Clubs = 2,
    Diamonds = 3,
    Spades = 4,
}

public enum Rank 
{
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14,
}

public class Card : MonoBehaviour
{
    [SerializeField]
    private Rank rank;
    [SerializeField]
    private int rankAsInt;
    [SerializeField]
    private int spriteIndex;
    [SerializeField]
    private int points;
    [SerializeField]
    private bool isPidro = false;
    [SerializeField]
    private bool isDeuce = false;
    [SerializeField]
    private Suit suit;
    [SerializeField]
    private Suit playedSuit;
    [SerializeField]
    private Sprite[] spriteArray;
    private bool isDragged;
    private Vector3 mousePos;
    private Vector3 dragOffset;
    private float dragSpeed = 30f;

    private SpriteRenderer spriteRenderer;
    private Camera cam;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    public void Start()
    {
        
        spriteArray = Resources.LoadAll<Sprite>("English_pattern_playing_cards_deck");
        switch (rank)
        {
            case Rank.Two:
                rankAsInt = 2;
                isDeuce = true;
                points = 1;
                spriteIndex = 1;
                break;
            case Rank.Three:
                rankAsInt = 3;
                 spriteIndex = 2;
                break;
            case Rank.Four:
                rankAsInt = 4;
                 spriteIndex = 3;
                break;
            case Rank.Five:
                rankAsInt = 5;
                isPidro = true;
                points = 5;
                spriteIndex = 4;
                break;
            case Rank.Six:
                rankAsInt = 7;
                spriteIndex = 5;
                break;
            case Rank.Seven:
                rankAsInt = 8;
                spriteIndex = 6;
                break;
            case Rank.Eight:
                rankAsInt = 9;
                spriteIndex = 7;
                break;
            case Rank.Nine:
                rankAsInt = 10;
                spriteIndex = 8;
                break;
            case Rank.Ten:
                rankAsInt = 11;
                spriteIndex = 9;
                points = 1;
                break;
            case Rank.Jack:
                rankAsInt = 12;
                spriteIndex = 10;
                points = 1;
                break;
            case Rank.Queen:
                rankAsInt = 13;
                spriteIndex = 11;
                break;
            case Rank.King:
                rankAsInt = 14;
                spriteIndex = 12;
                break;
            case Rank.Ace:
                rankAsInt = 15;
                points = 1;
                spriteIndex = 0;
                break;
        }

        if((rankAsInt == 5) && (suit != null))
        {
            if (suit == playedSuit)
                rankAsInt = 6;
        }

        switch (suit)
        {
            case Suit.Spades:
                spriteRenderer.sprite = spriteArray[spriteIndex];
                break;
            case Suit.Hearts:
                spriteIndex += 13;
                spriteRenderer.sprite = spriteArray[spriteIndex];
                break;
            case Suit.Diamonds:
                spriteIndex += 26;
                spriteRenderer.sprite = spriteArray[spriteIndex];
                break;
            case Suit.Clubs:
                spriteIndex += 39;
                spriteRenderer.sprite = spriteArray[spriteIndex];
                break;
        }
    }

    void Update()
    {
        // if(isDragged == true)
        // {
        //     transform.position =  new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //     print("dragged");
        // }
    }

    public int GetRank()
    {
        return rankAsInt;
    }

    public bool getDeuce()
    {
        return isDeuce;
    }

    public bool getPidro()
    {
        return isPidro;
    }

    private void OnMouseDown()
    {
        dragOffset = transform.position - GetMousePos();
    }

    public void OnMouseDrag()
    {
        transform.position = Vector3.MoveTowards(transform.position, GetMousePos() + dragOffset, dragSpeed * Time.deltaTime);
    }

    private Vector3 GetMousePos()
    {
        var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
}