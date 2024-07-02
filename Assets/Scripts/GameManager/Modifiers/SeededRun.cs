public class SeededRun : Modifier
{
    protected const int k_DaysInAWeek = 7;
    private int m_Seed;

    public SeededRun()
    {
        m_Seed = System.DateTime.Now.DayOfYear / k_DaysInAWeek;
    }

    public override void OnRunStart(GameState state)
    {
        trackManager.trackSeed = m_Seed;
    }

    public override bool OnRunEnd(GameState state)
    {
        state.QuitToLoadout();
        return false;
    }
}
