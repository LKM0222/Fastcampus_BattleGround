using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 근처에 장애물이나 엄폐물이 가깝게 있는지 더블 체크함.
/// /// 타겟 목표까지 장애물이나 엄폐물이 있는지 체크
/// /// 가장 처음 검출된 충돌체가 플레이어라면 장애물이 없다는 뜻.
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decision/ClearShot")]
public class ClearShotDecision : Decision
{
    [Header("Extra Decision")]
    public FocusDecision targetNear; // 가까운지 판단하기 위함.
    
    // 현재 clearShot이 가능한지
    private bool HaveClearShot(StateController controller)
    {
        Vector3 shotOrigin = controller.transform.position +
            Vector3.up * (controller.generalStats.aboveCoverHeight + controller.nav.radius);
        Vector3 shortDirection = controller.personalTarget - shotOrigin;

        bool blockedShot = Physics.SphereCast(shotOrigin, controller.nav.radius, shortDirection,
            out RaycastHit hit, controller.nearRadius, controller.generalStats.coverMask |
            controller.generalStats.obstacleMask);
        if(!blockedShot)
        {
            blockedShot = Physics.Raycast(shotOrigin, shortDirection, out hit, shortDirection.magnitude,
                controller.generalStats.coverMask | controller.generalStats.obstacleMask);
            if(blockedShot)
            {   
                //막힌게 있는지 확인
                blockedShot = (hit.transform.root == controller.aimTarget.root);
            }
        }
        return blockedShot;
    }

    public override bool Decide(StateController controller)
    {
        return targetNear.Decide(controller) || HaveClearShot(controller);
    }
}
