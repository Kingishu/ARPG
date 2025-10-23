using System;
using System.Collections.Generic;
using Game.Config;
using Unity.VisualScripting;
using UnityEngine;

public class FSM : MonoBehaviour
{
    //角色ID
    [Header("角色属性表ID")]
    public int ID=1001;
    [Header("移动速度")]
    public float _speed=5f;

    [Header("角色是否为AI")] public bool AI;
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

        animator = _transform.GetChild(0).GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        
        //状态初始化
        InitState();
        InitServices();

        

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

            ToGround();
        }

    }
    
    //拿到配置表
    private StateSO anmConfig;
    //初始化的方法
    public void InitState()
    {
        //拿到当前角色的配置表
        anmConfig = Resources.Load<StateSO>($"StateConfig/{ID}");
        //字典缓存配置信息
        Dictionary<int,StateEntity> states = new Dictionary<int, StateEntity>();
        //添加进入字典
        foreach (var item in anmConfig.states)
        {
            states[item.id] = item;
        }
        
        //对状态中的动画长度进行初始化
        var clips = animator.runtimeAnimatorController.animationClips;
        Dictionary<string, float> clipsLength = new();
        foreach (var clip in clips)
        {
            clipsLength[clip.name] = clip.length;
        }
        if (PlayerStateData.all != null)
        {
            foreach (var state in PlayerStateData.all)
            {
                PlayerState newState = new PlayerState();
                newState.id = state.Key;
                newState.excel_config = state.Value;
                newState.stateEntity=states[newState.id];
                if (clipsLength.TryGetValue(newState.excel_config.anm_name,out var length))
                {
                    newState.clipLength=length;
                }
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
            if (state.excel_config.on_move != null)
            {
                AddListener(state.id,StateEventType.update,OnMove);
            }

            if (state.excel_config.do_move ==1)
            {
                AddListener(state.id,StateEventType.update,PlayerMove);
            }

            if (state.excel_config.on_stop!=0)
            {
                AddListener(state.id,StateEventType.update,Stop);
            }

            if (state.excel_config.on_jump!=null)
            {
                AddListener(state.id,StateEventType.update,OnJump);
            }

            if (state.excel_config.on_jump_end!=0)
            {
                AddListener(state.id,StateEventType.update,OnJumpUpdate);
            }

            if (state.excel_config.add_f_move!=0)
            {
                AddListener(state.id,StateEventType.update,AddForwardMove);
            }
        }
        #endregion
    }

    private void AddForwardMove()
    {
        float x = UInput.GetAxis_Horizontal();
        float z = UInput.GetAxis_Vertical();
        if (x!=0 || z>0)
        {
            Vector3 v = new Vector3(x, 0, z>0?z:0).normalized * currentState.excel_config.add_f_move;
            Move(v,true,true,false,false);
        }

    }

    private void OnJumpUpdate()
    {
        if (Physics.Raycast(_transform.position,Vector3.up*-1f,0.15f,GameDefine.Ground_LayerMask))
        {
            ToNext(currentState.excel_config.on_jump_end);
        }
    }

    private void OnJump()
    {
        if (UInput.GetKeyDown_Space())
        {
            if (CheckConfig(currentState.excel_config.on_jump))
            {
                ToNext((int)currentState.excel_config.on_jump[2]);
            }
        }
    }

    private void Stop()
    {
        //WASD都没有按下,切换
        if (UInput.GetAxis_Horizontal() == 0 && UInput.GetAxis_Vertical() == 0)
        {
            ToNext(currentState.excel_config.on_stop);
        }
    }

    private float targetRotation;
    private float _rotationVelocity;
    private float rotationSmoothTime=0.05f;
    
    private void PlayerMove()
    {
        if (UInput.GetAxis_Horizontal() != 0 || UInput.GetAxis_Vertical() != 0)
        {
            float x = UInput.GetAxis_Horizontal();
            float z = UInput.GetAxis_Vertical();
            Vector3 inputDirection = new Vector3(x, 0f, z).normalized;
            //旋转
            targetRotation=Mathf.Atan2(x,z)*Mathf.Rad2Deg+GameDefine.camera.eulerAngles.y;
            //平滑过度
            float rotation=Mathf.SmoothDampAngle(_transform.eulerAngles.y,targetRotation,ref _rotationVelocity,rotationSmoothTime);
            //赋值
            transform.rotation=Quaternion.Euler(0,rotation,0);
            //计算移动方向
            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
            Move(targetDirection.normalized*(_speed*GameTime.deltaTime),false,false,false,true);
        }
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
    /// <summary>
    /// 接地检测方法
    /// </summary>
    public void ToGround()
    {
        if (groundCheck)
        {
            if (Physics.Linecast(_transform.position,_transform.position+GameDefine.Ground_Dst,GameDefine.Ground_LayerMask))
            {
                groundCheck = false;
            }
            else
            {
                Move(Vector3.up * -9.81f,false,false,false,false);
            }
        }
    }
    
    
    private bool groundCheck=false;
    public void Move(Vector3 dir,bool transformDirection,bool deltaTime=true,bool add_Gravity=true,bool do_ground_check=true)
    {
        if (transformDirection)
        {
            dir = _transform.TransformDirection(dir);
        }
        Vector3 d2;
        if (add_Gravity)
        {
            d2=(dir+GameDefine.gravity)*(deltaTime?GameTime.deltaTime:1);
        }
        else
        {
            d2=dir*(deltaTime?GameTime.deltaTime:1);
        }

        if (do_ground_check)
        {
            groundCheck = true;
        }
        characterController.Move(d2);

    }
    private void OnMove()
    {
        if (UInput.GetAxis_Horizontal()!=0 || UInput.GetAxis_Vertical()!=0)
        {
            if (CheckConfig(currentState.excel_config.on_move))
            {
                ToNext((int)currentState.excel_config.on_move[2]);
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
        //存储一下当前状态id
        int crn_id = currentState.id;
        DoStateEvent(currentState.id, StateEventType.animEnd);
        ServicesOnAnimationEnd();

        if (crn_id == currentState.id)
        {
            switch (currentState.excel_config.on_anm_end)
            {
                case -1:
                    //不需要做任何处理
                    break;
                case 0:
                    //循环动画
                    ServicesReStart();
                    return;
                default:
                    //切换
                    ToNext(currentState.excel_config.on_anm_end);
                    break;
            }
        }
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
    PhysicsService physicsService;
    public void InitServices()
    {
        animationService = AddService<AnimationService>();
        physicsService=AddService<PhysicsService>();
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

    public void AddForce(Vector3 force, bool currentEntityIgnoreGravity)
    {
        Move(force,false,true,currentEntityIgnoreGravity==false,!currentEntityIgnoreGravity);
    }

    public int GetEnemyLayer()
    {
        if (AI)
        {
            return GameDefine.Player_LayerMask;
        }
        else
        {
            return GameDefine.Enemy_LayerMask;
        }
    }

    public void RemoveForce()
    {
        
    }
}
public class PlayerState
{
    public int id;
    public float beginTime;
    public float clipLength;//动作片段的长度
    public PlayerStateEntity excel_config;
    public StateEntity stateEntity;
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
        return $"{excel_config.info}";
    }
}

public enum StateEventType
{
    begin,
    update,
    end,
    animEnd
}
