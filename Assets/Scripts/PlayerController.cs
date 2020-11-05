using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField, Range(1f, 10f), Tooltip("The speed the player moves")]
    private float speed = 5f;
    
    private Rigidbody2D _playerRigidbody;

    private float _moveHorizontal;
    private float _moveVertical;

   
    
    

    private void Start()
    {
       
        _playerRigidbody = GetComponent<Rigidbody2D>();
    }

  

    private void FixedUpdate()
    {
        //Allows the player to be controlled
        _moveHorizontal = Input.GetAxis("Horizontal");
        _moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(_moveHorizontal, _moveVertical);
        _playerRigidbody.AddForce(movement * speed);
    }

   

}
