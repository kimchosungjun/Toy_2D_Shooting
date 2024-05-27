using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy
{
    Transform trsBossPosition;//도착할 위치

    bool isMovingTrsBossPosition = false;//보스가 원위치까지 이동을 완료했는지
    Vector3 createPos = Vector3.zero;
    float timer = 0.0f;

    protected override void Start()
    {
        gameManager = GameManager.Instance;
        trsBossPosition = gameManager.TrsBossPostion;
        fabExplosion = gameManager.FabExplosion;
        createPos = transform.position;
    }

    protected override void moving()
    {
        if(isMovingTrsBossPosition == false) //위치까지 이동 
        {
            if (timer < 1.0f)
            { 
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(createPos, trsBossPosition.position, timer);
            }
        }
        else//이동 완료후 좌우로 이동하면서 패턴에 의한 공격
        {

        }
    }

    public override void Hit(float _damage)
    {
        if(isDied == true)
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

            //gameManager.AddKillCount();//보스가 죽었다고 전달 //다시 적들이 출동하도록 설계
        }
        else
        {
            //이 친구는 스프라이트만 활용하는것이 아니라 스프라이트 애니메이션을 활용함으로 애니메이션에서 히트 애님을 실행
        }
    }
}
