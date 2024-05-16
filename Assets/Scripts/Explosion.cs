using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void DoDestroy()
    {
        Destroy(gameObject);
    }

    public void SetImageSize(float _imageSize)
    {
        Vector3 scale = transform.localScale;
        scale *= _imageSize/24f;
        transform.localScale = scale;
    }
}
