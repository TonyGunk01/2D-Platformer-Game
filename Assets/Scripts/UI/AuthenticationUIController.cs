using UnityEngine;
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
    public GameObject buttonPlay;

    [Header("Delete Account UI Elements")]
    public TMP_InputField deleteUser;
    public TMP_InputField deleteKey;
    public TMP_InputField deletePass;

    private string validatedUsername;
    private string validatedRecoveryKey;

    public void OnClickLoginSubmit()
    {
        bool success = authenticationSystem.Login(loginUser.text, loginPass.text);

        if (success)
        {
            statusText.text = "<color=green>Login Successful!</color>";
            ClearAllInputs();

            if (levelSelectionPopUp != null)
                levelSelectionPopUp.SetActive(true);

            loginPopUp.SetActive(false);
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
            statusText.text = $"Account Created! Write down your recovery key: <color=yellow>{generatedKey}</color>";
            buttonPlay.SetActive(true);
            ClearAllInputs();
        }

        else
            statusText.text = $"<color=red>{result}</color>";
    }

    public void OnClickVerifyKeySubmit()
    {
        string username = verifyUser.text.Trim();
        string keyInput = verifyKey.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(keyInput))
        {
            statusText.text = "<color=red>Please fill in all fields.</color>";
            ClearAllInputs();
            return;
        }

        string checkResult = authenticationSystem.ResetPasswordWithKey(username, keyInput, "", "");

        if (checkResult == "Username not found.")
        {
            statusText.text = "<color=red>Username not found.</color>";
            ClearAllInputs();
            return;
        }

        else if (checkResult == "Invalid recovery key.")
        {
            statusText.text = "<color=red>Incorrect recovery key.</color>";
            ClearAllInputs();
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
            deleteUser.text = ""; deletePass.text = "";
        }

        else
            statusText.text = $"<color=red>{result}</color>";
    }

    private void ClearAllInputs()
    {
        loginUser.text = ""; loginPass.text = "";
        regUser.text = ""; regPass.text = ""; regPassConfirm.text = "";
        verifyUser.text = ""; verifyKey.text = "";
        newPass.text = ""; newPassConfirm.text = "";
    }
}