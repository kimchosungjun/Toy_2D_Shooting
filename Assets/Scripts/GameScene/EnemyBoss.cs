using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy
{
    public float Hp => hp;//get
    //{
    //    get
    //    {
    //        return hp;
    //    }
    //}


    Transform trsBossPosition;//도착할 위치

    bool isMovingTrsBossPosition = false;//보스가 원위치까지 이동을 완료했는지
    bool patternChange = false;//패턴을 바꾸고 그동안 유저가 극딜할 타이밍을 만들어줌

    Vector3 createPos = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    float timer = 0.0f;
    float velocityX = 0;
    float velocityY = 0;

    bool isSwayRight = false;

    [Header("현재위치에서 전방으로")]
    [SerializeField] private int pattern1Count = 10;
    [SerializeField] private float pattern1Reload = 0.5f;
    [SerializeField] private GameObject pattern1Fab;

    [Header("샷건")]
    [SerializeField] private int pattern2Count = 5;
    [SerializeField] private float pattern2Reload = 0.3f;
    [SerializeField] private GameObject pattern2Fab;

    [Header("조준발사")]
    [SerializeField] private int pattern3Count = 30;
    [SerializeField] private float pattern3Reload = 0.1f;
    [SerializeField] private GameObject pattern3Fab;

    Limiter limiter;

    private int curPattern = 1;
    private int curPatternShootCount = 0;

    [Header("발사위치")]
    [SerializeField] List<Transform> trsShootPos;//public으로 선언하거나 시리얼라이즈 필드로 선언하여 인스펙터에서 변형해서 사용시에는 따로 동적할당을 받을 필요가 없음

    Animator anim;

    //[System.Serializable]
    //public class cPattern//여러분들이 정의
    //{
    //    public string explain;
    //    public int patternCount;
    //    public float patternReload;
    //    public GameObject patternFab;
    //}
    //[SerializeField] List<cPattern> listPattern;

    protected override void Start()
    {
        gameManager = GameManager.Instance;
        trsBossPosition = gameManager.TrsBossPostion;
        fabExplosion = gameManager.FabExplosion;
        createPos = transform.position;
        anim = GetComponent<Animator>();
    }

    protected override void moving()
    {
        //float posX = Mathf.SmoothDamp(transform.position.x, trsBossPosition.position.x, ref velocityX, 0.5f);
        //float posY = Mathf.SmoothDamp(transform.position.y, trsBossPosition.position.y, ref velocityY, 0.5f);
        //transform.position = new Vector3(posX, posY, 0);

        if (isMovingTrsBossPosition == false) //위치까지 이동 
        {
            if (timer < 1.0f)
            {
                timer += Time.deltaTime;
                //선형보간
                //transform.position = Vector3.Lerp(createPos, trsBossPosition.position, timer);

                float posX = Mathf.SmoothStep(createPos.x, trsBossPosition.position.x, timer);//0~1
                float posY = Mathf.SmoothStep(createPos.y, trsBossPosition.position.y, timer);
                transform.position = new Vector3(posX, posY, 0);

                if (timer >= 1.0f)
                {
                    isMovingTrsBossPosition = true;
                    timer = 0f;
                }
            }
            return;
        }

        if (isSwayRight == true)
        {
            transform.position += Vector3.right * Time.deltaTime * moveSpeed;
        }
        else
        {
            transform.position += Vector3.left * Time.deltaTime * moveSpeed;
        }

        checkMovingLimit();
    }

    protected override void shooting()
    {
        if (isMovingTrsBossPosition == false)
        {
            return;
        }

        timer += Time.deltaTime;

        if (patternChange == true)
        {
            if (timer >= 1.0f)
            {
                timer = 0.0f;
                patternChange = false;
            }
            return;
        }

        if (curPattern == 1)//전방으로 발사
        {
            if (timer >= pattern1Reload)
            {
                timer = 0.0f;
                shootStraight();
                if (curPatternShootCount >= pattern1Count)
                {
                    curPattern++;
                    patternChange = true;
                    curPatternShootCount = 0;
                }
            }
        }
        else if (curPattern == 2)//샷건
        {
            if (timer >= pattern2Reload)
            {
                timer = 0.0f;
                shootShotgun();
                if (curPatternShootCount >= pattern2Count)
                {
                    curPattern++;
                    patternChange = true;
                    curPatternShootCount = 0;
                }
            }
        }
        else if (curPattern == 3)//조준발사
        {
            if (timer >= pattern3Reload)
            {
                timer = 0.0f;
                shootgatling();
                if (curPatternShootCount >= pattern3Count)
                {
                    curPattern = 1;
                    patternChange = true;
                    curPatternShootCount = 0;
                }
            }
        }
    }

    private void shootgatling()
    {
        Vector3 playerPos;//디폴트 x 0, y 0, z 0;
        if (gameManager.GetPlayerPosition(out playerPos) == true)
        {
            Vector3 distance = playerPos - transform.position;//플레이어의 위치로부터 내위치의 거리

            float angle = Quaternion.FromToRotation(Vector3.up, distance).eulerAngles.z;
            //플레이어와 보스와의 거리 오차를 이용해 y축 0도로 부터 오차 위치의 각도 z를 구함

            Instantiate(pattern3Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, angle)), transform.parent);
        }

        curPatternShootCount++;
    }

    private void shootShotgun()
    {
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f - 15f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f + 15f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f - 30f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f + 30f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f - 45f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f + 45f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f - 60f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(new Vector3(0, 0, 180f + 60f)), transform.parent);

        curPatternShootCount++;
    }

    private void shootStraight()
    {
        int count = 4;
        for(int iNum = 0; iNum < count; ++iNum)
        {
            Instantiate(pattern1Fab, trsShootPos[iNum].position, Quaternion.Euler(new Vector3(0, 0, 180)), transform.parent);
        }

        curPatternShootCount++;
    }

    private void checkMovingLimit()
    {
        if (limiter == null)
        {
            limiter = gameManager._Limiter;
        }

        if (limiter.checkMovePosition(transform.position) == true)
        {
            isSwayRight = !isSwayRight;
        }
    }

    public override void Hit(float _damage)
    {
        if (isDied == true)
        {
            return;
        }

        hp -= _damage;
        gameManager.ModifyBossHp(hp);

        if (hp <= 0)
        {
            isDied = true;
            Destroy(gameObject);
            //매니저로부터 받아온 폭발 연출을 내 위치에 생성하고 부모로 사용중인 레이어에 만들어줌
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            Explosion goSc = go.GetComponent<Explosion>();

            //직사각형
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//현재 기체의 이미지 길이를 넣어줌

            gameManager.createItem(transform.position, Item.eItemType.PowerUp);
            gameManager.createItem(transform.position, Item.eItemType.HpRecovery);

            gameManager.AddScore(score);
            //gameManager.AddKillCount();//보스가 죽었다고 전달 //다시 적들이 출동하도록 설계
            gameManager.KillBoss();
        }
        else
        {
            //이 친구는 스프라이트만 활용하는것이 아니라 스프라이트 애니메이션을 활용함으로 애니메이션에서 히트 애님을 실행
            anim.SetTrigger("bossHit");
        }
    }
}
