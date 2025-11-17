using Microsoft.Unity.VisualStudio.Editor;
using System;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class View
{
    public string resPath;//维护当前面板资源的路径
    public GameObject gameObject;//维护当前游戏对象
    public Transform transform;//维护当前的Trasnfrom
    public Canvas canvas;//维护面板的canvas组件
    public Action<bool> close;
    public bool isAddUpdateListener = false;
    public bool stopAI_OnOpen = false;
    public bool _enable;
    public CanvasScaler cs;
    public void Init(string resPath,bool isAddUpdateListener=false,Action<bool> close=null,bool stopAI_OnOpen=false)
    {
        this.resPath=resPath;
        this.isAddUpdateListener=isAddUpdateListener;
        this.close=close;
        this.stopAI_OnOpen=stopAI_OnOpen;
    }
    //获取组件的方法,传入一个名字,来获取面板其中的某个子对象的组件
    public T GetComponent<T>(string path) where T : Component
    {
        return this.transform.Find(path).GetComponent<T>();
    }
    //设置图片的方法,传入资源中的一个路径
    public void SetSprite(Image image, string sprite)
    {
        Sprite s=ResourcesManager.Instance.Load<Sprite>(sprite);
        image.sprite=s;
    }
    //设置文本的方法
    public void SetText(Text text,string content)
    {
        text.text = content;
    }
    //设置面板尺寸的方法
    public void SetSize()
    {
        //计算当前屏幕分辨率和我们制作时候的标准分辨率做比较
        //我们制作时候按照1920*1080  16:9的屏幕来制作
        //如果比我们的屏幕宽,我们需要高度匹配,完全显示画面,但是左右有黑边
        //如果比我们的屏幕高,我们用宽度匹配,完全显示画面,但是上下有黑边
        bool b=Screen.width/(float)Screen.height > cs.referenceResolution.x/cs.referenceResolution.y;
        if (b)
        {
            cs.matchWidthOrHeight = 1;//匹配高度
        }
        else
        {
            cs.matchWidthOrHeight = 0;//匹配宽度
        }
    }
    //重构MONO的生命周期
    public virtual void Awake()
    {
        
    }

    public virtual void OnEnable()
    {
        if (isAddUpdateListener)
        {
            ViewManager.Instance.OnUpdate+=Update;
        }
    }

    public virtual void Start()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void OnDisable()
    {
        if (isAddUpdateListener)
        {
            ViewManager.Instance.OnUpdate-=Update;
        }   
    }

    public virtual void OnDestroy()
    {
        
    }
}
