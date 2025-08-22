using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아군 적군 모두 쓸 수 있는 범용 스크립트 
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/GeneralStats")]
public class GeneralStats : ScriptableObject
{
    [Header("General")]
    public float patrolSpeed = 2f; //npc 정찰 속도 clear state(평상시)
    public float chaseSpeed = 5f; //npc 따라오는 속도
    public float evadeSpeed = 15f; //npc 회피속도 (engage state 교전중)
    public float patrolWaitTime = 2f; // 웨이포인트에서 대기하는 시간

    [Header("Animation")]
    public LayerMask obstacleMask; // 장애물 레이어 마스크
    public float angleDeadZone = 5f; //조준시 깜빡임을 피하기 위한 최소 확정 앵글
    public float speedDampTime = 0.4f; //속도 댐핑 시간
    public float angularSpeedDampTime = 0.2f; // 각속도 댐핑 시간
    public float angleResponseTime = 0.2f; // 각속도 안에서 각도 회전에 따른 반응 시간.

    [Header("Cover")]
    public float aboveCoverHeight = 1.5f; // 캐릭터가 숨기 위한 장애물 최소 높이값
    public LayerMask coverMask; // 장애물 레이어 마스크
    public LayerMask shotMask; // 사격 레이어 마스크 (총이 맞을 수 있는곳)
    public LayerMask targetMask; // 타겟 레이어 마스크 (타겟 지정)

}
