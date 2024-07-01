using Random = UnityEngine.Random;

public class MultiplierMission : MissionBase
{
    public override bool HaveProgressBar()
    {
        return false;
    }

    public override void Created()
    {
        float[] maxValue = { 3, 5, 8, 10 };
        int choosen = Random.Range(0, maxValue.Length);

        max = maxValue[choosen];
        reward = (choosen + 1);

        progress = 0;
    }

    public override string GetMissionDesc()
    {
        return "Reach a x" + ((int)max) + " multiplier";
    }

    public override MissionType GetMissionType()
    {
        return MissionType.MULTIPLIER;
    }

    public override void RunStart(TrackManager manager)
    {
        progress = 0;
    }

    public override void Update(TrackManager manager)
    {
        if (manager.multiplier > progress)
        {
            progress = manager.multiplier;
        }
    }
}