using System;
using UnityEngine;
using UnityEngine.Serialization;

public class UCameraController : MonoBehaviour
{
    
    [Header("跟随目标")]
    public Transform target;
    [Header("跟随差值")] 
    public Vector3 high_offset;

    [Header("旋转平滑速度")] public float rotationSpeed = 25f;
    [Header("位置平滑速度")] public float positionSpeed = 8f;
    [Header("鼠标滚轮灵敏度(缩放灵敏度)")]public float ScrollWheelSpeed = 2.5f;
    [Header("缩放位置限制")] public Vector2 scrollWheelClamp=new Vector2(4,10);
    
    //围绕Y轴旋转量,也就是鼠标的x轴
    private float xMouse;
    //围绕X轴旋转两,控制上下,也就是鼠标的Y轴
    private float yMouse;
    //控制距离远近
    private float distance;
    //Controller
    private CharacterController characterController;
    //增加一个状态机
    private int state = 0;//0空闲 1跟随
    
    void Start()
    {
        if (target!=null)
        {
            Cursor.lockState=CursorLockMode.Locked;
            Cursor.visible=false;
            characterController = target.GetComponent<CharacterController>();
            high_offset = characterController.center * 1.75f;
        }
    }

    public void SetTarget(Transform target)
    {
        state = 1;
        this.target = target;
        if (target!=null)
        {
            Cursor.lockState=CursorLockMode.Locked;
            Cursor.visible=false;
            characterController = target.GetComponent<CharacterController>();
            high_offset = characterController.center * 1.75f;
        }
        Follow(false);
        this.gameObject.SetActive(true);
    }
    // Update is called once per frame
    private void LateUpdate()
    {
        Follow();
    }

    private void Follow(bool lerp=true)
    {
        if (target!=null && state==1)
        {
            xMouse += UInput.GetAxis_Mouse_X();
            yMouse -= UInput.GetAxis_Mouse_Y();
            distance-= UInput.GetAxis_Mouse_ScrollWheel()*ScrollWheelSpeed;
            //对上述参数进行限制
            yMouse = Mathf.Clamp(yMouse, -35, 70f);
            distance = Mathf.Clamp(distance, scrollWheelClamp.x, scrollWheelClamp.y);
            //计算一下旋转量
            Quaternion targetRotation=Quaternion.Euler(yMouse,xMouse,0);
            //计算一下位置
            Vector3 targetPosition=target.position+targetRotation*new Vector3(0,0,-distance)+high_offset;
            //平滑过度
            if (lerp)
            {
                transform.position=Vector3.Lerp(transform.position,targetPosition,GameTime.deltaTime*positionSpeed);
                transform.rotation=Quaternion.Slerp(transform.rotation,targetRotation,GameTime.deltaTime*rotationSpeed);
            }
            else
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
            }
            
        }
    }
}
