/// <summary>
/// This class is used to modify the game state (e.g. limit length run, seed etc.)
/// Subclass it and override wanted messages to handle the state.
/// </summary>
public class Modifier
{
    public virtual void OnRunStart(GameState state)
    {

    }

    public virtual void OnRunTick(GameState state)
    {

    }

    //return true if the gameobver screen should be displayed, returning false will return directly to loadout (useful for challenge)
    public virtual bool OnRunEnd(GameState state)
    {
        return true;
    }
}