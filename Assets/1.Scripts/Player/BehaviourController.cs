using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 동작, 기본 동작, 오버라이딩 동작, 잠긴 동작, 마우스 이동값
/// 땅에 서있는지, GenericBehaviour를 상속받은 동작들을 업데이트 시켜준다.
/// 조건에 대한 제한사항 (점프 중 이동 금지 등)을 깔끔하게 제어하기 위해서 GenericBehaviour를 따로 만들어서 제어하게 됨.
/// </summary>
public class BehaviourController : MonoBehaviour
{
    private List<GenericBehaviour> behaviours; // 동작들
    private List<GenericBehaviour> overrideBehaviours; // 우선시 되는 동작
    private int currentBehaviour; // 현재 동작 hash code
    private int defaultBehaviour; // 기본 동작 hash code
    private int behaviourLocked; // 잠긴 동작 hash code

    //캐싱
    public Transform playerCamera;
    private Animator myAnimator;
    private Rigidbody myRigidBody;
    private ThirdPersonOrbitCam camScript;

    // 속성
    private float h; // horizontal axis
    private float v; // vertical axis
    public float turnSmooting = 0.06f; // 카메라를 향하도록 움직일 떄, 회전 속도    
    private bool changedFOV; // 달리기 동작에 카메라 시야각이 변했을 때, 저장되어있는지 확인
    public float sprintFOV = 100f; // 달리기 시야각
    private Vector3 lastDirection; // 마지막 향했던 방향
    public bool sprint; // 달리기 중인가
    private int hFloat; // 애니메이터 관련 가로축 값
    private int vFloat; // 애니메이터 관련 세로축 값
    private int groundedBool; // 애니메이터 지상에 있는가
    private Vector3 colExtents; // 땅과 충돌체크를 위한 충돌체 영역

    public float GetH { get => h; }
    public float GetV { get => v; }
    public ThirdPersonOrbitCam GetCamScript { get => camScript; }
    public Rigidbody GetRigidbody { get => myRigidBody; }
    public Animator GetAnimator { get => myAnimator; }
    public int GetDefaultBehaviour { get => defaultBehaviour; }
}

public abstract class GenericBehaviour : MonoBehaviour
{
    protected int speedFloat;
    protected BehaviourController behaviourController;
    protected int behaviourCode;
    protected bool canSprint;

    private void Awake()
    {
        behaviourController = GetComponent<BehaviourController>();
        speedFloat = Animator.StringToHash("Speed");
        canSprint = true;

        // 동작 타입을 HashCode로 가지고 있다가 추후에 구별용으로사용
        behaviourCode = GetType().GetHashCode(); // Behaviour를 구분하기 위한 코드 (Hash로 구분하면 좋음)
    }

    public int GetBehaviourCode
    {
        get => behaviourCode;
    }

    public bool AllowSprint
    {
        get => canSprint;
    }

    public virtual void LocalLateUpdate()
    {

    }

    public virtual void LocalFixedUpdate()
    {

    }

    public virtual void OnOverride()
    {

    }
}
