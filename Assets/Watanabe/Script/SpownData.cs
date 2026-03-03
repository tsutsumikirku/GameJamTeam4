using UnityEngine;

[System.Serializable]
public class SpownData
{
    public Transform point;
    public SpawnItemData[] items;
    [Header("‚±‚Ì—ñ‚ÌoŒ»ŒÂ””ÍˆÍ")]
    public int minCount = 10;
    public int maxCount = 15;
    [Header("X•ûŒü‚ÌL‚ª‚è")]
    public float rangeX = 10f;
}
