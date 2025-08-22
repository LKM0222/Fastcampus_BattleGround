using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// alertCheck를 통해 경고를 들었거나 (총소리가 들렸거나)
/// /// 특정 거리에서 시야가 막혀있어도 특정 위치에서 타겟의 위치가 여러번 인지되었을 경우 
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decision/Hear")]
public class HearDecision : Decision
{
    // 처음에 의미없는 포지션 갖고 있다가 타겟의 위치를 찾아서, lastpos에 넣어두고, currentPos와 달라지면
    // 의미없는 값은 아닌데 비슷한 값이라고 인지시키기 위함.
    public Vector3 lastPos, currentPos;

    public override void OnEnableDecision(StateController controller)
    {
        //초기화
        lastPos = currentPos = Vector3.positiveInfinity;
    }

    private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetInHearRadius)
    {
        if( hasTarget)
        {
            currentPos = targetInHearRadius[0].transform.position;
            if(!Equals(lastPos, Vector3.positiveInfinity))
            {
                if(!Equals(lastPos, currentPos))
                {
                    controller.personalTarget = currentPos;
                    return true;
                }
            }
            lastPos = currentPos;
        }
        return false;
    }

    public override bool Decide(StateController controller)
    {
        if(controller.variables.hearAlert)
        {
            controller.variables.hearAlert = false;
            return true;
        }
        else
        {
            return CheckTargetInRadius(controller, controller.perceptionRadius, MyHandleTargets);
        }
    }
}
