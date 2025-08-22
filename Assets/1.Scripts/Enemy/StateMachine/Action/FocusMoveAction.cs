using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격과 동시에 이동하는 액션, 일단 회전할 땐, 회전을 하고, 회전이 끝나면 strafing이 활성화됨.
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Actions/FocusMove")]
public class FocusMoveAction : Action
{
    public ClearShotDecision clearShotDecision; // 총을 쏠 수 있는지

    private Vector3 currentDest; // 현재 내가 이동하고자 하는 목표
    private bool aligned; // 타겟과 정렬이 되었나? 이후 strafing진행

    public override void OnReadyAction(StateController controller)
    {
        controller.hadClearShot = controller.haveClearShot = false;
        currentDest = controller.nav.destination;
        controller.focusSight = true;
        aligned = false;
    }

    public override void Act(StateController controller)
    {
        if(!aligned)
        {
            controller.nav.destination = controller.personalTarget;
            controller.nav.speed = 0f;
            if(controller.enemyAnimation.angularSpeed == 0)
            {
                controller.Strafing = true;
                aligned = true;
                controller.nav.destination = currentDest;
                controller.nav.speed = controller.generalStats.evadeSpeed;
            }
        }
        else
        {
            controller.haveClearShot = clearShotDecision.Decide(controller);
            if(controller.hadClearShot != controller.haveClearShot)
            {
                controller.Aiming = controller.haveClearShot;
                //사격이 가능하다면, 현재 이동 목표가 엄폐물과 다르더라도 이동하지 않는다.
                if(controller.haveClearShot && !Equals(currentDest, controller.CoverSpot))
                {
                    controller.nav.destination = controller.transform.position;
                }
            }
            controller.hadClearShot = controller.haveClearShot;
        }
    }
}
