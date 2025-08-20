using System.Collections;
using System.Collections.Generic;
using FC;
using UnityEditor.Search;
using UnityEngine;

/// <summary>
/// 사격 기능 : 사격이 가능한지 여부를 체크하는 기능.
/// 발사 키 입력 받아서 애니메이션 재생, 이펙트 생성, 충돌 체크 기능
/// UI 관련해서 십자선 표시 기능
/// 발사 속도 조절
/// 캐릭터 상체를 IK이용해서 조준 시점에 맞춰서 회전 (반동 구현)
/// 벽이나 충돌체에 총알이 피격되었을경우, 피탄 이펙트 생성
/// 인벤토리 역할(임시)/ 무기를 소지하고 있는지 확인
/// 재장전과 무기 교체 기능까지 포함.
/// </summary>
public class ShootBehaviour : GenericBehaviour
{
    public Texture2D aimCrossHair, shootCrossHair;
    public GameObject muzzleFlash, shot, sparks;
    public Material bulletHole;
    public int maxBulletHoles = 50;
    public float shootErrorRate = 0.01f;
    public float shootRateFactor = 1f;

    public float armsRotation = 8f;

    public LayerMask shotMask = ~(TagAndLayer.LayerMasking.IgnoreRayCast | TagAndLayer.LayerMasking.IgnoreShot |
                                TagAndLayer.LayerMasking.CoverInvisible | TagAndLayer.LayerMasking.Player);
    public LayerMask organicMask = TagAndLayer.LayerMasking.Player | TagAndLayer.LayerMasking.Enemy;

    //짧은 총, 피스톨 같은 총을 들었을 때, 조준시 왼팔의 위치 보정.
    public Vector3 leftArmShortAim = new Vector3(-4.0f, 0.0f, 2.0f);


    private int activeWeapon = 0;
    // animator value
    private int weaponType;
    private int changeWeaponTrigger;
    private int shootingTrigger;
    private int aimBool, blockedAimBool, reloadBool;


    private List<InteractiveWeapon> weapons; // 소지하고 있는 무기들.
    private bool isAiming, isAimBlocked;

    private Transform gunMuzzle;
    private float distToHand;

    private Vector3 castRelativeOrigin;

    private Dictionary<InteractiveWeapon.WeaponType, int> slotMap;
    private Transform hips, spine, chest, rightHand, leftArm;
    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Vector3 initialChestRotation;

    private float shotInterval, originalShotinterval = 0.5f;
    private List<GameObject> bulletHoles;
    private int bulletHoleSlot = 0;
    private int brustShotCount = 0;
    private AimBehaviour aimBehaviour;
    private Texture2D originalCrossHair;
    private bool isShooting = false;
    private bool isChangingWeapon = false;
    private bool isShotAlive = false;

    private void Start()
    {
        weaponType = Animator.StringToHash("Weapon");
        aimBool = Animator.StringToHash("Aim");
        blockedAimBool = Animator.StringToHash("BlockedAim");
        changeWeaponTrigger = Animator.StringToHash("ChangeWeapon");
        shootingTrigger = Animator.StringToHash("Shooting");
        reloadBool = Animator.StringToHash("Reload");
        weapons = new List<InteractiveWeapon>();
        aimBehaviour = GetComponent<AimBehaviour>();
        bulletHoles = new List<GameObject>();

        muzzleFlash.SetActive(false);
        shot.SetActive(false);
        sparks.SetActive(false);

        slotMap = new Dictionary<InteractiveWeapon.WeaponType, int>
        {
            {InteractiveWeapon.WeaponType.SHORT, 1},
            {InteractiveWeapon.WeaponType.LONG, 2}
        };

        Transform neck = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Neck);
        if (!neck)
        {
            neck = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Head).parent;
        }
        hips = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        spine = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine);
        chest = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Chest);
        rightHand = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.RightHand);
        leftArm = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm);

        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;
        initialChestRotation = chest.localEulerAngles;
        originalCrossHair = aimBehaviour.crossHair;
        shotInterval = originalShotinterval;
        castRelativeOrigin = neck.position - transform.position;
        distToHand = (rightHand.position - neck.position).magnitude * 1.5f;
    }

    private void DrawShoot(GameObject weapon, Vector3 destination, Vector3 targetNormal, Transform parent, bool plackSparks = true, bool placeBulletHole = true)
    {
        Vector3 origin = gunMuzzle.position - gunMuzzle.right * 0.5f;

        muzzleFlash.SetActive(true);
        muzzleFlash.transform.SetParent(gunMuzzle);
        muzzleFlash.transform.localPosition = Vector3.zero;
        muzzleFlash.transform.localEulerAngles = Vector3.back * 90f;

        GameObject instantShot = EffectManager.Instance.EffectOneShot((int)EffectList.tracer, origin);
        instantShot.SetActive(true);
        instantShot.transform.rotation = Quaternion.LookRotation(destination - origin);
        instantShot.transform.parent = shot.transform.parent;

        GameObject instantSpark = EffectManager.Instance.EffectOneShot((int)EffectList.sparks, destination);
        instantSpark.SetActive(true);
        instantSpark.transform.parent = sparks.transform.parent;
    }




    /// <summary>
    /// 인벤토리 역할을 하게 될 함수.
    /// </summary>
    /// <param name="weapon"></param>
    public void AddWeapon(InteractiveWeapon weapon)
    {

    }
}
