using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Button buttonRestart, buttonLobby, buttonQuit; 

    public void Awake()
    {
        buttonRestart.onClick.AddListener(RespawnPlayer);
        buttonLobby.onClick.AddListener(Lobby);
        buttonQuit.onClick.AddListener(QuitGame);
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

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}