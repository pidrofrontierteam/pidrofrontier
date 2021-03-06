using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Selectable : MonoBehaviour
{
    public bool faceUp = false;

    private bool isOverdropZone;
    private bool isOverHand;

    public bool played = false;

    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Quaternion startRotation;

    public GameObject Canvas;
    public GameObject DropZone;
    private GameObject startParent;
    private GameObject dropZone;
    private GameObject hand;
    
    private Vector3 mousePos;
    private Vector3 dragOffset;   // offset for grabbing a card without it being centered to the mousePos, e.g. grabbing a card by a corner
    public float dragSpeed = 30f; // The speed at which cards are dragged

    private Camera cam;

    private GameManager gameManager;
    private TurnManager turnManager;


    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        gameManager = GameManager.Instance;
        turnManager = TurnManager.Instance;

        cam = Camera.main;
        Canvas = GameObject.Find("Main Canvas");
        DropZone = GameObject.Find("PlayerDropZone");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Manages click and dragging card gameobjects between hand and field, as well as the corresponding lists
    #region Mouse Input

    private void OnMouseDown()
    {
        if(!played)
        {
            dragOffset = transform.position - GetMousePos();
            startParent = transform.parent.gameObject;
            startPosition = transform.position;
            startRotation = transform.rotation;
        }
    }

    public void OnMouseDrag()
    {
        // Check if card belongs to current player and hasn't been played
        if(!played && turnManager.canMove && startParent.transform.transform.name == gameManager.currentPlayer_hand_area.transform.name || startParent.transform.transform.name == gameManager.currentPlayer_field_area.transform.name)
        {
            transform.position = Vector3.MoveTowards(transform.position, GetMousePos() + dragOffset, dragSpeed * Time.deltaTime);
            transform.SetParent(Canvas.transform, true);
        }
    }

    private void OnMouseUp()
    {
        if(!played)
        {
            // Checks whether the cursor is above the drop zone (field) when released, if so it puts the card gameobject into the drop zone and manages the string lists
            if (isOverdropZone)
            {
                // CHECK WHETHER POSSIBLE TO ADD CARDS, I.E., HAVE WE PLAYED A CARD THIS ROUND?
                if(turnManager.playedCards >= gameManager.currentPlayer_field.Count)
                {
                    transform.SetParent(dropZone.transform, false);

                    // Checks whether card is already in the field
                    string result = gameManager.currentPlayer_field.FirstOrDefault(s => s.Contains(transform.gameObject.name));
                    if (result == null)
                    {
                        gameManager.MoveCardBetweenLists(transform.gameObject.name, gameManager.currentPlayer_hand, gameManager.currentPlayer_field);
                    }
                }
                else
                {
                    transform.position = startPosition;
                    transform.SetParent(startParent.transform, false);
                }
            }
            // Checks whether the cursor is above the hand when released, if so it puts the card gameobject into the hand and manages the string lists
            else if (isOverHand)
            {
                transform.SetParent(hand.transform, false);

                // Checks whether card is already in the hand
                string result = gameManager.currentPlayer_hand.FirstOrDefault(s => s.Contains(transform.gameObject.name));
                if (result == null)
                {
                    gameManager.MoveCardBetweenLists(transform.gameObject.name, gameManager.currentPlayer_field, gameManager.currentPlayer_hand);
                }
            } 
            else 
            {
                transform.position = startPosition;
                transform.SetParent(startParent.transform, false);
            }
            transform.rotation = startRotation;

            gameManager.currentPlayer_field = gameManager.SortCardsList(gameManager.currentPlayer_field);
            gameManager.SortCardsGameObjects(gameManager.currentPlayer_field, gameManager.currentPlayer_field_area);

            gameManager.currentPlayer_hand = gameManager.SortCardsList(gameManager.currentPlayer_hand);
            gameManager.SortCardsGameObjects(gameManager.currentPlayer_hand, gameManager.currentPlayer_hand_area);

            gameManager.UpdateCurrentPlayerLists(gameManager.GetCurrentPlayer());
        }
    }

    private Vector3 GetMousePos()
    {
        var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    #endregion


    // Detection for moving the card gameobject between the hand and the drop zone
    #region collisions

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "DropZone" && collider.gameObject.name == gameManager.currentPlayer_field_area.transform.name)
        {
            isOverdropZone = true;
            dropZone = collider.gameObject;
            dropZone.GetComponent<DropZone>().AddCardToList(gameObject);
            
            
        } 
        else if(collider.gameObject.tag == "Hand" && collider.gameObject.name == gameManager.currentPlayer_hand_area.transform.name) 
        {
            isOverHand = true;
            hand = collider.gameObject;
            hand.GetComponent<DropZone>().AddCardToList(gameObject);

        }
        
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "DropZone")
        {
            isOverdropZone = false;
            if(dropZone != null)
            {
                dropZone.GetComponent<DropZone>().RemoveCardFromList(gameObject);
                dropZone = null;
            }
        } 
        else if(collider.gameObject.tag == "Hand") 
        {
            isOverHand = false;
            if(hand != null) 
            {
                hand.GetComponent<DropZone>().RemoveCardFromList(gameObject);
                hand = null;
            }
        }
    }
    #endregion
}