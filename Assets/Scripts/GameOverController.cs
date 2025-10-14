using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Button buttonRestart;

    public void Awake()
    {
        buttonRestart.onClick.AddListener(RespawnPlayer);
    }

    public void PlayerDied()
    {
        gameObject.SetActive(true);
    }

    public void RespawnPlayer()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
