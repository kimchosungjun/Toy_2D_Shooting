using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//null 채워줘야함
    [Header("적기들")]
    [SerializeField] List<GameObject> listEnemy;
    GameObject fabExplosion;//실제 데이터를 가지고 있는 변수는 private를 유지하고
    [SerializeField] GameObject fabBoss;

    [Header("적 생성 여부")]
    [SerializeField] bool isSpawn = false;//보스가 등장하거나 원하는 사유가 있을때 이값을
    [SerializeField] Color sliderDefaultColor;
    [SerializeField] Color sliderBossSpawnColor;

    bool isSpawnBoss = false;//보스가 현재 게임에 등장 중인지
    bool IsSpawnBoss
    {
        set
        {
            isSpawnBoss = value;
            StartCoroutine(sliderColorChange(value));
        }
    }

    IEnumerator sliderColorChange(bool _spawnBoss)//true가 되면 보스가 출동해서 체력바로 사용할 용도
    {
        float timer = 0.0f;

        while (timer < 1.0f)//조건문이 참이라면 반복
        {
            timer += Time.deltaTime;
            if (timer > 1.0f)
            {
                timer = 1.0f;
            }

            if (_spawnBoss == true)
            {
                imgSliderFill.color = Color.Lerp(sliderDefaultColor, sliderBossSpawnColor, timer);
            }
            else
            {
                imgSliderFill.color = Color.Lerp(sliderBossSpawnColor, sliderDefaultColor, timer);
            }
            yield return null;
        }
    }


    //true 로 변경하면 적들이 더이상 나오지 않게하는 용도로 활용

    [Header("적 생성 시간")]
    float enemySpawnTimer = 0.0f;//0초에서 시작되는 타이머
    [SerializeField, Range(0.1f, 5f)] float spawnTime = 1.0f;

    [Header("적 생성 위치")]
    [SerializeField] Transform trsSpawnPosition;
    [SerializeField] Transform trsDynamicObject;

    [Header("드롭아이템")]
    [SerializeField] List<GameObject> listItem;

    [Header("드롭 확률")]
    [SerializeField, Range(0.0f, 100.0f)] float itemDropRate;

    [Header("체력 게이지")]
    [SerializeField] FunctionHP functionHP;
    [SerializeField] Slider slider;
    [SerializeField] Image imgSliderFill;

    [Header("보스 포지션")]
    [SerializeField] Transform trsBossPostion;
    public Transform TrsBossPostion => trsBossPostion;//get 기능


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

    [Header("보스출현 조건")]
    [SerializeField] int killCount = 100;
    [SerializeField] int curKillCount = 80;
    [SerializeField] TMP_Text textSlider;

    [SerializeField] float bossSpawnTime = 60;
    [SerializeField] float bossSpawnTimer = 0f;

    [Header("점수")]
    [SerializeField] TMP_Text textScore;
    private int score;

    private bool gameStart = false;

    [Header("스타트텍스트")]
    [SerializeField] TMP_Text textStart;

    [Header("게임오버메뉴")]
    [SerializeField] GameObject objGameOverMenu;
    [SerializeField] TMP_Text textGameOverMenuScore;
    [SerializeField] TMP_Text textGameOverMenuRank;
    [SerializeField] TMP_Text textGameOverMenuBtn;
    [SerializeField] TMP_InputField IFGameOverMenuRank;
    [SerializeField] Button btnGameOverMenu;


    public bool GetPlayerPosition(out Vector3 _pos)
    {
        _pos = default;
        if (player == null)
        {
            return false;
        }
        else
        {
            _pos = player.transform.position;
            return true;
        }
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

        initSlider();
    }

    private void initSlider()
    {
        //킬카운트 버전 
        //slider.minValue = 0;
        //slider.maxValue = killCount;
        //slider.value = 0;
        //textSlider.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";

        //타이머 버전
        slider.minValue = 0;
        slider.maxValue = bossSpawnTime;
        modifySlider();
    }

    void Start()
    {
        StartCoroutine(doStartText());
    }

    IEnumerator doStartText()
    {
        Color color = textStart.color;
        color.a = 0f;
        textStart.color = color;

        while (true)
        {
            color = textStart.color;
            color.a += Time.deltaTime;
            if (color.a > 1.0f)
            {
                color.a = 1.0f;
            }
            textStart.color = color;

            if (color.a == 1.0f)
            {
                break;
            }
            yield return null;
        }

        while (true)
        {
            color = textStart.color;
            color.a -= Time.deltaTime;
            if (color.a < 0.0f)
            {
                color.a = 0.0f;
            }
            textStart.color = color;

            if (color.a == 0.0f)
            {
                break;
            }
            yield return null;
        }

        Destroy(textStart.gameObject);

        gameStart = true;
        isSpawn = true;
    }
    
    void Update()//프레임당 한번 실행되는 함수
    {
        if (gameStart == false) return;
        createEnemy();
        checkTimer();
    }

    private void checkTimer()
    {
        if (isSpawnBoss == false)
        {
            bossSpawnTimer += Time.deltaTime;
            modifySlider();
            if (bossSpawnTimer >= bossSpawnTime)//시간 변경이 완료되고 난뒤 보스가 출현
            {
                checkSpawnBoss();
            }
        }
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

            GameObject go = Instantiate(listEnemy[iRand], defulatPos, Quaternion.identity, trsDynamicObject);
            //생성할 위치, 다이나믹 오브젝트 위치가 필요

            //주사위를 굴림
            float rate = Random.Range(0.0f, 100.0f);
            if (rate <= itemDropRate)
            {
                //적기가 아이템을 가지고 있음
                Enemy goSc = go.GetComponent<Enemy>();
                goSc.SetItem();
            }

            enemySpawnTimer = 0.0f;
        }
    }

    public void createItem(Vector3 _pos)
    {
        int count = listItem.Count;
        int iRand = Random.Range(0, count);
        Instantiate(listItem[iRand], _pos, Quaternion.identity, trsDynamicObject);
    }

    public void createItem(Vector3 _pos, Item.eItemType _type)//0은 없음, 1 파워업, 2 체력회복
    {
        if (_type == Item.eItemType.None) return;
        Instantiate(listItem[(int)_type - 1], _pos, Quaternion.identity, trsDynamicObject);
    }

    public void SetHp(float _maxHp, float _curHp)
    {
        //펑션 hp에게 알려줘야함
        functionHP.SetHp(_maxHp, _curHp);
    }

    public void AddKillCount()
    {
        curKillCount++;
        //modifySlider();
        //checkSpawnBoss();
    }

    public void AddScore(int _value)//자신이 몇점인지 전달
    {
        score += _value;
        textScore.text = $"{score.ToString("d8")}";
    }

    private void modifySlider()
    {
        //킬 카운트 버전
        //slider.value = curKillCount;
        //textSlider.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";

        //타이머 버전
        slider.value = bossSpawnTimer;
        textSlider.text = $"{((int)bossSpawnTimer).ToString("d4")} / {((int)bossSpawnTime).ToString("d4")}";
    }

    private void checkSpawnBoss()
    {
        //킬 카운트 버전
        //if (isSpawnBoss == false && curKillCount >= killCount)//보스 출현
        //{
        //    isSpawn = false;
        //    isSpawnBoss = true;

        //    GameObject go = Instantiate(fabBoss, trsSpawnPosition.position, Quaternion.identity, trsDynamicObject);
        //}

        //타이버 버전
        if (isSpawnBoss == false)//보스 출현
        {
            isSpawn = false;
            IsSpawnBoss = true;

            GameObject go = Instantiate(fabBoss, trsSpawnPosition.position, Quaternion.identity, trsDynamicObject);
            //보스체력은 최대 몇으로 시작했는지
            EnemyBoss goSc = go.GetComponent<EnemyBoss>();
            setSliderBossType(goSc.Hp);
        }
    }

    private void setSliderBossType(float _maxHp)
    {
        slider.maxValue = _maxHp;
        slider.value = _maxHp;
        textSlider.text = $"{(int)_maxHp} / {(int)_maxHp}";
    }

    public void ModifyBossHp(float _hp)
    {
        slider.value = _hp;
        textSlider.text = $"{(int)_hp} / {(int)slider.maxValue}";
    }

    public void KillBoss()
    {
        bossSpawnTimer = 0;
        bossSpawnTime += 10;

        spawnTime -= 0.1f;

        //난이도 증가 기능을 추가하면 됨

        isSpawn = true;
        initSlider();

        IsSpawnBoss = false;
    }

    public void GameOver()
    {
        List<cUserData> listUserData = 
            JsonConvert.DeserializeObject<List<cUserData>>(PlayerPrefs.GetString(Tool.rankKey));

        int rank = -1;//0 이 1등
        int count = listUserData.Count;
        for (int iNum = 0; iNum < count; iNum++)
        {
            cUserData userData = listUserData[iNum];
            if (userData.Score < score)//내점수가 랭크보다 높다면 해당 랭크를 차지 해야함
            {
                rank = iNum;
                break;
            }
        }

        textGameOverMenuScore.text = $"점수 : {score.ToString("d8")}";

        //플레이어가 랭크에 들었는지 확인,몇등인지 데이터 필요
        if (rank != -1)
        {
            textGameOverMenuRank.text = $"랭킹 : {rank + 1}등";
            IFGameOverMenuRank.gameObject.SetActive(true);
            textGameOverMenuBtn.text = "등록";
        }
        else//랭크안에 들지 못했다면 이름을 적을 필요가 없음
        {
            textGameOverMenuRank.text = "랭크인 하지 못했습니다";
            IFGameOverMenuRank.gameObject.SetActive(false);
            textGameOverMenuBtn.text = "메인메뉴로";
        }

        //textGameOverMenuRank.text = rank != -1 ? $"랭킹 : {rank + 1}등" : "랭크인 하지 못했습니다";
        //IFGameOverMenuRank.gameObject.SetActive(rank != -1);
        //textGameOverMenuBtn.text = rank != -1 ? "등록" : "메인메뉴로";

        btnGameOverMenu.onClick.AddListener(() => 
        {
            //랭크인을 했다면 랭크와 이름을 저장
            if (rank != -1)
            {
                string name = IFGameOverMenuRank.text;

                if (name == string.Empty)
                {
                    name = "AAA";
                }

                cUserData newRank = new cUserData();
                newRank.Score = score;
                newRank.Name = name;

                listUserData.Insert(rank, newRank);
                listUserData.RemoveAt(listUserData.Count - 1);

                string value = JsonConvert.SerializeObject(listUserData);
                PlayerPrefs.SetString(PlayerPrefs.GetString(Tool.rankKey), value);
            }

            FunctionFade.Instance.ActiveFade(true, () => 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                FunctionFade.Instance.ActiveFade(false);
            });
        });

        objGameOverMenu.SetActive(true);
    }
}
