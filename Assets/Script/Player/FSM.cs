using System;
using System.Collections.Generic;
using Game.Config;
using Pathfinding;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    //属性表
    private UnitAttEntity att_base;
    //当前属性表
    public UnitAttEntity att_crn;
    [HideInInspector]
    public Transform _transform;
    [HideInInspector]
    public GameObject _gameObject;
    [HideInInspector]
    public int instance_ID;
    public Animator animator;
    public CharacterController characterController;
    public Seeker _seeker;
    void Awake()
    {
        //获取单位基础表
        unitEntity = UnitData.Get(ID);
        _transform = this.transform;
        _gameObject = this.gameObject;
        instance_ID = GetInstanceID();

        animator = _transform.GetChild(0).GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        _seeker = GetComponent<Seeker>();
        
        //属性初始化
        att_base = AttHelper.Instance.Creat(unitEntity.att_id);
        att_crn=AttHelper.Instance.Creat(att_base);
        
        //状态初始化
        InitState();
        InitServices();

        if (AI==false)
        {
            UnitManager.Instance.player=this;
        }
        
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
        if (AI == false)
        {
            foreach (var state in stateData.Values)
            {
                if (state.excel_config.on_move != null)
                {
                    AddListener(state.id, StateEventType.update, OnMove);
                }

                if (state.excel_config.do_move == 1)
                {
                    AddListener(state.id, StateEventType.update, PlayerMove);
                }

                if (state.excel_config.on_stop != 0)
                {
                    AddListener(state.id, StateEventType.update, Stop);
                }

                if (state.excel_config.on_jump != null)
                {
                    AddListener(state.id, StateEventType.update, OnJump);
                }

                if (state.excel_config.on_jump_end != 0)
                {
                    AddListener(state.id, StateEventType.update, OnJumpUpdate);
                }

                if (state.excel_config.add_f_move != 0)
                {
                    AddListener(state.id, StateEventType.update, AddForwardMove);
                }

                if (state.excel_config.on_atk != null)
                {
                    AddListener(state.id, StateEventType.update, OnAtk);
                }

                if (state.excel_config.on_skill1 != null)
                {
                    AddListener(state.id, StateEventType.update, OnSkill1);
                }

                if (state.excel_config.on_skill2 != null)
                {
                    AddListener(state.id, StateEventType.update, OnSkill2);
                }

                if (state.excel_config.on_skill3 != null)
                {
                    AddListener(state.id, StateEventType.update, OnSkill3);
                }

                if (state.excel_config.on_skill4 != null)
                {
                    AddListener(state.id, StateEventType.update, OnSkill4);
                }

                if (state.excel_config.on_defense != null)
                {
                    AddListener(state.id, StateEventType.update, OnDefense);
                }

                if (state.excel_config.on_defense_quit != 0)
                {
                    AddListener(state.id, StateEventType.update, OnDefenseQuit);
                }

                if (state.excel_config.on_sprint != null)
                {
                    AddListener(state.id, StateEventType.update, OnSprint);
                }

                if (state.excel_config.on_pow_atk != null)
                {
                    AddListener(state.id, StateEventType.update, OnPowAtk);
                }

                if (state.excel_config.do_rotate != 0)
                {
                    AddListener(state.id, StateEventType.update, DoRotate);
                }

                if (state.stateEntity.IgnoreCollision)
                {
                    AddListener(state.id, StateEventType.begin, DisableCollider);
                    AddListener(state.id, StateEventType.end, EnableCollider);
                }
                //当开始攻击或者技能的时候调用
                if (state.skillEntity!=null)
                {
                    AddListener(state.id, StateEventType.begin,OnSkillBegin);
                }
            }
        }
        //AI要注册的事件
        else
        {

            foreach (var state in stateData.Values) 
            {
                if (state.excel_config.active_attack>0)
                {
                    AddListener(state.id, StateEventType.update,AutoTriggerAtk_AI);
                }

                if (state.excel_config.trigger_pacing>0)
                {
                    AddListener(state.id, StateEventType.animEnd,TriggerPacing);
                }

                if (state.excel_config.tag==4)
                {
                    AddListener(state.id, StateEventType.update,PacingUpdate);
                }
                if (state.excel_config.trigger_patrol > 0)
                {
                    AddListener(state.id, StateEventType.update, TriggerPatrol);
                }
            }
            GameEvent.OnPlayerAtk += OnPlayerAtk;
            AddListener(1014,StateEventType.animEnd,OnDashEnd);
            AddListener(1042,StateEventType.update,OnMoveToPoint);
            AddListener(1042,StateEventType.end,NavStop);
            AddListener(1013,StateEventType.update,AI_Defencing);
            AddListener(10131,StateEventType.update,AI_Defencing);
            
            AddListener(1043, StateEventType.update, OnPatrolUpdate);
            AddListener(1043, StateEventType.begin, ChangeMoveSpeed);
            AddListener(1043, StateEventType.end, ResetMoveSpeed);
        }
        
        //AI和敌人共有的
        AddListener(1017,StateEventType.update,OnBashUpdate);
        AddListener(1017,StateEventType.end,OnBashEnd);
        AddListener(1018,StateEventType.update,OnBashUpdate);
        AddListener(1018,StateEventType.end,OnBashEnd);
        #endregion
    }

    private void TriggerPatrol()
    {
        if (atk_target == null || GetEnemyDistance() > 10f)
        {
            //在可以切换到巡视的状态下超过5秒,切换到巡视状态
            if (GameTime.time - currentState.beginTime >= currentState.excel_config.trigger_patrol)
            {
                //进入巡逻
                float radius = IntEx.Range(6, 8); //3m到6m随机生成
                float angle = IntEx.Range(1, 360);
                Vector3 point = _transform.GetOffsetPoint(radius, angle);
                navigationService.Move(point, ToPatrol);
            }
        }
    }
    /// <summary>
    /// 巡逻接口嗲用,将玩家的速度切换到走路速度
    /// </summary>
    private void ChangeMoveSpeed()
    {
        _speed = 2.5f;
    }

    /// <summary>
    /// 寻路结束接口调用,将玩家速度切换到正常速度,并且停止寻路接口.
    /// </summary>
    private void ResetMoveSpeed()
    {
        _speed = 5f;
        navigationService.Stop();
    }
    //路径查找结束调用,切换到1043寻路状态.
    public void ToPatrol()
    {
        ToNext(1043);
    }
    //巡逻状态调用,在巡逻结束或者巡逻超时状态下切换到1001
    public void OnPatrolUpdate()
    {
        if (navigationService.IsEnd() || GameTime.time - currentState.beginTime > 5f)
        {
            ToNext(1001);
        }
    }
    //看向攻击者方法,这里指AI看向玩家
    public void LookAtkTarget()
    {
        if (atk_target==null)
        {
            atk_target = UnitManager.Instance.player;
        }
        _transform.LookTarget(atk_target._transform);
    }
    //AI在防御中调用方法,每时每刻看向玩家,超时切换待机.
    private void AI_Defencing()
    {
        LookAtkTarget();
        if (GameTime.time-currentState.beginTime>2.5f)
        {
            //格挡超过2.5秒,切换
            ToNext(10132);
        }
    }
    //得到敌人和玩家的距离
    private float GetEnemyDistance()
    {
        if (atk_target==null)
        {
            atk_target = UnitManager.Instance.player;
        }
        var dst = Vector3.Distance(_transform.position, atk_target._transform.position);
        return dst;
    }
    //踱步状态调用,踱步超过3秒,切换到下一个踱步状态,踱步和玩家距离小于3m,进入攻击状态. 调用AIATK
    private void PacingUpdate()
    {
        if (GameTime.time - currentState.beginTime >= 3)
        {
            var nextState = IntEx.Range(1036, 1041);
            _transform.LookTarget(atk_target._transform);
            ToNext(nextState);
        }

        if (atk_target != null)
        {
            _transform.LookTarget(atk_target._transform.position);
            if (GetEnemyDistance() <= 3)
            {
                AIAtk();
                return;
            }
        }
    }
    //触发踱步接口.
    private void TriggerPacing()
    {
        if (IsDead())
        {
            return;
        }

        if (unitEntity.pacing_probability>0)
        {
            if (unitEntity.pacing_probability.InRange())
            {
                //判定成功,本次处罚踱步
                var dst = GetEnemyDistance();
                //位置判断
                if (atk_target==null)
                {
                    
                    if (dst <= 10f)
                    {
                        atk_target = UnitManager.Instance.player;
                    }
                    else
                    {
                        return;
                    }
                }
                //和角色距离10m之内才能踱步,大于10m我们走巡逻的状态.
                if (dst<=10)
                {
                    //随机获得一个踱步状态,进入改状态
                    var nextState = IntEx.Range(1036, 1041);
                    _transform.LookTarget(atk_target._transform);
                    ToNext(nextState);
                }
            }
            else
            {
                return;
            }
        }
    }
    //查询玩家是否死亡
    public bool IsDead()
    {
        return att_crn.hp <= 0;
    }
    //AI在某些状态下超过n秒,自动攻击,由配置表调度.
    public void AutoTriggerAtk_AI()
    {
        if (GameTime.time-currentState.beginTime >currentState.excel_config.active_attack)
        {
            AIAtk();
        }
    }
    private float _OnMoveToPoint_CheckTime;
    private void OnMoveToPoint()
    {
        if (GameTime.time- _OnMoveToPoint_CheckTime>0.1f)
        {
            _OnMoveToPoint_CheckTime=GameTime.time;
            if (next_Atk!=0 && atk_target!=null)
            {
                var dst=GetEnemyDistance();
                if (dst<=stateData[next_Atk].skillEntity.atk_distance)
                {
                    navigationService.Stop();
                    this._transform.LookTarget(atk_target.transform);
                    ToNext(next_Atk);
                    next_Atk=0;
                }
                else
                {
                    //寻路到终点了,也可能超时了
                    if (navigationService.IsEnd() || GameTime.time-currentState.beginTime>5f)
                    {
                        navigationService.Stop();
                        ToNext(1001);
                        next_Atk=0;
                        AIAtk();
                    }
                }
            }
        }
    }
    //反击策略,当玩家攻击的时候,我们可以执行格挡/躲避/强攻
    private void OnPlayerAtk(FSM atk, SkillEntity skill)
    {
        if (att_crn.hp<=0)
        {
            return;
        }
        //我们只考虑5米之内的敌人,节约性能,不开方,节约性能
        float sqrDistance=(this._transform.position-atk._transform.position).sqrMagnitude;
        if (sqrDistance<225)
        {
            //是否可以格挡
            if (unitEntity.block_probability.InRange() && false)
            {
                if (currentState.excel_config.on_defense!=null)
                {
                    if (CheckConfig(currentState.excel_config.on_defense))
                    {
                        //看向攻击者
                        _transform.LookTarget(atk._transform);
                        //切换格挡状态
                        bool result=ToNext((int)currentState.excel_config.on_defense[2]);
                        //切换成功,退出本次响应
                        if (result)
                        {
                            return;
                        }
                    }
                }
            }
            //闪避
            if (unitEntity.dodge_probability.InRange() && false)
            {
                if (currentState.excel_config.trigger_dodge>0)
                {
                    int nextState = IntEx.Range(1032, 1035);
                    bool result = ToNext(nextState);
                    if (result)
                    {
                        return;
                    }
                }
            }
            //主角攻击的时候,我们强攻
            if (unitEntity.atk_probability.InRange()||true) 
            {
                if (currentState.excel_config.first_strike>0)
                {
                    TriggerAtk_AI();
                }
            }
        }
    }
    //强攻接口
    private void TriggerAtk_AI()
    {
        if (animationService.normalizedTime>=currentState.excel_config.trigger_atk)
        {
            AIAtk();
        }
    }
    //存储下一次攻击
    private int next_Atk;
    //AI攻击的核心接口,所有让AI攻击的均调用该方法
    private void AIAtk()
    {
        if (att_crn.hp<=0)
        {
            return;
        }
        next_Atk = IntEx.Range(1005, 1012);
        if (stateData[next_Atk].skillEntity.atk_distance>0)
        {
            if (atk_target==null)
            {
                atk_target = UnitManager.Instance.player;
            }
            //得到距离的平方
            float sqrDistance=(this._transform.position-atk_target._transform.position).sqrMagnitude;
            //如果距离大于技能攻击距离
            if (sqrDistance >= Mathf.Pow(stateData[next_Atk].skillEntity.atk_distance,2))
            {
                //想办法靠近角色
                if (50.InRange() && false)
                {
                    //冲刺
                    _transform.LookTarget(atk_target._transform); //看向角色
                    ToNext(1014);//冲刺过去
                }
                else
                {
                    
                    if (navigationService.state==0)
                    {
                        navigationService.Move(atk_target._transform.position,MoveToPoint);
                    }
                }
            }
            else
            {
                _transform.LookTarget(atk_target._transform);
                ToNext(next_Atk);
            }
        }
    }
    //AI寻路结束接口,切换到1042踱步
    private void MoveToPoint()
    {
        ToNext(1042);
    }
    //AI寻路停止接口
    public void NavStop()
    {
        navigationService.Stop();
    }
    //我们的躲避接口,有冲刺的躲避办法,因此我们需要为冲刺结束后,检查位置进行下一次攻击
    public void OnDashEnd()
    {
        //冲刺结束之后,玩家可能移动,需要重新判定
        float sqrDistance=(this._transform.position-atk_target._transform.position).sqrMagnitude;
        if (sqrDistance >=  Mathf.Pow(stateData[next_Atk].skillEntity.atk_distance,2))
        {
            AIAtk();
        }
        else
        {
            _transform.LookTarget(atk_target._transform);
            ToNext(next_Atk);
            next_Atk = 0;
        }
    }
    //当玩家攻击的时候,调用事件中心的OnPlayerAtk,方便敌人AI监听.
    private void OnSkillBegin()
    {
        GameEvent.OnPlayerAtk.Invoke(this,currentState.skillEntity);
    }

    private void EnableCollider()
    {
        characterController.excludeLayers = 0;
    }

    private void DisableCollider()
    {
        characterController.excludeLayers = GameDefine.Enemy_LayerMask;
    }

    private float DoRotationSmoothTime=0.05f;
    private float DoRotationVelocity;
    private void DoRotate()
    {
        float x = UInput.GetAxis_Horizontal();
        float z=UInput.GetAxis_Vertical();
        if (x!=0 || z!=0)
        {
            if (animationService.normalizedTime <= currentState.excel_config.do_rotate)
            {
                Vector3 inputDirection = new Vector3(x, 0f, z).normalized;
                //旋转
                targetRotation= Mathf.Clamp(Mathf.Atan2(x,z)*Mathf.Rad2Deg,-45f,45f)+GameDefine.camera.eulerAngles.y;
                //对旋转角度做一个限制,不超过60度,不然非常不自然
                //平滑过度
                float rotation=Mathf.SmoothDampAngle(_transform.eulerAngles.y,targetRotation,ref DoRotationVelocity,DoRotationSmoothTime);
                //赋值
                transform.rotation=Quaternion.Euler(0,rotation,0);
            }
        }
    }

    private float powAtk_BeginTime;
    private void OnPowAtk()
    {
        if (UInput.GetMouseButtonDown_Left())
        {
            powAtk_BeginTime = 0;
        }

        if (UInput.GetMouseButton_Left())
        {
            if (GameTime.time-powAtk_BeginTime>=0.1f)
            {
                if (CheckConfig(currentState.excel_config.on_pow_atk))
                {
                    ToNext((int)currentState.excel_config.on_pow_atk[2]);
                }
            }
        }
    }

    private void OnSprint()
    {
        if (UInput.GetKeyDown_LeftShift())
        {
            if (CheckConfig(currentState.excel_config.on_sprint))
            {
                ToNext((int)currentState.excel_config.on_sprint[2]);
            }
        }
    }

    private void OnDefenseQuit()
    {
        if (UInput.GetMouseButtonUp_Right())
        {
            ToNext(currentState.excel_config.on_defense_quit);
        }
    }

    private void OnDefense()
    {
        if (UInput.GetMouseButtonDown_Right())
        {
            if (CheckConfig(currentState.excel_config.on_defense))
            {
                ToNext((int)currentState.excel_config.on_defense[2]);
            }
        }
    }

    private void OnSkill4()
    {
        if (UInput.GetKeyDown_T())
        {
            if (CheckConfig(currentState.excel_config.on_skill4))
            {
                ToNext((int)currentState.excel_config.on_skill4[2]);
            }
        }
    }
    private void OnSkill3()
    {
        if (UInput.GetKeyDown_R())
        {
            if (CheckConfig(currentState.excel_config.on_skill3))
            {
                ToNext((int)currentState.excel_config.on_skill3[2]);
            }
        }
    }

    private void OnSkill2()
    {
        if (UInput.GetKeyDown_E())
        {
            if (CheckConfig(currentState.excel_config.on_skill2))
            {
                ToNext((int)currentState.excel_config.on_skill2[2]);
            }
        }
    }

    private void OnSkill1()
    {
        if (UInput.GetKeyDown_Q())
        {
            if (CheckConfig(currentState.excel_config.on_skill1))
            {
                ToNext((int)currentState.excel_config.on_skill1[2]);
            }
        }
    }

    private void OnAtk()
    {
        if (UInput.GetMouseButtonUp_Left())
        {
            if (CheckConfig(currentState.excel_config.on_atk))
            {
                ToNext((int)currentState.excel_config.on_atk[2]);
            }
        }
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
    public bool ToNext(int next)
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
            return true;
        }

        return false;
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
    ObjService objService;
    HitlagService hitlagService;
    RadialBlurService radialBlurService;
    HitService hitService;
    NavigationService navigationService;
    public void InitServices()
    {
        animationService = AddService<AnimationService>();
        physicsService=AddService<PhysicsService>();
        objService=AddService<ObjService>();
        hitlagService=AddService<HitlagService>();
        radialBlurService=AddService<RadialBlurService>();
        hitService=AddService<HitService>();
        navigationService=AddService<NavigationService>();
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
        Move(force,true,true,currentEntityIgnoreGravity==false,!currentEntityIgnoreGravity);
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
    Dictionary<string,GameObject> hangPoint=new Dictionary<string, GameObject>();
    public GameObject GetHangPoint(string name)
    {
        if (hangPoint.TryGetValue(name,out GameObject temp))
        {
            return temp;
        }
        else
        {
            temp=_transform.Find(name).gameObject;
            if (temp!=null)
            {
                hangPoint[name] = temp;
                return temp;
            }
            else
            {
                hangPoint[name] = null;
                return null;
            }
            
        }
    }

    public void UpdateHP_OnHit(int damage)
    {
        att_crn.hp-=damage;
        if (att_crn.hp<=0)
        {
            att_crn.hp = 0;
        }
        Debug.Log($"当前的血量是{ att_crn.hp},收到了{damage}点伤害");
        //血条更新
        if (AI)
        {
            if (unitEntity.type==3)
            {
                 //更新BOss血条
            }
            else if (unitEntity.type==1 || unitEntity.type==2)
            {
               //更新小兵和精英怪的血条 
            }
        }
        else
        {
            //更新主角血条
        }
    }

    public FSM atk_target;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fd">1代表敌人在前方,-1代表敌人在后方,0代表敌人和玩家平行,平行视为敌人在前方</param>
    /// <param name="atk"></param>
    public void OnHit(int fd, FSM atk)
    {
        if (currentState.excel_config.on_hit!=null)
        {
            if (fd==1 || fd==0)//攻击者在自身的前方
            {
                //切换到前方受击
                ToNext(currentState.excel_config.on_hit[0]);
            }
            else if (fd==-1)
            {
                //切换到后方受击
                ToNext(currentState.excel_config.on_hit[1]);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fd">1代表敌人在前方,-1代表敌人在后方,0代表敌人和玩家平行,平行视为敌人在前方</param>
    /// <param name="atk"></param>
    public void OnDead(int fd, FSM atk)
    {
        if (currentState.excel_config.on_hit!=null)
        {
            if (fd==1 || fd==0)//攻击者在自身的前方
            {
                //切换到前方受击
                ToNext(currentState.excel_config.on_death[0]);
            }
            else if (fd==-1)
            {
                //切换到后方受击
                ToNext(currentState.excel_config.on_death[1]);
            }
            
        }
        characterController.enabled=false;
        //下面是特殊处理
    }

    public void Attack_Hitlag(PlayerState state)
    {
        hitlagService.DoHitlag_OnAtk(animationService.normalizedTime,state);
    }
/// <summary>
/// 格挡成功,进入格挡成功状态
/// </summary>
/// <param name="atk">攻击方</param>
/// <exception cref="NotImplementedException"></exception>
    public void OnBlockSucces(FSM atk)
    {
        if (currentState.excel_config.on_block_succes!=0)
        {
            ToNext(currentState.excel_config.on_block_succes);
        }
    }
/// <summary>
/// 攻击被格挡,进入弹反状态
/// </summary>
/// <param name="fsm">防守方</param>
/// <exception cref="NotImplementedException"></exception>
    public void BeBlock(FSM fsm)
    {
        if (currentState.excel_config.be_block!=null)
        {
            ToNext(currentState.excel_config.be_block[0]);
        }
    }

    public float GetPlayerSpeed()
    {
        return _speed;
    }

    private Vector3 bash_add_fly;
    private Vector3 bash_fly_dir;
    public void OnBash(int direction, FSM player, float[] skillEntityAddFly, Vector3 hitinfoPoint)
    {
        bash_add_fly=new Vector3(skillEntityAddFly[0],skillEntityAddFly[1],skillEntityAddFly[2]);
        int dir=direction>0?0:1;
        bash_fly_dir=(this._transform.position-atk_target._transform.position).normalized;
        if (currentState.excel_config.on_bash!=null)
        {
            ToNext(currentState.excel_config.on_bash[dir]);
        }
    }

    public void OnBashUpdate()
    {
        //击飞时长0.2   0.1上升,0.1下降
        float time = GameTime.time - currentState.beginTime;
        if (time < 0.1f)
        {
            var d = bash_fly_dir * (bash_add_fly.z / 0.2f);
            d.y = bash_add_fly.y / 0.2f;
            Move(d, false, add_Gravity: false, do_ground_check: false);
        }
        else if (time <= 0.2f)
        {
            var d = bash_fly_dir * (bash_add_fly.z / 0.2f);
            d.y = -(bash_add_fly.y / 0.2f);
            Move(d, false, add_Gravity: false, do_ground_check: false);
        }
    }
    public void OnBashEnd()
    {
        groundCheck = true;
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
