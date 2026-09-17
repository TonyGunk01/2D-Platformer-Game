using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AuthenticationUIController : MonoBehaviour
{
    [Header("Core Systems")]
    public LocalAuthenticationController authenticationSystem;
    public TMP_Text statusText;

    [Header("Login UI Elements")]

    public TMP_InputField loginUser;
    public TMP_InputField loginPass;

    [Header("Register UI Elements")]
    public TMP_InputField regUser;
    public TMP_InputField regPass;
    public TMP_InputField regPassConfirm;

    [Header("Verify Key UI Elements")]
    public TMP_InputField verifyUser;
    public TMP_InputField verifyKey;

    [Header("Change Password UI Elements")]
    public TMP_InputField newPass;
    public TMP_InputField newPassConfirm;

    [Header("Pop-ups")]
    public GameObject levelSelectionPopUp;
    public GameObject loginPopUp;
    public GameObject verifyPopUp;
    public GameObject changePswdPopUp;
    public GameObject deletePopUp;
    public GameObject optionsPopUp;
    public GameObject startscreenPopUp;
    public GameObject buttonCreate;
    public GameObject buttonPlay;

    [Header("Delete Account UI Elements")]
    public TMP_InputField deleteUser;
    public TMP_InputField deleteKey;
    public TMP_InputField deletePass;

    private string validatedUsername;
    private string validatedRecoveryKey;

    private System.Collections.IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    public void OnClickLoginSubmit()
    {
        bool success = authenticationSystem.Login(loginUser.text, loginPass.text);

        if (success)
        {
            statusText.text = "<color=green>Login Successful!</color>";
            ClearAllInputs();

            StartCoroutine(LoadSceneAfterDelay("Main Menu", 3f));
        }

        else
        {
            statusText.text = "<color=red>Invalid username or password. Please try again.</color>";
            ClearAllInputs();
            loginPopUp.SetActive(true);
        }
    }

    public void OnClickRegisterSubmit()
    {
        string result = authenticationSystem.CreateAccount(regUser.text, regPass.text, regPassConfirm.text);

        if (result.StartsWith("SUCCESS:"))
        {
            string generatedKey = result.Split(':')[1];

            PlayerPrefs.SetString("LastRecoveryKey", generatedKey);
            PlayerPrefs.Save();

            statusText.text = $"<color=green>Account Created! Logging in...</color>";
            ClearAllInputs();
            StartCoroutine(LoadSceneAfterDelay("Main Menu", 3f));
        }

        else
            statusText.text = $"<color=red>{result}</color>";
    }

    public void OnClickLogout()
    {
        statusText.text = $"<color=red>Logged out successfully...</color>";
        StartCoroutine(LoadSceneAfterDelay("Home Page", 3f));
    }

    public void OnClickVerifyKeySubmit()
    {
        string username = verifyUser.text.Trim();
        string keyInput = verifyKey.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(keyInput))
        {
            statusText.text = "<color=red>Please fill in all fields.</color>";
            return;
        }

        string checkResult = authenticationSystem.ResetPasswordWithKey(username, keyInput, "", "");

        if (checkResult == "Username not found.")
        {
            statusText.text = "<color=red>Username not found.</color>";
            return;
        }

        else if (checkResult == "Invalid recovery key.")
        {
            statusText.text = "<color=red>Incorrect recovery key.</color>";
            return;
        }

        validatedUsername = username;
        validatedRecoveryKey = keyInput;

        statusText.text = "<color=green>Verified! Enter your new password below.</color>";
        ClearAllInputs();

        if (verifyPopUp != null) 
            verifyPopUp.SetActive(false);

        if (changePswdPopUp != null) 
            changePswdPopUp.SetActive(true);
    }

    public void OnClickChangePasswordSubmit()
    {
        string result = authenticationSystem.ResetPasswordWithKey(
            validatedUsername,
            validatedRecoveryKey,
            newPass.text,
            newPassConfirm.text
        );

        if (result == "SUCCESS")
        {
            statusText.text = "<color=green>Password updated.</color>";
            ClearAllInputs();

            validatedUsername = "";
            validatedRecoveryKey = "";

            optionsPopUp.SetActive(false);
            startscreenPopUp.SetActive(true);

            if (SceneManager.GetActiveScene().name == "Main Menu")
                StartCoroutine(LoadSceneAfterDelay("Home Page", 3f));
        }

        else
            statusText.text = $"<color=red>{result}</color>";
    }

    public void OnClickDeleteAccountSubmit()
    {
        string username = deleteUser.text.Trim();
        string passInput = deletePass.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passInput))
        {
            statusText.text = "<color=red>Please fill in all fields.</color>";
            return;
        }

        string result = authenticationSystem.DeleteAccount(username, passInput);

        if (result == "SUCCESS")
        {
            statusText.text = "<color=green>Account permanently deleted from database.</color>";
            ClearAllInputs();

            deletePopUp.SetActive(false);
            startscreenPopUp.SetActive(true);

            deleteUser.text = ""; 
            deletePass.text = "";

            if (SceneManager.GetActiveScene().name == "Main Menu")
                StartCoroutine(LoadSceneAfterDelay("Home Page", 3f));
        }

        else
            statusText.text = $"<color=red>{result}</color>";
    }

    public void ClearAllInputs()
    {
        loginUser.text = ""; 
        loginPass.text = "";
        regUser.text = ""; 
        regPass.text = ""; 
        regPassConfirm.text = "";
        verifyUser.text = ""; 
        verifyKey.text = "";
        newPass.text = ""; 
        newPassConfirm.text = "";
    }
}