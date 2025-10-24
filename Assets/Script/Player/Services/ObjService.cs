using System.Collections.Generic;
using UnityEngine;

public class ObjService:FSMServiceBase
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
        List<Obj_State> states = state.stateEntity.ObjStates;
        if (states!=null && states.Count>0)
        {
            for (int i = 0; i < states.Count; i++)
            {
                Obj_State objState = states[i];
                if (normalizedTime>=objState.trigger && GetExecute(i)==false)
                {
                    SetExecute(i);
                    foreach (var item in objState.object_ID)
                    {
                        GameObject obj = player.GetHangPoint(item);
                        if (obj!=null)
                        {
                            obj.SetActive(objState.act);
                        }
                    }
                }
            }
        }
    }

    public override void OnEnd(PlayerState state)
    {
        base.OnEnd(state);
        List<Obj_State> states = state.stateEntity.ObjStates;
        if (states!=null && states.Count>0)
        {
            for (int i = 0; i < states.Count; i++)
            {
                Obj_State objState = states[i];
                if (objState.force && GetExecute(i)==false)
                {
                    foreach (var item in objState.object_ID)
                    {
                        GameObject obj = player.GetHangPoint(item);
                        if (obj!=null)
                        {
                            obj.SetActive(objState.act);
                        }
                    }
                }
            }
        }
        ReSetAllExecute();
    }

    public override void OnAnimationEnd(PlayerState state)
    {
        base.OnAnimationEnd(state);
    }

    public override void ReLoop(PlayerState state)
    {
        base.ReLoop(state);
        objResetExcuted(state);
    }

    public override void ReStart(PlayerState state)
    {
        base.ReStart(state);
        objResetExcuted(state);
    }

    public override void OnDisable(PlayerState state)
    {
        base.OnDisable(state);
    }
    private void objResetExcuted(PlayerState state)
    {
        List<Obj_State> states = state.stateEntity.ObjStates;
        if (states!=null && states.Count>0)
        {
            for (int i = 0; i < states.Count; i++)
            {
                Obj_State objState = states[i];
                if (objState.loop)
                {
                    ReSetExecute(i);
                }
            }
        }
    }

}
