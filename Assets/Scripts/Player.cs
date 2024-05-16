using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;
    GameManager gameManager;
    GameObject fabExplosion;
    [Header("플레이어 설정"),SerializeField,Tooltip("플레이어 이동속도")] float moveSpeed;
    Vector3 moveDir = new Vector3();
    Camera cam;
    [Space]

    [Header("<color=red>화면</color>경계")]
    [SerializeField] Vector2 viewPortLimitMin;
    [SerializeField] Vector2 viewPortLimitMax;

    [SerializeField, TextArea] string text;

    [Header("총알"),SerializeField] GameObject fabBullet; // 총알
    [SerializeField] Transform dynamicObject;
    [SerializeField] bool autoFire = false;
    [SerializeField,Range(0f,5f)] float fireRateTime = 1f;
    float fireTimer = 0f;
    private void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    private void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
    }

    void Update()
    {

        Moving();
        DoAnimation();
        CheckMovePosition();
        Shoot();
    }

    public void Moving()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.y = Input.GetAxisRaw("Vertical");
        transform.position += moveDir*Time.deltaTime*moveSpeed;

        // transform.position => 월드 포지션
        // transform.localPosition => 이 데이터가 Root데이터라면 월드 포지션 좌표를 출력, 이 데이터가 자식 데이터라면 부모로부터의 거리를 포지션 좌표로 출력 
    }

    public void DoAnimation()
    {
        anim.SetInteger("X", (int)moveDir.x);
    }

    public void CheckMovePosition()
    {
        Vector3 viewPortPos = cam.WorldToViewportPoint(transform.position);
        if (viewPortPos.x < viewPortLimitMin.x)
            viewPortPos.x = viewPortLimitMin.x;
        else if (viewPortPos.x > viewPortLimitMax.x)
            viewPortPos.x = viewPortLimitMax.x;
        if (viewPortPos.y < viewPortLimitMin.y)
            viewPortPos.y = viewPortLimitMin.y;
        else if (viewPortPos.y > viewPortLimitMax.y)
            viewPortPos.y = viewPortLimitMax.y;
        Vector3 fixedPos = cam.ViewportToWorldPoint(viewPortPos);
        transform.position = fixedPos;
    }
    private void Shoot()
    {
        if (!autoFire && Input.GetKeyDown(KeyCode.Space))
        {
            CreateBullet();
        }
        else if(autoFire)
        {
            fireTimer += Time.deltaTime; // 1초가 지나면 1이 될 수 있도록 소수점들이 frieTimer에 쌓인다.
            if (fireTimer > fireRateTime)
            {
                CreateBullet();
                fireTimer = 0f;
            }
        }
    }

    private void CreateBullet()
    {
        Instantiate(fabBullet, transform.position, Quaternion.identity, dynamicObject);
    }
}
