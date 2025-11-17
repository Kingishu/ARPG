using System;
using UnityEngine;
using UnityEngine.UI;

public class ViewController<T,V> where T:new() where V:View,new()
{
   static T instance=new T();
   public static T Instance => instance;
   public V view;
   public void Init(string resPath,bool isAddUpdateListener=false,bool stopAI_OnOpen=false)
   {
      view = new V();
      view.Init(resPath, isAddUpdateListener, Close, stopAI_OnOpen);
      GameEvent.ResetSortOrder += ResetSortOrder;
   }

   public void ResetSortOrder()
   {
      if (view!=null&&view._enable)
      {
         int o = view.canvas.sortingOrder - 10000;
         o = Mathf.Clamp(o, -30000, 30000);
         view.canvas.sortingOrder = o;
      }
   }

   //由于我们的UI界面是持久化存在,只是激活和不激活,所以不需要移除监听,只有AddListener
   public void AddListener()
   {
      
   }

   public bool IsOpen()
   {
      return view._enable;
   }
   public void Open()
   {
      if (view.gameObject==null)
      {
         //第一次打开这个view,需要实例化
         GameObject go = ResourcesManager.Instance.Instantiate<GameObject>(view.resPath);
         GameObject.DontDestroyOnLoad(go);
         view.gameObject=go;
         view.gameObject.name=view.gameObject.name.Split("(")[0];
         view.transform=go.transform;
         view.canvas=go.GetComponent<Canvas>();
         view.cs=go.GetComponent<CanvasScaler>();
         //生命周期 awake onenable start
         view.Awake();
         SetActive(true);
         view.Start();
      }
      else
      {
         //不是第一次,只需要激活就可以
         SetActive(true);
      }
   }
   //关闭接口
   public void Close(bool destroy=false)
   {
      if (view._enable==false)
      {
         return;
      }
      SetActive(false);
   }
   //激活禁用接口
   public void SetActive(bool act)
   {
      if (view._enable!=act)
      {
         //调用面板改变方法
         ViewManager.Instance.OnViewChange_Begin(view.resPath,act);
         //激活面板
         view.gameObject.SetActive(act);
         //改变view的enable属性
         view._enable = act;
         if (act)
         {
            view.OnEnable();
            //层级排序的问题
            ViewManager.Instance.order += 1;
            view.canvas.sortingOrder = ViewManager.Instance.order;
            if ( ViewManager.Instance.order>=30000)
            {
               ViewManager.Instance.order-=10000;
               GameEvent.ResetSortOrder?.Invoke();
            }
            view.SetSize();
         }
         else
         {
            view.OnDisable();
         }

         ViewManager.Instance.OnViewChange_End(view.resPath,act);
      }
   }
}
