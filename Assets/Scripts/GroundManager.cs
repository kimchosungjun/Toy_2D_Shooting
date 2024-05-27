using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    Material matTop;
    Material matMid;
    Material matBot;

    [SerializeField] private float speedTop;
    [SerializeField] private float speedMid;
    [SerializeField] private float speedBot;

    GameObject objBackGround;

    void Start()
    {
        objBackGround = transform.Find("BackGround").gameObject;

        Transform trsTop = objBackGround.transform.Find("Top");
        Transform trsMid = objBackGround.transform.Find("Mid");
        Transform trsBot = objBackGround.transform.Find("Bot");

        SpriteRenderer sprTopRenderer = trsTop.GetComponent<SpriteRenderer>();
        SpriteRenderer sprMidRenderer = trsMid.GetComponent<SpriteRenderer>();
        SpriteRenderer sprBotRenderer = trsBot.GetComponent<SpriteRenderer>();

        matTop = sprTopRenderer.material;
        matMid = sprMidRenderer.material;
        matBot = sprBotRenderer.material;
    }

    void Update()//사양이 좋은 컴퓨터 1초에 700번, 사양이 안좋은 컴퓨터 1초에 30번
    {
        Vector2 vecTop = matTop.mainTextureOffset;
        Vector2 vecMid = matMid.mainTextureOffset;
        Vector2 vecBot = matBot.mainTextureOffset;

        vecTop += new Vector2(0, speedTop * Time.deltaTime);
        vecMid += new Vector2(0, speedMid * Time.deltaTime);
        vecBot += new Vector2(0, speedBot * Time.deltaTime);

        //if (vecTop.y > 1.0f)
        //{
        //    vecTop.y = 0.0f;
        //}

        vecTop.y = Mathf.Repeat(vecTop.y, 1.0f);
        vecMid.y = Mathf.Repeat(vecMid.y, 1.0f);
        vecBot.y = Mathf.Repeat(vecBot.y, 1.0f);

        matTop.mainTextureOffset = vecTop; 
        matMid.mainTextureOffset = vecMid; 
        matBot.mainTextureOffset = vecBot; 
    }
}
