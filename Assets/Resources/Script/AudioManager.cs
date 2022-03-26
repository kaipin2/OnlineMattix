using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Const; //定数を定義している

public class AudioManager : MonoBehaviour
{


    //[SerializeField]
    public static AudioClip[] seList;//BGMを読み込む
    public static AudioSource audioSourceBGM;//BGMの音の大きさを調節するために読み込む

    public static AudioClip[] bgmList;//SEを読み込む
    public static AudioSource audioSourceSE;//SEの音の大きさを調節するために読み込む

    public static AudioSource[] audioSources;

    //BGMのボリューム調節する関数を作成
    public float BGMVolume
    {
        //audioSourceBGMのvolumeをいじることでBGMを調整
        get {return audioSourceBGM.volume;}
        set { audioSourceBGM.volume = value; }
    }

    //SEのボリュームを調節する関数を作成
    public float SEVolume
    {
        //audioSourceSEのvolumeをいじることでSEを調整
        get {return audioSourceSE.volume;}
        set { audioSourceSE.volume = value; }
    }

    //SecneをまたいでもObjectが破壊されないようにする
    static AudioManager Instance = null;

    public static AudioManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<AudioManager>();
        }
        return Instance;
    }

    private void Awake()
    {
        if (this != GetInstance())
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        bgmList = Resources.LoadAll<AudioClip>(Const.CO.BGMFolder);
        seList = Resources.LoadAll<AudioClip>(Const.CO.SEFolder);

        audioSources = GetComponents<AudioSource>();
        audioSourceBGM = audioSources[0];
        audioSourceSE = audioSources[1];

    }

    //BGMを再生する関数を作成
    public void PlayBGM(int index)
    {
        if(audioSourceBGM.clip != bgmList[index])
        {
            audioSourceBGM.clip = bgmList[index];
            audioSourceBGM.Play();
        }
    }

    //SEを再生する関数を作成
    public void PlaySound(int index)
    {
        audioSourceSE.clip = seList[index];
        audioSourceSE.PlayOneShot(seList[index]);
    }

}