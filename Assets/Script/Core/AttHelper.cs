using Game.Config;
using UnityEngine;

public class AttHelper
{
    #region 单例模式
    private static AttHelper instance=new AttHelper();
    public static AttHelper Instance => instance;
    private AttHelper(){}
    #endregion

    public UnitAttEntity Creat(int id)
    {
        var a=UnitAttData.Get(id);
        if (a==null)
        {
            return null;
        }
        var b=new UnitAttEntity();
        b.id = a.id;
        b.hp = a.hp;
        b.phy_atk=a.phy_atk;
        b.magic_atk=a.magic_atk;
        b.phy_def=a.phy_def;
        b.magic_def=a.magic_def;
        b.critical_hit_rate=a.critical_hit_rate;
        b.critical_hit_multiple=a.critical_hit_multiple;
        b.skill_speed=a.skill_speed;
        return b;
    }
    public UnitAttEntity Creat(UnitAttEntity a)
    {
        if (a==null)
        {
            return null;
        }
        var b=new UnitAttEntity();
        b.id = a.id;
        b.hp = a.hp;
        b.phy_atk=a.phy_atk;
        b.magic_atk=a.magic_atk;
        b.phy_def=a.phy_def;
        b.magic_def=a.magic_def;
        b.critical_hit_rate=a.critical_hit_rate;
        b.critical_hit_multiple=a.critical_hit_multiple;
        b.skill_speed=a.skill_speed;
        return b;
    }

    public int Damage(FSM atk,PlayerState state,FSM def)
    {
        int damage = 0;
        bool probability=UnityEngine.Random.Range(0,101) <=atk.att_crn.critical_hit_rate;
        //暴击
        if (probability)
        {
            damage = (int)((atk.att_crn.phy_atk - def.att_crn.phy_def + state.skillEntity.phy_damage)*atk.att_crn.critical_hit_multiple);
        }
        //没暴击
        else
        {
            damage = (int)(atk.att_crn.phy_atk - def.att_crn.phy_def + state.skillEntity.phy_damage);
        }
        return damage;
    }
}
