public class SingleLifeRun : Modifier
{
    public override void OnRunTick(GameState state)
    {
        if (state.trackManager.characterController.currentLife > 1)
        {
            state.trackManager.characterController.currentLife = 1;
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
