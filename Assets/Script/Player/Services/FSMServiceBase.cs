using System.Collections.Generic;
using UnityEngine;

public class FSMServiceBase
{
    public FSM player;
    public virtual void Init(FSM fsm)
    {
        player = fsm;
    }
    public virtual void OnBegin(PlayerState state) { }
    public virtual void OnUpdate(float normalizedTime, PlayerState state) { }
    public virtual void OnEnd(PlayerState state) { }
    public virtual void OnAnimationEnd(PlayerState state) { }
    public virtual void ReLoop(PlayerState state) { }
    public virtual void ReStart(PlayerState state) { }
    public virtual void OnDisable(PlayerState state) { }

    Dictionary<int, bool> executed = new Dictionary<int, bool>();
    public void SetExecute(int index)
    {
        executed[index] = true;
    }
    public void ReSetExecute(int index)
    {
        executed[index] = false;
    }
    public void ReSetAllExecute()
    {
        if (executed.Count > 0)
        {
            executed.Clear();
        }
    }
    public bool GetExecute(int index)
    {
        if (executed.TryGetValue(index, out var temp))
        {
            return temp;
        }
        return false;
    }


}
