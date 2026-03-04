using UnityEngine;

[System.Serializable]

public class SpawnItemData 
{
    [Header("プレハブ入れて")]
    public GameObject prefab;
    [Header("この列での出現確率 その列で計100にして")]
    public int rate;
}
