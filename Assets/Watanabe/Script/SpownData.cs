using UnityEngine;

[System.Serializable]
public class SpownData
{
    public Transform point;
    public SpawnItemData[] items;
    [Header("‚±‚Ì—ñ‚ÌoŒ»ŒÂ””ÍˆÍ")]
    public int minCount = 10;
    public int maxCount = 15;
    [Header("XY•ûŒü‚ÌL‚ª‚è")]
    public float rangeX = 10f;
    public float rangeY = 3f;
}
