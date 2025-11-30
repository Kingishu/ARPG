using Game.Config;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameEvent
{
    //执行顿帧
    public static Action<int, bool> DoHitlag;
    //执行镜像模糊
    public static Action<RadialBlurConfig> DoRadialBlur;
    //当玩家攻击的时候,需要执行的配置
    public static Action<FSM,SkillEntity> OnPlayerAtk;
    //UI重新排序
    public static Action ResetSortOrder;
    //UI事件  EventSystem
    public static EventSystem EventSystem;
}
