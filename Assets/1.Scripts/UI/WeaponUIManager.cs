using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 무기를 획득하면, 획득한 무기를 UI를 통해 보여주고
/// 현재 잔탄량과 전체 소지할 수 있는 총알량을 출력력
/// </summary>
public class WeaponUIManager : MonoBehaviour
{
    public Color bulletColor = Color.white;
    public Color emptyBulletColor = Color.black;
    private Color noBulletColor; // 투명하게 색깔 표시.

    [SerializeField] private Image weaponHUD;
    [SerializeField] private GameObject builletMag;
    [SerializeField] private Text totalBulletsHUD;

    private void Start()
    {
        noBulletColor = new Color(0f, 0f, 0f, 0f);
        Toggle(false);
    }

    public void Toggle(bool active)
    {
        weaponHUD.transform.parent.gameObject.SetActive(active);
    }

    public void UpdateWeaponHUD(Sprite weaponSprite, int bulletLeft, int fullMug, int extraBullets)
    {
        if (weaponSprite != null && weaponHUD.sprite != weaponSprite)
        {
            weaponHUD.sprite = weaponSprite;
            weaponHUD.type = Image.Type.Filled;
            weaponHUD.fillMethod = Image.FillMethod.Horizontal;
        }
        int bulletCount = 0;
        foreach (Transform bullet in builletMag.transform)
        {
            // 잔탄
            if (bulletCount < bulletLeft)
            {
                bullet.GetComponent<Image>().color = bulletColor;
            }
            else if (bulletCount >= fullMug)
            {
                // 넘치는 탄
                bullet.GetComponent<Image>().color = noBulletColor;
            }
            else
            {
                // 사용한 탄
                bullet.GetComponent<Image>().color = emptyBulletColor;
            }
            bulletCount++;
        }

        totalBulletsHUD.text = bulletLeft + "/" + extraBullets;
    }
}
