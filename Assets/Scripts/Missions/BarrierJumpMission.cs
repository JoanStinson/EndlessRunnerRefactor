using UnityEngine;
using Random = UnityEngine.Random;

public class BarrierJumpMission : MissionBase
{
    protected const int k_HitColliderCount = 8;
    protected readonly Vector3 k_CharacterColliderSizeOffset = new Vector3(-0.3f, 2f, -0.3f);

    private Obstacle m_Previous;
    private Collider[] m_Hits;

    public override void Created()
    {
        float[] maxValues = { 20, 50, 75, 100 };
        int choosen = Random.Range(0, maxValues.Length);

        max = maxValues[choosen];
        reward = choosen + 1;
        progress = 0;
    }

    public override string GetMissionDesc()
    {
        return "Jump over " + ((int)max) + " barriers";
    }

    public override MissionType GetMissionType()
    {
        return MissionType.OBSTACLE_JUMP;
    }

    public override void RunStart(TrackManager manager)
    {
        m_Previous = null;
        m_Hits = new Collider[k_HitColliderCount];
    }

    public override void Update(TrackManager manager)
    {
        if (manager.characterController.isJumping)
        {
            Vector3 boxSize = manager.characterController.characterCollider.collider.size + k_CharacterColliderSizeOffset;
            Vector3 boxCenter = manager.characterController.transform.position - Vector3.up * boxSize.y * 0.5f;

            int count = Physics.OverlapBoxNonAlloc(boxCenter, boxSize * 0.5f, m_Hits);

            for (int i = 0; i < count; i++)
            {
                if (m_Hits[i].TryGetComponent<Obstacle>(out var obs) &&
                    obs is AllLaneObstacle)
                {
                    if (obs != m_Previous)
                    {
                        progress += 1;
                    }

                    m_Previous = obs;
                }
            }
        }
    }
}
