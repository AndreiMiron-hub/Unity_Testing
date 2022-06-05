using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager: MonoBehaviour
{

    public enum AudioChannel { Master, Sfx, Music};

    //public float masterVolumePercent { get; private set; }
    //public float sfxVolumePercent { get; private set; }
    //public float musicVolumePercent { get; private set; }

    public float masterVolumePercent = 1f;
    public float sfxVolumePercent = 1f;
    public float musicVolumePercent = 1f;

    public static AudioManager instance;

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;


    Transform audioListener;
    Transform playerT;

    SoundLibrary library;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            library = GetComponent<SoundLibrary>();
            instance = this;

            DontDestroyOnLoad(gameObject);

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source" + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }

            // 2d sound create.
            GameObject newSfx2DSource = new GameObject("2D sfx source");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            newSfx2DSource.transform.parent = transform;
            // end 2d sound create.
            audioListener = FindObjectOfType<AudioListener>().transform;
            
            if(FindObjectOfType<Player>() != null)
            { 
                playerT = FindObjectOfType<Player>().transform;
            }

            if (PlayerPrefs.GetFloat("master vol", masterVolumePercent) != 0)
            {
                masterVolumePercent = PlayerPrefs.GetFloat("master vol", masterVolumePercent);

            }
            if (PlayerPrefs.GetFloat("sfx vol", sfxVolumePercent) != 0)
            {
                sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", sfxVolumePercent);
            }
            if (PlayerPrefs.GetFloat("music vol", musicVolumePercent) != 0)
            {
                musicVolumePercent = PlayerPrefs.GetFloat("music vol", musicVolumePercent);

            }
            //masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1f);
            //sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1f);
            //musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1f);
        }


    }

    private void Update()
    {
        if(playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                //print("S-a modificat master volume" + masterVolumePercent);
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                //print("S-a modificat music volume"  + musicVolumePercent);

                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                //print("S-a modificat sfx volume" + sfxVolumePercent);

                break;
        }

        musicSources[0].volume = volumePercent * masterVolumePercent;
        musicSources[1].volume = volumePercent * masterVolumePercent;


        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();

    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();


        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {

        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.getClipFromName(soundName), pos);
    }

    public void Play2DSound(string soundName)
    {
        sfx2DSource.PlayOneShot(library.getClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
    void OnLevelWasLoaded(int index)
    {
        if (playerT == null)
        {
            if (FindObjectOfType<Player>() != null)
            {
                playerT = FindObjectOfType<Player>().transform;
            }
        }
    }

}
