using UnityEngine;

public class RestartRunning : StateMachineBehaviour
{
    static int s_DeadHash = Animator.StringToHash("Dead");

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // We don't restart if we go toward the death state
        if (animator.GetBool(s_DeadHash))
        {
            return;
        }

        ServiceLocator.Instance.GetService<ITrackManager>().StartMove();
    }
}
