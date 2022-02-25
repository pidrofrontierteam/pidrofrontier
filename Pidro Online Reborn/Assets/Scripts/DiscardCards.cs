using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardCards : MonoBehaviour
{
    public Pidro pidro;
    
    private void Awake()
    {
        pidro = GameObject.Find("GameManager").GetComponent<Pidro>();
    }

    public void OnClick()
    {
        pidro.DiscardCards();
    }
}