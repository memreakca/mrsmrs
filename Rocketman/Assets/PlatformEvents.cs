using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEvents : MonoBehaviour
{
    public delegate void LandingEventHandler(GameObject currentPlatform);
    public static event LandingEventHandler OnPlayerLanded;

    public static void PlayerLanded(GameObject currentPlatform)
    {
        OnPlayerLanded?.Invoke(currentPlatform);
    }
}
