using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void doDestroy()
    {
        Destroy(gameObject);
    }

    public void setImageSize(float _imgSize)
    {
        Vector3 scale = transform.localScale;
        scale *= _imgSize / 24f;
        transform.localScale = scale;
    }
}
