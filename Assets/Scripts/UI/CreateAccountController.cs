using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreateAccountController : MonoBehaviour
{
    string username, password;

    public void UpdateUsername (string user_name)
    {
        username = user_name;
    }

    public void UpdatePassword (string pass_word)
    {
        password = pass_word;
    }

    public void CreateAccount()
    {
        UserAccountController.Instance.CreateAccount(username, password);
    }
}
