using UnityEngine;

public class GameTime
{
    public static float time;
    public static float deltaTime;
    public static int now_frame;

    public static void Update()
    {
        time=Time.time;
        deltaTime=Time.deltaTime;
        now_frame=Time.frameCount;  
    }
}
