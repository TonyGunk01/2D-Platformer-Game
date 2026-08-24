using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class UserAccountController : MonoBehaviour
{
    public static UserAccountController Instance;

    void Awake()
    {
        Instance = this;
    }

    public void CreateAccount (string username, string password)
    {
        PlayFabClientAPI.RegisterPlayFabUser
        (
            new RegisterPlayFabUserRequest()
            {
                Username = username,
                Password = password,
            },

            response => 
            { 
                Debug.Log("Account creation successful!"); 
            },

            error => 
            { 
                Debug.Log("Account creation failed!"); 
            }
        );
    }
}