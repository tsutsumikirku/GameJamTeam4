using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInfo", menuName = "ScriptableObjects/Info/Tsutsumi")]
public class InfoScriptableObj : ScriptableObject
{
    public InfoData[] InfoDataArray;
}
[System.Serializable]
public class InfoData
{
    public string Text;
    public Sprite Image;
}