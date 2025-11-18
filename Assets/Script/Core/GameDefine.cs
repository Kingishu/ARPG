using UnityEngine;

public class GameDefine
{
    public static Vector3 gravity=new Vector3(0,-9.81f,0);
    
    public static Transform camera;
    
    public static Vector3 Ground_Dst=new Vector3(0,-0.02f,0);

    public static int Ground_LayerMask;
    public static int Enemy_LayerMask;
    public static int Player_LayerMask;

    public static string WeaponTag = "Weapon";

    public static void Init()
    {
        camera = GameObject.Find("Main").transform.Find("Camera").transform;
        Ground_LayerMask=LayerMask.GetMask("Default");
        Enemy_LayerMask=LayerMask.GetMask("Enemy");
        Player_LayerMask = LayerMask.GetMask("Player");
    }
}
