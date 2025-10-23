using UnityEngine;

public static class UInput
{
    public static float GetAxis_Horizontal()
    {
        return Input.GetAxis("Horizontal");
    }
    public static float GetAxis_Vertical()
    {
        return Input.GetAxis("Vertical");
    }

    public static float GetAxis_Mouse_X()
    {
        return Input.GetAxis("Mouse X");
    }
    public static float GetAxis_Mouse_Y()
    {
        return Input.GetAxis("Mouse Y");
    }

    public static float GetAxis_Mouse_ScrollWheel()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }

    public static bool GetKeyDown_Space()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    
}
