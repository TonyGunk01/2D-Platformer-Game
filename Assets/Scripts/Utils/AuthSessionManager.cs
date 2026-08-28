using System;
using UnityEngine;

public static class AuthSessionManager
{
    private const string LoggedInUserKey = "LoggedInUser";
    private static string currentUser = null;

    static AuthSessionManager()
    {
        if (PlayerPrefs.HasKey(LoggedInUserKey))
            currentUser = PlayerPrefs.GetString(LoggedInUserKey, null);
    }

    public static bool IsLoggedIn => !string.IsNullOrEmpty(currentUser);

    public static string CurrentUsername => currentUser;

    public static void SetCurrentUser(string username)
    {
        currentUser = username?.ToLower().Trim();

        if (!string.IsNullOrEmpty(currentUser))
        {
            PlayerPrefs.SetString(LoggedInUserKey, currentUser);
            PlayerPrefs.Save();
        }

        else
            Clear();
    }

    public static void Clear()
    {
        currentUser = null;

        if (PlayerPrefs.HasKey(LoggedInUserKey))
        {
            PlayerPrefs.DeleteKey(LoggedInUserKey);
            PlayerPrefs.Save();
        }
    }
}
