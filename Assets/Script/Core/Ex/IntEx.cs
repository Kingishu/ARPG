using UnityEngine;

public static class IntEx
{
    /// <summary>
    /// 封装Unity的随机数方法，修改为左闭右闭区间
    /// </summary>
    /// <param name="min">最小区间</param>
    /// <param name="max">最大区间</param>
    /// <returns></returns>
    public static int Range(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }
    /// <summary>
    /// 传入一个概率,判断本次随机是否成功,例如概率为80,那么会从0-99随机生成一个数,小于80即为判定成功
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static bool InRange(this int x)
    {
        return IntEx.Range(0, 99) < x;
    }
}
