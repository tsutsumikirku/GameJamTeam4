using UnityEngine;

[System.Serializable]

public class SpawnItemData 
{
    [Header("プレハブを設定")]
    public GameObject prefab;
    [Header("この割合での出現確率 この列で合計100にする")]
    public int rate;
}
