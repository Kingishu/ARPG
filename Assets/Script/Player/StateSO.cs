using Game.Config;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "配置/角色状态配置表")]
public class StateSO:ScriptableObject,ISerializationCallbackReceiver
{
    [SerializeField]
    [ListDrawerSettings(ShowIndexLabels = true,ShowPaging = false,ListElementLabelName = "info")]
    public List<StateEntity> states=new List<StateEntity>();
    //在保存之前
    public void OnBeforeSerialize()
    {
    }
    //在加载之后
    public void OnAfterDeserialize()
    {
        //核心就是读取all的数据
        if (PlayerStateData.all == null)
        {
            //不存在直接return出去
            return;
        }
        //首先将列表转换为字典,方便查找
        Dictionary<int, StateEntity> existingData;
        existingData=states.ToDictionary(state=>state.id,state=>state);
        states.Clear();
        foreach (var item in PlayerStateData.all)
        {
            if (existingData.TryGetValue(item.Key,out StateEntity state))
            {
                //如果老数据中存在,直接添加就可以
                state.info=item.Value.info;
                states.Add(state);
            }
            else
            {
                //如果老数据中不存在,创建一个新的
                var newState=new StateEntity();
                newState.id = item.Key;
                newState.info=item.Value.info;
                states.Add(newState);

            }
        }
    }
}
[System.Serializable]
public class StateEntity
{
    [Header("动作ID")]
    public int id;
    [Header("动作信息")]
    public string info;
    [Header("是否忽略碰撞")] 
    public bool IgnoreCollision;
    [Header("物理位移配置")]
    public List<PhysicsConfig> physicsConfig;

    [Header("物体控制配置")] public List<Obj_State> ObjStates;
    [Header("顿帧配置")] public List<HitLagConfig> hitLagConfig;
    [Header("径向模糊配置")]public List<RadialBlurConfig> radialBlurConfig;
    [Header("命中检测配置")] public List<HitConfig> hitConfig;
}
[System.Serializable]
public class PhysicsConfig
{
    [Header("触发点")]
    public float trigger;
    [Header("结束点")]
    public float end;
    [Header("位移距离")]
    public Vector3 force;
    [Header("动作曲线配置")]
    public AnimationCurve cure = AnimationCurve.Constant(0, 1, 1);
    [Header("是否忽略重力")]
    public bool ignore_Gravity;
    [Header("检测单位后停下")]
    public float stop_dst;
}
[System.Serializable]
public class Obj_State
{
    [Header("介绍")]
    public string info;
    [Header("触发点")] 
    public float trigger;
    [Header("控制的物体")] 
    public List<string> object_ID;
    [Header("启用/隐藏")] 
    public bool act;
    [Header("状态被打断,改状态是否继续执行")] 
    public bool force;
    [Header("状态循环,该状态是否循环")]
    public bool loop;
}
[System.Serializable]
public class HitLagConfig
{
    [Header("触发点")] public float trigger;
    [Header("顿帧帧数")] public int frame;
    [Header("触发方式:0直接触发 1命中触发")] public int Type;
    [Header("触发点2")] public float trigger2;
    [Header("是否插值")] public bool lerp;
}
[System.Serializable]
public class RadialBlurConfig
{
    [Header("触发点")] public float trigger;
    [Header("启用/关闭")]public bool act;
    [Header("平滑时间")] public float lerpTime;
}
[System.Serializable]
public class HitConfig
{
    [Header("触发点")] 
    public float trigger;
    [Header("结束点")]
    public float end;
    [Header("检测方式:0射线检测,1盒子检测")]
    public int type;
    [Header("射线起点")] 
    public string begin; //通过string获得挂点,从而获得物体
    [Header("射线长度")]
    public float length;
    [Header("盒子碰撞器中心点")] 
    public Vector3 center;
    [Header("盒子碰撞体边长")]
    public Vector3 size;
    [Header("命中特效")] 
    public string hitObj;
}