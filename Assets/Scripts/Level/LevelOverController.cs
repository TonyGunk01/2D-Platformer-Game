using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelOverController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            LevelManager.Instance.MarkCurrentLevelComplete();
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex <= 4)
                SceneManager.LoadScene(currentSceneIndex+1);
            else
            {
                
            }
        }
    }
}