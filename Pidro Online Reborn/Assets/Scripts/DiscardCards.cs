using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardCards : MonoBehaviour
{
    public GameManager gameManager;
    
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnClick()
    {
        // gameManager.DiscardCards();
        Debug.Log("DEBUG: DISCARD CARDS");
    }
}