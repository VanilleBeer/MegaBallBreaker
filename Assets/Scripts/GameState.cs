using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
public enum GameStates {
    AIMING, SHOOTING
}

public static class GameState 
{
    public static GameStates ballState = GameStates.AIMING;  
    public static GameStates fakeBallState = GameStates.AIMING;  
    public static Scene scene1;
    public static PhysicsScene2D scene1Physics;

}
