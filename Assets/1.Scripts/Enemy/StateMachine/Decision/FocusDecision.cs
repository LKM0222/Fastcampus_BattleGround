using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인지 타입에 따라 특정 거리로부터 가깝진 않지만, 시야는 막히지 않았지만, 위험요소를 감지했거나,
/// 너무 가까운 거리에 타겟(플레이어)이 있는지 판단단
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decision/Focus")]
public class FocusDecision : Decision
{
    public enum Sense
    {
        NEAR, // 가까운가
        PERCEPTION, // 시야각 인가
        VIEW, // 보이는가
    }

    public Sense sense; // 어떤 크기로 위험요소 감지를 할지 선택

    public bool invaildateCoverSpot; // 현재 엄폐물을 해제할지

    private float radius; // Sense에 따른 범위 -> range

    public override void OnEnableDecision(StateController controller)
    {
        switch (sense)
        {
            case Sense.NEAR:
                radius = controller.nearRadius;
                break;
            case Sense.PERCEPTION:
                radius = controller.perceptionRadius;
                break;
            case Sense.VIEW:
                radius = controller.viewRadius;
                break;
        }
    }

    private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetsInHearRadius)
    {
        // 타겟(플레이어)이 존재하고, 시야가 막히지 않았다면
        if (hasTarget && !controller.BlockedSight())
        {
            if (invaildateCoverSpot) // 엄폐물에 있다면,
            {
                controller.CoverSpot = Vector3.positiveInfinity; // 값을 무작위 큰 값으로 설정해, 엄폐물 해제.
            }
            controller.targetInSight = true;
            controller.personalTarget = controller.aimTarget.position;

            return true;
        }
        return false;
    }

    public override bool Decide(StateController controller)
    {
        // (가깝지 않은 상태이고, 경고를 느꼇거나, 시야가 막히지 않고) || 타겟이 범위안에 들어왔다면 
        return (sense != Sense.NEAR && controller.variables.feelAlert && !controller.BlockedSight()) ||
            Decision.CheckTargetInRadius(controller, radius, MyHandleTargets);
    }
}
