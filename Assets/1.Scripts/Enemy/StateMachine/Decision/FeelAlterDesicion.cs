using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 경고를 감지 했는가
/// /// 플래그가 현재 상태로 판단
/// </summary> 
[CreateAssetMenu(menuName = "PluggableAI/Decision/FeelAlter")]
public class FeelAlterDesicion : Decision
{
    public override bool Decide(StateController controller)
    {
        return controller.variables.feelAlert;
    }
}
