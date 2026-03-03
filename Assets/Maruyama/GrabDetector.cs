using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabDetector : MonoBehaviour
{
    List<Rigidbody> prizesInRange = new List<Rigidbody>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prize"))
        {
            var rb = other.GetComponent<Rigidbody>();
            if (rb != null) prizesInRange.Add(rb);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Prize"))
        {
            var rb = other.GetComponent<Rigidbody>();
            if (rb != null) prizesInRange.Remove(rb);
        }
    }

    /// <summary>
    /// ˆê”Ô‹ß‚¢Œi•i‚ð•Ô‚·
    /// </summary>
    public Rigidbody GetClosestPrize()
    {
        prizesInRange.RemoveAll(r => r == null);
        return prizesInRange
            .OrderBy(r => Vector3.Distance(r.position, transform.position))
            .FirstOrDefault();
    }
}