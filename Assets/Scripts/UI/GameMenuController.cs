using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    public Button buttonRestart, buttonMainMenu, buttonQuit; 

    public void Awake()
    {
        buttonRestart.onClick.AddListener(RespawnPlayer);
        buttonMainMenu.onClick.AddListener(MainMenu);
        buttonQuit.onClick.AddListener(QuitGame);
    }

    public void PlayerDied()
    {
        SoundManager.Instance.PlayMusic(Sounds.PlayerDeath);
        gameObject.SetActive(true);
    }

    public void RespawnPlayer()
    {
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}