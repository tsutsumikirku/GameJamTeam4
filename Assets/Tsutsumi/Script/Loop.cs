using UnityEngine;
using UnityEngine.UI;

public class Loop : MonoBehaviour
{
    [SerializeField] private RawImage _rawImage;
    void Update()
    {
        _rawImage.uvRect = new Rect(_rawImage.uvRect.x + Time.deltaTime * 0.1f, _rawImage.uvRect.y, _rawImage.uvRect.width, _rawImage.uvRect.height);
        if(_rawImage.uvRect.x >= 1f)
        {
            _rawImage.uvRect = new Rect(0, _rawImage.uvRect.y, _rawImage.uvRect.width, _rawImage.uvRect.height);
        }
    }
}
