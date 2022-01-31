using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool faceUp = false;

    private bool isOverdropZone;
    private bool isOverHand;

    [SerializeField] private Vector2 startPosition;

    public GameObject Canvas;
    public GameObject DropZone;
    private GameObject startParent;
    private GameObject dropZone;
    private GameObject hand;
    
    private Vector3 mousePos;
    private Vector3 dragOffset;   // offset for grabbing a card without it being centered to the mousePos, e.g. grabbing a card by a corner
    public float dragSpeed = 30f; // The speed at which cards are dragged

    private Camera cam;


    // Awake is called when the script instance is being loaded
    public void Awake()
    {
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
            transform.SetParent(dropZone.transform, false);
        }
        else if (isOverHand)
        {
            transform.SetParent(hand.transform, false);
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

    #region collisions

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "DropZone")
        {
            isOverdropZone = true;
            dropZone = collider.gameObject;
            dropZone.GetComponent<DropZone>().AddCardToList(gameObject);
        } 
        else if(collider.gameObject.tag == "Hand") 
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