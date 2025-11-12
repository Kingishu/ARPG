using UnityEngine;
using UnityEngine.PlayerLoop;

public class PhysicsService:FSMServiceBase
{
    public override void Init(FSM fsm)
    {
        base.Init(fsm);
    }

    public override void OnBegin(PlayerState state)
    {
        base.OnBegin(state);
        ReSetAllExecute();
    }

    public override void OnUpdate(float normalizedTime, PlayerState state)
    {
        base.OnUpdate(normalizedTime, state);
        var configs = state.stateEntity.physicsConfig;
        if (configs != null && configs.Count > 0)
        {
            for (int i = 0; i < configs.Count; i++)
            {
                PhysicsConfig config=configs[i];
                if (normalizedTime>=config.trigger && GetExecute(i)==false)
                {
                    DO(config, state);
                    SetExecute(i);
                }
            }
        }

        if (begin)
        {
            if (normalizedTime<crn_config.end)
            {
                if (crn_config.end>0)
                {
                    //当前进度的半分比.     相对于trigger-end,而不是全程
                    float percent=(normalizedTime-crn_config.trigger)/(crn_config.end-crn_config.trigger);
                    //得到曲线当中的y坐标
                    float y=crn_config.cure.Evaluate(percent);
                    //得到曲线优化过的速度
                    Vector3 speed = Vector3.Lerp(Vector3.zero, force, y);
                    //具体的进行位移配置,调用相对应的接口
                    player.AddForce(speed,crn_config.ignore_Gravity);
                    //检测到前面有敌人就暂停
                    if (crn_config.stop_dst>0)
                    {
                        if (Physics.Raycast(player._transform.position+Vector3.up,player._transform.forward,crn_config.stop_dst,player.GetEnemyLayer()))
                        {
                            Stop();
                        }
                    }
                }
            }
            else
            {
                Stop();
            }
           
        }
    }

    private void Stop()
    {
        begin = false;
        player.RemoveForce();
    }

    private bool begin=false;
    private PhysicsConfig crn_config;
    private Vector3 force;
    private void DO(PhysicsConfig config, PlayerState state)
    {
        //具体执行动作的时长是多少
        float time = state.clipLength * (config.end-config.trigger);
        if (time<=0)
        {
            begin = false;
        }
        else
        {
            crn_config=config;
            if (crn_config.end>0)
            {
                force=crn_config.force/time;
            }
            else
            {
                force=crn_config.force;
            }

            begin = true;
        }
    }

    public override void OnEnd(PlayerState state)
    {
        base.OnEnd(state);
        ReSetAllExecute();
        Stop();
    }

    public override void OnAnimationEnd(PlayerState state)
    {
        base.OnAnimationEnd(state);
        ReSetAllExecute();
        Stop();
    }

    public override void ReLoop(PlayerState state)
    {
        base.ReLoop(state);
    }

    public override void ReStart(PlayerState state)
    {
        base.ReStart(state);
        ReSetAllExecute();
    }

    public override void OnDisable(PlayerState state)
    {
        base.OnDisable(state);
    }
}
