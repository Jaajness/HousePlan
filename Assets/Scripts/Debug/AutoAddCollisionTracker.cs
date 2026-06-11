using UnityEngine;

[ExecuteAlways]
public class AutoAddCollisionTracker : MonoBehaviour
{
    void Update()
    {
        var bodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

        foreach (var rb in bodies)
        {
            if (!rb.GetComponent<CollisionTracker>())
            {
                rb.gameObject.AddComponent<CollisionTracker>();
            }
        }
    }
}
