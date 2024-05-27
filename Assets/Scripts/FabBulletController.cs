using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FabBulletController : MonoBehaviour
{
    private void Awake()
    {
        int count = transform.childCount;
        for (int iNum = 0; iNum < count; iNum++)
        {
            GameObject go = transform.GetChild(iNum).gameObject;
            Bullet goSc = go.GetComponent<Bullet>();
            goSc.ShootPlayer();

            //transform.GetChild(iNum).gameObject.GetComponent<Bullet>().ShootPlayer();
        }
    }

    void Update()
    {
        checkChild();
    }

    private void checkChild()
    {
        int count = transform.childCount;
        if (count == 0)
        {
            Destroy(gameObject);
        }
    }
}
