using Game.Config;
using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.UI.Image;

public class BagItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //记录当前的格子
    public int grild_id1;
    //缓存我们创建出来用来跟随的物品
    public GameObject temp;
    //缓存一下我们最开始格子的物品
    public Transform org;
    //记录交换的格子ID
    public int grild_id2;
    private void Awake()
    {
        grild_id1 = int.Parse(this.gameObject.name.Split('_')[1]);
    }
    //开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.childCount==0)
        {
            return;
        }
        Transform child = transform.GetChild(0);
        org = child;
        //如果子物体失活,我们不希望它可以被找到
        if (!child.gameObject.activeInHierarchy)
        {
            child = null;
        }
        //如果是空不做处理
        if (child != null)
        {
            //获取当前格子的背包的物品  ID以及物品数量
            int id = int.Parse(child.gameObject.name);
            int count = int.Parse(child.Find("key/key_text").GetComponent<Text>().text);
            //隐藏当前格子的物品
            child.gameObject.SetActive(false);
            //根据当前的格子,创建一个新的
            GameObject obj = ResourcesManager.Instance.CreatePropItem(id, count);
            //设置一下格子的锚点信息
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            //设置一下大小
            rect.sizeDelta = new Vector2(120, 120);
            //将它放在canvas下面,这样才能显示出来
            obj.transform.SetParent(BagViewController.Instance.view.transform);
            //设置temp,方便跟随
            temp = obj;

        }
    }
    //正在拖拽
    public void OnDrag(PointerEventData eventData)
    {
        if (temp != null)
        {
            //让新创建出来的跟随
            temp.transform.position = eventData.position;
        }
    }
    //结束拖拽
    public void OnEndDrag(PointerEventData eventData)
    {
        //这里做交换逻辑和结束拖拽的逻辑
        if (temp != null)
        {
            temp.gameObject.SetActive(false);
            ResourcesManager.Instance.Destroy(temp);
            temp = null;
            if (eventData.pointerEnter==null)
            {
                org.gameObject.SetActive(true);
                return;
            }
            var n = eventData.pointerEnter.gameObject.name;
            //交换逻辑  如果物体是以Prop_开头,证明是格子,可以交换
            if (n.StartsWith("Prop_"))
            {
                grild_id2 = int.Parse(n.Split("_")[1]);


                //数据层处理,需要得到他们两个的格子ID,进行交换
                BagData.Instance.Modify_Grild(grild_id1, grild_id2, null);

                //这里虽然下面的逻辑是重复的,但是不能统一提出来.
                //会造成一个BUG,我们先进行了交换,然后判断子物体就出现了错误.
                if (eventData.pointerEnter.gameObject.transform.childCount == 0)
                {
                    org.transform.SetParent(eventData.pointerEnter.gameObject.transform, false);
                    org.gameObject.SetActive(true);
                }
                else
                {
                    org.transform.SetParent(eventData.pointerEnter.gameObject.transform, false);
                    org.gameObject.SetActive(true);

                    //有物品,走交换
                    var c = eventData.pointerEnter.gameObject.transform.GetChild(0);
                    c.SetParent(this.transform, false);
                }

            }
            else
            {
                org.gameObject.SetActive(true);
            }
            org = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BagViewController.Instance.OnPointEnter_Grild(grild_id1, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BagViewController.Instance.OnPointExit_Grild();
    }
}
