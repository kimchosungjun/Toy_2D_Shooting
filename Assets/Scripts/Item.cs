using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //int, string
    public enum eItemType//자료형으로 정의
    {
        None,
        PowerUp,
        HpRecovery,
    }

    [SerializeField] eItemType ItemType;

    float moveSpeed;//움직이는 속도
    Vector3 moveDirection;//움직일 방향

    [SerializeField] float minSpeed = 1;
    [SerializeField] float maxSpeed = 3;

    Limiter limiter;


    private void Awake()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed); //1~3까지 어떤 사이값의 속도
        moveDirection.x = Random.Range(-1.0f, 1.0f);
        moveDirection.y = Random.Range(-1.0f, 1.0f);

        moveDirection.Normalize();//벡터에서 힘을 버리고 방향만 지시
    }

    void Start()
    {
        limiter = GameManager.Instance._Limiter;
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        checkItemPos();
    }

    private void checkItemPos()
    {
        (bool _x, bool _y) rData = limiter.IsReflectItem(transform.position, moveDirection);
        //var rData = limiter.IsReflectItem(transform.position);
        if (rData._x == true)
        {
            moveDirection = Vector3.Reflect(moveDirection, Vector3.right);
        }
        if(rData._y == true) 
        {
            moveDirection = Vector3.Reflect(moveDirection, Vector3.up);
        }
    }

    public eItemType GetItemType()
    {
        return ItemType;
    }
}
