using UnityEngine;

public class MainViewController : ViewController<MainViewController,MainView>
{
    public void UpdatePlayerHP(float v)
    {
        view.UpdatePlayerHP(v);
    }

    public void UpdatePlayerMP(float v)
    {
       view.UpdatePlayerMP(v);
    }

    public void UpdateBossHP(float v)
    {
        view.UpdateBossHP(v); 
    }

    public void EnableBossHP(bool act,string name)
    {
        view.EnableBossHP(act,name);
    }
    public void SetSkillCD(int type, float cd)
    {
        view.SetSkillCD(type, cd);
    }

    public void OpenCD_Tips()
    {
        view.OpenCD_Tips();
    }
}
