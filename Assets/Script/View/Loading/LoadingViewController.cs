using UnityEngine;

public class LoadingViewController:ViewController<LoadingViewController,LoadingView>
{
    public void UpdateLoadingProgress(float progress)
    {
        if (IsOpen()==false)
        {
            Open();
        }
        view.UpdateLoadingProgress(progress);   
    }
}
