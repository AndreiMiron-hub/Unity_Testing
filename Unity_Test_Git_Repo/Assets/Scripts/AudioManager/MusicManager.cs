using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
	public AudioClip mainTheme;
	public AudioClip menuTheme;
	public string sceneName;

	void Start()
	{
		AudioManager.instance.PlayMusic(menuTheme, 2);
		OnLevelWasLoaded(0);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Period))
		{
			AudioManager.instance.PlayMusic(mainTheme, 3);
		}

	}
	void OnLevelWasLoaded(int sceneIndex)
	{
		string newSceneName = SceneManager.GetActiveScene().name;
		if (newSceneName != sceneName)
		{
			sceneName = newSceneName;
			Invoke("PlayMusic", .2f);
		}
	}
	void PlayMusic()
	{
		AudioClip clipToPlay = null;

		if (sceneName == "Menu")
		{
			clipToPlay = menuTheme;
		}
		else if (sceneName == "Dev")
		{
			clipToPlay = mainTheme;
		}

		if (clipToPlay != null)
		{
			AudioManager.instance.PlayMusic(clipToPlay, 2);
			Invoke("PlayMusic", clipToPlay.length);
		}

	}
}
