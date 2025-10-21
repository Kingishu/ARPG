using System;
using System.Collections.Generic;
using Game.Config;
using Unity.VisualScripting;
using UnityEngine;

public class FSM : MonoBehaviour
{
    //角色ID
    [Range(1000,10000)]
    public int ID;
    //单位基础表
    public UnitEntity unitEntity;
    //状态表
    Dictionary<int, PlayerState> stateData = new Dictionary<int, PlayerState>();
    //当前状态
    public PlayerState currentState;
    [HideInInspector]
    public Transform _transform;
    [HideInInspector]
    public GameObject _gameObject;
    public Animator animator;
    public CharacterController characterController;
    void Awake()
    {
        //获取单位基础表
        unitEntity = UnitData.Get(ID);
        _transform = this.transform;
        _gameObject = this.gameObject;

        //状态初始化
        InitState();
        InitServices();

        animator = _transform.GetChild(0).GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        //切换到1001 待机状态
        ToNext(1001);
    }
    void Update()
    {
        if (currentState != null)
        {
            //服务组件可能会修改当前状态
            if (ServicesOnUpdate() == true)
            {
                //执行当前状态绑定的事件
                DoStateEvent(currentState.id, StateEventType.update);
            }
        }

    }
    //初始化的方法
    public void InitState()
    {
        if (PlayerStateData.all != null)
        {
            foreach (var state in PlayerStateData.all)
            {
                PlayerState newState = new PlayerState();
                newState.id = state.Key;
                newState.stateEntity = state.Value;
                stateData.Add(newState.id, newState);
            }
        }
        //设置攻击状态的技能表数据
        stateData[1005].skillEntity = SkillData.Get(unitEntity.ntk1); //普攻1
        stateData[1006].skillEntity = SkillData.Get(unitEntity.ntk2);
        stateData[1007].skillEntity = SkillData.Get(unitEntity.ntk3);
        stateData[1008].skillEntity = SkillData.Get(unitEntity.ntk4); //普攻4
        //设置技能状态的技能表数据
        stateData[1009].skillEntity = SkillData.Get(unitEntity.skill1); //普攻1
        stateData[1010].skillEntity = SkillData.Get(unitEntity.skill2);
        stateData[1011].skillEntity = SkillData.Get(unitEntity.skill3);
        stateData[1012].skillEntity = SkillData.Get(unitEntity.skill4);
        
        #region 事件绑定
        //绑定移动输入相关事件
        foreach (var state in stateData.Values)
        {
            if (state.stateEntity.on_move != null)
            {
                AddListener(state.id,StateEventType.update,OnMove);
            }
        }
        
        
        #endregion
    }
    /// <summary>
    /// 检测状态是否都和前后摇
    /// </summary>
    /// <param name="config">前后摇消息</param>
    /// <returns></returns>
    public bool CheckConfig(float[] config)
    {
        if ((animationService.normalizedTime >0 && animationService.normalizedTime < config[0]) || 
            (animationService.normalizedTime>config[1] && animationService.normalizedTime<1))
        {
            return true;
        }

        return false;
    }
    
    private void OnMove()
    {
        if (Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!=0)
        {
            if (CheckConfig(currentState.stateEntity.on_move))
            {
                ToNext((int)currentState.stateEntity.on_move[2]);
            }
        }
    }
    /// <summary>
    /// 切换状态的方法
    /// </summary>
    /// <param name="next">下一个状态的id</param>
    public void ToNext(int next)
    {
        if (stateData.ContainsKey(next))
        {
            //info信息显示
            if (currentState != null)
            {
                Debug.Log($"角色ID:{this.ID},切换状态:{stateData[next].Info()},当前状态:{currentState.Info()}");
            }
            else
            {
                Debug.Log($"角色ID:{this.ID},切换状态:{stateData[next].Info()}");
            }
            //切换逻辑
            if (currentState != null)
            {
                DoStateEvent(currentState.id, StateEventType.end);
                ServicesOnEnd();

            }

            currentState = stateData[next];
            currentState.SetBegin();

            DoStateEvent(currentState.id, StateEventType.begin);
            ServicesOnBegin();
        }
    }

    Dictionary<int, Dictionary<StateEventType, List<Action>>> actions = new();
    public void AddListener(int id, StateEventType type, Action action)
    {
        if (!actions.TryGetValue(id, out var events))
        {
            events = new();
            actions.Add(id, events);
        }
        if (!events.TryGetValue(type, out var list))
        {
            list = new List<Action>();
            events.Add(type, list);
        }
        list.Add(action);
    }

    public void DoStateEvent(int id, StateEventType type)
    {
        if (actions.TryGetValue(id, out var events))
        {
            if (events.TryGetValue(type, out var actionList))
            {
                for (int i = 0; i < actionList.Count; i++)
                {
                    actionList[i].Invoke();
                }
            }
        }
    }

    public void AnimationOnPlayEnd()
    {
        DoStateEvent(currentState.id, StateEventType.animEnd);
    }

    #region 服务组件内容


    List<FSMServiceBase> fsmServices = new List<FSMServiceBase>();
    //提前缓存当前服务组件的个数
    int services_Count;
    //添加服务组件
    public T AddService<T>() where T : FSMServiceBase, new()
    {
        T service = new T();
        fsmServices.Add(service);
        service.Init(this);
        return service;
    }
    //初始化服务组件
    AnimationService animationService;
    public void InitServices()
    {
        animationService = AddService<AnimationService>();
        services_Count = fsmServices.Count;
    }
    //Services的各种生命周期函数
    public void ServicesOnBegin()
    {
        for (int i = 0; i < services_Count; i++)
        {
            fsmServices[i].OnBegin(currentState);
        }
    }
    public bool ServicesOnUpdate()
    {
        int cur_id = currentState.id;
        for (int i = 0; i < services_Count; i++)
        {
            fsmServices[i].OnUpdate(animationService.normalizedTime, currentState);
            if (currentState.id != cur_id)
            {
                return false;
            }
        }
        return true;
    }
    public void ServicesOnEnd()
    {
        for (int i = 0; i < services_Count; i++)
        {
            fsmServices[i].OnEnd(currentState);
        }
    }
    public void ServicesOnAnimationEnd()
    {
        for (int i = 0; i < services_Count; i++)
        {
            fsmServices[i].OnAnimationEnd(currentState);
        }
    }
    public void ServicesOnDisable()
    {
        for (int i = 0; i < services_Count; i++)
        {
            fsmServices[i].OnDisable(currentState);
        }
    }
    public void ServicesReLoop()
    {
        for (int i = 0; i < services_Count; i++)
        {
            fsmServices[i].ReLoop(currentState);
        }
    }
    public void ServicesReStart()
    {
        for (int i = 0; i < services_Count; i++)
        {
            fsmServices[i].ReStart(currentState);
        }
    }
    #endregion


}
public class PlayerState
{
    public int id;
    public float beginTime;
    public PlayerStateEntity stateEntity;
    public SkillEntity skillEntity;
    public void SetBegin()
    {
        beginTime = Time.time;
    }
    public bool IsCD()
    {
        if (skillEntity == null)
        {
            return false;
        }
        else
        {
            return Time.time - beginTime < skillEntity.cd;
        }
    }
    public string Info()
    {
        return $"{stateEntity.info}";
    }
}

public enum StateEventType
{
    begin,
    update,
    end,
    animEnd
}
