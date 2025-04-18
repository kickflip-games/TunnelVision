﻿/*
================================
Assets for Unity by Makaka Games
================================
 
[Online  Docs -> Updated]: https://makaka.org/unity-assets
[Offline Docs - PDF file]: find it in the package folder.

[Support]: https://makaka.org/support
*/

using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

[HelpURL("https://makaka.org/unity-assets")]
public class LoadScreenControl : MonoBehaviour
{
	// Singleton
	public static LoadScreenControl Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject("_LoadScreenControl");

				go.hideFlags = HideFlags.HideInHierarchy;

				instance = go.AddComponent<LoadScreenControl>();

				DontDestroyOnLoad(go);
			}

			return instance;
		}
	}

	private static LoadScreenControl instance;

	/// <summary>
	/// Loads the level with a custom loading screen.
	/// </summary>
	/// <param name="levelName">Level name to load.</param>
	/// <param name="manualSceneActivation">If set to <c>true</c>,
	/// when the loading is complete,
	/// you must call ActivateScene() to continue.</param>
	/// <param name="customLoadScene">Custom Loading Scene name
	/// (optional).</param>
	public void LoadScene(string levelName,
		bool manualSceneActivation = false,
		string customLoadScene = "LoadScreen")
	{
		StopAllCoroutines();

		StartCoroutine(
			DoLoadLevel(levelName, customLoadScene, manualSceneActivation));
	}

	public void LoadLevel(string levelName, string customLoadScene)
	{
		LoadScene(levelName, false, customLoadScene);
	}
	
	/// <summary>
	/// Return the percentagem of the loading.
	/// The value is between 0-100.
	/// </summary>
	public int Progress = 0;

	private bool _activeScene = false;

	/// <summary>
	/// Activates the scene when you are
	/// </summary>
	public void ActivateScene()
	{
		_activeScene = true;
	}

	public delegate void LoadEvent();
	/// <summary>
	/// Occurs when on scene finished the loading and start waiting
	/// to a call for ActivateScene() to continue.
	/// </summary>
	public event LoadEvent OnStartWaitingEventToActivateScene;

	private IEnumerator DoLoadLevel(
		string name, string customLoadScene, bool manualActivation)
	{	
		// Reset
		Progress = 0;
		OnStartWaitingEventToActivateScene = null;
		_activeScene = true;

		// Load the loading scene
		SceneManager.LoadScene(customLoadScene);

		yield return null;
		yield return null;

		AsyncOperation async = SceneManager.LoadSceneAsync(name);

		if (manualActivation)
		{
			async.allowSceneActivation = false;

			_activeScene = false;
		}

		while (!async.isDone)
		{
			if (manualActivation)
			{
				if (async.progress < 0.9f)
				{
					Progress = Mathf.RoundToInt(
						Mathf.Clamp(async.progress / 0.9f * 100f, 0f, 100f));
				}
				else 
				{
					// the first load phase was completed!
					Progress = 100;

					if (!async.allowSceneActivation)
					{
                        //  notify the end of first loading phase
                        OnStartWaitingEventToActivateScene?.Invoke();

                        // Attention!! You must call
                        // Instance.ActivateScene(); to continue
                        while (!_activeScene)
						{
							yield return null;
						}

						async.allowSceneActivation = true;
					}
				}
			}
			else
			{
				Progress = Mathf.RoundToInt(async.progress * 100f);
			}

			yield return null;
		}

		OnStartWaitingEventToActivateScene = null;
	}

	/// <summary>
	/// Loads the level addictively. but do not use the loading scene.
	/// You must create your GUI, images, etc to display the progress.
	/// Also you must use the two callbacks to display
	/// and hide these elements.
	/// </summary>
	/// <param name="name">Level Name.</param>
	public void LoadLevelAddictive(string name)
	{
		StopAllCoroutines();

		StartCoroutine(DoLoadLevelAddictive(name));
	}

	/// <summary>
	/// Occurs when a scene started to load addictively.
	/// Use this do display your progress GUI, images, etc.
	/// Register this callback before call LoadLevelAddictive.
	/// </summary>
	public event LoadEvent OnStartLoadEventAddictive;

	/// <summary>
	/// Occurs when a scene finishes the loading.
	/// Use this to hide your progress GUI, images, etc.
	/// Register this callback before call LoadLevelAddictive.
	/// </summary>
	public event LoadEvent OnEndLoadEventAddictive;

	private IEnumerator DoLoadLevelAddictive(string name)
	{	
		// Reset the percentage
		Progress = 0;

        OnStartLoadEventAddictive?.Invoke();

        yield return null;
		
		// Load the scene async
		AsyncOperation async =
			SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

		while (!async.isDone) 
		{
			// updates the percentage
			Progress = Mathf.RoundToInt(async.progress * 100f);

			yield return null;
		}

        // Reset the events
        OnEndLoadEventAddictive?.Invoke();

        OnStartLoadEventAddictive = null;
		OnEndLoadEventAddictive = null;
	}
}
