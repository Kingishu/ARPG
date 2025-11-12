using Pathfinding;
using UnityEngine;

public class NavHelper : MonoBehaviour
{
    #region 单例模式
    private static NavHelper instance=new NavHelper();
    public static NavHelper Instance=>instance;

    private NavHelper()
    {
        
    }
    #endregion

    private NNConstraint nnConstraint;
    public Vector3 GetWalkPosition(Vector3 position)
    {
        if (nnConstraint == null)
        {
            nnConstraint = new NNConstraint();
            nnConstraint.walkable=true;
            nnConstraint.constrainWalkability=true;
            nnConstraint.constrainDistance=true;
        }
        return  AstarPath.active.GetNearest(position, nnConstraint).position;
    }
}
