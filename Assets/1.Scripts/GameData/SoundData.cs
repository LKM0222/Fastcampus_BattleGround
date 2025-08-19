using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Text;

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
    private string dataPath = "Data/soundData";
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

    public void LoadData()
    {
        xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset asset = (TextAsset)Resources.Load(dataPath, typeof(TextAsset));

        if (asset == null || asset.text == null)
        {
            AddData("New Sound");
            return;
        }

        using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
        {
            int currentID = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "length":
                            {
                                int length = int.Parse(reader.ReadString());
                                names = new string[length];
                                soundClips = new SoundClip[length];
                            }
                            break;

                        case "clip":
                            break;

                        case "id":
                            {
                                currentID = int.Parse(reader.ReadString());
                                soundClips[currentID] = new SoundClip();
                                soundClips[currentID].realId = currentID;
                            }
                            break;

                        case "name":
                            {
                                names[currentID] = reader.ReadString();
                            }
                            break;

                        case "loops":
                            {
                                int count = int.Parse(reader.ReadString());
                                soundClips[currentID].checkTime = new float[count];
                                soundClips[currentID].setTime = new float[count];
                            }
                            break;

                        case "maxvol":
                            {
                                soundClips[currentID].maxVolume = float.Parse(reader.ReadString());
                            }
                            break;

                        case "pitch":
                            {
                                soundClips[currentID].pitch = float.Parse(reader.ReadString());
                            }
                            break;

                        case "dolpplerlevel":
                            {
                                soundClips[currentID].dopplerLevel = float.Parse(reader.ReadString());
                            }
                            break;

                        case "rollofmode":
                            {
                                soundClips[currentID].rolloffMode = Enum.Parse<AudioRolloffMode>(reader.ReadString());
                            }
                            break;

                        case "mindistance":
                            {
                                soundClips[currentID].minDistance = float.Parse(reader.ReadString());
                            }
                            break;

                        case "maxdistance":
                            {
                                soundClips[currentID].maxDistance = float.Parse(reader.ReadString());
                            }
                            break;

                        case "spratialblend":
                            {
                                soundClips[currentID].sparialBlend = float.Parse(reader.ReadString());
                            }
                            break;

                        case "loop":
                            {
                                soundClips[currentID].isLoop = true;
                            }
                            break;

                        case "clippath":
                            {
                                soundClips[currentID].clipPath = reader.ReadString();
                            }
                            break;

                        case "clipname":
                            {
                                soundClips[currentID].clipName = reader.ReadString();
                            }
                            break;

                        case "checktimeCount":
                            {

                            }
                            break;

                        case "checktime":
                            {
                                SetLoopTime(true, soundClips[currentID], reader.ReadString());
                            }
                            break;

                        case "settime":
                            {
                                SetLoopTime(false, soundClips[currentID], reader.ReadString());
                            }
                            break;

                        case "type":
                            {
                                soundClips[currentID].playType = Enum.Parse<SoundPlayType>(reader.ReadString());
                            }
                            break;
                    }
                }
            }
        }

        //사운드를 미리 로딩함. 나중에 무거우면 제거하기
        foreach (SoundClip clip in soundClips)
        {
            clip.PreLoad();
        }
    }

    void SetLoopTime(bool isCheck, SoundClip clip, string timeString)
    {
        string[] time = timeString.Split('/');
        for (int i = 0; i < time.Length; i++)
        {
            if (time[i] != string.Empty)
            {
                if (isCheck == true)
                {
                    clip.checkTime[i] = float.Parse(time[i]);
                }
                else
                {
                    clip.setTime[i] = float.Parse(time[i]);
                }
            }
        }
    }


    public override int AddData(string newName)
    {
        if (names == null)
        {
            names = new string[] { newName };
            soundClips = new SoundClip[] { new SoundClip() };
        }
        else
        {
            names = ArrayHelper.Add(newName, names);
            soundClips = ArrayHelper.Add(new SoundClip(), soundClips);
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
        soundClips = ArrayHelper.Remove(index, soundClips);
    }

    public SoundClip GetCopy(int index)
    {
        if (index < 0 || index >= soundClips.Length)
        {
            return null;
        }

        SoundClip clip = new SoundClip();
        SoundClip original = soundClips[index];
        clip.realId = index;
        clip.clipPath = original.clipPath;
        clip.clipName = original.clipName;
        clip.maxVolume = original.maxVolume;
        clip.pitch = original.pitch;
        clip.dopplerLevel = original.dopplerLevel;
        clip.rolloffMode = original.rolloffMode;
        clip.minDistance = original.minDistance;
        clip.maxDistance = original.maxDistance;
        clip.sparialBlend = original.sparialBlend;
        clip.isLoop = original.isLoop;
        clip.checkTime = original.checkTime;
        clip.setTime = original.setTime;
        clip.playType = original.playType;

        for (int i = 0; i < clip.checkTime.Length; i++)
        {
            clip.checkTime[i] = original.checkTime[i];
            clip.setTime[i] = original.setTime[i];
        }

        clip.PreLoad();
        return clip;
    }

    public override void Copy(int index)
    {
        names = ArrayHelper.Add(names[index], names);
        soundClips = ArrayHelper.Add(GetCopy(index), soundClips);
    }
}
