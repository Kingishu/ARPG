using Game.Config;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HitService:FSMServiceBase
{
    public override void Init(FSM fsm)
    {
        base.Init(fsm);
    }

    public override void OnBegin(PlayerState state)
    {
        base.OnBegin(state);
        ReSetAllExecute();
        lastEnd=Vector3.zero;
    }

    public override void OnUpdate(float normalizedTime, PlayerState state)
    {
        base.OnUpdate(normalizedTime, state);
        List<HitConfig> configs = state.stateEntity.hitConfig;
        if (configs!=null && configs.Count>0)
        {
            for (int i = 0; i < configs.Count; i++)
            {
                HitConfig config=configs[i];
                if (normalizedTime>config.trigger && normalizedTime<config.end && GetExecute(i)==false)
                {
                    DO(config,state);
                }
            }
        }
    }

    private Vector3 lastEnd;
    private void DO(HitConfig config, PlayerState state)
    {
        GameObject sword = player.GetHangPoint(config.begin);
        Transform begin= sword.transform;
        Vector3 end=begin.position+begin.forward*config.length;
        switch (config.type)
        {
            case 0:
                if (lastEnd==Vector3.zero)
                {
                    LineCast(begin.position,end,config,state);
                }
                else
                {
                    var crn_id = player.currentState.id;
                    //在两帧之间,进行5次补充
                    for (int i = 0; i < 10; i++)
                    {
                        Vector3 newEnd = Vector3.Lerp(lastEnd, end, i / 10f);
                        LineCast(begin.position,newEnd,config,state);
                        if (crn_id!= player.currentState.id)
                        {
                            return;
                        }
                    }
                }
                break;
            case 1:
                //盒子检测
                BoxCast(begin, config, state);
                break;
            default:
                break;
        }

        lastEnd = end;
    }

    private List<int> hit_target=new List<int>();
    public void LineCast(Vector3 start, Vector3 end,HitConfig config, PlayerState state)
    {
        if (Physics.Linecast(start,end,out var hitinfo,player.GetEnemyLayer(),QueryTriggerInteraction.Collide))
        {
            if (hitinfo.collider.CompareTag(GameDefine.WeaponTag))
            {
                OnBlock(hitinfo);
            }
            else
            {
                OnHit(config, state, hitinfo);
            }
        }
    }
    RaycastHit[] raycastHits = new RaycastHit[10];
    public bool BoxCast(Transform start,HitConfig config, PlayerState state)
    {
        int count=Physics.BoxCastNonAlloc(start.transform.position+start.transform.TransformDirection(config.center),config.size,start.forward,raycastHits,start.rotation,config.length,player.GetEnemyLayer(),QueryTriggerInteraction.Collide);
        if (count>0)
        {
            for (int i = 0; i < count; i++)
            {
                var crn_id=player.currentState.id;
                var hitinfo = raycastHits[i];
                if (hitinfo.collider.CompareTag(GameDefine.WeaponTag))
                {
                    OnBlock(hitinfo);
                }
                else
                {
                    OnHit(config, state, hitinfo);
                }
                if (crn_id!=player.currentState.id)
                {
                    break;
                }
            }
            return true;
        }
        return false;
    }
    
    
    private void OnHit(HitConfig config, PlayerState state, RaycastHit hitinfo)
    {
        //这个fsm就是敌人
        FSM fsm=hitinfo.collider.GetComponentInParent<FSM>();
        //计算敌人和玩家的前后关系
        var direction = fsm.transform.ForwardOrBack(player._transform.position);
        if (direction >0)
        {
            direction = 1;
        }
        else if (direction <0)
        {
            direction = -1;
        }
        else
        {
            direction = 0;
        }
                
        if (hit_target.Contains(fsm.instance_ID) == false)
        {
            hit_target.Add(fsm.instance_ID);
            //生成特效
            GameObject hitEffect = ResourcesManager.Instance.CreatEffext(config.hitObj);
            hitEffect.SetActive(true);
            //设置特效位置
            if (hitEffect!=null)
            {
                hitEffect.transform.position = hitinfo.point;
                hitEffect.transform.forward=hitinfo.normal;
            }
            //计算血量
            int damage = AttHelper.Instance.Damage(this.player, state, fsm);
            fsm.UpdateHP_OnHit(damage);
            //通知敌人进入挨打状态
            if (fsm.att_crn.hp>0)
            {
                if (state.skillEntity.add_fly!=null)
                {
                    //击飞流程
                    fsm.OnBash((int)direction, player,state.skillEntity.add_fly,hitinfo.point);
                }
                else
                {
                    fsm.OnHit((int)direction,player);
                }
            }
            else
            {
                fsm.OnDead((int)direction,player);
            }
            //顿帧
            player.Attack_Hitlag(state);
            //生成音效
            AudioManager.Instance.Play("AudioClip/atk_enemy",hitinfo.point);
        }
    }

    private void OnBlock(RaycastHit hitinfo)
    {
        //格挡方
        FSM fsm=hitinfo.collider.GetComponentInParent<FSM>();
        if (fsm!=null && hit_target.Contains(fsm.instance_ID)==false)
        {
            hit_target.Add(fsm.instance_ID);
            //生成特效
            GameObject blockEffect =
                ResourcesManager.Instance.CreatEffext(CombatConfig.Instance.Config.block_effect);
            if (blockEffect != null)
            {
                blockEffect.transform.position = hitinfo.point;
                blockEffect.transform.forward=hitinfo.normal;
            }
            //生成音效
            AudioManager.Instance.Play(CombatConfig.Instance.Config.block_audio, hitinfo.point);
            //格挡方进入格挡成功
            fsm.OnBlockSucces(player);
            //攻击方进入反弹
            player.BeBlock(fsm);
        }
    }


    public override void OnEnd(PlayerState state)
    {
        base.OnEnd(state);
        ReSetAllExecute();
        hit_target.Clear();
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
}
