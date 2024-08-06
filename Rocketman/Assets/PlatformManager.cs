using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager instance;
    public GameObject parent;

    public GameObject platformPrefab;
    public GameObject fuelPrefab;

    public float maxVerticalLimit;
    public float minVerticalLimit;

    [Header("NormalPlatformAttributes")]
    public float minNormalVerticalOffset = 2f;
    public float maxNormalVerticalOffset = 5f;
    public float minNormalHorizontalOffset = 1f;
    public float maxNormalHorizontalOffset = 3f;

    [Header("FuelPlatformAttributes")]
    public float minFuelVerticalOffset = 2f;
    public float maxFuelVerticalOffset = 5f;
    public float minFuelHorizontalOffset = 1f;
    public float maxFuelHorizontalOffset = 3f;

    public float fuelPosminVerticalFuelOffset = 1f;
    public float fuelPosmaxVerticalFuelOffset = 1f;
    public float fuelPosminHorizontalFuelOffset = 1f;
    public float fuelPosmaxHorizontalFuelOffset = 1f;

    [Header("Camera Attiributes")]
    public float fuelCameraFarSightSize;
    public float normalCameraFarSightSize;
    public GameObject deathPlatform;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject midpointTarget;

    public int platformCount;

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

        Player.instance.maxFuel += 0.2f;
        fuelCameraFarSightSize += 0.01f;
        normalCameraFarSightSize += 0.01f;
        UpgradeDifficulty();

        if (Random.value > 0.49f)
        {
            SpawnNormalPlatform(currentPlatform);
        }
        else
        {
            SpawnFuelPlatform(currentPlatform);
        }


    }

    public void SpawnNormalPlatform(GameObject currentPlatform)
    {
        float randomVerticalOffset = Random.Range(minNormalVerticalOffset, maxNormalVerticalOffset);
        float randomHorizontalOffset = Random.Range(minNormalHorizontalOffset, maxNormalHorizontalOffset);

        Vector3 spawnPosition = currentPlatform.transform.position + new Vector3(randomHorizontalOffset, randomVerticalOffset, 0);
        spawnPosition = resetSpawnPosY(spawnPosition);

        GameObject spawnedplatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        spawnedplatform.transform.SetParent(parent.transform);

        if (Player.instance.isGrounded)
        {
            Vector3 midpoint = (spawnedplatform.transform.position + currentPlatform.transform.position) / 2 + new Vector3(0, 2, 0);
            StartCoroutine(LerpCamera(midpoint, normalCameraFarSightSize));
            midpointTarget.transform.position = midpoint;
            virtualCamera.Follow = midpointTarget.transform;
        }

        float deathPlatformOffsetY = -9.0f;
        deathPlatform.transform.position = spawnedplatform.transform.position + new Vector3(0, deathPlatformOffsetY, 0);
    }

    public void SpawnFuelPlatform(GameObject currentPlatform)
    {
        float randomPlatformVerticalOffset = Random.Range(minFuelVerticalOffset, maxFuelVerticalOffset);
        float randomPlatformHorizontalOffset = Random.Range(minFuelHorizontalOffset, maxFuelHorizontalOffset);

        Vector3 spawnPosition = currentPlatform.transform.position + new Vector3(randomPlatformHorizontalOffset, randomPlatformVerticalOffset, 0);
        spawnPosition = resetSpawnPosY(spawnPosition);

        float randomFuelVerticalOffset = Random.Range(fuelPosminVerticalFuelOffset, fuelPosmaxVerticalFuelOffset);
        float randomFuelHorizontalOffset = Random.Range(fuelPosminHorizontalFuelOffset, fuelPosmaxHorizontalFuelOffset);

        Vector3 fuelSpawnPosition = currentPlatform.transform.position + new Vector3(randomFuelHorizontalOffset, randomFuelVerticalOffset, 0);

        GameObject spawnedFuel = Instantiate(fuelPrefab, fuelSpawnPosition, Quaternion.identity);
        spawnedFuel.transform.SetParent(parent.transform);

        GameObject spawnedplatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        spawnedplatform.transform.SetParent(parent.transform);

        if (Player.instance.isGrounded)
        {
            Vector3 midpoint = (spawnedplatform.transform.position + currentPlatform.transform.position) / 2 + new Vector3(0, 2, 0);
            StartCoroutine(LerpCamera(midpoint, fuelCameraFarSightSize));
            midpointTarget.transform.position = midpoint;
            virtualCamera.Follow = midpointTarget.transform;
        }

        float deathPlatformOffsetY = -9.0f;
        deathPlatform.transform.position = spawnedplatform.transform.position + new Vector3(0, deathPlatformOffsetY, 0);
    }

    private IEnumerator LerpCamera(Vector3 targetPosition, float targetSize)
    {
        Vector3 startPosition = virtualCamera.transform.position;
        float startSize = virtualCamera.m_Lens.OrthographicSize;
        float elapsedTime = 0f;
        float lerpDuration = 1f; 

        while (elapsedTime < lerpDuration)
        {
            virtualCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / lerpDuration);
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.transform.position = targetPosition;
        virtualCamera.m_Lens.OrthographicSize = targetSize;
    }


    public Vector3 resetSpawnPosY(Vector3 spawnPosition)
    {
        if (spawnPosition.y > maxVerticalLimit)
        {
            spawnPosition.y = maxVerticalLimit;
        }
        else if (spawnPosition.y < minVerticalLimit)
        {
            spawnPosition.y = minVerticalLimit;
        }
        return spawnPosition;
    }

    public void UpgradeDifficulty()
    {
        minNormalHorizontalOffset += 0.025f;
        maxNormalHorizontalOffset += 0.025f;
        minFuelHorizontalOffset += 0.025f;
        maxFuelHorizontalOffset += 0.025f;
        fuelPosminHorizontalFuelOffset += 0.025f;
        fuelPosmaxHorizontalFuelOffset += 0.025f;
    }
}
