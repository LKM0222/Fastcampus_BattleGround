using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;

/// <summary>
/// 사운드 클립을 배열로 소지, 사운드 데이터를 저장하고, 로드하고, 
/// 프리로딩을 갖고 있다.
/// </summary>
public class SoundData : BaseData
{
    public SoundClip[] soundClips = new SoundClip[0];

    private string clipPath = "Sound/";
    private string xmlFilePath = "";
    private string xmlFileName = "soundData.xml";
    private static string SOUND = "sound";
    private static string CLIP = "clip";

    public SoundData() { }

    public void SaveData()
    {
        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(SOUND);
            xml.WriteElementString("length", GetDataCount().ToString());
            xml.WriteWhitespace("\n");

            for (int i = 0; i < names.Length; i++)
            {
                SoundClip clip = soundClips[i];
                xml.WriteStartElement(CLIP);
                xml.WriteElementString("id", i.ToString());
                xml.WriteElementString("name", names[i]);
                xml.WriteElementString("loops", clip.checkTime.Length.ToString());
                xml.WriteElementString("maxvol", clip.maxVolume.ToString());
                xml.WriteElementString("pitch", clip.pitch.ToString());
                xml.WriteElementString("dopplerlevel", clip.dopplerLevel.ToString());
                xml.WriteElementString("rollofmode", clip.rolloffMode.ToString());
                xml.WriteElementString("mindistance", clip.minDistance.ToString());
                xml.WriteElementString("maxdistance", clip.maxDistance.ToString());
                xml.WriteElementString("spartialblend", clip.sparialBlend.ToString());

                if (clip.isLoop == true)
                {
                    xml.WriteElementString("loop", "true");
                }

                xml.WriteElementString("clippath", clip.clipPath);
                xml.WriteElementString("clipname", clip.clipName);
                xml.WriteElementString("checktimecount", clip.checkTime.Length.ToString());

                string str = "";
                foreach (float t in clip.checkTime)
                {
                    str += t.ToString() + "/";
                }
                xml.WriteElementString("checktime", str);
                str = "";
                xml.WriteElementString("settimecount", clip.setTime.Length.ToString());
                foreach (float t in clip.setTime)
                {
                    str += t.ToString() + "/";
                }
                xml.WriteElementString("settime", str);
                xml.WriteElementString("type", clip.playType.ToString());

                xml.WriteEndElement();
            }
            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }


    public override int AddData(string newName)
    {
        return base.AddData(newName);
    }

    public override void RemoveData(int index)
    {
        base.RemoveData(index);
    }
}
