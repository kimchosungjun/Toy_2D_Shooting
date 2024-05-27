using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //private 자식에게 아무데이터도 전달 혹은 활용할수 있도록 제공하지 않음
    //protected 선언시 자식도 활용할수 있도록 해줌

    #region 프로텍티드 데이터들
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float hp;
    protected bool isDied = false;//적기가 죽고나면 더이상 기능을 반복실행하지 않도록 해줌
    protected GameObject fabExplosion;
    protected GameManager gameManager;
    protected SpriteRenderer spriteRenderer;
    #endregion
    #region 프리베이트 데이터
    Sprite defaultSprite;
    [SerializeField] private Sprite hitSprite;
    bool haveItem = false;
    [Header("아이템 보유시 컬러")]
    [SerializeField] Color colorHaveItem;
    #endregion

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        defaultSprite = spriteRenderer.sprite;

        if (haveItem == true)
        {
            spriteRenderer.color = colorHaveItem;
        }

        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
    }

    protected virtual void Update()
    {
        moving();
    }

    protected virtual void moving()
    {
        transform.position -= transform.up * moveSpeed * Time.deltaTime;

        //transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        //transform.position += transform.rotation * Vector3.down * moveSpeed * Time.deltaTime;
    }

    public virtual void Hit(float _damage)
    {
        if (isDied == true)
        {
            return;    
        }

        hp -= _damage;

        if (hp <= 0)
        {
            isDied = true;
            Destroy(gameObject);
            //매니저로부터 받아온 폭발 연출을 내 위치에 생성하고 부모로 사용중인 레이어에 만들어줌
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            Explosion goSc = go.GetComponent<Explosion>();

            //직사각형
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//현재 기체의 이미지 길이를 넣어줌

            //매니저를 호출후 현재 내 위치를 전달하면 매니저가 아이템을 그 위치에 만들어줌
            if (haveItem == true)
            { 
                gameManager.createItem(transform.position);
            }

            gameManager.AddKillCount();
        }
        else
        {
            //hit 연출 스프라이트 변경기능
            spriteRenderer.sprite = hitSprite;
            //약간의 시간이 지난뒤에 어떤 함수를 실행하고 싶을때
            Invoke("setDefaultSprite", 0.04f);
        }
    }

    private void setDefaultSprite()
    {
        spriteRenderer.sprite = defaultSprite;
    }

    public void SetItem()
    {
        haveItem = true;
    }
}
