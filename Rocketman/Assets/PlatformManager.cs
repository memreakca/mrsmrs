using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager instance;
    public GameObject parent;
    public GameObject platformPrefab;
    public float maxVerticalLimit;
    public float minVerticalLimit;
    public float minVerticalOffset = 2f;
    public float maxVerticalOffset = 5f; 
    public float minHorizontalOffset = 1f; 
    public float maxHorizontalOffset = 3f;
    public GameObject deathPlatform;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject midpointTarget;

    private void Start()
    {
        deathPlatform = Instantiate(deathPlatform);
    }
    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        PlatformEvents.OnPlayerLanded += SpawnPlatformOnLand;
    }
    private void OnDisable()
    {
        PlatformEvents.OnPlayerLanded -= SpawnPlatformOnLand;
    }

    public void SpawnPlatformOnLand(GameObject currentPlatform)
    {
        if (currentPlatform == null)
        {
            Debug.LogWarning("Current platform is null. Cannot spawn a new platform.");
            return;
        }
        float randomVerticalOffset = Random.Range(minVerticalOffset, maxVerticalOffset);
        float randomHorizontalOffset = Random.Range(minHorizontalOffset, maxHorizontalOffset);

        
        if (Random.value > 0.5f)
        {
            randomVerticalOffset = -randomVerticalOffset;
        }
        

        Vector3 spawnPosition = currentPlatform.transform.position + new Vector3(randomHorizontalOffset, randomVerticalOffset, 0);

        if (spawnPosition.y > maxVerticalLimit)
        {
            spawnPosition.y = maxVerticalLimit;
        }
        else if (spawnPosition.y < minVerticalLimit)
        {
            spawnPosition.y = minVerticalLimit;
        }
        Debug.Log(spawnPosition.y);
        GameObject spawnedplatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        spawnedplatform.transform.SetParent(parent.transform);

        if (Player.instance.isGrounded)
        {
            
            Vector3 midpoint = (spawnedplatform.transform.position + currentPlatform.transform.position) / 2 + new Vector3(0, 2, 0);
            midpointTarget.transform.position = midpoint;
            virtualCamera.Follow = midpointTarget.transform;
            
        }

        float deathPlatformOffsetY = -9.0f;
        deathPlatform.transform.position = spawnedplatform.transform.position + new Vector3(0, deathPlatformOffsetY, 0);
       
        
    }
}
