using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private const string MENU = "MainMenu";
    private const string GAME = "GameScene";

    [SerializeField]
    private Transform sceneTransition;

    private void Start()
    {
        PlayTransition();
    }

    public void PlayTransition()
    {
        sceneTransition.GetComponent<Animator>().Play("SceneTransition");
    }

    public void ChangeToMenu()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScene(MENU));
    }

    public void ChangeToGameScene()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScene(GAME));
    }

    public void ChangeToNextLevel()
    {
        StopAllCoroutines();
        if (LevelManager.instance.currentLevelIndex < LevelManager.instance.levelData.GetLevels().Count - 1)
        {
            LevelManager.instance.currentLevelIndex++;
            StartCoroutine(ChangeScene(GAME));
        }
        else
        {
            Debug.Log("No more levels");
        }
    }


    private IEnumerator ChangeScene(string sceneName)
    {
        //Optional: Add animation here
        sceneTransition.GetComponent<Animator>().Play("SceneTransitionReverse");
        yield return new WaitForSecondsRealtime(1f);

        SceneManager.LoadSceneAsync(sceneName);

    }
}
