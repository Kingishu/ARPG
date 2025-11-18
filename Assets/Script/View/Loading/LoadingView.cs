using UnityEngine;
using UnityEngine.UI;

public class LoadingView : View
{
    private Image progress_Value;
    public override void Awake()
    {
        base.Awake();
        progress_Value = GetComponent<Image>("Progress/Progress_Value");
    }

    public void UpdateLoadingProgress(float progress)
    {
        progress_Value.fillAmount= progress;
    }
}
