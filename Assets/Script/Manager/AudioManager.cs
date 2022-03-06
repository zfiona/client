using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private AudioSource bg_source;
    private AudioSource click_source;
    private AudioSource other_source;
    private Dictionary<string, AudioClip> audioClipDic;

    private static AudioManager instance = null;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(AudioManager)) as AudioManager;
                if (instance == null)
                {
                    instance = GameObject.Find(AppConst.SingleObj).AddComponent<AudioManager>();
                    instance.Init();
                }
            }
            return instance;
        }
    }
    private void Init()
    {
        audioClipDic = new Dictionary<string, AudioClip>();

        GameObject bgmusic = new GameObject("bgmusic");
        bgmusic.transform.parent = instance.transform;
        bg_source = bgmusic.AddComponent<AudioSource>();
        bg_source.playOnAwake = false;

        GameObject btnclick = new GameObject("btnclick");
        btnclick.transform.parent = instance.transform;
        click_source = btnclick.AddComponent<AudioSource>();
        click_source.playOnAwake = false;
    }

    private AudioClip GetClip(string name, string suffix)
    {
        name = name + "." + suffix;
        if (!audioClipDic.ContainsKey(name))
        {
            AudioClip clip = ResourceMgr.GetInstance.LoadAudio(name);
            if (clip != null)
                audioClipDic.Add(name, clip);
            else
                GameDebug.LogError("can not find audio:" + name);
        }
        return audioClipDic[name];
    }

    public void PlayBg(string name, string suffix = "ogg")
    {
        AudioClip clip = GetClip(name, suffix);
        if (clip)
        {
            bg_source.clip = clip;
            bg_source.loop = true;
            bg_source.volume = PlayerPrefs.GetFloat("music", 0.5f);
            bg_source.Play();
        }
    }

    public void SetBgVoice()
    {
        bg_source.volume = PlayerPrefs.GetFloat("music", 0.5f);
    }
    public void PauseBg(bool stop)
    {
        if (stop)
            bg_source.Pause();
        else
            bg_source.Play();
    }

    public void PlayClick(string name, string suffix = "ogg")
    {
        AudioClip clip = GetClip(name, suffix);
        if (clip)
        {
            click_source.volume = PlayerPrefs.GetFloat("sound", 0.5f);
            click_source.clip = clip;
            click_source.loop = false;
            click_source.Play();
        }
    }

    public void PlayAudio(string name,string suffix = "ogg")
    {
        AudioClip clip = GetClip(name, suffix);
        if (clip)
        {
            GameObject other = new GameObject("audio");
            other.transform.parent = instance.transform;

            other_source = other.AddComponent<AudioSource>();
            other_source.clip = clip;
            other_source.loop = false;
            other_source.volume = PlayerPrefs.GetFloat("sound", 0.5f);
            other_source.Play();
            Destroy(other, other_source.clip.length + 1);
        }
    }

    public void PlayAudio(string name, int times, string suffix = "ogg")
    {
        AudioClip clip = GetClip(name, suffix);
        if (clip)
        {
            GameObject other = new GameObject("audio");
            other.transform.parent = instance.transform;

            other_source = other.AddComponent<AudioSource>();
            other_source.playOnAwake = true;
            other_source.clip = clip;
            other_source.volume = PlayerPrefs.GetFloat("sound", 0.5f);
            other_source.loop = true;
            other_source.Play();
            Destroy(other, other_source.clip.length * times + 1);

        }
    }

}


