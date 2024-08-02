using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackGroundScroller : MonoBehaviour
{
    public GameObject[] backgroundPrefabs;
    public Transform player;
    public float backgroundWidth;

    private Queue<GameObject> activeBackgrounds = new Queue<GameObject>();
    private int currentBackgroundIndex = 0;

    private void Start()
    {
        backgroundWidth = backgroundPrefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        for (int i = 0; i < backgroundPrefabs.Length; i++)
        {
            GameObject newBackground = Instantiate(
                backgroundPrefabs[i],
                new Vector3(i * backgroundWidth, 0, 0),
                Quaternion.identity
            );
            activeBackgrounds.Enqueue(newBackground);
        }
    }

    private void Update()
    {
        // Check if player has moved past the middle of the current background
        if (player.position.x > activeBackgrounds.Peek().transform.position.x + backgroundWidth)
        {
            ShiftBackground();
        }
    }

    private void ShiftBackground()
    {
        GameObject lastBackground = activeBackgrounds.Last();
        Vector3 lastPosition = lastBackground.transform.position;
        GameObject newBackground = Instantiate(
            backgroundPrefabs[currentBackgroundIndex],
            new Vector3(lastPosition.x + backgroundWidth, 0, 0),
            Quaternion.identity
        );
        activeBackgrounds.Enqueue(newBackground);

        GameObject oldBackground = activeBackgrounds.Dequeue();
        Destroy(oldBackground);

        currentBackgroundIndex = (currentBackgroundIndex + 1) % backgroundPrefabs.Length;
    }
}
