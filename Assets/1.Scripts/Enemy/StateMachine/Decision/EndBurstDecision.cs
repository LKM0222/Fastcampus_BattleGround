using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사격이 시작되면, 재장전 전까지, 쏠 수 있는 총알의 수 판단.
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decision/EndBurst")]
public class EndBurstDecision : Decision
{
    public override bool Decide(StateController controller)
    {   
        // 사격을 많이 했으면 true; 내가 가진 탄창 갯수보다 많이 했다면. (재장전으로 넘어감)
        return controller.variables.currentShots >= controller.variables.shotsInRounds;
    }
}
