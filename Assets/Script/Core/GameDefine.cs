using System.Collections.Generic;
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
    //UI父目录
    public static string prop_root= "UI/Icon/Prop_Icon/";//物品
    public static string eq_root= "UI/Icon/EQ_Icon/";//装备
    public static string mat_root = "UI/Icon/Mat_Icon/";//材料
    //装备需要有父目录,根据不同部位
    public static Dictionary<int,string> part=new Dictionary<int,string>();
    //UI创建需要
    public static string itemPath = "UI/Item/Prop_Item";//Item的路径
    public static string content = "Bag/Scroll View/Viewport/Content/Prop_";//放置Item的容器路径


    public static void Init()
    {
        camera = GameObject.Find("Main").transform.Find("Camera").transform;
        Ground_LayerMask=LayerMask.GetMask("Default");
        Enemy_LayerMask=LayerMask.GetMask("Enemy");
        Player_LayerMask = LayerMask.GetMask("Player");
        part[1] = "Helmet/";
        part[2] = "Cloth/";
        part[3] = "Capes/";
        part[4] = "Pants/";
        part[5] = "Boots/";
        part[6] = "Weapon/";
        part[7] = "Earrings/";
        part[8] = "Necklaces/";
        part[9] = "Rings/";
        part[10] = "Belt/";
    }
    public static string Get_EQ_Icon(int part_id)
    {
        return eq_root + part[part_id];
    }
}
