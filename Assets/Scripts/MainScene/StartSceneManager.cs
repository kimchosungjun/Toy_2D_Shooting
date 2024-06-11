using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] Button btnStart;
    [SerializeField] Button btnRanking;
    [SerializeField] Button btnExitRanking;
    [SerializeField] Button btnExit;
    [SerializeField] GameObject viewRank;

    [Header("랭크 프리팹")]
    [SerializeField] GameObject fabRank;
    [SerializeField] Transform contents;

    private string dataPath;
    private void Awake()
    {
        #region 수업내용
        //btnStart.onClick.AddListener(function);

        //UnityAction<float> action = (float _value) => 
        //{
        //    Debug.Log($"람다식이 실행 되었음 => {_value}");
        //};

        //람다식
        //이름없는 함수
        //action.Invoke(0.1f);//람다식은 특정 이벤트나 invoke를 통해서 실행가능

        //btnStart.onClick.AddListener(() => 
        //{
        //    gameStart(1, 2, 3, 4, 5);
        //});
        #endregion
        btnStart.onClick.AddListener(gameStart);
        btnRanking.onClick.AddListener(showRanking);
        btnExitRanking.onClick.AddListener(() => { viewRank.SetActive(false); });
        btnExit.onClick.AddListener(gameExit);
        #region 수업내용
        //json
        //string 문자열, 키와 벨류
        //{key:value};

        //save기능, 씬과 씬을 이동할때 가지고 가야하는 데이터가 있다면

        //1.플레이어 프랩스를 이용해 유니티에 저장하는 방법
        //PlayerPrefs//유니티가 꺼져도 데이터를 보관하도록 유니티 내부에 저장 

        //PlayerPrefs.SetInt("test", 999);//숫자 데이터 1개만 저장 setint setfloat
        //데이터를 삭제하지 않는한 //test 999, 게임을 삭제하면 이데이터는 삭제되고 불러올수 없음
        //int value = PlayerPrefs.GetInt("test", -1);//int의 디폴트 0을 출력
        //Debug.Log(value);

        //PlayerPrefs.hasKey
        //PlayerPrefs.DeleteKey("test");

        //string path = Application.streamingAssetsPath;//os에 따라 읽기전용으로 사용됨
        //~/Assets/StreamingAssets
        //File.WriteAllText(path + "/abc.json", "테스트22");
        //File.Delete(path + "/abc.json");
        //string result = File.ReadAllText(path + "/abc.json");
        //Debug.Log(result);

        //string path = Application.persistentDataPath + "/Jsons";//R/W가 가능한 폴더위치
        //~/AppData/LocalLow/DefaultCompany/Class6/Jsons

        //if (Directory.Exists(path) == false)
        //{
        //    Directory.CreateDirectory(path);
        //}
        //if (File.Exists(path + "/Test/abc.json") == true)
        //{
        //    string result = File.ReadAllText(path + "Test/abc.json");
        //}
        //else//저장한 파일이 존재하지 않음
        //{
        //    //새로운 저장 위치와 데이터를 만들어 줘야함
        //    File.Create(path + "/Test");//폴더를 만들어줌
        //}

        //string jsonData = JsonUtility.ToJson(cUserData);
        //{"Name":"가나다","Score":100}

        //cUserData user2 = new cUserData();
        //user2 = JsonUtility.FromJson<cUserData>(jsonData);

        //string jsonData = JsonUtility.ToJson(listUserData);
        //JsonUtility 는 list를 json으로 변경하면 트러블이 존재함

        //List<cUserData> listUserData = new List<cUserData>();
        //listUserData.Add(new cUserData() { Name = "가나다", Score = 100 });
        //listUserData.Add(new cUserData() { Name = "라마바", Score = 200 });

        //string jsonData = JsonConvert.SerializeObject(listUserData);

        //List<cUserData> afterData = JsonConvert.DeserializeObject<List<cUserData>>(jsonData);
        #endregion

        initRankView();
        viewRank.SetActive(false);
    }

    /// <summary>
    /// 랭크가 저장되어 있다면 저장된 랭크 데이터를 이용해서 랭크뷰를 만들어주고
    /// 랭크가 저장되어 있지 않다면 비어있는 랭크를 만들어 랭크뷰를 만들어줌
    /// </summary>
    private void initRankView()
    {
        List<cUserData> listUserData = null;
        clearRankView();
        if (PlayerPrefs.HasKey(Tool.rankKey) == true)//랭크 데이터가 저장이 되어있었다면
        {
            listUserData = JsonConvert.DeserializeObject<List<cUserData>>(PlayerPrefs.GetString(Tool.rankKey));
        }
        else//랭크데이터가 저장되어 있지 않았다면
        {
            listUserData = new List<cUserData>();
            int rankCount = Tool.rankCount;
            for (int iNum = 0; iNum < rankCount; ++iNum)
            {
                listUserData.Add(new cUserData() { Name = "None", Score = 0 });
            }

            string value = JsonConvert.SerializeObject(listUserData);
            PlayerPrefs.SetString(Tool.rankKey, value);
        }

        int count = listUserData.Count;
        for (int iNum = 0; iNum < count; ++iNum)
        {
            cUserData data = listUserData[iNum];

            GameObject go = Instantiate(fabRank, contents);
            FabRanking goSc = go.GetComponent<FabRanking>();
            goSc.SetData((iNum + 1).ToString(), data.Name, data.Score);
        }
    }

    private void clearRankView()
    {
        int count = contents.childCount;
        for (int iNum = count - 1; iNum > -1; --iNum)
        {
            Destroy(contents.GetChild(iNum).gameObject);
        }
    }

    private void gameStart()
    {
        FunctionFade.Instance.ActiveFade(true, () => 
        {
            SceneManager.LoadScene(1);
            FunctionFade.Instance.ActiveFade(false);
        });
    }

    private void showRanking()
    {
        viewRank.SetActive(true);
    }

    private void gameExit()
    {
        //에디터에서 플레이를 끄는 방법, 에디터 전용 기능
        //빌드를 통해서 밖으로 가지고 나가서는 안됨
        //전처리,코드가 조건에 의해서 본인이 없는것처럼 혹은 있는것처럼 
        //동작하게 해줌

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else//유니티 에디터에서 실행하지 않았을때
        //빌드했을때 게임 종료
        Application.Quit();
#endif
    }
}

