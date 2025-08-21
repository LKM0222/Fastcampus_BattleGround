using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// state -> actions update -> transition (decision) check..
/// state에 필요한 기능들. 애니메이션 콜백들
/// 시야 체크, 찾아 놓은 엄폐물 장소 중 가장 가까운 위치를 찾는 기능
/// </summary>
public class StateController : MonoBehaviour
{
    public GeneralStats generalStats; // 기본 스텟
    public ClassStats statData;
    public string classID; // PISTOL, RIFLE, AK 등

    public ClassStats.Param classStats
    {
        get // Get 할때마다 classId에 지정된 값에 대한 데이터를 불러옴. (AK, LIFLE, PISTOL 등. 엑셀 데이터)
        {
            foreach (ClassStats.Sheet sheet in statData.sheets)
            {
                foreach (ClassStats.Param param in sheet.list)
                {
                    if (param.ID.Equals(classID))
                    {
                        return param;
                    }
                }
            }
            return null;
        }
    }

    public State currentState; // 현재 스테이트
    public State remainState; // 남아있는 스테이트

    public Transform aimTarget;

    public List<Transform> patrolWaypoints; // 패트롤을 위한 wayPoints

    public int bullets;
    [Range(0, 50)] public float viewRadius;// 캐릭터가 볼 수 있는 시야 반경
    [Range(0, 360)] public float viewAngle;
    [Range(0, 25)] public float perceptionRadius;

    [HideInInspector] public float nearRadius;
    [HideInInspector] public NavMeshAgent nav;
    [HideInInspector] public int wayPointIndex;
    [HideInInspector] public int maximumBurst = 7;
    [HideInInspector] public float blindEngageTime = 30f; // 플레이어를 다시 찾는 시간 (플레이어 못찾았을 때, 다시 상태로 돌아가기전, 대기시간.)
    [HideInInspector] public bool targetInSight;
    [HideInInspector] public bool focusSight;
    [HideInInspector] public bool reloading;
    [HideInInspector] public bool hadClearShot; // 쏠 수 있었냐
    [HideInInspector] public bool haveClearShot; // 쏠 수 있냐 // 장애물이 갑자기 생길 수 있기 때문에 하나 더 선언
    [HideInInspector] public int coverHash = -1; // 각자 다른 장애물에 숨을 수 있도록 하는 변수 (적군이 같은 장애물에 숨지 않음.)
    [HideInInspector] public EnemyVariables variables;
    [HideInInspector] public Vector3 personalTarget = Vector3.zero;

    private int magBullets; // 내 잔탄량
    private bool aiActive; // ai가 활성화 되어있는지
    private static Dictionary<int, Vector3> coverSpot; // 장애물 해쉬 딕셔너리 (Static)
    private bool strafing; // npc가 strafing중이냐 (플레이어가 조준하면서 적을 맞추고 있냐 무빙샷중이냐 (구글 검색 해보기))
    private bool aiming; // 조준중인지
    private bool checkedOnLoop, blockedSight;

    [HideInInspector] public EnemyAnimation enemyAnimation;
    [HideInInspector] public CoverLookUp coverLookUp;

    public Vector3 CoverSpot
    {
        get { return coverSpot[GetHashCode()]; }
        set { coverSpot[GetHashCode()] = value; }
    }

    public void TransitionToState(State nextState, Decision decision)
    {
        // decision 에서 nextState로 넘어가라 
        if (nextState != remainState) // 현재 스테이트 유지중인지 확인
        {
            currentState = nextState;
        }
    }

}
