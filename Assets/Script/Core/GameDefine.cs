using UnityEngine;

public class GameDefine
{
    public static Vector3 gravity=new Vector3(0,-9.81f,0);
    
    public static Transform camera;
    
    public static Vector3 Ground_Dst=new Vector3(0,-0.02f,0);

    public static int Ground_LayerMask;

    public static void Init()
    {
        camera=Camera.main.transform;
        Ground_LayerMask=LayerMask.GetMask("Default");
    }
}
