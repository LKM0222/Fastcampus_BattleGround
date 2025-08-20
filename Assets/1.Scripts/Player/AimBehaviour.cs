using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마우스 오른쪽 버튼 : 조준 (다른 behaviour보다 상위가 됨. 다른 동작을 대체해서 동작하게 됨. 조준일 땐 가장 먼저 처리됨)
/// 마우스 휠 버튼 : 좌우 카메라 변경 (조준 상태에서 좌우)
/// 벽의 모서리에서 조준할 때, 상체 기울이기 기능 
/// 크로스 헤어 관리 (십자선 텍스쳐 관리, 총에 따라 크로스헤어 변경됨. (OnGUI로 제작))
/// </summary>
public class AimBehaviour : GenericBehaviour
{
    public Texture2D crossHair; // 십자선 이미지
    public float aimTurnSmoothing = 0.15f; // 조준 시 정밀조준 위한 보정값
    public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0.0f); // 조준 시 카메라와 가까워지게 (크게 보이게)
    public Vector3 aimCamOffset = new Vector3(0.0f, 0.4f, -0.7f); // 조준 시 카메라 오프셋

    private int aimBool; // 애니메이터 파라미터 : 조준
    private bool aim; // 조준중인지
    private int cornerBool; // 애니메이터 관련, (모서리에서 조준중인지)
    private bool peekCorner; // 플레이어가 코너 모서리에 있는지 여부.
    //IK용 회전값들
    private Vector3 initialRootRotation; // 루트 본 로컬 회전값.
    private Vector3 initialHipRotation; // 
    private Vector3 initialSpineRotation; // 

    private void Start()
    {
        // setup
        aimBool = Animator.StringToHash("Aim");
        cornerBool = Animator.StringToHash("Corner");

        // Value
        Transform hips = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipRotation = hips.localEulerAngles;
        initialSpineRotation = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;
    }
}
