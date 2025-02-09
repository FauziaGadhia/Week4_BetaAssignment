using System.Collections;
using UnityEngine;

public class ShakeAndDrop : MonoBehaviour
{
    public float shakeDuration = 0.5f; // Duration of the shake
    public float shakeIntensity = 0.2f; // How much it shakes
    public GameObject fruitPrefab; // Assign a ball prefab in Unity
    public Transform dropPoint; // The point where the ball spawns

    private Vector3 originalPosition; // Store the original position

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(Shake());
            DropFruit();
        }
    }

    IEnumerator Shake()
    {
        originalPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetZ = Random.Range(-shakeIntensity, shakeIntensity);

            transform.position = originalPosition + new Vector3(offsetX, offsetY, offsetZ);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition; // Reset position after shaking
    }

    void DropFruit()
    {
        if (fruitPrefab != null && dropPoint != null)
        {
            Instantiate(fruitPrefab, dropPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Fruit Prefab or Drop Point is not assigned!");
        }
    }
}