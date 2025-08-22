using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타겟이 있다면 타겟까지 이동, 없다면 가만히 서있음.
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Actions/Search")]
public class SearchAction : Action
{
    // 탐색 전 초기화해야될것들
    public override void OnReadyAction(StateController controller)
    {
        controller.focusSight = false;
        controller.enemyAnimation.AbortPendingAim();
        controller.enemyAnimation.anim.SetBool("Crouch", false);
        controller.CoverSpot = Vector3.positiveInfinity;
    }

    public override void Act(StateController controller)
    {
        if(Equals(controller.personalTarget, Vector3.positiveInfinity))
        {
            controller.nav.destination = controller.transform.position;
        }
        else
        {
            controller.nav.speed = controller.generalStats.chaseSpeed;
            controller.nav.destination = controller.personalTarget;
        }
    }
}
