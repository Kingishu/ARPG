using Pathfinding;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NavigationService : FSMServiceBase
{
    public override void Init(FSM fsm)
    {
        base.Init(fsm);
    }

    public override void OnBegin(PlayerState state)
    {
        base.OnBegin(state);
    }

    public override void OnUpdate(float normalizedTime, PlayerState state)
    {
        base.OnUpdate(normalizedTime, state);
        OnMove();
    }

    private List<Vector3> _path;
    private int currentIndex;
    public int state; //状态机 0空闲  1正在搜索  2返回搜索结果
    private Vector3 _point; //目的地
    public Vector3 _pathLast;
    public Action _success;
    //外部调用寻路的核心接口
    public void Move(Vector3 position,Action success)
    {
        if (state==0||(state==1 && position!=_point))
        {
            _point=position;
            this._success=success;
            state = 1;
            var v = NavHelper.Instance.GetWalkPosition(position);
            player._seeker.StartPath(player._transform.position, v, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (state==1)
        {
            state = 2;
            if (p.error == false)
            {
                _path=p.vectorPath;
                currentIndex = 0;
                _pathLast=_path[_path.Count-1];
                this.player._transform.LookTarget(_path[currentIndex]);
                _success?.Invoke();
            }
            else
            {
                Stop();
            }
        }
    }

    public void Stop()
    {
        _path=null;
        state = 0;
    }
    public void OnMove()
    {
        if (_path==null)
        {
            return;
        }

        if (currentIndex>_path.Count-1)
        {
            Stop();
            return;
        }
        else
        {
            var nextPosition=_path[currentIndex];
            var dir=(nextPosition-player._transform.position).normalized;
            dir.y = 0;
            player.Move(dir*player.GetPlayerSpeed(),false);
            if (Vector3.Distance(player._transform.position,nextPosition)<0.5f)
            {
                if (currentIndex>=_path.Count-1)
                {
                    Stop();
                }
                else
                {
                    currentIndex++;
                    player._transform.LookTarget(_path[currentIndex]);
                }
            }
        }
    }
    public override void OnEnd(PlayerState state)
    {
        base.OnEnd(state);
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

    public bool IsEnd()
    {
        return _path==null;
    }
}
