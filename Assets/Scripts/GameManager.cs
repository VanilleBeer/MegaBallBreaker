using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    // public GameObject goBall; 
    private Vector3 lastBallPos; 
    // private static Rigidbody2D rbBall; 
    // static Ball ball;
    public Ball originalBall;
    static Dictionary<int, Ball> balls = new Dictionary<int, Ball>();  
    public int ballCount = 1; 
        // PREDICT
    private GameObject goFakeBall; 
Rigidbody2D rbFakeBall;
    Ball fakeBall;
    // Start is called before the first frame update
    // public GameObject marker; 
    // private List<GameObject> markers = new List<GameObject>();
    LineRenderer lineRenderer; 
private Vector3 predictMousePos;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        Physics2D.simulationMode = SimulationMode2D.Update;
 
        // PREDICT
        //the LocalPhysicsMode is what create a new PhysicsScene separate from the default
        CreateSceneParameters csp = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
        GameState.scene1 = SceneManager.CreateScene("MyScene1", csp);
        GameState.scene1Physics = GameState.scene1.GetPhysicsScene2D();
        Physics.autoSimulation = false;

        balls.Add(0,originalBall);
        for(int i=1; i<ballCount; i++){
            GameObject newBall = Instantiate(originalBall.gameObject, new Vector2(0,-4), Quaternion.identity);
            balls.Add(i, newBall.GetComponent<Ball>());
            newBall.SetActive(false);
        }
 
        goFakeBall = Instantiate(originalBall.gameObject, new Vector2(0,-4), Quaternion.identity);
        Debug.Log("goFakeBall.tag: " + goFakeBall.tag);
        goFakeBall.name = "FakeBall";
        Debug.Log("goFakeBall.tag: " + goFakeBall.tag);
        fakeBall = goFakeBall.GetComponent<Ball>();
        fakeBall.strength = 0;
        fakeBall.Speed = 20;
        fakeBall.GetComponent<Renderer>().enabled=false;
        rbFakeBall = goFakeBall.GetComponent<Rigidbody2D>();
        SpriteRenderer srFakeBall = goFakeBall.GetComponent<SpriteRenderer>();
        srFakeBall.color = Color.gray;

        SceneManager.MoveGameObjectToScene(goFakeBall, GameState.scene1);
        copyAllToFakeScene("Wall");
        copyAllToFakeScene("Wall_Bottom");
        copyAllToFakeScene("Block");
    }

    void Update()
    {
        Simulate();

        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Aiming(Camera.main.ScreenToWorldPoint(touch.position));
                    break;
                case TouchPhase.Moved:
                    Aiming(Camera.main.ScreenToWorldPoint(touch.position));
                    break;
                case TouchPhase.Ended:
                    Shoot(Camera.main.ScreenToWorldPoint(touch.position));
                    break;
            }
        }else{
            if(Input.GetMouseButton(0)){
                Aiming(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            if(Input.GetMouseButtonUp(0) && GameState.ballState != GameStates.SHOOTING){
                Shoot(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }

    private void Shoot(Vector3 aimingPos)
    {
        GameState.ballState = GameStates.SHOOTING; 
        Vector2 direction = aimingPos - balls[0].rigidBody.transform.position;

        foreach(var ball in balls.Values){
            Debug.Log("Force (Speed): " + direction + "("+ ball.Speed + ")");
            ball.gameObject.SetActive(true);
            ball.rigidBody.AddForce((direction.normalized)*(ball.Speed), ForceMode2D.Impulse);
            StartCoroutine(new WaitForSecondsRealtime(1000));
        }
    }

    private void Aiming(Vector3 aimingPoint)
    {
        Debug.Log("Set GameState.fakeBallState Shooting");
        GameState.fakeBallState = GameStates.SHOOTING; 
        fakeBall.BackToStart();
        predictMousePos = aimingPoint;
        Vector2 direction = aimingPoint - rbFakeBall.transform.position;
        Debug.Log("Force (Speed): " + direction + "("+ fakeBall.Speed + ")");
        rbFakeBall.AddForce((direction.normalized)*(fakeBall.Speed), ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {

    }
    public static void NewRound(){
        var blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            var blockObject = block.GetComponent<Block>();
            // block.transform.position = blockObject.movementVector;
            block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y -1 , block.transform.position.z);
        }
        foreach(var keyVal in balls){
            if(keyVal.Key == 0) continue;
            keyVal.Value.gameObject.SetActive(false);
        }
    } 

    private void Simulate()
    {
        lineRenderer.positionCount = 50;
        for(int i =0; i<50; i++){

            GameState.scene1Physics.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, fakeBall.transform.position);

            // markers.Add(Instantiate(marker, goFakeBall.transform.position, Quaternion.identity));
            lastBallPos = goFakeBall.transform.position;
        }
    }
    private void simulate_old(){
        /*
        timer1 += Time.deltaTime;
        // Debug.Log("timer1: " + timer1 + "  -  Time.deltaTime: " + Time.deltaTime);
        if (GameState.scene1Physics != null && GameState.scene1Physics.IsValid()) {
            while (timer1 >= Time.fixedDeltaTime) {
                timer1 -= Time.fixedDeltaTime;
 
                //commenting out this line will stop the physics in scene1
                GameState.scene1Physics.Simulate(Time.fixedDeltaTime);
            }
        }    
        */
}
    private void copyAllToFakeScene(string tag){
        var objectsToCopy = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject o in objectsToCopy)
        {
            var newObject = Instantiate(o, o.transform.position, Quaternion.identity);
            newObject.name = o.name + "_fake";
            SceneManager.MoveGameObjectToScene(newObject, GameState.scene1);
            newObject.GetComponent<Renderer>().enabled=false;
            destroyChildren(newObject);
        }    
    }
    private void destroyChildren(GameObject parent){
        foreach (Transform child in parent.transform)
        {
            if(child.gameObject.tag == "InnerBlock"){
                Renderer rend = child.gameObject.GetComponent<Renderer>();
                if(rend != null){
                    rend.enabled=false;
                }
                destroyChildren(child.gameObject);
            }else{
                Destroy(child.gameObject,0);
            }
        }    
    }   
    private void hideChildren(GameObject parent){
        foreach (Transform child in parent.transform)
        {
            Debug.Log("Hide child " + child.gameObject.name + "(" + child.gameObject.GetType() + ")");
            if(child.gameObject.tag != "InnerBlock"){
                Renderer rend = child.gameObject.GetComponent<Renderer>();
                if(rend != null){
                    rend.enabled=false;
                }
                if(gameObject.GetComponent<RectTransform>() != null )
                {
                    // This is a UI element since it has a RectTransform component on it
                    Debug.Log("Destroy Canvas");
                    Destroy(child);
                }
                // if(child.gameObject.GetType() == typeof(Canvas)){
                //     Debug.Log("Destroy Canvas");
                //     Destroy(child);
                // }
            }
            hideChildren(child.gameObject);
        }    
    }
}
