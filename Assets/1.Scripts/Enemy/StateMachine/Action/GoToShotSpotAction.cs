using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 특정 지점으로 이동
[CreateAssetMenu(menuName = "PluggableAI/Actions/Go To Shot Spot")]
public class GoToShotSpotAction : Action
{
    public override void OnReadyAction(StateController controller)
    {
        controller.focusSight = false;
        controller.nav.destination = controller.personalTarget;
        controller.nav.speed = controller.generalStats.chaseSpeed;
        controller.enemyAnimation.AbortPendingAim();
    }

    public override void Act(StateController controller)
    {
        
    }
}
