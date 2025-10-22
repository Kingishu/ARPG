using System;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        InitSystem();
    }

    private void InitSystem()
    {
        GameDefine.Init();
    }

    private void Update()
    {
        GameTime.Update();
    }
}
