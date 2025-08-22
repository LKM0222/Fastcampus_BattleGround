using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 타겟이 보이거나 근처에 있으면, 교전 대기시간을 초기화
/// /// 반대로 보이지 않거나, 멀어져있거나 한다면 blindEngageTim만큼 기다릴건지 
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decision/Engage")]
public class EngageDecision : Decision
{
    [Header("Extra Decision")]
    public LookDecision isViewing; //보이는지
    public FocusDecision targetNear; // 타겟이 가까이 있는지

    public override bool Decide(StateController controller)
    {
        if(isViewing.Decide(controller) || targetNear.Decide(controller))
        {
            controller.variables.blindEngageTimer = 0;
        }
        else if(controller.variables.blindEngageTimer >= controller.blindEngageTime)
        {
            controller.variables.blindEngageTimer = 0;
            return false;
        }
        return true;
    }
}
