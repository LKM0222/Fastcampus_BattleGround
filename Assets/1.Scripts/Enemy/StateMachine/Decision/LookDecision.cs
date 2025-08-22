using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타겟이 시야가 막히지 않은 상태에서 타겟이 시야각 1/2 사이에 있는지 판정하는 클래스
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decision/Look")]
public class LookDecision : Decision
{
    private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetsRadius)
    {
        if(hasTarget)
        {
            //플레이어의 위치
            Vector3 target = targetsRadius[0].transform.position;
            Vector3 dirToTarget = target - controller.transform.position;
            bool inFOVCondition = (Vector3.Angle(controller.transform.forward, dirToTarget) <
                controller.viewAngle / 2);
            if(inFOVCondition && controller.BlockedSight())
            {
                controller.targetInSight = true;
                controller.personalTarget = controller.aimTarget.position;
                return true;
            }
        }
        return false;
    }

    public override bool Decide(StateController controller)
    {
        controller.targetInSight = false;
        return CheckTargetInRadius(controller, controller.viewRadius, MyHandleTargets);
    }
}
