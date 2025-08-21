using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemyVariables
{
    public bool feelAlert; // 현재 위협이 있는지 
    public bool hearAlert; // 발자국 소리같은걸 들었는지
    public bool advanceCoverDecision; // 현재 엄폐물보다 타겟을 공격하는데 더 좋은 엄폐물이 있는지
    public int waitRounds; // 공격중에 다른 총알이 오면 얼마나 기다릴지 (플레이어가 몇발 쏘는거까지 기다릴지)
    public bool repeatShot; // 반복적으로 공격할것이냐
    public float waitInCoverTime; // 엄폐물에 얼마나 기다릴거냐
    public float coverTime; // 얼마동안 숨어있었냐
    public float patrolTimer; // 얼마나 패트롤 하는중이냐
    public float shotTimer; // 총 쏘는 딜레이
    public float startShootTimer;
    public float currentShots; // 현재 발사한 총알 갯수 체크
    public float shotsInRounds; // 얼마나 현재 교전중에서 얼마나 총알을 썼냐 나중에 burst와 연결됨.
    public float blindEngageTimer; // 플레이어 공격 도중, 플레이어가 사라졌을 때, 얼마만큼 대기 후, 패트롤 할것이냐
}
