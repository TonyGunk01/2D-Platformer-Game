using System;

[Serializable]
public class UserAccountController
{
    public string username;
    public string passwordHash;
    public string recoveryKey;
}