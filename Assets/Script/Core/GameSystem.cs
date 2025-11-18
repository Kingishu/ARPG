using UnityEngine;

public class GameSystem
{
    private static GameSystem instance=new GameSystem();
    public static GameSystem Instance => instance;
    
    public SceneController SceneController;
    public UCameraController CameraController;
}
