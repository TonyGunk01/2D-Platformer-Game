using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    public Button buttonRestart, buttonLobby, buttonQuit, buttonResume, buttonPause;

    public void Awake()
    {
        buttonRestart.onClick.AddListener(RespawnPlayer);
        buttonLobby.onClick.AddListener(Lobby);
        buttonQuit.onClick.AddListener(QuitGame);
        buttonResume.onClick.AddListener(ResumeGame);
        buttonPause.onClick.AddListener(PauseGame);
    }

    public void PlayerDied()
    {
        SoundManager.Instance.PlayMusic(Sounds.PlayerDeath);
        gameObject.SetActive(true);
    }

    public void RespawnPlayer()
    {
        Debug.Log("Respawning player...");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void Lobby()
    {
        Debug.Log("Returning to Lobby...");
        SceneManager.LoadScene("Lobby");
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game...");
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void PauseGame()
    {
        Debug.Log("Pausing game...");
        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}