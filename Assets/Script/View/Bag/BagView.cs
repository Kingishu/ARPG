using Game.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class BagView : View
{
    public Transform Content;
    //拿到物品 材料 装备的详情页面
    public Transform Prop;
    public Transform Equip;
    public Transform Material;
    //Prop物品
    Text prop_name;
    Text prop_info;
    Text prop_act;
    //Prop中的Att信息
    Text prop_att_info1;
    Text prop_att_info2;
    Text prop_att_info3;

    //材料UI需要的东西
    Text material_name;
    Text material_info;
    Text material_act;

    //装备
    Text equip_name;
    Text equip_info;
    Text equip_act;
    //装备Att信息
    Text equip_att_info1;
    Text equip_att_info2;
    Text equip_att_info3;
    Text equip_att_info4;
    Text equip_att_info5;
    Text equip_att_info6;
    //背包选中的图片
    Image select;
    public override void Awake()
    {
        base.Awake();
        Content = transform.Find("Bag/Scroll View/Viewport/Content");
        //为四个显示按钮添加注册时间,面板是持久化存在的,不需要取消订阅,不用放在Onenable里面,为了防止内存泄漏,不要用匿名函
        //数,我们麻烦一下多写几个方法
        GetComponent<Button>("Bag/Menu/All").onClick.AddListener(ShowAll);
        GetComponent<Button>("Bag/Menu/Drug").onClick.AddListener(ShowDrug);
        GetComponent<Button>("Bag/Menu/Equip").onClick.AddListener(ShowEquip);
        GetComponent<Button>("Bag/Menu/Material").onClick.AddListener(ShowMaterial);
        GetComponent<Button>("Bag/Sort").onClick.AddListener(SortByType);
        //获取物品 装备 材料的UI物品
        Prop = transform.Find("Prop");
        Equip = transform.Find("Equip");
        Material = transform.Find("Material");
        //获取Prop身上的,我们需要的信息
        prop_name= transform.Find("Prop/Name").GetComponent<Text>();
        prop_info = transform.Find("Prop/Info").GetComponent<Text>();
        prop_act = transform.Find("Prop/Act").GetComponent<Text>();
        //获取ATT info
        prop_att_info1 = transform.Find("Prop/Att/Info_1").GetComponent<Text>();
        prop_att_info2 = transform.Find("Prop/Att/Info_2").GetComponent<Text>();
        prop_att_info3 = transform.Find("Prop/Att/Info_3").GetComponent<Text>();
        //材料相关
        material_name= transform.Find("Material/Name").GetComponent<Text>();
        material_info = transform.Find("Material/Info").GetComponent<Text>();
        material_act = transform.Find("Material/Act").GetComponent<Text>();
        //装备
        equip_name = transform.Find("Equip/Name").GetComponent<Text>();
        equip_info = transform.Find("Equip/Info").GetComponent<Text>();
        equip_act = transform.Find("Equip/Act").GetComponent<Text>();
        //att info
        equip_att_info1 = transform.Find("Equip/Att/Info1").GetComponent<Text>();
        equip_att_info2 = transform.Find("Equip/Att/Info2").GetComponent<Text>();
        equip_att_info3 = transform.Find("Equip/Att/Info3").GetComponent<Text>();
        equip_att_info4 = transform.Find("Equip/Att/Info4").GetComponent<Text>();
        equip_att_info5 = transform.Find("Equip/Att/Info5").GetComponent<Text>();
        equip_att_info6 = transform.Find("Equip/Att/Info6").GetComponent<Text>();
        //背包选中的图片
        select = GetComponent<Image>("Bag/Select");
        //为背包当中所有的物品,他们上面的Button,添加选中事件
        for (int i = 0; i < Content.childCount; i++)
        {
            Content.GetChild(i).GetComponent<Button>().onClick.AddListener(GrildOnClick);
        }

    }
    public void ShowDrug()
    {
        Show(0);
    }
    public void ShowEquip()
    {
        Show(1);
    }
    public void ShowMaterial()
    {
        Show(2);
    }
    public void ShowAll()
    {
        Show(-1);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    Dictionary<int,GameObject> bag_item=new Dictionary<int,GameObject>();
    //type=-1 是全部 0是物品 1装备 2材料
    int currentShowType;
    public void Show(int Type)
   {
        Clear();
        //直接创建好,我们下面根据类型匹配去显示对应的内容
        currentShowType = Type;
        var data = BagData.Instance.dct;
        foreach (var item in data)
        {
            var obj = CreatePropItem(item.Value);
            //设置位置,放到父物体下面
            string parent = GameDefine.content + item.Key;
            obj.transform.SetParent(transform.Find(parent), false);
            //缓存背包中的物品
            bag_item[item.Value.grild_id] = obj;
        }
        if (Type==-1)
        {
            for (global::System.Int32 i = 0; i < Content.childCount; i++)
            {
                Content.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            for (global::System.Int32 i = 0; i < Content.childCount; i++)
            {
                var obj=Content.GetChild(i);
                var grildInfo=BagData.Instance.Get(i);
                if (grildInfo!=null)
                {
                    obj.gameObject.SetActive(grildInfo.entity.type == Type);
                }
                else
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }
    
    public void SortByType()
    {
        BagData.Instance.SortByType();
        Show(currentShowType);
    }

    public GameObject CreatePropItem(BagEntity bagEntity)
    {
        //逐一创建物品
        var obj = ResourcesManager.Instance.Create_BagItem(GameDefine.itemPath);
        //更新icon
        var icon = obj.transform.Find("icon").GetComponent<Image>();
        icon.sprite = ResourcesManager.Instance.LoadIcon(bagEntity.entity);
        //更新数量
        var text = obj.transform.Find("key/key_text").GetComponent<Text>();
        text.text = bagEntity.count.ToString();
        //设置一下名称
        obj.gameObject.name = bagEntity.id.ToString();
        //返回出去
        return obj;
    }
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

    }
    /// <summary>
    /// 清除显示在背包中的物品
    /// </summary>
    public void Clear()
    {
        foreach (var item in bag_item.Values)
        {
            ResourcesManager.Instance.Destroy_BagItem(item);
        }
        bag_item.Clear();
    }
    public void ShowPropInfo(int grild_id,Vector3 point)
    {
        //从背包中获取到物品
        var bagEntity = BagData.Instance.Get(grild_id);
        if (bagEntity != null)
        {
            Prop.gameObject.SetActive(true);
            Material.gameObject.SetActive(false);
            Equip.gameObject.SetActive(false);
            //更新物品名称
            SetText(prop_name, bagEntity.entity.name);
            //物品介绍
            SetText(prop_info, bagEntity.entity.info);
            //计算一下level信息
            string level = bagEntity.entity.level > 0 ? "不限制等" : bagEntity.entity.level.ToString();
            //物品使用信息,等级限制级
            SetText(prop_act, $"类型:物品\n使用等级:{bagEntity.entity.level}级");
            //设置Att信息,如果装备信息为空,我们就不设置
            SetPropAttInfo(bagEntity);
            //设置中心点位置
            SetInfoPivot(Prop, point);
            //设置位置信息
            Prop.transform.position = point;
        }
    }
    public void ShowEquipInfo(int grild_id, Vector3 point)
    {
        var bagEntity = BagData.Instance.Get(grild_id);
        if (bagEntity != null)
        {
            Prop.gameObject.SetActive(false);
            Material.gameObject.SetActive(false);
            Equip.gameObject.SetActive(true);

            //更新物品名称
            SetText(equip_name, bagEntity.entity.name);
            //物品介绍
            SetText(equip_info, bagEntity.entity.info);
            //设置加成信息
            SetEquipAttInfo(bagEntity);
            //计算一下level信息
            string level = bagEntity.entity.level > 0 ? "不限制等" : bagEntity.entity.level.ToString();
            //物品使用信息,等级限制级
            SetText(equip_act, $"类型:装备/{PartNameData.Get(bagEntity.entity.part).name}\n使用等级:{bagEntity.entity.level}级");
            //设置中心点位置
            SetInfoPivot(Equip, point);
            //设置位置信息
            Equip.transform.position = point;
        }
    }
    public void ShowMaterialInfo(int grild_id, Vector3 point)
    {
        //从背包中获取到物品
        var bagEntity = BagData.Instance.Get(grild_id);
        if (bagEntity != null)
        {
            Prop.gameObject.SetActive(false);
            Material.gameObject.SetActive(true);
            Equip.gameObject.SetActive(false);
            //更新物品名称
            SetText(material_name, bagEntity.entity.name);
            //物品介绍
            SetText(material_info, bagEntity.entity.info);
            //计算一下level信息
            string level = bagEntity.entity.level > 0 ? "不限制等" : bagEntity.entity.level.ToString();
            //物品使用信息,等级限制级
            SetText(material_act, $"类型:材料\n使用等级:{bagEntity.entity.level}级");
            //设置中心点位置
            SetInfoPivot(Material, point);
            //设置位置信息
            Material.transform.position = point;
        }
    }

    internal void CloseInfo()
    {
        Prop.gameObject.SetActive(false);
        Material.gameObject.SetActive(false);
        Equip.gameObject.SetActive(false);
    }
    public string GetAttText(int type,int value,int calType)
    {
        // 属性+数字+百分比
        string text = "";
        if (value>0)
        {
            text = AttNameData.Get(type).name + ":+" + (calType == 0 ? value : value + "%");
        }
        else
        {
            text = AttNameData.Get(type).name + ":" + (calType == 0 ? value : value + "%");
        }
        return text;
    }
    private void SetPropAttInfo(BagEntity bagEntity)
    {
        //查看recover1是否为空,如果是空就关闭这个词条,不为空就设置好他的信息.
        if (bagEntity.entity.recover1 != null)
        {
            prop_att_info1.gameObject.SetActive(true);
            SetText(prop_att_info1, GetAttText(bagEntity.entity.recover1[0], bagEntity.entity.recover1[1], bagEntity.entity.recover1[2]));
        }
        else
        {
            prop_att_info1.gameObject.SetActive(false);
        }


        if (bagEntity.entity.recover2 != null)
        {
            prop_att_info2.gameObject.SetActive(true);
            SetText(prop_att_info2, GetAttText(bagEntity.entity.recover2[0], bagEntity.entity.recover2[1], bagEntity.entity.recover2[2]));
        }
        else
        {
            prop_att_info2.gameObject.SetActive(false);
        }


        if (bagEntity.entity.recover3 != null)
        {
            prop_att_info3.gameObject.SetActive(true);
            SetText(prop_att_info3, GetAttText(bagEntity.entity.recover3[0], bagEntity.entity.recover3[1], bagEntity.entity.recover3[2]));
        }
        else
        {
            prop_att_info3.gameObject.SetActive(false);
        }
    }
    private void SetEquipAttInfo(BagEntity bagEntity)
    {
        //1
        if (bagEntity.entity.att1 != null)
        {
            equip_att_info1.gameObject.SetActive(true);
            SetText(equip_att_info1, GetAttText(bagEntity.entity.att1[0], bagEntity.entity.att1[1], bagEntity.entity.att1[2]));
        }
        else
        {
            equip_att_info1.gameObject.SetActive(false);
        }
        //2
        if (bagEntity.entity.att2 != null)
        {
            equip_att_info2.gameObject.SetActive(true);
            SetText(equip_att_info2, GetAttText(bagEntity.entity.att2[0], bagEntity.entity.att2[1], bagEntity.entity.att2[2]));
        }
        else
        {
            equip_att_info2.gameObject.SetActive(false);
        }
        //3
        if (bagEntity.entity.att3 != null)
        {
            equip_att_info3.gameObject.SetActive(true);
            SetText(equip_att_info3, GetAttText(bagEntity.entity.att3[0], bagEntity.entity.att3[1], bagEntity.entity.att3[2]));
        }
        else
        {
            equip_att_info3.gameObject.SetActive(false);
        }
        //4
        if (bagEntity.entity.att4 != null)
        {
            equip_att_info4.gameObject.SetActive(true);
            SetText(equip_att_info4, GetAttText(bagEntity.entity.att4[0], bagEntity.entity.att4[1], bagEntity.entity.att4[2]));
        }
        else
        {
            equip_att_info4.gameObject.SetActive(false);
        }
    }
    public void SetInfoPivot(Transform info, Vector3 point)
    {
        //设置位置信息,由于位置信息可能会造成信息面被遮挡,我们需要移动他的位置,我想的方法是设置他的锚点,
        //经过我们的计算,以(1600,520)这个点建系,左上角的内容,设置轴心点在左上角,右上角的内容设置轴心点右上角
        //计算x大于1600还是小于1600,然后计算y小于520还是大于520.
        //这里可能会有问题,就是如果屏幕分辨率修改的情况下,我们计算出来的1600 520是不精准的,后续可以防止一个空的Gameobject作为基准点.通过计算他的坐标来得出象限.
        bool bigX = point.x > 1600;
        bool bigY = point.y > 520;
        RectTransform rt = info as RectTransform;
        if (bigX && bigY)//该物品在基准点的右上角,也就是第一象限,我们设置Prop的轴心点位置在右上角
        {
            rt.pivot = new Vector2(1, 1);
        }
        else if (bigX && !bigY) //右下角
        {
            rt.pivot = new Vector2(1, 0);
        }
        else if (!bigX && !bigY) //左下角
        {
            rt.pivot = new Vector2(0, 0);
        }
        else if (!bigX && bigY) //左上角
        {
            rt.pivot = new Vector2(0, 1);
        }
    }

    private void GrildOnClick()
    {
        //如果这个背包格子有`物品我们才这样做.
        if (GameEvent.EventSystem.currentSelectedGameObject.transform.childCount>0)
        {
            //点击的时候,显示图片,设置图片位置
            select.gameObject.SetActive(true);
            select.transform.position = GameEvent.EventSystem.currentSelectedGameObject.transform.position;
        }
        else
        {
            return;
        }
    }
}
