using Random = UnityEngine.Random;

public class PickupMission : MissionBase
{
    private int previousCoinAmount;

    public override void Created()
    {
        float[] maxValues = { 1000, 2000, 3000, 4000 };
        int choosen = Random.Range(0, maxValues.Length);
        max = maxValues[choosen];
        reward = choosen + 1;
        progress = 0;
    }

    public override string GetMissionDesc()
    {
        return "Pickup " + max + " fishbones";
    }

    public override MissionType GetMissionType()
    {
        return MissionType.PICKUP;
    }

    public override void RunStart(TrackManager manager)
    {
        previousCoinAmount = 0;
    }

    public override void Update(TrackManager manager)
    {
        int coins = manager.characterController.coins - previousCoinAmount;
        progress += coins;

        previousCoinAmount = manager.characterController.coins;
    }
}
