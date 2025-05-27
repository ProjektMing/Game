using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class botton : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("level1");
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void TryAgain()
    {
        SceneManager.LoadScene("Start");
    }

    public void Choose()
    {
        SceneManager.LoadScene("choose");
    }

    public void Level2()
    {
        SceneManager.LoadScene("level2");
    }
}