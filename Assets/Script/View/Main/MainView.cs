using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MainView : View
{
    //血条组件
    private Image player_hp_top;
    private Image player_hp_middle;
    private Image player_mp_top;
    private Image player_mp_middle;
    private Image boss_hp_top;
    private Image boss_hp_middle;
    //血条的百分比
    private float player_hp=-1;
    private float player_mp=-1;
    private float boss_hp=-1;
    //血条的速度
    private float middleSpeed=1;
    //BOSS血条跟物体
    private Transform boss_Root;
    private Text boss_text;
    public override void Awake()
    {
        base.Awake();
        player_hp_top = GetComponent<Image>("PlayerHP/PlayerHP_Top");
        player_hp_middle = GetComponent<Image>("PlayerHP/PlayerHP_Middle");
        player_mp_top = GetComponent<Image>("PlayerMP/PlayerMP_Top");
        player_mp_middle = GetComponent<Image>("PlayerMP/PlayerMP_Middle");
        boss_hp_top = GetComponent<Image>("BOSS/HP_Base/HP_Top");
        boss_hp_middle = GetComponent<Image>("BOSS/HP_Base/HP_Middle");

        boss_Root = transform.Find("BOSS");
        boss_text = GetComponent<Text>("BOSS/Name/Name_Text");

        //CD相关
        mask_q = GetComponent<Image>("Quik/Skill_Q/Item/mask");
        mask_e = GetComponent<Image>("Quik/Skill_E/Item/mask");
        mask_r = GetComponent<Image>("Quik/Skill_R/Item/mask");
        mask_t = GetComponent<Image>("Quik/Skill_T/Item/mask");
        countdown_q = GetComponent<Text>("Quik/Skill_Q/Item/countdown");
        countdown_e = GetComponent<Text>("Quik/Skill_E/Item/countdown");
        countdown_r = GetComponent<Text>("Quik/Skill_R/Item/countdown");
        countdown_t = GetComponent<Text>("Quik/Skill_T/Item/countdown");

        skill_CD_Tips = transform.Find("Skill_CD_Tips").gameObject;
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        DoUpdateBossHP();
        DoUpdatePlayerHP();
        DoUpdatePlayerMP();
        //CD相关
        DOUpdatePlayerCD();
        StopCD_Tips();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    //对外暴露的方法
    public void UpdatePlayerHP(float v)
    {
        player_hp = v;
    }

    public void UpdatePlayerMP(float v)
    {
        player_mp = v;
    }

    public void UpdateBossHP(float v)
    {
        boss_hp = v;   
    }
    //更新血条的内部逻辑
    private void DoUpdatePlayerHP()
    {  
        //浮点数精度不够,不能用==做比较
        if (player_hp==-1)
        {
            return;
        }
        //先更新顶层血条,top的速度比较快
        if (player_hp_top.fillAmount>player_hp)
        {
            //血条多,需要减少
            player_hp_top.SetFillAmount(player_hp,middleSpeed*2);
        }
        else if (player_hp_top.fillAmount<player_hp)
        {
            //血条少,要增多
            player_hp_top.SetFillAmount(player_hp,middleSpeed);
        }

        if (player_hp_middle.fillAmount>player_hp)
        {
            //血条多,要减少
            player_hp_middle.SetFillAmount(player_hp,middleSpeed);
        }
        else if (player_hp_middle.fillAmount<player_hp)
        {
            //血条多,要增加
            player_hp_middle.SetFillAmount(player_hp,middleSpeed*2);
        }
    }
    private void DoUpdatePlayerMP()
    {  
        //浮点数精度不够,不能用==做比较
        if (player_mp==-1)
        {
            return;
        }
        //先更新顶层血条,top的速度比较快
        if (player_mp_top.fillAmount>player_mp)
        {
            //血条多,需要减少
            player_mp_top.SetFillAmount(player_mp,middleSpeed*2);
        }
        else if (player_mp_top.fillAmount<player_mp)
        {
            //血条少,要增多
            player_mp_top.SetFillAmount(player_mp,middleSpeed);
        }

        if (player_mp_middle.fillAmount>player_mp)
        {
            //血条多,要减少
            player_mp_middle.SetFillAmount(player_mp,middleSpeed);
        }
        else if (player_mp_middle.fillAmount<player_mp)
        {
            //血条多,要增加
            player_mp_middle.SetFillAmount(player_mp,middleSpeed*2);
        }
    }
    private void DoUpdateBossHP()
    {  
        //浮点数精度不够,不能用==做比较
        if (boss_hp==-1)
        {
            return;
        }
        //先更新顶层血条,top的速度比较快
        if (player_hp_top.fillAmount>boss_hp)
        {
            //血条多,需要减少
            boss_hp_top.SetFillAmount(boss_hp,middleSpeed*2);
        }
        else if (player_hp_top.fillAmount<boss_hp)
        {
            //血条少,要增多
            boss_hp_top.SetFillAmount(boss_hp,middleSpeed);
        }

        if (player_hp_middle.fillAmount>boss_hp)
        {
            //血条多,要减少
            boss_hp_middle.SetFillAmount(boss_hp,middleSpeed);
        }
        else if (player_hp_middle.fillAmount<boss_hp)
        {
            //血条多,要增加
            boss_hp_middle.SetFillAmount(boss_hp,middleSpeed*2);
        }
    }

    public void EnableBossHP(bool act,string name)
    {
        Debug.Log($"我调用了显示方法,根节点是{boss_Root}名字是{boss_text}");
        boss_Root.gameObject.SetActive(act);
        boss_text.text = name;
    }
    //缓存四个技能的CD
    public float q_cd;
    public float e_cd;
    public float r_cd;
    public float t_cd;
    //缓存四个技能的释放时间
    public float q_cd_begin;
    public float e_cd_begin;
    public float r_cd_begin;
    public float t_cd_begin;
    //各个技能的Image
    private Image mask_q;
    private Image mask_e;
    private Image mask_r;
    private Image mask_t;
    //各个技能的Text
    private Text countdown_q;
    private Text countdown_e;
    private Text countdown_r;
    private Text countdown_t;
    //CD提示不足
    private GameObject skill_CD_Tips;
    /// <summary>
    /// 处理技能CD
    /// </summary>
    /// <param name="cd">传入cd</param>
    /// <param name="begin">传入技能开始的时间</param>
    /// <param name="mask">传入遮罩</param>
    /// <param name="countDown">传入CD显示的TEXT</param>
    public void UpdateSkillCD(ref float cd,float begin,Image mask,Text countDown)
    {
        //处理mask显示
        if (cd!=0)
        {
            if (mask.fillAmount!=0)
            {
                var result=mask.SetFillAmount(0, 1 / 5f);//speed就是一秒减去多少.我们传入0.2f,意味着5秒内,可以从当前值减到0
                if (result)
                {
                    cd = 0;
                    mask.gameObject.SetActive(false);
                    countDown.gameObject.SetActive(false);
                    return;
                    //播放CD好了的音频
                }
            }
            //处理countDown显示
            countDown.text = Math.Ceiling(cd - (GameTime.time - begin)).ToString();
        }
    }
    //对外暴露设置cd的接口
    public void SetSkillCD(int type,float cd)
    {
        switch(type)
        {
            //1234代表qert技能
            case 1:
                q_cd= cd;
                q_cd_begin = GameTime.time;
                mask_q.gameObject.SetActive(true);
                mask_q.fillAmount = 1;
                countdown_q.gameObject.SetActive(true);
                break;
            case 2:
                e_cd= cd;
                e_cd_begin = GameTime.time;
                mask_e.gameObject.SetActive(true);
                mask_e.fillAmount = 1;
                countdown_e.gameObject.SetActive(true);
                break;
            case 3:
                r_cd= cd;
                r_cd_begin = GameTime.time;
                mask_r.gameObject.SetActive(true);
                mask_r.fillAmount = 1;
                countdown_r.gameObject.SetActive(true);
                break;
            case 4:
                t_cd= cd;
                t_cd_begin = GameTime.time;
                mask_t.gameObject.SetActive(true);
                mask_t.fillAmount = 1;
                countdown_t.gameObject.SetActive(true);
                break;

        }
    }
    public void DOUpdatePlayerCD()
    {
        UpdateSkillCD(ref q_cd, q_cd_begin, mask_q, countdown_q);
        UpdateSkillCD(ref e_cd, e_cd_begin, mask_e, countdown_e);
        UpdateSkillCD(ref r_cd, r_cd_begin, mask_r, countdown_r);
        UpdateSkillCD(ref t_cd, t_cd_begin, mask_t, countdown_t);
    }
    private float cd_tips_begin = 0;
    public void OpenCD_Tips()
    {
        cd_tips_begin = GameTime.time;
        skill_CD_Tips.SetActive(true);
        //播放技能正在CD的音频
    }

    public void StopCD_Tips()
    {
        if (cd_tips_begin == 0)
        {
            return;
        }
        if (GameTime.time - cd_tips_begin > 2f)
        {
            cd_tips_begin = 0;
            skill_CD_Tips.SetActive(false);
        }
    }
}
