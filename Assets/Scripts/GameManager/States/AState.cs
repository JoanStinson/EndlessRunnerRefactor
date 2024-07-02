using UnityEngine;

public abstract class AState : MonoBehaviour
{
    protected IGameManager gameManager;
    protected ITrackManager trackManager;

    private void Start()
    {
        gameManager = ServiceLocator.Instance.GetService<IGameManager>();
        trackManager = ServiceLocator.Instance.GetService<ITrackManager>();
    }

    public abstract void Enter(AState from);
    public abstract void Exit(AState to);
    public abstract void Tick();
    public abstract string GetName();
}