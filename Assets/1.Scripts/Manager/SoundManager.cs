using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonMonobehaviour<SoundManager>
{
    public const string MasterGroupName = "Master";
    public const string EffectGroupName = "Effect";
    public const string BGMGroupName = "BGM";
    public const string UIGroupName = "UI";
    public const string MixerName = "AudioMixer";
    public const string ContainerName = "SoundContainer";
    public const string FadeA = "FadeA";
    public const string FadeB = "FadeB";
    public const string UI = "UI";
    public const string EffectVolumeParam = "Volume_Effect";
    public const string BGMVolumeParam = "Volume_BGM";
    public const string UIVolumeParam = "Volume_UI";

    public enum MusicPlayingType
    {
        NONE = 0,
        SourceA = 1,
        SourceB = 2,
        AtoB = 3,
        BtoA = 4,
    }

    public AudioMixer mixer = null;
    public Transform audioRoot = null;
    public AudioSource fadeA_audio = null;
    public AudioSource fadeB_audio = null;
    public AudioSource[] effect_audios = null;
    public AudioSource UI_audio = null;

    public float[] effect_PlayStartTime = null;
    private int EffectChannelCount = 5;
    private MusicPlayingType currentPlayingType = MusicPlayingType.NONE;
    private bool isTicking = false;
    private SoundClip currentSound = null;
    private SoundClip lastSound = null;
    private float minVolume = -80.0f;
    private float maxVolume = 0.0f;

    private void Start()
    {
        if (mixer == null)
        {
            mixer = Resources.Load(MixerName) as AudioMixer;
        }
        if (audioRoot == null)
        {
            audioRoot = new GameObject(ContainerName).transform;
            audioRoot.SetParent(transform);
            audioRoot.localPosition = Vector3.zero;
        }
        if (fadeA_audio == null)
        {
            GameObject fadeA = new GameObject(FadeA, typeof(AudioSource));
            fadeA.transform.SetParent(audioRoot);
            fadeA_audio = fadeA.GetComponent<AudioSource>();
            fadeA_audio.playOnAwake = false;
        }
        if (fadeB_audio == null)
        {
            GameObject fadeB = new GameObject(FadeB, typeof(AudioSource));
            fadeB.transform.SetParent(audioRoot);
            fadeB_audio = fadeB.GetComponent<AudioSource>();
            fadeB_audio.playOnAwake = false;
        }
        if (UI_audio == null)
        {
            GameObject ui = new GameObject(UI, typeof(AudioSource));
            ui.transform.SetParent(audioRoot);
            UI_audio = ui.GetComponent<AudioSource>();
            UI_audio.playOnAwake = false;
        }
        if (effect_audios == null || effect_audios.Length == 0)
        {
            effect_PlayStartTime = new float[EffectChannelCount];
            effect_audios = new AudioSource[EffectChannelCount];
            for (int i = 0; i < EffectChannelCount; i++)
            {
                effect_PlayStartTime[i] = 0.0f;
                GameObject effect = new GameObject("Effect" + i.ToString(), typeof(AudioSource));
                effect.transform.SetParent(audioRoot);
                effect_audios[i] = effect.GetComponent<AudioSource>();
                effect_audios[i].playOnAwake = false;
            }
        }

        if (mixer != null)
        {
            fadeA_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];
            fadeB_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];
            UI_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(UIGroupName)[0];
            for (int i = 0; i < effect_audios.Length; i++)
            {
                effect_audios[i].outputAudioMixerGroup = mixer.FindMatchingGroups(EffectGroupName)[0];
            }
        }

        VolumeInit();
    }

    public void SetBGMVolume(float currentRatio)
    {
        currentRatio = Mathf.Clamp01(currentRatio);
        float volume = Mathf.Lerp(minVolume, maxVolume, currentRatio);
        mixer.SetFloat(BGMVolumeParam, volume);
        PlayerPrefs.SetFloat(BGMVolumeParam, volume);
    }

    public float GetBGMVolume()
    {
        if (PlayerPrefs.HasKey(BGMVolumeParam))
        {
            return Mathf.Lerp(minVolume, maxVolume, PlayerPrefs.GetFloat(BGMVolumeParam));
        }
        else
        {
            return maxVolume;
        }
    }

    public void SetEffectVolume(float currentRatio)
    {
        currentRatio = Mathf.Clamp01(currentRatio);
        float volume = Mathf.Lerp(minVolume, maxVolume, currentRatio);
        mixer.SetFloat(EffectVolumeParam, volume);
        PlayerPrefs.SetFloat(EffectVolumeParam, volume);
    }

    public float GetEffectVolume()
    {
        if (PlayerPrefs.HasKey(EffectVolumeParam))
        {
            return Mathf.Lerp(minVolume, maxVolume, PlayerPrefs.GetFloat(EffectVolumeParam));
        }
        else
        {
            return maxVolume;
        }
    }

    public void SetUIVolume(float currentRatio)
    {
        currentRatio = Mathf.Clamp01(currentRatio);
        float volume = Mathf.Lerp(minVolume, maxVolume, currentRatio);
        mixer.SetFloat(UIVolumeParam, volume);
        PlayerPrefs.SetFloat(UIVolumeParam, volume);
    }

    public float GetUIVolume()
    {
        if (PlayerPrefs.HasKey(UIVolumeParam))
        {
            return Mathf.Lerp(minVolume, maxVolume, PlayerPrefs.GetFloat(UIVolumeParam));
        }
        else
        {
            return maxVolume;
        }
    }

    private void VolumeInit()
    {
        if (mixer != null)
        {
            mixer.SetFloat(BGMVolumeParam, GetBGMVolume());
            mixer.SetFloat(EffectVolumeParam, GetEffectVolume());
            mixer.SetFloat(UIVolumeParam, GetUIVolume());
        }
    }

    void PlayAudioSource(AudioSource source, SoundClip clip, float volume)
    {
        if (source == null || clip == null)
        {
            return;
        }
        source.Stop();
        source.clip = clip.GetClip();
        source.volume = volume;
        source.loop = clip.isLoop;
        source.pitch = clip.pitch;
        source.dopplerLevel = clip.dopplerLevel;
        source.rolloffMode = clip.rolloffMode;
        source.minDistance = clip.minDistance;
        source.maxDistance = clip.maxDistance;
        source.spatialBlend = clip.spartialBlend;
        source.Play();
    }

    void PlayAudioSourceAtPoint(SoundClip clip, Vector3 position, float volume)
    {
        AudioSource.PlayClipAtPoint(clip.GetClip(), position, volume);
    }

    public bool IsPlaying()
    {
        return (int)currentPlayingType > 0; //None만 아니면 뭐든 재생중인것.
    }

    public bool IsDifferentSound(SoundClip clip) //동일한 사운드인지 확인 (같은 사운드 재생하지 않게 체크하는 함수)
    {
        if (clip == null)
        {
            return false;
        }

        if (currentSound != null && currentSound.realId == clip.realId && IsPlaying() && currentSound.isFadeOut == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private IEnumerator CheckProcess()
    {
        while (isTicking == true && IsPlaying() == true)
        {
            yield return new WaitForSeconds(0.05f); // 최적화 필요하다면 체크 간격 줄이기
            if (currentSound.HasLoop())
            {
                if (currentPlayingType == MusicPlayingType.SourceA)
                {
                    currentSound.CheckLoop(fadeA_audio);
                }
                else if (currentPlayingType == MusicPlayingType.SourceB)
                {
                    currentSound.CheckLoop(fadeB_audio);
                }
                else if (currentPlayingType == MusicPlayingType.AtoB)
                {
                    lastSound.CheckLoop(fadeA_audio);
                    currentSound.CheckLoop(fadeB_audio);
                }
                else if (currentPlayingType == MusicPlayingType.BtoA)
                {
                    lastSound.CheckLoop(fadeB_audio);
                    currentSound.CheckLoop(fadeA_audio);
                }
            }
        }
    }

    public void DoCheck()
    {
        StartCoroutine(CheckProcess());
    }

    public void FadeIn(SoundClip clip, float time, Interpolate.EaseType ease)
    {
        if (IsDifferentSound(clip))
        {
            fadeA_audio.Stop();
            fadeB_audio.Stop();
            lastSound = currentSound;
            currentSound = clip;
            PlayAudioSource(fadeA_audio, currentSound, 0.0f);
            currentSound.FadeIn(time, ease);
            currentPlayingType = MusicPlayingType.SourceA;
            if (currentSound.HasLoop() == true)
            {
                isTicking = true;
                DoCheck();
            }
        }
    }

    public void FadeIn(int index, float time, Interpolate.EaseType ease)
    {
        this.FadeIn(DataManager.SoundData().GetCopy(index), time, ease);
    }

    public void FadeOut(float time, Interpolate.EaseType ease)
    {
        if (currentSound != null)
        {
            currentSound.FadeOut(time, ease);
        }
    }

    private void Update()
    {
        if (currentSound == null)
        {
            return;
        }

        switch (currentPlayingType)
        {
            case MusicPlayingType.SourceA:
                {
                    currentSound.DoFade(Time.deltaTime, fadeA_audio);
                }
                break;
            case MusicPlayingType.SourceB:
                {
                    currentSound.DoFade(Time.deltaTime, fadeB_audio);
                }
                break;
            case MusicPlayingType.AtoB:
                {
                    lastSound.DoFade(Time.deltaTime, fadeA_audio);
                    currentSound.DoFade(Time.deltaTime, fadeB_audio);
                }
                break;
            case MusicPlayingType.BtoA:
                {
                    lastSound.DoFade(Time.deltaTime, fadeB_audio);
                    currentSound.DoFade(Time.deltaTime, fadeA_audio);
                }
                break;
        }

        if (fadeA_audio.isPlaying && fadeB_audio.isPlaying == false)
        {
            currentPlayingType = MusicPlayingType.SourceA;
        }
        else if (fadeA_audio.isPlaying == false && fadeB_audio.isPlaying)
        {
            currentPlayingType = MusicPlayingType.SourceB;
        }
        else if (fadeA_audio.isPlaying == false && fadeB_audio.isPlaying == false)
        {
            currentPlayingType = MusicPlayingType.NONE;
        }
    }

    public void FadeTo(SoundClip clip, float time, Interpolate.EaseType ease) // 한 사운드를 다른 사운드로 바꿈
    {
        if (currentPlayingType == MusicPlayingType.NONE)
        {
            FadeIn(clip, time, ease);
        }
        else if (IsDifferentSound(clip))
        {
            if (currentPlayingType == MusicPlayingType.AtoB)
            {
                fadeA_audio.Stop();
                currentPlayingType = MusicPlayingType.SourceB;
            }
            else if (currentPlayingType == MusicPlayingType.BtoA)
            {
                fadeB_audio.Stop();
                currentPlayingType = MusicPlayingType.SourceA;
            }

            lastSound = currentSound;
            currentSound = clip;
            lastSound.FadeOut(time, ease);
            currentSound.FadeIn(time, ease);
            if (currentPlayingType == MusicPlayingType.SourceA)
            {
                PlayAudioSource(fadeB_audio, currentSound, 0.0f);
                currentPlayingType = MusicPlayingType.AtoB;
            }
            if (currentPlayingType == MusicPlayingType.SourceB)
            {
                PlayAudioSource(fadeA_audio, currentSound, 0.0f);
                currentPlayingType = MusicPlayingType.BtoA;
            }

            if (currentSound.HasLoop())
            {
                isTicking = true;
                DoCheck();
            }
        }
    }

    public void FadeTo(int index, float time, Interpolate.EaseType ease)
    {
        FadeTo(DataManager.SoundData().GetCopy(index), time, ease);
    }

    public void PlayBGM(SoundClip clip)
    {
        if (IsDifferentSound(clip))
        {
            fadeB_audio.Stop();
            lastSound = currentSound;
            currentSound = clip;
            PlayAudioSource(fadeA_audio, clip, clip.maxVolume);
            if (currentSound.HasLoop())
            {
                isTicking = true;
                DoCheck();
            }
        }
    }

    public void PlayBGM(int index)
    {
        SoundClip clip = DataManager.SoundData().GetCopy(index);
        PlayBGM(clip);
    }

    public void PlayUISound(SoundClip clip)
    {
        PlayAudioSource(UI_audio, clip, clip.maxVolume);
    }

    public void PlayEffectSound(SoundClip clip)
    {
        bool isPlaySuccess = false;
        for (int i = 0; i < EffectChannelCount; i++)
        {
            if (effect_audios[i].isPlaying == false)
            {
                PlayAudioSource(effect_audios[i], clip, clip.maxVolume);
                effect_PlayStartTime[i] = Time.realtimeSinceStartup;
                isPlaySuccess = true;
                break;
            }
            else if (effect_audios[i].clip == clip.GetClip())
            {
                effect_audios[i].Stop();
                PlayAudioSource(effect_audios[i], clip, clip.maxVolume);
                effect_PlayStartTime[i] = Time.realtimeSinceStartup;
                isPlaySuccess = true;
                break;
            }
        }

        if (isPlaySuccess == false)
        {
            float maxTime = 0.0f;
            int selectIndex = 0;
            for (int i = 0; i < EffectChannelCount; i++)
            {
                if (effect_PlayStartTime[i] > maxTime)
                {
                    maxTime = effect_PlayStartTime[i];
                    selectIndex = i;
                }
            }

            PlayAudioSource(effect_audios[selectIndex], clip, clip.maxVolume);
        }
    }

    public void PlayEffectSound(SoundClip clip, Vector3 pos, float volume)
    {
        bool isPlaySuccess = false;
        for (int i = 0; i < EffectChannelCount; i++)
        {
            if (effect_audios[i].isPlaying == false)
            {
                PlayAudioSourceAtPoint(clip, pos, volume);
                effect_PlayStartTime[i] = Time.realtimeSinceStartup;
                isPlaySuccess = true;
                break;
            }
            else if (effect_audios[i].clip == clip.GetClip())
            {
                effect_audios[i].Stop();
                PlayAudioSourceAtPoint(clip, pos, volume);
                effect_PlayStartTime[i] = Time.realtimeSinceStartup;
                isPlaySuccess = true;
                break;
            }
        }

        if (isPlaySuccess == false)
        {
            PlayAudioSourceAtPoint(clip, pos, volume);
        }
    }

    public void PlayOneShotEffect(int index, Vector3 pos, float volume)
    {
        if (index == (int)SoundList.None)
        {
            return;
        }

        SoundClip clip = DataManager.SoundData().GetCopy(index);
        if (clip == null)
        {
            return;
        }
        PlayEffectSound(clip, pos, volume);
    }

    public void PlayOneShot(SoundClip clip)
    {
        if (clip == null)
        {
            return;
        }

        switch (clip.playType)
        {
            case SoundPlayType.EFFECT:
                {
                    PlayEffectSound(clip);
                }
                break;
            case SoundPlayType.BGM:
                {
                    PlayBGM(clip);
                }
                break;
            case SoundPlayType.UI:
                {
                    PlayUISound(clip);
                }
                break;
        }
    }

    public void Stop(bool allStop = false)
    {
        if (allStop)
        {
            fadeA_audio.Stop();
            fadeB_audio.Stop();
        }

        FadeOut(0.5f, Interpolate.EaseType.Linear);
        currentPlayingType = MusicPlayingType.NONE;
        StopAllCoroutines();
    }
}
