using System;
using UnityEngine;

public class GameEvent
{
    public static Action<int, bool> DoHitlag;
    public static Action<RadialBlurConfig> DoRadialBlur;
}
