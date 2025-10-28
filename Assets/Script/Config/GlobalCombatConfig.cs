using UnityEngine;
[CreateAssetMenu(menuName = "配置/全局战斗配置")]
public class GlobalCombatConfig:ScriptableObject
{
    //顿帧
    [Header("顿帧配置")]
    public HitLagConfig hitlagConfig;
    //镜像模糊
    [Header("镜像模糊")]
    public RadialBlurConfig radialBlurConfig;
    //格挡特效
    [Header("格挡特效")]
    public string block_effect;
    //格挡音效
    [Header("格挡音效")]
    public string block_audio;
    //攻击成功特效
    [Header("攻击成功特效")]
    public string hit_enemy_effect;
    //攻击成功音效
    [Header("攻击成功音效")]
    public string hit_enemy_audio;
}
