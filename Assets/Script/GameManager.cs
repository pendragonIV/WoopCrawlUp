using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public SceneChanger sceneChanger;
    public Player playerScript;
    public GameScene gameScene;
    public GameObject player;
    public GameObject tutorHand;
    #region Game status
    [SerializeField]
    private bool isGameWin = false;
    [SerializeField]
    private bool isLose = false;

    private Level currentLevelData;
    #endregion

    private void Start()
    {
        currentLevelData = LevelManager.instance.levelData.GetLevelAt(LevelManager.instance.currentLevelIndex);
        player.transform.localPosition = currentLevelData.playerSpawnPosition;
        Instantiate(currentLevelData.map);
    }

    public void Win()
    {
        isGameWin = true;

        gameScene.ShowWinPanel();
        LevelManager.instance.levelData.SaveDataJSON();
    }


    public void Lose()
    {
        isLose = true;
        sceneChanger.ChangeToGameScene();   
    }

    public bool IsGameWin()
    {
        return isGameWin;
    }

    public bool IsGameLose()
    {
        return isLose;
    }
}

