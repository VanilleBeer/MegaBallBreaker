using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public int HP;
    public Text text_HP;
    public GameObject ExplostionEffekt;
    public Vector3 movementVector = new Vector3(0,-0.5f,0);
    void Update()
    {
        if(text_HP == null){
            return;
        }
        text_HP.text = HP.ToString();
        if(HP <= 0){
            Debug.Log("Destroy");
            GameObject.Destroy(gameObject);
            return; 
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log("collider.tag: " + collider.tag);
        if(collider.gameObject.tag == "Ball" && collider.gameObject.name != "FakeBall"){
            Ball ball = collider.gameObject.GetComponent<Ball>();
            HP-=ball.strength;
            Debug.Log("New HP:" + HP);
            
            var effekt = Instantiate(ExplostionEffekt, transform.position, Quaternion.identity);
            Destroy(effekt, 0.8f);

        }
    }

    private void OnDestroy() {
        if(name.Contains("_fake")){
            return; 
        }

        var effekt = Instantiate(ExplostionEffekt, transform.position, Quaternion.identity);
        Destroy(effekt, 1);

        // Destroy Block from fakeScene
        foreach(var o in GameState.scene1.GetRootGameObjects()){
            Debug.Log(o.name + " ==? " + name+"_clone");
            if(o.name == (name+"_fake")){
                Destroy(o);
                return;
            }
        }
    }
}


