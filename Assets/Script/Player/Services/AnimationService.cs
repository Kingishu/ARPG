using UnityEngine;

public class AnimationService : FSMServiceBase
{
    public string now_play_id;
    public float normalizedTime;
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
        call_End=false;
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

    public bool call_End;
    public override void OnUpdate(float _normalizedTime, PlayerState state)
    {
        base.OnUpdate(normalizedTime, state);
        if (!string.IsNullOrEmpty(now_play_id))
        {
            var info = player.animator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime>1)
            {
                if (call_End == false)
                {
                    normalizedTime = 1;
                    player.ServicesOnAnimationEnd();
                    call_End = true;
                }
                else
                {
                    if (call_End==true)
                    {
                        call_End=false;
                    }

                    normalizedTime = normalizedTime % 1;
                }
            }
            else
            {
                normalizedTime = 0;
            }
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
