using System;

[Serializable]
public class UserAccountController
{
    public string username;
    public string passwordHash;
    public string recoveryKey;

    [Serializable]
    public class LevelStats
    {
        public string levelName;
        public int bestScore;
        public float bestTime;
        public string bestTimeDisplay;
        public string date;
    }

    public LevelStats[] levelStats;
}
