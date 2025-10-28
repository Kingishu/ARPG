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
    
    //受击对象池
    Stack<GameObject> hit_Effect=new Stack<GameObject>(10);
    public GameObject CreatHitEffect(string path)
    {
        GameObject effect;
        if (hit_Effect.Count>0)
        {
            effect=hit_Effect.Pop();
            effect.SetActive(true);
            CoroutineHelper.Instance.ExecuteAfterDelay(2f, () =>
            {
                DestroyHitEffect(effect);
            });
            return effect;
        }
        else
        {
            effect=Instantiate<GameObject>(path);
            Object.DontDestroyOnLoad(effect);
            CoroutineHelper.Instance.ExecuteAfterDelay(2f, () =>
            {
                DestroyHitEffect(effect);
            });
            return effect;
        }
    }
    public void DestroyHitEffect(GameObject effect)
    {
        if (effect!=null)
        {
            effect.SetActive(false);
            hit_Effect.Push(effect);
        }
    }
    
    //格挡对象池
    Stack<GameObject> block_Effect=new Stack<GameObject>(10);
    public GameObject CreatBlockEffect(string path)
    {
        GameObject effect;
        if (block_Effect.Count>0)
        {
            effect=block_Effect.Pop();
            effect.SetActive(true);
            CoroutineHelper.Instance.ExecuteAfterDelay(2f, () =>
            {
                DestroyBlockEffect(effect);
            });
            return effect;
        }
        else
        {
            effect=Instantiate<GameObject>(path);
            Object.DontDestroyOnLoad(effect);
            CoroutineHelper.Instance.ExecuteAfterDelay(2f, () =>
            {
                DestroyBlockEffect(effect);
            });
            return effect;
        }
    }
    public void DestroyBlockEffect(GameObject effect)
    {
        if (effect!=null)
        {
            effect.SetActive(false);
            block_Effect.Push(effect);
        }
    }
}
