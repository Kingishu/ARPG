using Game.Config;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesManager
{
    # region 单例模式
    private static ResourcesManager instance;

    public static ResourcesManager Instance
    {
        get
        {
            if (instance==null)
            {
                instance = new ResourcesManager();
            }
            return instance;
        }
        
    }
    private ResourcesManager()
    {
        
    }
    #endregion
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public T Instantiate<T>(string path) where T : Object
    {
        var obj = Resources.Load<T>(path);
        if (obj!=null)
        {
            return Object.Instantiate(obj);
        }
        return null;
    }
    public void Destroy(GameObject go)
    {
        Object.Destroy(go);
    }

    Dictionary<string,Stack<GameObject>> effectPool=new Dictionary<string,Stack<GameObject>>();

    public GameObject CreatEffext(string path)
    {
        GameObject effect;
        if (effectPool.ContainsKey(path))
        {
            if (effectPool[path].Count>0)
            {
                //从其中取
                effect=effectPool[path].Pop();
                effect.gameObject.SetActive(true);
                CoroutineHelper.Instance.ExecuteAfterDelay(2f, () =>
                {
                    DestroyEffect(path,effect);
                });
                return effect;
            }
            else
            {
                //自己创建
                effect=Instantiate<GameObject>(path);
                Object.DontDestroyOnLoad(effect);
                CoroutineHelper.Instance.ExecuteAfterDelay(2f, () =>
                {
                    DestroyEffect(path,effect);
                });
                return effect;
            }
        }
        else
        {
            effectPool[path]=new Stack<GameObject>();
            effect=Instantiate<GameObject>(path);
            Object.DontDestroyOnLoad(effect);
            CoroutineHelper.Instance.ExecuteAfterDelay(2f, () =>
            {
                DestroyEffect(path,effect);
            });
            return effect;
        }
    }

    public void DestroyEffect(string path,GameObject effect)
    {
        if (effect != null)
        {
            effect.SetActive(false);
            effectPool[path].Push(effect);
        }
    }

    private Stack<EnemyHUD> enemy_HUD = new Stack<EnemyHUD>();
    public EnemyHUD CreateEnemyHUD()
    {
        if (enemy_HUD.Count>0)
        {
            return enemy_HUD.Pop();
        }
        else
        {
            var go = Instantiate<GameObject>("UI/HUD/Enemy_HUD");
            return go.GetComponent<EnemyHUD>();
        }
    }

    public void DestroyEnemyHUD(EnemyHUD hud)
    {
        hud.gameObject.SetActive(false);
        enemy_HUD.Push(hud);
    }

    private Stack<GameObject> bag_item = new Stack<GameObject>(64);

    public void Destroy_BagItem(GameObject item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(null,false);
        bag_item.Push(item);
    }
    public GameObject Create_BagItem(string item_path)
    {
        if (bag_item.Count>0)
        {
            var go = bag_item.Pop();
            go.gameObject.SetActive(true);
            return go;
        }
        else
        {
            return Instantiate<GameObject>(item_path);
        }
    }

    public Sprite LoadIcon(PropEntity entity)
    {
        switch (entity.type)
        {
            case 0:
                //装备
                return Load<Sprite>(GameDefine.prop_root + entity.icon);
            case 1:
                //物品
                return Load<Sprite>(GameDefine.Get_EQ_Icon(entity.part) + entity.icon);
            case 2:
                //材料
                return Load<Sprite>(GameDefine.mat_root + entity.icon);
            default:
                throw new System.Exception("获取出错,没有该icon类型");
        }
    }

    public GameObject CreatePropItem(int id, int count)
    {
        //逐一创建物品
        var obj = ResourcesManager.Instance.Create_BagItem(GameDefine.itemPath);
        //更新icon
        var icon = obj.transform.Find("icon").GetComponent<Image>();
        PropEntity bagEntity = PropData.Get(id);
        icon.sprite = ResourcesManager.Instance.LoadIcon(bagEntity);
        //更新数量
        var text = obj.transform.Find("key/key_text").GetComponent<Text>();
        text.text = count.ToString();
        //设置一下名称
        obj.gameObject.name = bagEntity.id.ToString();
        //返回出去
        return obj;
    }
}
