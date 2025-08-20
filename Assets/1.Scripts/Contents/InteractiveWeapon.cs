using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 충돌체를 생성, 무기를 획득할 수 있도록 한다. (루팅)
/// 루팅 후, 충돌체는 제거.
/// 무기를 다시 버릴 수 있어야하며, 땅에 붙어있게 하기 위해 충돌체 다시 붙여줌.
/// 관련해서 UI도 컨트롤 할 수 있고,
/// ShootBehaviour에 획득한 무기를 넣어줌.
/// </summary>
public class InteractiveWeapon : MonoBehaviour
{
    public string label_weaponName; // 무기 이름.
    public SoundList shotSount, reloadSound, pickSound, dropSound, noBulletSound;
    public Sprite weaponSprite;
    public Vector3 rightHandPosition; // 플레이어 오른손의 보정 위치
    public Vector3 relativeRotation; // 플레이어 맞춘 보정을 위한 회전값.
    public float bulletDamage = 10f;
    public float recoilAngle; // 반동.
    public enum WeaponType
    {
        NONE,
        SHORT,
        LONG,
    }

    public enum WeaponMode
    {
        SEMI, // 단발
        BURST, // 점사
        AUTO, // 연사
    }

    public WeaponType weaponType = WeaponType.NONE;
    public WeaponMode weaponMode = WeaponMode.SEMI;
    public int burstSize = 1; // 점사 시 발사 수
    public int currentMagCapacity; // 현재 탄창 량
    public int totalBullets; // 소지하고 있는 전체 총알 량
    public int fullMug; // 재장전 시, 꽉 채우는 탄창 량
    public int maxBullets; // 장전 한번에 채울 수 있는 최대 량

    private GameObject player, gameController;
    private ShootBehaviour playerInventory;
    private BoxCollider weaponCollider;
    private Rigidbody weaponRigidbody;
    private bool pickable;

    //UI
    public GameObject screenHUD;
    public WeaponUIManager weaponHUD;
    private Transform pickHUD;
    public Text pickupHUD_Label;

    public Transform muzzleTransform;
}