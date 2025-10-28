using System.Collections.Generic;
using UnityEngine;

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
}
