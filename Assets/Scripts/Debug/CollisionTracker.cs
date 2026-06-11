using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionTracker : MonoBehaviour
{
    public List<DebugContact> contacts = new List<DebugContact>();

    void OnCollisionStay(Collision collision)
    {
        contacts.Clear();

        foreach (var c in collision.contacts)
        {
            DebugContact dc = new DebugContact
            {
                point = c.point,
                normal = c.normal,
                separation = c.separation,
                impulse = collision.impulse.magnitude,
                lifetime = Time.time
            };

            contacts.Add(dc);
        }
    }

    void OnTriggerStay(Collider other)
    {
        Vector3 point = other.ClosestPoint(transform.position);

        DebugContact dc = new DebugContact
        {
            point = point,
            normal = (transform.position - point).normalized,
            separation = 0,
            impulse = 0,
            lifetime = Time.time
        };

        contacts.Add(dc);
    }

    void LateUpdate()
    {
        // Fade out old contacts
        float time = Time.time;

        contacts.RemoveAll(c => time - c.lifetime > 0.2f);
    }
}
