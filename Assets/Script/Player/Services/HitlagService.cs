using System.Collections.Generic;
using UnityEngine;

public class HitlagService:FSMServiceBase
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
        List<HitLagConfig> configs = state.stateEntity.hitLagConfig;
        if (configs!=null && configs.Count>0)
        {
            for (int i = 0; i < configs.Count; i++)
            {
                HitLagConfig config=configs[i];
                if (normalizedTime>config.trigger && config.Type==0 && GetExecute(i)==false)
                {
                    SetExecute(i);
                    GameEvent.DoHitlag(config.frame, config.lerp);
                }
            }
        }
    }

    public override void OnEnd(PlayerState state)
    {
        base.OnEnd(state);
        ReSetAllExecute();
    }

    public override void OnAnimationEnd(PlayerState state)
    {
        base.OnAnimationEnd(state);
    }

    public override void ReLoop(PlayerState state)
    {
        base.ReLoop(state);
    }

    public override void ReStart(PlayerState state)
    {
        base.ReStart(state);
    }

    public override void OnDisable(PlayerState state)
    {
        base.OnDisable(state);
    }

    public void DoHitlag_OnAtk(float normalizedTime, PlayerState state)
    {
        List<HitLagConfig> configs = state.stateEntity.hitLagConfig;
        if (configs!=null && configs.Count>0)
        {
            for (int i = 0; i < configs.Count; i++)
            {
                HitLagConfig config=configs[i];
                if (normalizedTime>config.trigger && config.Type==1 && GetExecute(i)==false && normalizedTime<config.trigger2)
                {
                    SetExecute(i);
                    GameEvent.DoHitlag(config.frame, config.lerp);
                }
            }
        }
    }
}
