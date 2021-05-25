using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int strength = 20;
    public float Speed;
    public Rigidbody2D rigidBody {get; private set;} 
    // public GameObject goBall {get; private set;} 

    void Start(){
        rigidBody = gameObject.GetComponent<Rigidbody2D>(); 
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log("collider.tag" + collider.tag);
        // Collider2D ballCollider = GetComponent<Collider2D>();
        if(collider.tag == "Wall_Bottom"){
            Debug.Log("GAME OVER ! " + name);
            if(name == "Ball"){
                GameState.ballState = GameStates.AIMING;
                GameManager.NewRound();
            }else{
                GameState.fakeBallState = GameStates.AIMING;
            }
            BackToStart();
        }
    }

    public void BackToStart(){
            GetComponent<Rigidbody2D>().Sleep();
            transform.position = new Vector2(0,-4);
    }

}
