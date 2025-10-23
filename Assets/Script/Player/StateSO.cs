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
    [Header("物理位移配置")]
    public List<PhysicsConfig> physicsConfig;
    
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