using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    bool isShootEnemy = true;

    //적기에 닿았을때 or 플레이어에 닿았을때
    //몇초뒤에 사라진다고 명령했을때
    //화면밖으로 나갔을때

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)//collision은 상대 콜리전
    {
        //때릴 대상을 정확히 할 필요가 있음
        if (isShootEnemy == false && collision.tag == "Enemy")
        {
            Destroy(gameObject);//총알 본인이 삭제
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.Hit(1);
        }
        if (isShootEnemy == true && collision.tag == "Player")
        {
            Destroy(gameObject);//총알 본인이 삭제
            Player player = collision.GetComponent<Player>();
            player.Hit();
        }
    }

    void Start()
    {
        //Destroy(gameObject, 2.5f);
    }

    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }

    public void ShootPlayer()
    {
        isShootEnemy = false;
    }
}
