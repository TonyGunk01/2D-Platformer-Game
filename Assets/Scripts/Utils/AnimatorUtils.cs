using System;
using UnityEngine;

public static class AnimatorUtils
{
    // Pause all Animator components in the scene except those matching the provided name or tag (case-insensitive).
    public static void PauseAllExcept(string exceptNameOrTag)
    {
        if (string.IsNullOrEmpty(exceptNameOrTag))
            exceptNameOrTag = "Ellen"; // default

        Animator[] anims = UnityEngine.Object.FindObjectsOfType<Animator>();
        foreach (var a in anims)
        {
            if (a == null)
                continue;

            bool isExcept = string.Equals(a.gameObject.name, exceptNameOrTag, StringComparison.OrdinalIgnoreCase)
                            || (a.gameObject.CompareTag(exceptNameOrTag));

            if (isExcept)
            {
                // ensure the excepted animator continues updating when timeScale = 0
                a.updateMode = AnimatorUpdateMode.UnscaledTime;
                // leave its speed as-is
            }
            else
            {
                // pause other animators by setting speed to 0
                try
                {
                    a.speed = 0f;
                }
                catch { }
            }
        }
    }
}
