using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public Button buttonPlay;
    public GameObject LevelSelection;

    private void Awake()
    {
        buttonPlay.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        LevelSelection.SetActive(true);
        buttonPlay.gameObject.SetActive(false); 
    }
}
