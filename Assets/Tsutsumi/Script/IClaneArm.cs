using System;
public interface IClaneArm
{
    public Action OnArmActionEnd { get; set; }
    public Action OnArmReleaseEnd { get; set; }
    public void OnArmStart();
    public void OnArmEnd();
    public void OnArmRelease();
}
