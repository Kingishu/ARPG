using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        InitSystem();
    }

    private void InitSystem()
    {
        GameEvent.DoHitlag += DoHitLag;
        GameEvent.DoRadialBlur += DoRadialBlurConfig;
        GameDefine.Init();
    }

    private void Update()
    {
        GameTime.Update();
    }

    #region 镜像模糊

    private Volume volume;
    private RadialBlur radialBlur;
    private Coroutine radialBlurCoroutine;
    public void DoRadialBlurConfig(RadialBlurConfig config)
    {
        if (volume==null)
        {
            volume = FindFirstObjectByType<Volume>();
            volume.profile.TryGet(out radialBlur);
        }
        if (radialBlur!=null)
        {
            if (radialBlurCoroutine!=null)
            {
                StopCoroutine(radialBlurCoroutine);
            }

            StartCoroutine(RadialBlurConfig(config));
        }
    }
    IEnumerator RadialBlurConfig(RadialBlurConfig config)
    {
        if (config!=null)
        {
            float temp=0;
            while (temp<=config.lerpTime)
            {
                temp += GameTime.deltaTime;
                float value=Mathf.Lerp(0f, 0.106f, temp / config.lerpTime);
                radialBlur.intensity.value=config.act?value:0.106f-value;
                print("当前的值是"+radialBlur.intensity.value);
                yield return new WaitForEndOfFrame();
            }

            if (config.act)
            {
                //隐藏一下
                RadialBlurConfig newConfig = new RadialBlurConfig();
                newConfig.act = false;
                newConfig.lerpTime = 0.3f;
                DoRadialBlurConfig(newConfig);
            }
        }
    }

    #endregion
    
    
    
    #region  顿帧效果

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
    #endregion
}
