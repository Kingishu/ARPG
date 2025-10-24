using System;
using System.Collections;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        InitSystem();
    }

    private void InitSystem()
    {
        GameEvent.DoHitlag += DoHitLag;
        GameDefine.Init();
    }

    private void Update()
    {
        GameTime.Update();
    }

    public void DoHitLag(int frame,bool lerp)
    {
        if (hitLagCoroutine!=null)
        {
            StopCoroutine(hitLagCoroutine);
        }
        hitLagCoroutine=StartCoroutine(HitLag(frame,lerp));
    }
    //顿帧的逻辑
    private Coroutine hitLagCoroutine;
    IEnumerator HitLag(int frame,bool lerp)
    {
        for (int i = 0; i < frame; i++)
        {
            Time.timeScale=lerp?Mathf.Lerp(0,1,i/frame):0;
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 1;
        hitLagCoroutine = null;
    }
}
