using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    private static CoroutineHelper instance;

    public static CoroutineHelper Instance
    {
        get
        {
            if (instance==null)
            {
                GameObject go=new GameObject("IEnumeratorHelper");
                instance=go.AddComponent<CoroutineHelper>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public void ExecuteAfterDelay(float delay, Action action)
    {
        StartCoroutine(ExecuteAfterDelayCoroutine(delay, action));
    }
    private IEnumerator ExecuteAfterDelayCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    
}
