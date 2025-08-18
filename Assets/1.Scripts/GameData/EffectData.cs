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

    public void LoadData()
    {
        // Application.dataPath = Assets 폴더까지의 경로
        xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset asset = (TextAsset)ResourceManager.Load(dataPath);

        if (asset == null || asset.text == null) // 데이터가 하나도 없다면
        {
            AddData("New Effect");
            return;
        }

        // 
        using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
        {
            int currentId = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "length":
                            int length = int.Parse(reader.ReadString());
                            names = new string[length];
                            effectClips = new EffectClip[length];
                            break;

                        case "id":
                            currentId = int.Parse(reader.ReadString());
                            effectClips[currentId] = new EffectClip();
                            effectClips[currentId].realId = currentId;
                            break;

                        case "name":
                            names[currentId] = reader.ReadString();
                            break;

                        case "effectType":
                            effectClips[currentId].effectType = Enum.Parse<EffectType>(reader.ReadString());
                            break;

                        case "effectName":
                            effectClips[currentId].effectName = reader.ReadString();
                            break;

                        case "effectPath":
                            effectClips[currentId].effectPath = reader.ReadString();
                            break;
                    }
                }
            }
        }
    }

    public void SaveData()
    {
        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(EFFECT); // 이 키에 대한 값을 넣을것이다 선언
            xml.WriteElementString("length", GetDataCount().ToString()); // 데이터의 길이 얻어옴.

            for (int i = 0; i < names.Length; i++)
            {
                EffectClip clip = effectClips[i];
                xml.WriteStartElement(CLIP);
                xml.WriteElementString("id", i.ToString());
                xml.WriteElementString("name", names[i]);
                xml.WriteElementString("effectType", clip.effectType.ToString());
                xml.WriteElementString("effectPath", clip.effectPath);
                xml.WriteElementString("effectName", clip.effectName);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }

    public override int AddData(string newName)
    {
        if (names == null)
        {
            names = new string[] { newName };
            effectClips = new EffectClip[] { new EffectClip() };
        }
        else
        {
            // 강의에서 제공하는 Array전용 헬퍼 함수 
            names = ArrayHelper.Add(newName, names); //배열에 name을 넣는다.
            effectClips = ArrayHelper.Add(new EffectClip(), effectClips);
        }

        return GetDataCount();
    }

    public override void RemoveData(int index)
    {
        names = ArrayHelper.Remove(index, names);
        if (names.Length == 0)
        {
            names = null;
        }

        effectClips = ArrayHelper.Remove(index, effectClips);
    }

    public void ClearData()
    {
        foreach (EffectClip clip in effectClips)
        {
            clip.ReleaseEffect();
        }
        effectClips = null;
        names = null;
    }

    public EffectClip GetCopy(int index)
    {
        if (index < 0 || index >= effectClips.Length)
        {
            return null;
        }

        EffectClip original = effectClips[index];
        EffectClip clip = new EffectClip();
        clip.effectFullPath = original.effectFullPath;
        clip.effectName = original.effectName;
        clip.effectType = original.effectType;
        clip.effectPath = original.effectPath;
        clip.realId = effectClips.Length;

        return clip;
    }


    /// <summary>
    /// 원하는 인덱스를 프리로딩 해서 찾아준다.
    /// </summary>
    public EffectClip GetClip(int index)
    {
        if (index < 0 || index >= effectClips.Length)
        {
            return null;
        }
        effectClips[index].PreLoad();
        return effectClips[index];
    }

    public override void Copy(int index)
    {
        names = ArrayHelper.Add(names[index], names);
        effectClips = ArrayHelper.Add(GetCopy(index), effectClips);
    }
}
