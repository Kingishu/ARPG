using System;
using UnityEngine;

public static class UTransfrom
{
    public static void LookTarget(this Transform t,Transform target)
    {
        if (target!=null)
        {
            Vector3 targetPosition=new Vector3(target.position.x,t.position.y,target.position.z);
            t.LookAt(targetPosition);
        }
        else
        {
            throw new ArgumentException("目标为空,无法看向目标");
        }
    }
    public static void LookTarget(this Transform t,Vector3 target)
    {
        if (target!=null)
        {
            Vector3 targetPosition=new Vector3(target.x,t.position.y,target.z);
            t.LookAt(targetPosition);
        }
        else
        {
            throw new ArgumentException("目标为空,无法看向目标");
        }
    }
    public static Vector3 GetOffsetPoint(this Transform t, float radius, float angle)
    {
        if (radius==0&&angle==0)
        {
            return t.transform.position;
        }
        float x=Mathf.Sin(angle*Mathf.PI/180)*radius;
        float z=Mathf.Cos(angle*Mathf.PI/180)*radius;
        Vector3 end = t.position + t.rotation * new Vector3(x, 0, z);
        return end;
    }
}
