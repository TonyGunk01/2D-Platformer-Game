using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text statusText;

    [Header("Credentials")]
    [SerializeField] private string correctUsername = "admin";
    [SerializeField] private string correctPassword = "password123";

    public GameObject UserLogin;
    public GameObject LevelSelection;

    void Start()
    {
        statusText.text = "";
        loginButton.onClick.AddListener(AttemptLogin);
    }

    public void AttemptLogin()
    {
        string enteredUsername = usernameInputField.text;
        string enteredPassword = passwordInputField.text;

        if (string.IsNullOrEmpty(enteredUsername) || string.IsNullOrEmpty(enteredPassword))
        {
            statusText.color = Color.yellow;
            statusText.text = "Please fill in all fields.";
            return;
        }

        if (enteredUsername == correctUsername && enteredPassword == correctPassword)
        {
            statusText.color = Color.green;
            statusText.text = "Login Successful!";
            Invoke("OnLoginSuccess", 1.5f);
        }

        else
        {
            statusText.color = Color.red;
            statusText.text = "Invalid username or password.";
        }
    }

    private void OnLoginSuccess()
    {
        UserLogin.SetActive(false);
        LevelSelection.SetActive(true);
    }
}