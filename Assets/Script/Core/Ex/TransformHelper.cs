using UnityEngine;

public static class TransformHelper
{
    /// <summary>
    /// 获取target与自身的前后关系
    /// </summary>
    /// <param name="t"></param>
    /// <param name="target">敌人位置</param>
    /// <returns>返回值(0,1]敌人在角色前方.  返回这==0,敌人与角色平行  返回值[-1,0)敌人在后面</returns>
   public static float ForwardOrBack(this Transform t,Vector3 target)
   {
         Vector3 forward=t.forward.normalized;
         Vector3 toTarget=(target-t.position).normalized;
         var value=Vector3.Dot(forward, toTarget);
         return value;
   }
}
