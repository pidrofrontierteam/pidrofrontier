using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public GameManager gameManager;
    
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnClick()
    {
        // gameManager.PlayCards();
        Debug.Log("DEBUG: DRAW CARDS");
    }
}