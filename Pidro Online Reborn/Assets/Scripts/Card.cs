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

    //--- Public variables


    //--- Private variables     -   for reference, [SerializeField] exposes private variables in the inspector, so they don't need to be public
    [SerializeField] private Rank rank;

    [SerializeField] private Suit suit;
    [SerializeField] private Suit playedSuit;

    [SerializeField] private Sprite[] spriteArray;

    [SerializeField] private int rankAsInt;         // Used to compare cards' values amongst one another
    [SerializeField] private int spriteIndex;       // Used to assign a sprite for each card
    [SerializeField] private int points;            // Two, Five, Ten, Jack, Queen, King, Ace have points

    [SerializeField] private float dragSpeed = 30f; // The speed at which cards are dragged

    [SerializeField] private bool isPidro = false;  // Fives
    [SerializeField] private bool isDeuce = false;  // Twos
    private bool isOverdropZone;

    [SerializeField] private Vector2 startPosition;

    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject DropZone;
    private GameObject startParent;
    private GameObject dropZone;
    

    private Vector3 mousePos;
    private Vector3 dragOffset;                     // offset for grabbing a card without it being centered to the mousePos, e.g. grabbing a card by a corner

    //--- Component references
    private SpriteRenderer spriteRenderer;
    private Camera cam;
    


    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        Canvas = GameObject.Find("Main Canvas");
        DropZone = GameObject.Find("PlayerDropZone");
    }

    // Start is called before the first frame update
    public void Start()
    {
        spriteArray = Resources.LoadAll<Sprite>("English_pattern_playing_cards_deck");

        // Sets variables based on the card's rank.
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
                rankAsInt = 5; // "Wrong Pidro" is ranked as 5, while "Right Pidro" is ranked as 6 which is why...
                isPidro = true;
                points = 5;
                spriteIndex = 4;
                break;
            case Rank.Six:
                rankAsInt = 7; // ...the rankAsInt here is 7
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

        // Checks whether a Five card is "Right Pidro" or "Wrong Pidro"
        if((rankAsInt == 5) && (suit != null))
        {
            if (suit == playedSuit)
                rankAsInt = 6;
        }

        // Assigns a sprite based on the card's Suit, and the spriteIndex assigned in the Rank switch statement
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

    #region Mouse Input

    private void OnMouseDown()
    {
        dragOffset = transform.position - GetMousePos();
        startParent = transform.parent.gameObject;
        startPosition = transform.position;

    }

    public void OnMouseDrag()
    {
        transform.position = Vector3.MoveTowards(transform.position, GetMousePos() + dragOffset, dragSpeed * Time.deltaTime);
        transform.SetParent(Canvas.transform, true);
    }

    private void OnMouseUp()
    {
        if (isOverdropZone)
        {
            transform.SetParent(DropZone.transform, false);
        }
        else 
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }

    private Vector3 GetMousePos()
    {
        var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }


    #endregion

    #region Getter methods
    
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

    #endregion
}