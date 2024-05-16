using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float hp;

    protected Sprite defaultSprite;
    [SerializeField] protected Sprite hitSprite;
    SpriteRenderer spr;
    GameManager gameManager;
    GameObject fabExplosion;
    //Transform dynamicObject;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        defaultSprite = spr.sprite;
        // Resources.Load는 Resources에 데이터를 많이 넣으면 넣을수록 비효율적인 문제가 생긴다.
        // 모든 파일들을 검색하기 때문이다.
        //fabExplosion = Resources.Load<GameObject>("FabExplosion");
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
    }

    private void Update()
    {
        Moving();
    }

    public void Moving()
    {
        // 적은 거꾸로 오기 때문에 -=을 사용한다.
        transform.position -= transform.up * moveSpeed * Time.deltaTime;
    }

    public void Hit(float _damage)
    {
        hp -= _damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity,transform.parent);
            Explosion goSC = go.GetComponent<Explosion>();
            goSC.SetImageSize(spr.sprite.rect.width);
        }
        else
        {
            spr.sprite = hitSprite;
            Invoke("SetDefaultSprite", 0.04f);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
    
    private void SetDefaultSprite()
    {
        spr.sprite = defaultSprite;
    }
}
