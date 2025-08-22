using System.Collections;
using System.Collections.Generic;
using NPOI.SS.Formula.Functions;
using UnityEngine;

/// <summary>
/// 타겟이 멀리있고, 엄폐물에서 최소 한타임 정도 공격을 기다린 후에 다음 장애물로 이동할지
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decision/AdvenceCover")]
public class AdvenceCoverDecision : Decision
{
    public int waitRounds = 1;

    [Header("Extra Decision")]
    public FocusDecision targetNear; // 바로 앞에 타겟인데 장애물로 숨지 않아도 됨.

    public override void OnEnableDecision(StateController controller)
    {
        controller.variables.waitRounds += 1;
        //판단
        controller.variables.advanceCoverDecision = // 아래의 changeCoverChance가 높을수록 똑똑해짐.
            Random.Range(0f, 1f) < controller.classStats.ChangeCoverChance / 100;
    }

    public override bool Decide(StateController controller)
    {
        if(controller.variables.waitRounds <= waitRounds)
        {
            return false;
        }

        controller.variables.waitRounds = 0;
        return controller.variables.advanceCoverDecision && !targetNear.Decide(controller);
    }
}
