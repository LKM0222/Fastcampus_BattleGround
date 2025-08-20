using System.Collections;
using System.Collections.Generic;
using FC;
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
    public int fullMag; // 재장전 시, 꽉 채우는 탄창 량
    public int maxBullets; // 장전 한번에 채울 수 있는 최대 량

    private GameObject player, gameController;
    private ShootBehaviour playerInventory;
    private BoxCollider weaponCollider;
    private Rigidbody weaponRigidbody;
    private SphereCollider interactiveRadius;
    private bool pickable;

    //UI
    public GameObject screenHUD;
    public WeaponUIManager weaponHUD;
    private Transform pickHUD;
    public Text pickupHUD_Label;

    public Transform muzzleTransform;

    private void Awake()
    {
        //setting
        gameObject.name = label_weaponName;
        gameObject.layer = LayerMask.NameToLayer(TagAndLayer.LayerName.IgnoreRayCast); // 총에 총을 쏠 수 없어서 ignore
        foreach (Transform tr in transform)
        {
            tr.gameObject.layer = LayerMask.NameToLayer(TagAndLayer.LayerName.IgnoreRayCast);
        }
        player = GameObject.FindGameObjectWithTag(TagAndLayer.TagName.Player);
        playerInventory = player.GetComponent<ShootBehaviour>();
        gameController = GameObject.FindGameObjectWithTag(TagAndLayer.TagName.GameController);

        if (weaponHUD == null)
        {
            if (screenHUD == null)
            {
                screenHUD = GameObject.Find("ScreenHUD");
            }
            weaponHUD = screenHUD.GetComponent<WeaponUIManager>();
        }

        if (pickHUD == null)
        {
            pickHUD = gameController.transform.Find("PickupHUD");
        }

        // 인터랙션을 위한 충돌체 설정
        // 자식에다 붙이는 이유 : 자기 자신에게는 트리거형 구형 콜라이더 붙여야하기 때문, 서로 간섭 안하도록
        weaponCollider = transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        CreateInteractiveRadius(weaponCollider.center);
        weaponRigidbody = gameObject.AddComponent<Rigidbody>();

        if (weaponType.Equals(WeaponType.NONE))
        {
            weaponType = WeaponType.SHORT;
        }
        fullMag = currentMagCapacity;
        maxBullets = totalBullets;
        pickHUD.gameObject.SetActive(false);
        if (muzzleTransform == null)
        {
            muzzleTransform = transform.Find("muzzle");
        }
    }

    private void CreateInteractiveRadius(Vector3 center)
    {
        interactiveRadius = gameObject.AddComponent<SphereCollider>();
        interactiveRadius.center = center;
        interactiveRadius.radius = 1;
        interactiveRadius.isTrigger = true;
    }

    private void TogglePickHUD(bool toggle)
    {
        pickHUD.gameObject.SetActive(toggle);
        if (toggle)
        {
            pickHUD.position = transform.position + Vector3.up * 0.5f;
            Vector3 direction = player.GetComponent<BehaviourController>().playerCamera.forward;
            direction.y = 0f;
            pickHUD.rotation = Quaternion.LookRotation(direction);
            pickupHUD_Label.text = "Pick " + gameObject.name;
        }
    }

    private void UpdateHUD()
    {
        weaponHUD.UpdateWeaponHUD(weaponSprite, currentMagCapacity, fullMag, totalBullets);
    }

    public void Toggle(bool active)
    {
        if (active)
        {
            SoundManager.Instance.PlayOneShotEffect((int)pickSound, transform.position, 0.5f);
        }
        weaponHUD.Toggle(active);
        UpdateHUD();
    }

    private void Update()
    {
        if (pickable && Input.GetKeyDown("Pick"))
        {
            //disable physics weapon
            weaponRigidbody.isKinematic = true;
            weaponCollider.enabled = false;
            playerInventory.AddWeapon(this);
            Destroy(interactiveRadius);
            Toggle(true);
            pickable = false;

            TogglePickHUD(false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject != player && Vector3.Distance(transform.position, player.transform.position) <= 5f)
        {
            SoundManager.Instance.PlayOneShotEffect((int)dropSound, transform.position, 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            pickable = false;
            TogglePickHUD(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player && playerInventory && playerInventory.isActiveAndEnabled)
        {
            pickable = true;
            TogglePickHUD(true);
        }
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        transform.position += Vector3.up;
        weaponRigidbody.isKinematic = false;
        transform.parent = null;
        CreateInteractiveRadius(weaponCollider.center);
        weaponCollider.enabled = true;
        weaponHUD.Toggle(false);
    }

    public bool StartReload()
    {
        // 현재 탄창이 꽉 찼거나, 총 량이 0이라면면
        if (currentMagCapacity == fullMag || totalBullets == 0)
        {
            return false;
        }
        else if (totalBullets < fullMag - currentMagCapacity)
        {
            currentMagCapacity += totalBullets;
            totalBullets = 0;
        }
        else
        {
            totalBullets -= fullMag = currentMagCapacity;
            currentMagCapacity = fullMag;
        }
        return true;
    }

    public void EndReload()
    {
        UpdateHUD();
    }

    public bool Shoot(bool firstShot = true)
    {
        if (currentMagCapacity > 0)
        {
            currentMagCapacity--;
            UpdateHUD();
            return true;
        }

        if (firstShot && noBulletSound != SoundList.None)
        {
            SoundManager.Instance.PlayOneShotEffect((int)noBulletSound, muzzleTransform.position, 5f);
        }
        return false;
    }

    public void ResetBullet()
    {
        currentMagCapacity = fullMag;
        totalBullets = maxBullets;
    }
}