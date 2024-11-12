using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    private bool isBgmLooping;

    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // 초기 볼륨 값 설정
        bgmSource.volume = 0.5f;
        sfxSource.volume = 0.5f;

        // 슬라이더 초기 값 설정
        bgmSlider.value = bgmSource.volume;
        sfxSlider.value = sfxSource.volume;

        // 슬라이더와 볼륨 연동
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(int index, bool loop = true)
    {
        if (index < 0 || index >= bgmClips.Length) return;

        bgmSource.clip = bgmClips[index];
        bgmSource.loop = false;
        isBgmLooping = loop;

        bgmSource.Play();
        if (isBgmLooping)
        {
            InvokeRepeating("LoopBGMCheck", bgmSource.clip.length, bgmSource.clip.length);
        }
    }

    public void StopBGM()
    {
        CancelInvoke("LoopBGMCheck");
        bgmSource.Stop();
    }

    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxClips.Length) return;

        sfxSource.PlayOneShot(sfxClips[index]);
    }

    private void LoopBGMCheck()
    {
        if (!isBgmLooping)
        {
            CancelInvoke("LoopBGMCheck");
            return;
        }

        bgmSource.Play();
    }
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    void Update()
    {
        Debug.Log(bgmSource.volume);
        Debug.Log(sfxSource.volume);
    }
}
