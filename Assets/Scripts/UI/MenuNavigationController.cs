using UnityEngine;

public class MenuNavigationController : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject startScreen;
    public GameObject optionsParent;

    [Header("Sub Options Panels")]
    public GameObject userLoginPanel;
    public GameObject userRegisterPanel;
    public GameObject userVerifyKeyPanel;
    public GameObject userChangePasswordPanel;

    public GameObject buttonBack;

    private void Start()
    {
        ShowStartScreen();
    }

    public void ShowStartScreen()
    {
        startScreen.SetActive(true);
        optionsParent.SetActive(false);
        buttonBack.SetActive(false);
    }

    private void HideAllSubPanels()
    {
        startScreen.SetActive(false);
        optionsParent.SetActive(true);
        buttonBack.SetActive(true);

        userLoginPanel.SetActive(false);
        userRegisterPanel.SetActive(false);
        userVerifyKeyPanel.SetActive(false);
        userChangePasswordPanel.SetActive(false);
    }

    public void OpenLoginPanel()
    {
        HideAllSubPanels();
        userLoginPanel.SetActive(true);
    }

    public void OpenRegisterPanel()
    {
        HideAllSubPanels();
        userRegisterPanel.SetActive(true);
    }

    public void OpenVerifyKeyPanel()
    {
        HideAllSubPanels();
        userVerifyKeyPanel.SetActive(true);
    }

    public void OpenChangePasswordPanel()
    {
        HideAllSubPanels();
        userChangePasswordPanel.SetActive(true);
    }
}