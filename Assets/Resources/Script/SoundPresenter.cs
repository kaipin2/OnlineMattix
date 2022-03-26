using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Const; //定数を定義している

public class SoundPresenter : MonoBehaviour
{

    private float _BGMVolume; //前回の音量を保存(BGM)
    private float _SEVolume; //前回の音量を保存(SE)


    
    //private AudioManager AudioManager; //(同オブジェクト内に存在する)AudioManager.csのスクリプトを保存

    //BGM [SerializeField]
    public static TextMeshProUGUI bgmVolumeText;//BGMMenuViewのvolumeTextを取得
    public static Slider bgmSlider;//BGMMenuViewのsliderを取得

    //SE [SerializeField]
    public static TextMeshProUGUI seVolumeText;//SEMenuViewのvolumeTextを取得
    public static Slider seSlider;//SEMenuViewのsliderを取得

    void Start()
    {
        bgmVolumeText = GameObject.Find(Const.CO.Option + Const.CO.BGM + Const.CO.Volume).GetComponent<TextMeshProUGUI>(); ;
        seVolumeText = GameObject.Find(Const.CO.Option + Const.CO.SE + Const.CO.Volume).GetComponent<TextMeshProUGUI>();
        bgmSlider = GameObject.Find(Const.CO.Option + Const.CO.BGM).GetComponent<Slider>();
        seSlider = GameObject.Find(Const.CO.Option + Const.CO.SE).GetComponent<Slider>();

        _BGMVolume = PlayerPrefs.GetFloat(Const.CO.bgmNamePrefKey, bgmSlider.value);
        _SEVolume = PlayerPrefs.GetFloat(Const.CO.seNamePrefKey, seSlider.value);
        bgmSlider.value = _BGMVolume;
        seSlider.value = _SEVolume;
        AudioManager.GetInstance().BGMVolume = bgmSlider.value;
        AudioManager.GetInstance().SEVolume = seSlider.value;

    }

    //BGMを変更(開始)するときに呼び出す関数
    public void ChangeBGM(int number)
    {
        //BGMを再生
        AudioManager.GetInstance().PlayBGM(number);
    }
    //BGMMenuViewのSliderを動かしたときに呼び出す関数
    public void OnChangedBGMSlider()
    {
        PlayerPrefs.SetFloat(Const.CO.bgmNamePrefKey, bgmSlider.value);    //今回の音量をセーブ
        _BGMVolume = bgmSlider.value;
        //Sliderの値に応じてBGMを変更
        AudioManager.GetInstance().BGMVolume = bgmSlider.value;
        //volumeTextの値をSliderのvalueに変更
        bgmVolumeText.text = string.Format("{0:0}", bgmSlider.value * 100);
    }

    //SEMenuViewのSliderを動かしたときに呼び出す関数を作成
    public void OnChangedSESlider()
    {
        PlayerPrefs.SetFloat(Const.CO.seNamePrefKey, seSlider.value);    //今回の音量をセーブ
        _SEVolume = seSlider.value;
        //Sliderの値に応じてSEを変更
        AudioManager.GetInstance().SEVolume = seSlider.value;
        //volumeTextの値をSliderのvalueに変更
        seVolumeText.text = string.Format("{0:0}", seSlider.value * 100);
    }
}