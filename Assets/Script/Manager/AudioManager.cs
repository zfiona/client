using UnityEngine;
using System.Collections.Generic;

public class AudioManager  
{
    private AudioSource bg_source;
    private AudioSource click_source;
    private AudioSource other_source;
    private Dictionary<string, AudioClip> audioClipDic;
    private static AudioManager instance = null;
    private Transform audioRoot;
    private float bgVolume = 0.5f;
    private float audioVolume = 1f;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AudioManager();
                instance.Init();
            }
            return instance;
        }
    }
    private void Init()
    {
        audioRoot = GameObject.Find(AppConst.SingleObj).transform;
        audioClipDic = new Dictionary<string, AudioClip>();

        GameObject bgmusic = new GameObject("bgmusic");
        bgmusic.transform.parent = audioRoot;
        bg_source = bgmusic.AddComponent<AudioSource>();
        bg_source.playOnAwake = false;

        GameObject btnclick = new GameObject("btnclick");
        btnclick.transform.parent = audioRoot;
        click_source = btnclick.AddComponent<AudioSource>();
        click_source.playOnAwake = false;

        bgVolume = PlayerPrefs.GetFloat("volumeBg", 1f);
        audioVolume = PlayerPrefs.GetFloat("volumeAudio", 1f);
    }

    private AudioClip GetClip(string name, string suffix)
    {
        if (!audioClipDic.ContainsKey(name))
        {
            AudioClip clip = ResourceMgr.GetInstance.LoadAudio(name, suffix);
            if (clip != null)
                audioClipDic.Add(name, clip);
            else
                GameDebug.LogError("can not find audio:" + name);
        }
        return audioClipDic[name];
    }

    public void PlayBg(string name, string suffix = ".mp3")
    {
        AudioClip clip = GetClip(name, suffix);
        if (clip)
        {
            bg_source.clip = clip;
            bg_source.loop = true;
            bg_source.volume = bgVolume;
            bg_source.Play();
        }
    }

    public void PauseBg(bool stop)
    {
        if (stop)
            bg_source.Pause();
        else
            bg_source.Play();
    }

    public void PlayClick(string name, string suffix = ".mp3")
    {
        AudioClip clip = GetClip(name, suffix);
        if (clip)
        {
            click_source.clip = GetClip(name, suffix);
            click_source.loop = false;
            click_source.volume = audioVolume;
            click_source.Play();
        }
    }

    public void PlayAudio(string name,string suffix = ".mp3")
    {
        AudioClip clip = GetClip(name, suffix);
        if (clip)
        {
            GameObject other = new GameObject("audio");
            other.transform.parent = audioRoot;

            other_source = other.AddComponent<AudioSource>();
            other_source.clip = clip;
            other_source.loop = false;
            other_source.volume = audioVolume;
            other_source.Play();
            Object.Destroy(other, other_source.clip.length + 1);
        }
    }

    public void PlayAudio(string name, int times, string suffix = ".mp3")
    {
        AudioClip clip = GetClip(name, suffix);
        if (clip)
        {
            GameObject other = new GameObject("audio");
            other.transform.parent = audioRoot;

            other_source = other.AddComponent<AudioSource>();
            other_source.playOnAwake = true;
            other_source.clip = clip;
            other_source.volume = audioVolume;
            other_source.loop = true;
            other_source.Play();
            Object.Destroy(other, other_source.clip.length * times + 1);

        }
    }
    public void SetBgVolume(float val)
    {
        bgVolume = val;
        bg_source.volume = val;
        PlayerPrefs.SetFloat("volumeBg", val);
    }

    public void SetAudioVolume(float val)
    {
        audioVolume = val;
        click_source.volume = val;
        PlayerPrefs.SetFloat("volumeAudio", val);
    }
}


