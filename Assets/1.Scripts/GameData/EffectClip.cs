using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이펙트 프리팹과 경로, 타입 등의 속성 데이터를 가지고있음.
/// 프리팹 사전로딩 기능을 가짐 - 풀링을 위한 기능.
/// 이펙트 인스턴스 기능도 갖고 있음. - 풀링과 연계해서 사용하기도 함.
/// </summary>
public class EffectClip
{
    public int realId = 0; // 어떤 클립인지 구별하기 위한 Id
    public EffectType effectType = EffectType.NORMAL;
    public GameObject effectPrefab = null;
    public string effectName = string.Empty;
    public string effectPath = string.Empty;
    public string effectFullPath = string.Empty;

    public EffectClip() { }

    public void PreLoad()
    {
        effectFullPath = effectPath + effectName;
        if (effectFullPath != string.Empty && effectPrefab == null)
        {
            effectPrefab = ResourceManager.Load(effectFullPath) as GameObject;
        }
    }

    public void ReleaseEffect()
    {
        if (effectPrefab != null)
        {
            effectPrefab = null;
        }
    }

    /// <summary>
    /// 원하는 위치에 내가 원하는 이펙트를 생성함.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GameObject Instantiate(Vector3 pos)
    {
        if (effectPrefab == null)
        {
            PreLoad();
        }

        if (effectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(effectPrefab, pos, Quaternion.identity);
            return effect;
        }

        return null;
    }
}
