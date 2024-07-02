using Random = UnityEngine.Random;

public class SlidingMission : MissionBase
{
    private float m_PreviousWorldDist;

    public override void Created()
    {
        float[] maxValues = { 20, 30, 75, 150 };
        int choosen = Random.Range(0, maxValues.Length);
        reward = choosen + 1;
        max = maxValues[choosen];
        progress = 0;
    }

    public override string GetMissionDesc()
    {
        return "Slide for " + ((int)max) + "m";
    }

    public override MissionType GetMissionType()
    {
        return MissionType.SLIDING;
    }

    public override void RunStart(ITrackManager manager)
    {
        m_PreviousWorldDist = manager.worldDistance;
    }

    public override void Update(ITrackManager manager)
    {
        if (manager.CharacterController.isSliding)
        {
            float dist = manager.worldDistance - m_PreviousWorldDist;
            progress += dist;
        }

        m_PreviousWorldDist = manager.worldDistance;
    }
}
