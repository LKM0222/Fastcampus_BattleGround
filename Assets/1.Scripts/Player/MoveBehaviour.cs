using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이동과 점프 동작을 담당하는 컴포넌트
/// 충돌 처리에 대한 기능도 포함
/// 기본 동작으로써 작동동
/// </summary>
public class MoveBehaviour : GenericBehaviour
{
    public float walkSpeed = 0.15f; // 이동 속도
    public float runSpeed = 1.0f; // 달리기 속도
    public float sprintSpeed = 2.0f; // 전력질주 속도
    public float speedDampTime = 0.1f; //

    public float jumpHeight = 1.5f; //점프 높이
    public float jumpInertialForce = 10f; // 점프 관성
    public float speed, speedSeeker; // 스피드?

    private int jumpBool;
    private int groundedBool;
    private bool isColliding; // 충돌 체크
    private CapsuleCollider capsuleCollider;
    private Transform myTransform;


    private void Start()
    {
        myTransform = transform;
        capsuleCollider = GetComponent<CapsuleCollider>();
        jumpBool = Animator.StringToHash("Jump");
        groundedBool = Animator.StringToHash("Grounded");
        behaviourController.GetAnimator.SetBool(groundedBool, true);

        // 기본 Behaviour 등록. 모든 컨트롤러의 기본으로 설정함. 어떤 상태에서 항상 여기로 돌아옴.
        behaviourController.SubscribeBehaviour(this);
        behaviourController.RegisterDefaultBehaviour(behaviourCode);
        speedSeeker = runSpeed;
    }

    // 회전


}
