using UnityEngine;
using UnityEngine.UI;

public class LoginView : View
{
    public Button newGameButton;
    public override void Awake()
    {
        base.Awake();
        newGameButton = GetComponent<Button>("NewGame");
        newGameButton.onClick.AddListener(NewGame);
    }

    private void NewGame()
    {
        //打开LoadingView
        //切换场景
        GameSystem.Instance.SceneController.Load("NewGame");
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
