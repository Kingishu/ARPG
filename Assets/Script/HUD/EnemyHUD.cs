using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    public Image hp_top;
    public Image hp_middle;
    public float middle_speed = 1;//中间差值的速度
    public float hp = -1;//目标当前的血量,是百分比
    public Transform target;
    public Text name_text;//修改这个名字
    public Vector3 offset = new Vector3(0, 1.8f, 0);
    //循环判断
    private bool do_Update_Hp;
    private void Awake()
    {
        do_Update_Hp=false;
        hp_top=transform.Find("HealthBar/HP_Top").GetComponent<Image>();
        hp_middle=transform.Find("HealthBar/HP_Middle").GetComponent<Image>();
        name_text=transform.Find("Name").GetComponent<Text>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DOUpdateHP();
        if (hp<=0)
        {
            if (hp_top.fillAmount==0 && hp_middle.fillAmount==0)
            {
                this.gameObject.SetActive(false);
                ResourcesManager.Instance.DestroyEnemyHUD(this);
            }
        }
        if (target!=null)
        {
            this.transform.position=target.position+offset;
            this.transform.rotation = GameDefine.camera.rotation;
        }
    }

    public void UpdateHP(float v,Transform _target,string _name)
    {
        do_Update_Hp=true;
        hp = v;
        target=_target;
        name_text.text=_name;
        if (v>0)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
    private void DOUpdateHP()
    {
        //如果是默认值就退出,证明没有血量被更新
        if (hp==-1 || do_Update_Hp==false)
        {
            return;
        }
        //血条的血逼实际的多,让血条变小
        if (hp_top.fillAmount > hp)
        {
            //让血条变小
            hp_top.SetFillAmount(hp, middle_speed);
        }
        else if(hp_top.fillAmount < hp)
        {
            //填充血条
            hp_top.SetFillAmount(hp, middle_speed/2);
        }
        //注意事项:在血条较小的时候,top先减小,中间层在减小
        //血条变大的时候,中间层先变大,top在变大
        //血条的血逼实际的多,让血条变小
        if (hp_middle.fillAmount > hp)
        {
            //让中间血条变小
            hp_middle.SetFillAmount( hp, middle_speed/2);
        }
        else if(hp_middle.fillAmount < hp)
        {
            //填充中间血条
            hp_middle.SetFillAmount(hp, middle_speed);
        }

        if (hp_top.fillAmount==hp && hp_middle.fillAmount==hp)
        {
            do_Update_Hp=false;
        }
    }

   
}
