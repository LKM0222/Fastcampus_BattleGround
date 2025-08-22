using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장애물로 이동할 수 있는 상황인지 판단하는 클래스
/// /// 쏴야 할 총알이 남아있거나, 엄폐물로 이동하기 전에 대기시간이 남아있거나
/// /// 근처에 숨을만한 엄폐물이 없을경우는 false
/// /// 그 외에는 true
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Decision/TakeCover")]
public class TakeCoverDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        // 지금 쏴야 할 총알이 남아있거나, 대기시간이 필요하거나 엄폐물 위치 못찾았다면
        if(controller.variables.currentShots < controller.variables.shotsInRounds ||
            controller.variables.waitInCoverTime < controller.variables.coverTime ||
            Equals(controller.CoverSpot, Vector3.positiveInfinity))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
