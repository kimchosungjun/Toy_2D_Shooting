using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;
    
    [Header("플레이어 설정"), SerializeField, Tooltip("플레이어의 이동속도")] float moveSpeed;

    Vector3 moveDir;

    [Header("총알")]
    [SerializeField] GameObject fabBullet;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] Transform dynamicObject;
    [SerializeField] bool autoFire = false;//자동공격기능
    [SerializeField] float fireRateTime = 0.5f;//이시간이 지나면 총알이 발사됨
    float fireTimer = 0;

    GameManager gameManager;
    GameObject fabExplosion;
    Limiter limiter;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static  void initCode()
    //{
    //    Debug.Log("initCode");
    //}

    private void Awake()
    {
        anim = transform.GetComponent<Animator>();
    }

    private void Start()
    {
        //cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
        gameManager._Player = this;
    }

    void Update()
    {
        moving();
        doAnimation();
        checkPlayerPos();

        shoot();
    }

    /// <summary>
    /// 플레이어 기체의 기동을 정의합니다.
    /// </summary>
    private void moving()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");//왼쪽 혹은 오른쪽 입력// -1 0 1
        moveDir.y = Input.GetAxisRaw("Vertical");//위 혹은 아래 입력 // -1 0 1

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 애니메이션에 어떤 애니메이션을 실행할지 파라미터를 전달 합니다.
    /// </summary>
    private void doAnimation()//하나의 함수에는 하나의 기능
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
    }

    private void checkPlayerPos()
    {
        if (limiter == null)
        {
            limiter = gameManager._Limiter;
        }
        transform.position = limiter.checkMovePosition(transform.position);
    }

    private void shoot()
    {
        if (autoFire == false && Input.GetKeyDown(KeyCode.Space) == true)//유저가 스페이스 키를 누른다면
        {
            createBullet();
        }
        else if (autoFire == true)
        {
            //일정시간이 지나면 총알 한발 발사
            fireTimer += Time.deltaTime;//1초가 지나면 1이 될수있도록 소수점들이 fireTimer에 쌓임
            if(fireTimer > fireRateTime) 
            {
                createBullet();
                fireTimer = 0;
            }
        }
    }

    private void createBullet()//총알을 생성한다
    {
        Instantiate(fabBullet, transform.position, Quaternion.identity, dynamicObject);
    }
}
