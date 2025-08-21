using System.Collections;
using System.Collections.Generic;
using FC;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 생명력을 담당.
/// UI업데이트를 함.
/// /// 죽었을 경우, 모든 동작 스크립트를 멈춤.
/// /// 
/// </summary> 
public class PlayerHealth : HealthBase
{
    public float health = 100f;
    public float criticalHealth = 30f;
    public Transform healthHUD;
    public SoundList deathSound;
    public SoundList hitSound;
    public GameObject hurtPrefab;
    public float decayFactor = 0.8f;

    private float totalHealth;
    private RectTransform healthBar, placeHolderBar;
    private Text healthLabel;
    private float originalBarScale;
    private bool critical;

    private void Awake() 
    {
        myAnimator = GetComponent<Animator>();
        totalHealth = health;

        healthBar = healthHUD.Find($"HealthBar/Bar").GetComponent<RectTransform>();
        placeHolderBar = healthHUD.Find($"HealthBar/PlaceHolder").GetComponent<RectTransform>();
        healthLabel = healthHUD.Find($"HealthBar/Label").GetComponent<Text>();
        originalBarScale = healthBar.sizeDelta.x;
        healthLabel.text = "" + (int)health;
    }

    private void Update() {
        if(placeHolderBar.sizeDelta.x > healthBar.sizeDelta.x)
        {
            placeHolderBar.sizeDelta = Vector2.Lerp(placeHolderBar.sizeDelta, healthBar.sizeDelta,
                2f * Time.deltaTime);
        }
    }

    public bool IsFullLife()
    {
        return Mathf.Abs(health - totalHealth) < float.Epsilon; 
    }

    private void UpdateHealthBar()
    {
        healthLabel.text = "" + (int)health;
        float scaleFactor = health / totalHealth;
        healthBar.sizeDelta = new Vector2(scaleFactor * originalBarScale, healthBar.sizeDelta.y);
    }

    private void Kill()
    {
        isDead = true;
        gameObject.layer = TagAndLayer.GetLayerByName(TagAndLayer.LayerName.Default);
        gameObject.tag = TagAndLayer.TagName.Untagged;
        healthHUD.gameObject.SetActive(false);
        healthHUD.parent.Find("WeaponHUD").gameObject.SetActive(false);
        myAnimator.SetBool("Aim", false);
        myAnimator.SetBool("Cover", false);
        myAnimator.SetFloat("Speed", 0f);

        foreach(GenericBehaviour behaviour in GetComponentsInChildren<GenericBehaviour>())
        {
            behaviour.enabled = false;
        }

        SoundManager.Instance.PlayOneShotEffect((int)deathSound, transform.position, 5f);
    }

    public override void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject origin = null)
    {
        health -= damage;

        UpdateHealthBar();

        if(health <= 0)
        {
            Kill();
        }
        else if(health<= criticalHealth && !critical)
        {
            critical = true;
        }

        SoundManager.Instance.PlayOneShotEffect((int)hitSound, location, 1f);
    }
}
