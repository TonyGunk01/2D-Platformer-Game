using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class LocalAuthenticationController : MonoBehaviour
{
    private string saveDirectory;

    private void Awake()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        saveDirectory = Path.Combine(projectRoot, "UserDatabase");

        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);
    }

    private string HashString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++) 
                builder.Append(bytes[i].ToString("x2"));

            return builder.ToString();
        }
    }

    private string GetFilePath(string username)
    {
        return Path.Combine(saveDirectory, username.ToLower().Trim() + ".json");
    }

    public string DeleteAccount(string username, string password)
    {
        string path = GetFilePath(username);

        if (!File.Exists(path)) 
            return "Username not found.";

        string json = File.ReadAllText(path);
        UserAccountController account = JsonUtility.FromJson<UserAccountController>(json);

        if (account.passwordHash != HashString(password))
            return "Incorrect password.";

        try
        {
            File.Delete(path);
            Debug.Log($"[AuthSystem] Account for {username} was permanently deleted.");
            return "SUCCESS";
        }

        catch (System.Exception e)
        {
            return $"Error deleting file: {e.Message}";
        }
    }

    public bool IsPasswordValid(string password)
    {
        if (password.Length < 8 || password.Length > 16) 
            return false;

        bool hasLetter = Regex.IsMatch(password, @"[a-zA-Z]");
        bool hasNumber = Regex.IsMatch(password, @"[0-9]");
        bool hasSpecial = Regex.IsMatch(password, @"[%#@]");

        return hasLetter && hasNumber && hasSpecial;
    }

    private string GenerateStrictRecoveryKey()
    {
        string specials = "%@#";
        string caps = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string lows = "abcdefghijklmnopqrstuvwxyz";
        string nums = "0123456789";

        char[] keyArray = new char[7];

        keyArray[0] = specials[UnityEngine.Random.Range(0, specials.Length)];
        keyArray[1] = caps[UnityEngine.Random.Range(0, caps.Length)];
        keyArray[2] = lows[UnityEngine.Random.Range(0, lows.Length)];
        keyArray[3] = lows[UnityEngine.Random.Range(0, lows.Length)];
        keyArray[4] = nums[UnityEngine.Random.Range(0, nums.Length)];
        keyArray[5] = nums[UnityEngine.Random.Range(0, nums.Length)];
        keyArray[6] = nums[UnityEngine.Random.Range(0, nums.Length)];

        for (int i = keyArray.Length - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i + 1);
            char temp = keyArray[i];
            keyArray[i] = keyArray[rnd];
            keyArray[rnd] = temp;
        }

        return new string(keyArray);
    }

    public bool Login(string username, string password)
    {
        string path = GetFilePath(username);

        if (!File.Exists(path)) 
            return false;

        string json = File.ReadAllText(path);
        UserAccountController account = JsonUtility.FromJson<UserAccountController>(json);

        return account.passwordHash == HashString(password);
    }

    public string CreateAccount(string username, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return "Fields cannot be empty.";

        if (password != confirmPassword)
            return "Passwords do not match.";

        if (!IsPasswordValid(password))
            return "Password must be 8-16 characters and contain letters, numbers, and one special character (%, #, @).";

        string path = GetFilePath(username);

        if (File.Exists(path))
            return "Username already exists.";

        string strictKey = GenerateStrictRecoveryKey();

        UserAccountController newAccount = new UserAccountController
        {
            username = username.Trim(),
            passwordHash = HashString(password),
            recoveryKey = HashString(strictKey.Trim())
        };

        File.WriteAllText(path, JsonUtility.ToJson(newAccount, true));

        return $"SUCCESS:{strictKey}";
    }

    public string ResetPasswordWithKey(string username, string recoveryKey, string newPassword, string confirmNewPassword)
    {
        string path = GetFilePath(username);

        if (!File.Exists(path)) 
            return "Username not found.";

        if (newPassword != confirmNewPassword) 
            return "Passwords do not match.";

        if (!IsPasswordValid(newPassword)) 
            return "New password does not meet criteria.";

        string json = File.ReadAllText(path);
        UserAccountController account = JsonUtility.FromJson<UserAccountController>(json);

        if (account.recoveryKey == HashString(recoveryKey.Trim()))
        {
            account.passwordHash = HashString(newPassword);
            File.WriteAllText(path, JsonUtility.ToJson(account, true));
            return "SUCCESS";
        }

        return "Invalid recovery key.";
    }
}