using System.IO;

/// <summary>
/// Base abstract class used to define a mission the player needs to complete to gain some premium currency.
/// Subclassed for every mission.
/// </summary>
public abstract class MissionBase
{
    // Mission type
    public enum MissionType
    {
        SINGLE_RUN,
        PICKUP,
        OBSTACLE_JUMP,
        SLIDING,
        MULTIPLIER,
        MAX
    }

    public bool isComplete { get { return (progress / max) >= 1.0f; } }
    public float progress;
    public float max;
    public int reward;

    public void Serialize(BinaryWriter w)
    {
        w.Write(progress);
        w.Write(max);
        w.Write(reward);
    }

    public void Deserialize(BinaryReader r)
    {
        progress = r.ReadSingle();
        max = r.ReadSingle();
        reward = r.ReadInt32();
    }

    public virtual bool HaveProgressBar() { return true; }
    public abstract void Created();
    public abstract MissionType GetMissionType();
    public abstract string GetMissionDesc();
    public abstract void RunStart(ITrackManager manager);
    public abstract void Update(ITrackManager manager);

    static public MissionBase GetNewMissionFromType(MissionType type)
    {
        switch (type)
        {
            case MissionType.SINGLE_RUN:
                return new SingleRunMission();

            case MissionType.PICKUP:
                return new PickupMission();

            case MissionType.OBSTACLE_JUMP:
                return new BarrierJumpMission();

            case MissionType.SLIDING:
                return new SlidingMission();

            case MissionType.MULTIPLIER:
                return new MultiplierMission();
        }

        return null;
    }
}