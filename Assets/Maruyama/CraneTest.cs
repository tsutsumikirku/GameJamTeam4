using UnityEngine;

/// <summary>
/// GameManager�̊ȈՃe�X�g�ŁB�N���[���̏������Ƒ���J�n���s���B
/// </summary>
public class CraneTest : MonoBehaviour
{
    [Header("�v���C�ݒ�")]
    [SerializeField, Range(1, 2)] int playerCount = 1;

    [Header("P1")]
    //[SerializeField] CraneController playerOneCrane;
    [SerializeField] CraneType playerOneCraneType;

    [Header("P2")]
    //[SerializeField] CraneController playerTwoCrane;
    [SerializeField] CraneType playerTwoCraneType;

    /// <summary>
    /// �N���[���̎�ނ�ݒ肵�A������J�n����B
    /// </summary>
    // void Start()
    // {
    //     playerOneCrane.CraneType = playerOneCraneType;
    //     playerOneCrane.StartControl();

    //     if (playerCount >= 2 && playerTwoCrane != null)
    //     {
    //         playerTwoCrane.CraneType = playerTwoCraneType;
    //         playerTwoCrane.StartControl();
    //     }
    // }
}