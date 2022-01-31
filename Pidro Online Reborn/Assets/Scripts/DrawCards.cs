using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    //public GameObject Card1;
    //public GameObject playerArea;

    public Pidro pidro;
    
    private void Awake()
    {
        pidro = GameObject.Find("Pidro").GetComponent<Pidro>();
    }

    public void OnClick()
    {
        pidro.PlayCards();
        //FirstDraw();
    }


    /*
    private void FirstDraw()
    {
            for(int i = 0; i < 9; i++)
        {
            GameObject card = Instantiate(Card1, PlayerArea.transform, true);
            card.transform.SetParent(PlayerArea.transform, false);
            // card.transform.position = PlayerArea.transform.position;
            // card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -1);
        }
    }

    *

    /*private void SecondDraw()
    {

    }
    */
}
