using System;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager 
{
    private static ViewManager instance=new ViewManager();
    public static ViewManager Instance => instance;
    //生成UI面板的排序,每生成一个就+1.
    public int order=0;
    //初始化接口
    public void Init()
    {
        //注册所有界面
        LoginViewController.Instance.Init("UI/LoginCanvas",true,false);
        BagViewController.Instance.Init("UI/BagCanvas",true,false);
        ForgeViewController.Instance.Init("UI/ForgeCanvas",true,false);
        LoadingViewController.Instance.Init("UI/LoadingCanvas",true,false);
        MainViewController.Instance.Init("UI/MainCanvas",true,false);
        NavViewController.Instance.Init("UI/NavCanvas",true,false);
        PurifyViewController.Instance.Init("UI/PurifyCanvas",true,false);
        StoreViewController.Instance.Init("UI/StoreCanvas",true,false);
        TipsViewController.Instance.Init("UI/TipsCanvas",true,false);
    }
    List<string> open_view=new List<string>();
    public void OnViewChange_Begin(string path,bool act)
    {
        if (open_view.Contains(path)==false)
        {
            open_view.Add(path);
        }
    }
    public void OnViewChange_End(string path,bool act)
    {
        if (open_view.Contains(path))
        {
            open_view.Remove(path);
        }
    }

    public Action OnUpdate; 
    public void Update()
    {
        OnUpdate?.Invoke();
    }
}
