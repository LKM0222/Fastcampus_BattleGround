using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// target이 dead인지 체크
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Decision/TargetDead")]
public class TargetDeadDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        try
        {
            return controller.aimTarget.root.GetComponent<HealthBase>().isDead;
        }
        catch(UnassignedReferenceException)
        {
            Debug.LogError($"HealthBase 관련 컴포넌트 필요 {controller.name}", controller.gameObject);
        }

        return false;
    }
}
