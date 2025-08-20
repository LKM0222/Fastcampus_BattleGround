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
    private Transform myTransform;

    private void Start()
    {
        myTransform = transform;

        // setup
        aimBool = Animator.StringToHash("Aim");
        cornerBool = Animator.StringToHash("Corner");

        // Value
        Transform hips = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipRotation = hips.localEulerAngles;
        initialSpineRotation = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;
    }

    // 카메라에 따라 플레이어를 올바른 방향으로 회전
    void Rotating()
    {
        Vector3 forward = behaviourController.playerCamera.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;

        Quaternion targetRotation = Quaternion.Euler(0f, behaviourController.GetCamScript.GetH, 0.0f);
        float minSpeed = Quaternion.Angle(myTransform.rotation, targetRotation) * aimTurnSmoothing;

        if (peekCorner)
        {
            // 조준 중일때 플레이어 상체만 살짝 기울여주기 위함.
            myTransform.rotation = Quaternion.LookRotation(-behaviourController.GetLastDirection());
            targetRotation *= Quaternion.Euler(initialRootRotation);
            targetRotation *= Quaternion.Euler(initialHipRotation);
            targetRotation *= Quaternion.Euler(initialSpineRotation);
            Transform spine = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine);
            spine.rotation = targetRotation;
        }
        else
        {
            behaviourController.SetLastDirection(forward);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, minSpeed * Time.deltaTime);
        }
    }

    private void AimManagement()
    {
        Rotating();
    }

    // 조준 했다, 풀었다 하는 코루틴
    private IEnumerator ToggleAimOn()
    {
        yield return new WaitForSeconds(0.05f);
        //조준이 불가능한 상태일때에 대한 예외처리 (죽었거나, 특수한 상황(스턴)일경우)
        if (behaviourController.GetTempLockStatus(behaviourCode) || behaviourController.IsOverriding(this))
        {
            yield return false;
        }
        else
        {
            aim = true;
            int signal = 1;
            if (peekCorner)
            {
                signal = (int)Mathf.Sign(behaviourController.GetH);
            }
            aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
            aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;

            yield return new WaitForSeconds(0.1f);
            behaviourController.GetAnimator.SetFloat(speedFloat, 0.0f);
            behaviourController.OverrideWithBehaviour(this);
        }
    }

    private IEnumerator ToggleAimOff()
    {
        aim = false;
        yield return new WaitForSeconds(0.3f);
        behaviourController.GetCamScript.ResetTargetOffsets();
        behaviourController.GetCamScript.ResetMaxVertialAngle();
        yield return new WaitForSeconds(0.1f);
        behaviourController.RevokeOverridingBehaviour(this);
    }

    public override void LocalFixedUpdate()
    {
        if (aim)
        {
            behaviourController.GetCamScript.SetTargetOffset(aimPivotOffset, aimCamOffset);
        }
    }

    public override void LocalLateUpdate()
    {
        AimManagement();
    }

    private void Update()
    {
        peekCorner = behaviourController.GetAnimator.GetBool(cornerBool);
        // 버튼을 누르고 있을때만 조준한다.
        if (Input.GetAxisRaw("Aim") != 0 && !aim)
        {
            StartCoroutine(ToggleAimOn());
        }
        else if (aim && Input.GetAxisRaw("Aim") == 0)
        {
            StartCoroutine(ToggleAimOff());
        }

        // 조준중엔 전력질주를 할 수 없음.
        canSprint = !aim;

        if (aim && Input.GetButtonDown("Aim Shoulder") && !peekCorner)
        {
            aimCamOffset.x = aimCamOffset.x * (-1);
            aimPivotOffset.x = aimPivotOffset.x * (-1);
        }
        behaviourController.GetAnimator.SetBool(aimBool, aim);
    }

    private void OnGUI()
    {
        if (crossHair != null)
        {
            float length = behaviourController.GetCamScript.GetCurrentPivotMagnitude(aimPivotOffset);

            if (length < 0.05f)
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - (crossHair.width * 0.5f),
                                        Screen.height * 0.5f - (crossHair.height * 0.5f),
                                        crossHair.width, crossHair.height), crossHair);
            }
        }
    }
}
