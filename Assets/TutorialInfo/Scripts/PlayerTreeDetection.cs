using UnityEngine;
using System.Collections;

public class PlayerTreeDetection : MonoBehaviour
{
    public float detectionRadius = 10f;
    public LayerMask treeLayer;
    public Color highlightColor = Color.red;
    public GameObject fruitPrefab; // Fruit prefab to drop
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;
    public float shakeIntensity = 0.2f; // Shake intensity

    private Renderer currentTreeRenderer;
    private Color originalColor;
    private bool isDroppingFruit = false; // Flag to prevent multiple coroutines

    void Update()
    {
        DetectClosestTree();
        CheckForFruitDrop();
    }

    void DetectClosestTree()
    {
        Vector3 detectionPosition = GetPlayerCenter();
        Collider[] hitColliders = Physics.OverlapSphere(detectionPosition, detectionRadius, treeLayer);

        float closestDistance = Mathf.Infinity;
        Renderer closestTree = null;
        Transform closestTreeTransform = null;

        foreach (Collider hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTree = hitCollider.GetComponent<Renderer>();
                closestTreeTransform = hitCollider.transform;
            }
        }

        if (closestTree != null)
        {
            if (currentTreeRenderer != closestTree)
            {
                ResetTreeColor();
                currentTreeRenderer = closestTree;
                originalColor = closestTree.material.color;
                currentTreeRenderer.material.color = highlightColor;
            }
        }
        else
        {
            ResetTreeColor();
        }
    }

    void CheckForFruitDrop()
    {
        if (currentTreeRenderer != null && Input.GetKey(KeyCode.P))
        {
            if (!isDroppingFruit)
            {
                StartCoroutine(ShakeTree(currentTreeRenderer.transform));
                StartCoroutine(DropFruitRepeatedly(currentTreeRenderer.transform));
            }
        }
        else
        {
            isDroppingFruit = false;
        }
    }

    IEnumerator ShakeTree(Transform treeTransform)
    {
        Vector3 originalPosition = treeTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetZ = Random.Range(-shakeIntensity, shakeIntensity);

            treeTransform.position = originalPosition + new Vector3(offsetX, offsetY, offsetZ);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        treeTransform.position = originalPosition; // Reset position after shaking
    }

    IEnumerator DropFruitRepeatedly(Transform treeTransform)
    {
        isDroppingFruit = true; // Prevent multiple coroutine calls

        while (Input.GetKey(KeyCode.P))
        {
            DropFruit(treeTransform.position);
            yield return new WaitForSeconds(0.5f); // Delay between fruit drops
        }

        isDroppingFruit = false;
    }

    void DropFruit(Vector3 treePosition)
    {
        if (fruitPrefab != null)
        {
            Vector3 spawnPosition = treePosition + Vector3.down * 1f; // Adjust drop height
            Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Fruit Prefab is not assigned!");
        }
    }

    void ResetTreeColor()
    {
        if (currentTreeRenderer != null)
        {
            currentTreeRenderer.material.color = originalColor;
            currentTreeRenderer = null;
        }
    }

    Vector3 GetPlayerCenter()
    {
        Renderer playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {
            return playerRenderer.bounds.center;
        }
        return transform.position;
    }

    void OnDrawGizmos()
    {
        Vector3 detectionPosition = GetPlayerCenter();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(detectionPosition, detectionRadius);
    }
}