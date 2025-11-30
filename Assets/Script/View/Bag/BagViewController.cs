using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static Pathfinding.RVO.SimulatorBurst;

public class BagViewController : ViewController<BagViewController, BagView>
{
    public void OnPointEnter_Grild(int grild_id,PointerEventData eventData)
    {
        var entity = BagData.Instance.Get(grild_id);
        if (entity!=null)
        {
            if (entity.entity.type == 0)
            {
                view.ShowPropInfo(grild_id, eventData.pointerEnter.transform.position);
            }
            else if (entity.entity.type == 1)
            {
                
                view.ShowEquipInfo(grild_id, eventData.pointerEnter.transform.position);
            }
            else if (entity.entity.type == 2)
            {
                view.ShowMaterialInfo(grild_id, eventData.pointerEnter.transform.position);
            }
        }
    }
    public void OnPointExit_Grild()
    {
        view.CloseInfo();
    }
}
