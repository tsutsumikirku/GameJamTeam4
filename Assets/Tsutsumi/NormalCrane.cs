using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class NormalCrane : MonoBehaviour, IClaneArm
{
    public Action OnArmActionEnd { get; set; }
    public Action OnArmReleaseEnd { get; set; }
    private Transform rightArm;
    private Transform leftArm;
    public void OnArmEnd()
    {

    }

    public void OnArmRelease()
    {
    }

    public void OnArmStart()
    {
    }
}
