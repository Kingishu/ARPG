using UnityEngine;

public class AnimationService : FSMServiceBase
{
    public string now_play_id;
    public float normalizedTime;
    //用一个整数,来记录我们处理过的圈数
    public int lastProcessedLoop;
    private void Play(PlayerState state)
    {
        normalizedTime = 0;
        now_play_id=state.stateEntity.anm_name;
        player.animator.Play(now_play_id);
        player.animator.Update(0);
    }

    public override void Init(FSM fsm)
    {
        base.Init(fsm);

    }

    public override void OnAnimationEnd(PlayerState state)
    {
        base.OnAnimationEnd(state);
    }

    public override void OnBegin(PlayerState state)
    {
        base.OnBegin(state);
        lastProcessedLoop = 0;
        Play(state);
    }

    public override void OnDisable(PlayerState state)
    {
        base.OnDisable(state);
    }

    public override void OnEnd(PlayerState state)
    {
        base.OnEnd(state);
    }

    
    public override void OnUpdate(float _normalizedTime, PlayerState state)
    {
        base.OnUpdate(normalizedTime, state);
        if (!string.IsNullOrEmpty(now_play_id))
        {
            var info = player.animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName(now_play_id))
            {
                normalizedTime=info.normalizedTime;
                if (info.normalizedTime>1)
                {
                    //记录当前的圈数
                    int currentLoop = Mathf.FloorToInt(normalizedTime);
                    normalizedTime = normalizedTime % 1;
                    //如果我们的normalizedTime>1,证明已经进行了一次完整的动作,我们要检查是否执行过这个动作的结束事件
                    //如果当前的圈数大于已经处理过的圈数,证明有一个结束事件没有处理
                    if (currentLoop>lastProcessedLoop)
                    {
                        player.ServicesOnAnimationEnd();
                        Debug.Log("我执行了一次结束事件");
                        lastProcessedLoop=currentLoop;
                    }
                }
            }
            else
            {
                normalizedTime = 0;
            }
            Debug.Log(normalizedTime);
        }
       
    }


    public override void ReLoop(PlayerState state)
    {
        base.ReLoop(state);
    }

    public override void ReStart(PlayerState state)
    {
        base.ReStart(state);
    }
}
