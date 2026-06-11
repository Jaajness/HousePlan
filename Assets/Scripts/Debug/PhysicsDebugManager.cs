using UnityEngine;

[ExecuteAlways]
public class PhysicsDebugManager : MonoBehaviour
{
    public static PhysicsDebugManager Instance;

    [Header("Toggles")]
    public bool showColliders = true;
    public bool showContacts = true;
    public bool showImpulses = true;
    public bool showSeparation = true;

    void OnEnable()
    {
        Instance = this;
    }

    void OnDrawGizmos()
    {
        if (showColliders)
            DrawColliders();

        if (showContacts || showImpulses || showSeparation)
            DrawContacts();
    }

    void DrawColliders()
    {
        var colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        foreach (var col in colliders)
        {
            if (!col.enabled) continue;

            Gizmos.color = GetColor(col);
            DrawCollider(col);
        }
    }

    Color GetColor(Collider col)
    {
        if (col.isTrigger) return Color.yellow;

        var rb = col.attachedRigidbody;
        if (rb != null) return Color.red;

        return Color.green;
    }

    void DrawCollider(Collider col)
    {
        if (col is BoxCollider box)
        {
            Gizmos.matrix = box.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.matrix = sphere.transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(sphere.center, sphere.radius);
        }
        else if (col is CapsuleCollider capsule)
        {
            DrawCapsule(capsule);
        }
        else if (col is MeshCollider mesh)
        {
            Gizmos.matrix = mesh.transform.localToWorldMatrix;
            Gizmos.DrawWireMesh(mesh.sharedMesh);
        }

        Gizmos.matrix = Matrix4x4.identity;
    }

    void DrawCapsule(CapsuleCollider capsule)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = capsule.transform.localToWorldMatrix;

        Vector3 center = capsule.center;
        float radius = capsule.radius;
        float height = Mathf.Max(capsule.height, radius * 2);

        int direction = capsule.direction;

        Vector3 axis = Vector3.up;
        if (direction == 0) axis = Vector3.right;
        if (direction == 2) axis = Vector3.forward;

        float cylinderHeight = height - (radius * 2);
        Vector3 offset = axis * (cylinderHeight / 2);

        Vector3 top = center + offset;
        Vector3 bottom = center - offset;

        // Draw spheres
        Gizmos.DrawWireSphere(top, radius);
        Gizmos.DrawWireSphere(bottom, radius);

        // Draw cylinder lines (approximation)
        DrawCapsuleLines(top, bottom, radius, axis);

        Gizmos.matrix = oldMatrix;
    }

    void DrawCapsuleLines(Vector3 top, Vector3 bottom, float radius, Vector3 axis)
    {
        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;

        if (axis == Vector3.forward)
        {
            forward = Vector3.up;
            right = Vector3.right;
        }
        else if (axis == Vector3.right)
        {
            forward = Vector3.up;
            right = Vector3.forward;
        }

        Vector3[] directions = new Vector3[]
        {
            right,
            -right,
            forward,
            -forward
        };

        foreach (var dir in directions)
        {
            Gizmos.DrawLine(top + dir * radius, bottom + dir * radius);
        }
    }    

    void DrawContacts()
    {
        var trackers = FindObjectsByType<CollisionTracker>(FindObjectsSortMode.None);

        foreach (var tracker in trackers)
        {
            foreach (var c in tracker.contacts)
            {
                if (showContacts)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(c.point, 0.05f);
                }

                if (showImpulses)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(c.point, c.point + c.normal * c.impulse * 0.1f);
                }

                if (showSeparation)
                {
                    Gizmos.color = c.separation < 0 ? Color.blue : Color.white;
                    Gizmos.DrawLine(c.point, c.point + c.normal * c.separation);
                }
            }
        }
    }
}
