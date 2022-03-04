using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    [Header("Players' Hand Transforms")]
    public Transform player1_hand_area;
    public Transform player2_hand_area;
    public Transform player3_hand_area;
    public Transform player4_hand_area;
    [Space(20)]

    [Header("Players' Field Transforms")]
    public Transform player1_field_area;
    public Transform player2_field_area;
    public Transform player3_field_area;
    public Transform player4_field_area;
    [Space(20)]

    [Header("Misc Transforms")]
    public Transform deck;
    


    // Hand Positions
    private Vector3 north_hand_position;
    private Vector3 east_hand_position;
    private Vector3 south_hand_position;
    private Vector3 west_hand_position;

    // Hand Rotations
    private Quaternion north_hand_rotation;
    private Quaternion east_hand_rotation;
    private Quaternion south_hand_rotation;
    private Quaternion west_hand_rotation;

    // Field Positions
    private Vector3 north_field_position;
    private Vector3 east_field_position;
    private Vector3 south_field_position;
    private Vector3 west_field_position;

    // Field Rotations
    private Quaternion north_field_rotation;
    private Quaternion east_field_rotation;
    private Quaternion south_field_rotation;
    private Quaternion west_field_rotation;


    private static LayoutManager _instance;

    public static LayoutManager Instance { get { return _instance; } }


    void Awake()
    {

        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this; 
        }

        // Getting cardinal hand positions
        north_hand_position = player3_hand_area.position;
        east_hand_position = player4_hand_area.position;
        south_hand_position = player1_hand_area.position;
        west_hand_position = player2_hand_area.position;

        // Getting cardinal hand rotations
        north_hand_rotation = player3_hand_area.rotation;
        east_hand_rotation = player4_hand_area.rotation;
        south_hand_rotation = player1_hand_area.rotation;
        west_hand_rotation = player2_hand_area.rotation;

        // Getting cardinal hand positions
        north_field_position = player3_field_area.position;
        east_field_position = player4_field_area.position;
        south_field_position = player1_field_area.position;
        west_field_position = player2_field_area.position;

        // Getting cardinal field positions
        north_field_rotation = player3_field_area.rotation;
        east_field_rotation = player4_field_area.rotation;
        south_field_rotation = player1_field_area.rotation;
        west_field_rotation = player2_field_area.rotation;
    


    }

    void Start()
    {
        Player1Turn();
    }

    public void Update ()
    {
    }

    public void Player1Turn()
    {
        // Hand Positions
        player3_hand_area.position = north_hand_position;
        player4_hand_area.position = east_hand_position;
        player1_hand_area.position = south_hand_position; // CURRENT PLAYER
        player2_hand_area.position = west_hand_position;

        // Hand Rotations
        player3_hand_area.rotation = north_hand_rotation;
        player4_hand_area.rotation = east_hand_rotation;
        player1_hand_area.rotation = south_hand_rotation; // CURRENT PLAYER
        player2_hand_area.rotation = west_hand_rotation;

        // Field Positions
        player3_field_area.position = north_field_position;
        player4_field_area.position = east_field_position;
        player1_field_area.position = south_field_position; // CURRENT PLAYER
        player2_field_area.position = west_field_position;

        // Field Rotations
        player3_field_area.rotation = north_field_rotation;
        player4_field_area.rotation = east_field_rotation;
        player1_field_area.rotation = south_field_rotation; // CURRENT PLAYER
        player2_field_area.rotation = west_field_rotation;
    }

    public void Player2Turn()
    {
        // Hand Positions
        player4_hand_area.position = north_hand_position;
        player1_hand_area.position = east_hand_position;
        player2_hand_area.position = south_hand_position; // CURRENT PLAYER
        player3_hand_area.position = west_hand_position;

        // Hand Rotations
        player4_hand_area.rotation = north_hand_rotation;
        player1_hand_area.rotation = east_hand_rotation;
        player2_hand_area.rotation = south_hand_rotation; // CURRENT PLAYER
        player3_hand_area.rotation = west_hand_rotation;

        // Field Positions
        player4_field_area.position = north_field_position;
        player1_field_area.position = east_field_position;
        player2_field_area.position = south_field_position; // CURRENT PLAYER
        player3_field_area.position = west_field_position;

        // Field Rotations
        player4_field_area.rotation = north_field_rotation;
        player1_field_area.rotation = east_field_rotation;
        player2_field_area.rotation = south_field_rotation; // CURRENT PLAYER
        player3_field_area.rotation = west_field_rotation;
    }

    public void Player3Turn()
    {
        // Hand Positions
        player1_hand_area.position = north_hand_position;
        player2_hand_area.position = east_hand_position;
        player3_hand_area.position = south_hand_position; // CURRENT PLAYER
        player4_hand_area.position = west_hand_position;

        // Hand Rotations
        player1_hand_area.rotation = north_hand_rotation;
        player2_hand_area.rotation = east_hand_rotation;
        player3_hand_area.rotation = south_hand_rotation; // CURRENT PLAYER
        player4_hand_area.rotation = west_hand_rotation;

        // Field Positions
        player1_field_area.position = north_field_position;
        player2_field_area.position = east_field_position;
        player3_field_area.position = south_field_position; // CURRENT PLAYER
        player4_field_area.position = west_field_position;

        // Field Rotations
        player1_field_area.rotation = north_field_rotation;
        player2_field_area.rotation = east_field_rotation;
        player3_field_area.rotation = south_field_rotation; // CURRENT PLAYER
        player4_field_area.rotation = west_field_rotation;
    }
    
    public void Player4Turn()
    {
        // Hand Positions
        player2_hand_area.position = north_hand_position;
        player3_hand_area.position = east_hand_position;
        player4_hand_area.position = south_hand_position; // CURRENT PLAYER
        player1_hand_area.position = west_hand_position;

        // Hand Rotations
        player2_hand_area.rotation = north_hand_rotation;
        player3_hand_area.rotation = east_hand_rotation;
        player4_hand_area.rotation = south_hand_rotation; // CURRENT PLAYER
        player1_hand_area.rotation = west_hand_rotation;

        // Field Positions
        player2_field_area.position = north_field_position;
        player3_field_area.position = east_field_position;
        player4_field_area.position = south_field_position; // CURRENT PLAYER
        player1_field_area.position = west_field_position;

        // Field Rotations
        player2_field_area.rotation = north_field_rotation;
        player3_field_area.rotation = east_field_rotation;
        player4_field_area.rotation = south_field_rotation; // CURRENT PLAYER
        player1_field_area.rotation = west_field_rotation;
    }
}
