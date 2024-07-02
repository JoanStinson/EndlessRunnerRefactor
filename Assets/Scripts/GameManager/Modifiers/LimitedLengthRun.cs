public class LimitedLengthRun : Modifier
{
    public float distance;

    public LimitedLengthRun(float dist)
    {
        distance = dist;
    }

    public override void OnRunTick(GameState state)
    {
        if (trackManager.worldDistance >= distance)
        {
            trackManager.CharacterController.currentLife = 0;
        }
    }

    public override void OnRunStart(GameState state)
    {

    }

    public override bool OnRunEnd(GameState state)
    {
        state.QuitToLoadout();
        return false;
    }
}
