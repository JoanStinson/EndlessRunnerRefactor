public interface IGameManager
{
    AState topState { get; }

    AState FindState(string stateName);
    void PopState();
    void PushState(string name);
    void SwitchState(string newState);
}