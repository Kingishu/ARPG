using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    #region 单例模式
    private static AudioManager instance=new AudioManager();
    public static AudioManager Instance=>instance;
    private AudioManager()
    {
        
    }
    #endregion
    private Stack<AudioSource> pool=new Stack<AudioSource>();

    public void Play(string path,Vector3 point,bool loop=false,float volume=1,float spactialBlend=1)
    {
        AudioSource audio=new AudioSource();
        if (pool.Count>0)
        {
            audio=pool.Pop();
            audio.gameObject.SetActive(true);
        }
        else
        {
            GameObject go=new GameObject("audio");
            audio = go.AddComponent<AudioSource>();
        }
        audio.clip = ResourcesManager.Instance.Load<AudioClip>(path);
        audio.transform.position=point;
        audio.loop=loop;
        audio.volume=volume;
        audio.spatialBlend=spactialBlend;
        audio.Play();
        CoroutineHelper.Instance.ExecuteAfterDelay(audio.clip.length, () =>
        {
            Stop(audio);
        });
    }

    public void Stop(AudioSource audio)
    {
        if (audio.gameObject!=null)
        {
            audio.gameObject.SetActive(false);
            audio.Stop();
            pool.Push(audio);
        }
    }
}
