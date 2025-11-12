using UnityEngine;

public class UnitManager
{
    private static UnitManager instance = new UnitManager();
    public static UnitManager Instance => instance;
    private UnitManager()
    {
        
    }

    public FSM player;
}
