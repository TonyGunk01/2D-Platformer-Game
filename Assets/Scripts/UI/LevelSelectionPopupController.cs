using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelSelectionPopupController : MonoBehaviour
{
    [Tooltip("Container that holds level buttons (each child should be named after the scene/level)")]
    public Transform levelsContainer;

    [Tooltip("Optional sprite to use for level stars (GUI_24). Assign in inspector or place in Resources and load manually).")]
    public Sprite gui24Sprite;
    [Tooltip("Optional sprite to use for unfilled stars (GUI_25). Assign in inspector).")]
    public Sprite emptyStarSprite;

    private void OnEnable()
    {
        UpdateLevelStarsUI();
    }

    public void UpdateLevelStarsUI()
    {
        if (levelsContainer == null)
            return;

        for (int i = 0; i < levelsContainer.childCount; i++)
        {
            var child = levelsContainer.GetChild(i);
            if (child == null)
                continue;

            string levelName = child.gameObject.name;

            // Read stars directly from PlayerPrefs to avoid compile-time dependency on LevelManager
            string currentUser = PlayerPrefs.GetString("CurrentUser", "guest");
            if (string.IsNullOrEmpty(currentUser)) currentUser = "guest";
            int stars = PlayerPrefs.GetInt($"{currentUser}_{levelName}_stars", 0);

            var starImages = child.GetComponentsInChildren<Image>(true)
                .Where(img => img != null && img.gameObject != null && img.gameObject.name.ToLower().Contains("star"))
                .OrderBy(img => img.transform.GetSiblingIndex())
                .ToArray();

            for (int s = 0; s < starImages.Length; s++)
            {
                var img = starImages[s];
                if (img == null)
                    continue;

                bool filled = s < stars;
                if (filled)
                {
                    if (gui24Sprite != null)
                        img.sprite = gui24Sprite;

                    img.enabled = true;
                }
                else
                {
                    if (emptyStarSprite != null)
                    {
                        img.sprite = emptyStarSprite;
                        img.enabled = true;
                    }
                    else
                    {
                        img.enabled = false;
                    }
                }
            }
        }
    }
}
