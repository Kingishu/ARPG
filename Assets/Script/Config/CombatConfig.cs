using UnityEngine;

public class CombatConfig
{
    private static CombatConfig instance=new CombatConfig();
    public static CombatConfig Instance=>instance;
    private CombatConfig()
    {
        
    }
    private GlobalCombatConfig combatConfig;

    public GlobalCombatConfig Config
    {
        get
        {
            if (combatConfig==null)
            {
                combatConfig = ResourcesManager.Instance.Load<GlobalCombatConfig>("GlobalConfig/Config");
            }
            return combatConfig;
        }
    }
}
