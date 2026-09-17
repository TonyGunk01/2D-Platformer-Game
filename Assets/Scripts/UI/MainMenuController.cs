using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Assign the GameObjects that contain the text components")]
    public GameObject loggedInTextObject;
    public GameObject recoveryKeyTextObject;

    void Start()
    {
        string current = PlayerPrefs.GetString("CurrentUser", "");
        if (!string.IsNullOrEmpty(current) && loggedInTextObject != null)
        {
            SetTextOnObject(loggedInTextObject, $"Logged in as {current}");
        }

        if (PlayerPrefs.HasKey("LastRecoveryKey") && recoveryKeyTextObject != null)
        {
            string key = PlayerPrefs.GetString("LastRecoveryKey", "");
            if (!string.IsNullOrEmpty(key))
                SetTextOnObject(recoveryKeyTextObject, $"Recovery Key: <color=yellow><font=\"LiberationSans SDF\">{key}</font></color>");
        }
    }

    private void SetTextOnObject(GameObject go, string text)
    {
        if (go == null) 
            return;

        var uiText = go.GetComponent<Text>();

        if (uiText != null)
        {
            uiText.text = text;
            return;
        }

        var tmpType = System.Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");

        if (tmpType != null)
        {
            var tmpComp = go.GetComponent(tmpType);
            if (tmpComp != null)
            {
                var prop = tmpType.GetProperty("text");

                if (prop != null)
                    prop.SetValue(tmpComp, text, null);

                return;
            }
        }

        var childText = go.GetComponentInChildren<Text>();
        if (childText != null) 
            childText.text = text;
    }
}