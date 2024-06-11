using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;

    [Header("플레이어 설정"), SerializeField, Tooltip("플레이어의 이동속도")] float moveSpeed;

    Vector3 moveDir;

    [Header("총알")]
    [SerializeField] GameObject fabBullet;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] GameObject fabBullet2;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] GameObject fabBullet3;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] GameObject fabBullet4;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] GameObject fabBullet5;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] Transform dynamicObject;
    [SerializeField] bool autoFire = false;//자동공격기능
    [SerializeField] float fireRateTime = 0.5f;//이시간이 지나면 총알이 발사됨
    float fireTimer = 0;

    GameManager gameManager;
    GameObject fabExplosion;
    SpriteRenderer spriteRenderer;
    Limiter limiter;
    [Header("체력")]
    [SerializeField] int maxHp = 3;
    [SerializeField] int curHp;
    int beforeHp;
    bool invincibilty;//무적
    [SerializeField] float invincibiltyTime = 1f;//무적시간
    float invincibiltyTimer;

    [Header("플레이어 레벨")]
    [SerializeField] int minLevel = 1;
    [SerializeField] int maxLevel = 5;
    [SerializeField, Range(1, 5)] int curLevel;

    //[SerializeField] float distanceBullet;//2레벨 이상시 총알이 중심으로 부터 벌어지는 거리 //플레이어 전방에서 발사
    //[SerializeField] float angleBullet;//4레벨 이상시 총알이 회전된 값;
    [SerializeField] Transform shootTrs;
    //[SerializeField] Transform shootTrsLevel4;//4레벨 이상시 총알이 발사될 위치
    //[SerializeField] Transform shootTrsLevel5;//4레벨 이상시 총알이 발사될 위치

    private void OnValidate()//인스펙터에서 어떤값이 변동이 생기면 호출
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        if (beforeHp != curHp)
        {
            beforeHp = curHp;
            GameManager.Instance.SetHp(maxHp, curHp);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Tool.GetTag(GameTags.Enemy))
        {
            //체력 감소
            Hit();
            
            //체력이 0이 되면 게임이 끝남
            //점수가 랭크인이 되면 이름 입력하는 기능
            //메인 메뉴에서 1~10등 랭크

            //짧은시간 무적

            //게이지 변화코드 실행
        }
        else if (collision.tag == Tool.GetTag(GameTags.Item))
        {
            Item item = collision.GetComponent<Item>();
            Destroy(item.gameObject);//이 함수는 이 함수가 모든 동작 마치게 되면 삭제해달라 라고 예약하는 기능

            if (item.GetItemType() == Item.eItemType.PowerUp)
            {
                curLevel++;
                if(curLevel > maxLevel) 
                {
                    curLevel = maxLevel;
                }
            }
            else if (item.GetItemType() == Item.eItemType.HpRecovery)
            {
                //체력 회복
                curHp++;
                if (curHp > maxHp)
                {
                    curHp = maxHp;
                }
                gameManager.SetHp(maxHp, curHp);
            }
        }
    }

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static  void initCode()
    //{
    //    Debug.Log("initCode");
    //}

    private void Awake()
    {
        anim = transform.GetComponent<Animator>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        curHp = maxHp;
        curLevel = minLevel;
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
        checkInvincibilty();
    }

    private void checkInvincibilty()//무적일때만 작동하여 일정시간이 지나고 나면 다시 무적을 풀어줌
    {
        if (invincibilty == false) return;

        if (invincibiltyTimer > 0f)
        {
            invincibiltyTimer -= Time.deltaTime;
            if (invincibiltyTimer <= 0f)
            {
                setSprInvincibilty(false);
            }
        }
    }

    private void setSprInvincibilty(bool _value)
    {
        Color color = spriteRenderer.color;
        if (_value == true)//무적이 된것처럼 투명도를 줄여 유저에게 무적이라 알려줌
        {
            color.a = 0.5f;
            invincibilty = true;
            invincibiltyTimer = invincibiltyTime;
        }
        else
        {
            color.a = 1.0f;
            invincibilty = false;
            invincibiltyTimer = 0f;
        }
        spriteRenderer.color = color;
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
        transform.position = limiter.checkMovePosition(transform.position, false);
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
            if (fireTimer > fireRateTime)
            {
                createBullet();
                fireTimer = 0;
            }
        }
    }

    private void createBullet()//총알을 생성한다
    {
        if (curLevel == 1)
        {
            GameObject go = Instantiate(fabBullet, shootTrs.position, Quaternion.identity, dynamicObject);
            Bullet goSc = go.GetComponent<Bullet>();
            goSc.ShootPlayer();
            //instBullet(shootTrs.position, Quaternion.identity);
        }
        else if (curLevel == 2)
        {
            Instantiate(fabBullet2, shootTrs.position, Quaternion.identity, dynamicObject);

            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);
        }
        else if (curLevel == 3)
        {
            Instantiate(fabBullet3, shootTrs.position, Quaternion.identity, dynamicObject);

            //instBullet(shootTrs.position, Quaternion.identity);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);
        }
        else if (curLevel == 4)
        {
            Instantiate(fabBullet4, shootTrs.position, Quaternion.identity, dynamicObject);

            //instBullet(shootTrs.position, Quaternion.identity);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);

            //Vector3 lv4Pos = shootTrsLevel4.position;
            //instBullet(lv4Pos, new Vector3(0, 0, angleBullet));

            //Vector3 lv4localPos = shootTrsLevel4.localPosition;
            //lv4localPos.x *= -1;
            //lv4localPos += transform.position;

            //instBullet(lv4localPos, new Vector3(0, 0, -angleBullet));
        }
        else if (curLevel == 5)
        {
            Instantiate(fabBullet5, shootTrs.position, Quaternion.identity, dynamicObject);

            //instBullet(shootTrs.position, Quaternion.identity);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);

            //Vector3 lv4Pos = shootTrsLevel4.position;
            //instBullet(lv4Pos, new Vector3(0, 0, angleBullet));

            //Vector3 lv4localPos = shootTrsLevel4.localPosition;
            //lv4localPos.x *= -1;
            //lv4localPos += transform.position;

            //instBullet(lv4localPos, new Vector3(0, 0, -angleBullet));

            //Vector3 lv5Pos = shootTrsLevel5.position;
            //instBullet(lv5Pos, new Vector3(0, 0, angleBullet));

            //Vector3 lv5localPos = shootTrsLevel5.localPosition;
            //lv5localPos.x *= -1;
            //lv5localPos += transform.position;
            //instBullet(lv5localPos, new Vector3(0, 0, -angleBullet));
        }
    }

    private void instBullet(Vector3 _pos, Vector3 _angle)
    {
        GameObject go = Instantiate(fabBullet, _pos, Quaternion.Euler(_angle), dynamicObject);
        Bullet goSc = go.GetComponent<Bullet>();
        goSc.ShootPlayer();
    }
    private void instBullet(Vector3 _pos, Quaternion _quat)
    {
        GameObject go = Instantiate(fabBullet, _pos, _quat, dynamicObject);
        Bullet goSc = go.GetComponent<Bullet>();
        goSc.ShootPlayer();
    }

    public void Hit()
    {
        //무적상태라면 데미지를 받지 않음
        if (invincibilty == true) return;

        setSprInvincibilty(true);
        
        curHp--;
        if(curHp < 0)
        {
            curHp = 0;
        }
        GameManager.Instance.SetHp(maxHp, curHp);

        curLevel--;
        if(curLevel < minLevel)
        {
            curLevel = minLevel;
        }

        if (curHp == 0)
        {
            Destroy(gameObject);
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            Explosion goSc = go.GetComponent<Explosion>();

            //직사각형
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//현재 기체의 이미지 길이를 넣어줌

            gameManager.GameOver();
        }
    }
}
