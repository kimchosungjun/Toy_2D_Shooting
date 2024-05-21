using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float hp;

    Sprite defaultSprite;
    [SerializeField] Sprite hitSprite;
    SpriteRenderer spriteRenderer;
    GameObject fabExplosion;
    GameManager gameManager;
    bool haveItem = false;

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        if (haveItem == true)
        {
            spriteRenderer.color = new Color(0.3f, 0.5f, 1f, 1f);
        }
        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
    }

    void Update()
    {
        moving();
    }

    private void moving()
    {
        transform.position -= transform.up * moveSpeed * Time.deltaTime;

        //transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        //transform.position += transform.rotation * Vector3.down * moveSpeed * Time.deltaTime;
    }

    public void Hit(float _damage)
    {
        hp -= _damage;

        if (hp <= 0)
        {
            Destroy(gameObject);
            //매니저로부터 받아온 폭발 연출을 내 위치에 생성하고 부모로 사용중인 레이어에 만들어줌
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            Explosion goSc = go.GetComponent<Explosion>();

            //직사각형
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//현재 기체의 이미지 길이를 넣어줌
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
}
