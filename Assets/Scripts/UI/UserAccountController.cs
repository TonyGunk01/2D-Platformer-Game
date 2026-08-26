using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserAccountController : MonoBehaviour
{
    public static UserAccountController Instance;

    void Awake()
    {
        Instance = this;
    }

    public void CreateAccount (string username, string password)
    {

    }
}