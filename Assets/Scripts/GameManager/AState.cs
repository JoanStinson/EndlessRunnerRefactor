using UnityEngine;

public abstract class AState : MonoBehaviour
{
    [HideInInspector]
    public GameManager manager;

    public abstract void Enter(AState from);
    public abstract void Exit(AState to);
    public abstract void Tick();
    public abstract string GetName();
}