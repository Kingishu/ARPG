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

    public static bool GetMouseButtonUp_Left()
    {
        return Input.GetMouseButtonUp(0);
    }
    public static bool GetMouseButton_Left()
    {
        return Input.GetMouseButton(0);
    }
    public static bool GetMouseButtonDown_Left()
    {
        return Input.GetMouseButtonDown(0);
    }
    public static bool GetMouseButtonDown_Right()
    {
        return Input.GetMouseButtonDown(1);
    }
    public static bool GetMouseButtonUp_Right()
    {
        return Input.GetMouseButtonUp(1);
    }

    public static bool GetKeyDown_Q()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }
    public static bool GetKeyDown_E()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
    public static bool GetKeyDown_R()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
    public static bool GetKeyDown_T()
    {
        return Input.GetKeyDown(KeyCode.T);
    }

    public static bool GetKeyDown_LeftShift()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
    
}
