public class SingleLifeRun : Modifier
{
    public override void OnRunTick(GameState state)
    {
        if (trackManager.CharacterController.currentLife > 1)
        {                
            trackManager.CharacterController.currentLife = 1;
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
