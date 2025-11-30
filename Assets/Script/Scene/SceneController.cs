using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    
    public void Load(string newgame)
    {
        LoadingViewController.Instance.Open();
        LoginViewController.Instance.Close();
        StartCoroutine(LoadSceneAsync(newgame));
    }

    IEnumerator LoadSceneAsync(string next)
    {
        yield return null;
        var op = SceneManager.LoadSceneAsync(next);
        op.allowSceneActivation=false;
        while (op.progress<0.9f)
        {
            LoadingViewController.Instance.UpdateLoadingProgress(op.progress);
            yield return null;
        }
        var progress = op.progress;
        while (progress<=1)
        {
            progress+=Time.deltaTime;
            LoadingViewController.Instance.UpdateLoadingProgress(progress);
            yield return null;
        }
        op.allowSceneActivation = true;
        while (op.isDone==false)
        {
            yield return null;
        }
        yield return null;
        //关闭加载页面
        LoadingViewController.Instance.Close();
        //创建角色
        GameObject player=UnitManager.Instance.CreatePlayer();
        //控制摄像机
        GameSystem.Instance.CameraController.SetTarget(player.transform);
        //打开主页面
        MainViewController.Instance.Open();
    }
}
