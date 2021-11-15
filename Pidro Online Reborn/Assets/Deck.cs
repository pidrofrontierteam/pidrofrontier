using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public GameObject[] cards;

    void Awake()
    {
        cards = new GameObject[52];
    }

}
