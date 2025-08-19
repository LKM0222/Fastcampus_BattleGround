using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 루프, 페이드 관련 속성, 오디오클립 속성들.
public class SoundClip
{
    public SoundPlayType playType = SoundPlayType.None;
    public string clipName = string.Empty;
    public string clipPath = string.Empty;
    public float maxVolume = 1.0f; // 100%
    public bool isLoop = false;
    public float[] checkTime = new float[0];
    public float[] setTime = new float[0];
    public int realId = 0;

    private AudioClip clip = null;
    public int currentLoop = 0;
    public float pitch = 1.0f;
    public float dopplerLevel = 1.0f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    public float minDistance = 10000.0f;
    public float maxDistance = 50000.0f;
    public float sparialBlend = 1.0f;

    public float fadeTime1 = 0.0f;
    public float fadeTime2 = 0.0f;
    public Interpolate.Function interpolate_Func;
    public bool isFadeIn = false;
    public bool isFadeOut = false;

    public SoundClip() { }
    public SoundClip(string clipPath, string clipName)
    {
        this.clipPath = clipPath;
        this.clipName = clipName;
    }

    public void PreLoad()
    {
        if (clip == null)
        {
            string fullPath = clipPath + clipName;
            clip = ResourceManager.Load(fullPath) as AudioClip;
        }
    }

    public void AddLoop()
    {
        checkTime = ArrayHelper.Add(0.0f, checkTime);
        setTime = ArrayHelper.Add(0.0f, setTime);
    }

    public void RemoveLoop(int index)
    {
        checkTime = ArrayHelper.Remove(index, checkTime);
        setTime = ArrayHelper.Remove(index, setTime);
    }

    public AudioClip GetClip()
    {
        if (clip == null)
        {
            PreLoad();
        }

        if (clip == null && clipName != string.Empty)
        {
            Debug.LogWarning($"Can not load audio clip Resource {clipName}");
            return null;
        }

        return clip;
    }

    public void ReleaseClip()
    {
        if (clip != null)
        {
            clip = null;
        }
    }

    public bool HasLoop()
    {
        return checkTime.Length > 0;
    }

    public void NextLoop()
    {
        // 특정 구간을 반복하는데 checkTime보다 크다면 다시 반복
        currentLoop++;
        if (currentLoop >= checkTime.Length)
        {
            currentLoop = 0;
        }
    }

    public void CheckLoop(AudioSource source)
    {
        if (HasLoop() && source.time >= checkTime[currentLoop])
        {
            source.time = setTime[currentLoop];
            NextLoop();
        }
    }

    public void FadeIn(float time, Interpolate.EaseType easeType)
    {
        isFadeOut = false;
        fadeTime1 = 0.0f;
        fadeTime2 = time;
        interpolate_Func = Interpolate.Ease(easeType); //easeType은 보간용 그래프를 나타냄.
        isFadeIn = true;
    }

    public void FadeOut(float time, Interpolate.EaseType easeType)
    {
        isFadeIn = false;
        fadeTime1 = 0.0f;
        fadeTime2 = time;
        interpolate_Func = Interpolate.Ease(easeType);
        isFadeOut = true;
    }


    /// <summary>
    /// 페이드 인 아웃 효과 프로세스
    /// </summary>
    /// <param name="time"></param>
    /// <param name="audio"></param>
    public void DoFade(float time, AudioSource audio)
    {
        if (isFadeIn == true)
        {
            fadeTime1 += time;
            audio.volume = Interpolate.Ease(interpolate_Func, 0, maxVolume, fadeTime1, fadeTime2);

            if (fadeTime1 >= fadeTime2)
            {
                isFadeIn = false;
            }
        }
        else if (isFadeOut == true)
        {
            fadeTime1 += time;
            audio.volume = Interpolate.Ease(interpolate_Func, maxVolume, 0 - maxVolume, fadeTime1, fadeTime2);

            if (fadeTime1 >= fadeTime2)
            {
                isFadeOut = false;
                audio.Stop();
            }
        }
    }

}
