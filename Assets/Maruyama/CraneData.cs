using UnityEngine;

[CreateAssetMenu(menuName = "Crane/CraneData")]
public class CraneData : ScriptableObject
{
    // public CraneType type;
    public float moveSpeed;
    public float descendSpeed;
    public float grabPower; // Grab‚Ì‹­‚³
    public float grabRadius; // Grab‚Ì—LŒø”ÍˆÍ
    public GameObject visualPrefab;
}