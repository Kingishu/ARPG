using UnityEngine;

public class UnitManager
{
    private static UnitManager instance = new UnitManager();
    public static UnitManager Instance => instance;
    private UnitManager()
    {
        
    }
    public FSM player;
    //提供初始化创建角色的接口
    public GameObject CreatePlayer()
    {
        if (player==null)
        {
            var go=ResourcesManager.Instance.Instantiate<GameObject>("Unit/Character");
            Transform point=GameObject.Find("GatePoint/0").transform;
            go.transform.position=point.position;
            go.transform.forward=point.forward;
            player=go.GetComponent<FSM>();
        }
        return player.gameObject;
    }
}
