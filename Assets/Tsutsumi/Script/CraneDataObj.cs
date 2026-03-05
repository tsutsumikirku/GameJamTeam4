using UnityEngine;


[CreateAssetMenu(fileName = "CraneData", menuName = "ScriptableObjects/CraneData", order = 1)]
public class CraneDataObj : ScriptableObject
{
    [Header("移動範囲")]
    public float maxMoveX;
    
    [Header("移動速度")]
    public  float moveSpeed = 5f;
    
    [Header("下降速度")]
    public float descendSpeed = 3f;

    [Header("上昇時間")]
    public float ascendTime = 2f;
    [Header("戻る速度の秒速")]
    public float returnSpeed = 2f;

    [Header("アームの回転時間")]
    public float armRotationTime = 1f; 
}