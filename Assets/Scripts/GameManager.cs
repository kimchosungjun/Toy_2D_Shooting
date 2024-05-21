using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//null 채워줘야함
    [Header("적기들")]
    [SerializeField] List<GameObject> listEnemy;
    GameObject fabExplosion;//실제 데이터를 가지고 있는 변수는 private를 유지하고

    [Header("적 생성 여부")]
    [SerializeField] bool isSpawn = false;//보스가 등장하거나 원하는 사유가 있을때 이값을
    //true 로 변경하면 적들이 더이상 나오지 않게하는 용도로 활용

    [Header("적 생성 시간")]
    float enemySpawnTimer = 0.0f;//0초에서 시작되는 타이머
    [SerializeField, Range(0.1f,5f)] float spawnTime = 1.0f;

    [Header("적 생성 위치")]
    [SerializeField] Transform trsSpawnPosition;
    [SerializeField] Transform trsDynamicObject;

    [Header("드롭 아이템")]
    [SerializeField] List<GameObject> listItem;

    [Header("드롭 확률")]
    [SerializeField, Range(0.0f, 100.0f)] float itemDropRate;

    Limiter limiter;
    public Limiter _Limiter
    {
        get { return limiter; }
        set { limiter = value; }
    }

    Player player;
    public Player _Player
    {
        get { return player; }
        set { player = value; }
    }

    public GameObject FabExplosion//정보를 전달 혹은 가져와야할때만 함수로서 사용가능
    {
        get
        {
            return fabExplosion;
        }
        //set { fabExplosion = value; }
    }

    //인스펙터의 값이 변동이 있을때 이함수가 강제 호출
    //private void OnValidate()
    //{
    //    if (Application.isPlaying == false) return;

    //    if (spawnTime < 0.1f)
    //    {
    //        spawnTime = 0.1f;
    //    }
    //}

    private void Awake()
    {
        //1개만 존재해야함
        if (Instance == null)
        {
            Instance = this;
        }
        else//인스턴스가 이미 존재한다면 나는 지워져야함
        {
            //Destroy(this);//이러면 컴포넌트만 삭제됨
            Destroy(gameObject);//오브젝트가 지워지면서 스크립트도 같이 지워짐
        }

        fabExplosion = Resources.Load<GameObject>("Effect/Test/fabExplosion");
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()//프레임당 한번 실행되는 함수
    {
        createEnemy();
    }

    private void createEnemy()
    {
        if (isSpawn == false) return;

        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer > spawnTime)
        {
            //적을 생성
            int count = listEnemy.Count; //개의 적기 0 ~ 2
            int iRand = Random.Range(0, count);//0, 3

            Vector3 defulatPos = trsSpawnPosition.position;//y => 7 
            float x = Random.Range(limiter.WorldPosLimitMin.x, limiter.WorldPosLimitMax.x);//x => -2.4 ~ 2.4
            defulatPos.x = x;

            Instantiate(listEnemy[iRand], defulatPos, Quaternion.identity, trsDynamicObject);
            //생성할 위치, 다이나믹 오브젝트 위치가 필요

            //주사위를 굴림
            float rate = Random.Range(0.0f, 100.0f);
            if (rate <= itemDropRate)
            {
                // 적기가 아이템을 가지고 있음
            }

            enemySpawnTimer = 0.0f;
        }
    }
}
