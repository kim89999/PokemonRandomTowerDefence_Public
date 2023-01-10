using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMusic : MonoBehaviour
{
    public GameObject[] GameObjectName;
    public AudioSource AudioSourceMusic;
    public string tag;

    public int stagenum = 1;        // 활성화 시킬 스테이지


    //배경음 유지
    void Awake()
    {
        GameObjectName = GameObject.FindGameObjectsWithTag(tag);

        if (GameObjectName.Length >= 2)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(transform.gameObject);
        AudioSourceMusic = GetComponent<AudioSource>();

    }

    public void PlayMusic()
    {
        if (AudioSourceMusic.isPlaying) return;
        AudioSourceMusic.Play();
    }

    public void StopMusic()
    {
        AudioSourceMusic.Stop();
    }

}


