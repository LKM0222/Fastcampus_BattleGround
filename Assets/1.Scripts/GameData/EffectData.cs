using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 추가한 라이브러리
using System;
using System.Xml;
using System.IO;

/// <summary>
/// 이펙트 클립 리스트와 이펙트 파일 이름과 경로를 가지고 있음.
/// 파일을 읽고, 쓰는 기능을 가짐.
/// </summary>
public class EffectData : BaseData
{
    // 배열은 정해둔 크기만 가지기 때문에, 여기서는 안전을 위해 배열을 사용.
    public EffectClip[] effectClips = new EffectClip[0];
    public string clipPath = "Effects/";
    private string xmlFilePath = "";
    private string xmlFileName = "effectData.xml";
    private string dataPath = "Data/effectData";

    // XML 구분자
    private const string EFFECT = "effect"; // 저장 키
    private const string CLIP = "clip"; // 저장 키

    private EffectData() { }
}
