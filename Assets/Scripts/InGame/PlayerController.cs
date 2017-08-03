﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    float oldTime = 0.0f;
    public static PlayerController instance;

    public bool isLocal = false, isInput = false;

    public float moveSpeed, rotateSpeed, lerpTime = 20f;

    Vector3 oldPos;

    //받는 값
    public Vector3 currentPos, currentVel;

    Quaternion oldRot;

    public int hp;

    //받는 값
    public Quaternion currentRot;

    public GameObject bulletPrefab;

    float h, v;

    [SerializeField]
    float syncTime;

    //캐싱
    Transform tr;
    Rigidbody ri;

    public Transform camPos;
    public Transform bulletPos;
    public void SetLocal(bool _isLocal)
    {
        isLocal = _isLocal;
        ri.useGravity = true;
        if (_isLocal)
            cameraRot.instance.SetTarget(camPos);
    }

    public void SetInput(bool _isInput)
    {
        isInput = _isInput;
    }


    private void Awake()
    {
        instance = this;
        tr = GetComponent<Transform>();
        oldRot = tr.rotation;
        oldPos = tr.position;
        hp = 100;
        ri = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (isLocal && isInput)
        {

            //에디터 전용 움직임
            h = joyStick.instance.Horizontal();
            v = joyStick.instance.Vertical();

        }
        else
        {
            syncTime += Time.deltaTime;
            if (currentVel.normalized != Vector3.zero)
            {
                // 달리는 애니메이션 실행
            }
            else
            {
                //대기 상태 애니메이션 실행
            }

        }

    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (isLocal)
        {
            if (h == 0 && v == 0)
            {
                ri.velocity = new Vector3(0, ri.velocity.y, 0);
            }
            else {
                Vector3 movePos = ((tr.forward * v * moveSpeed) + (tr.right * h * moveSpeed));
                movePos.y = ri.velocity.y;

                //에디터 전용 움직임
                ri.velocity = movePos;
            }

            SendPosition();
            SendRotation();
        }
        else
        {
            //현재 위치 = 이전 위치 + ( 속도 * 시간 ) + ( 1 / 2 * 가속도 * 시간 ^ 2 )
            tr.position = Vector3.Lerp(tr.position, currentPos + ((currentVel * syncTime) + (0.5f * currentVel * syncTime * syncTime)), Time.smoothDeltaTime * lerpTime);
        }
    }

    public void SendHp()
    {
        NetworkManager.instance.EmitHp(hp);
    }

    public void SendPosition()
    {
        if (oldPos != tr.position)
        {
            NetworkManager.instance.EmitMove(tr.position, ri.velocity);
            oldPos = tr.position;
        }
    }

    public void SendRotation()
    {
        if (oldRot.x != tr.rotation.x)
        {
            oldRot = tr.rotation;
            NetworkManager.instance.EmitRotate(tr.rotation);
        }
    }

    public void SetPosition(Vector3 _currentPos, Vector3 _currentVel)
    {
        syncTime = 0;
        currentPos = _currentPos;
        currentVel = _currentVel;
    }

    public void SetRotation(Quaternion _rot)
    {
        tr.rotation = _rot;
    }

    public void SetHp(int _hp)
    {
        hp = _hp;
        if (hp <= 0)
        {
            NetworkManager.instance.EmitDie(PlayerPrefs.GetInt("isCat"));
        }
    }
    /// <summary>
    /// 서버에서 발사를 받아서 쏩니다.
    /// </summary>
    public void ShotBullet()
    {
        GameObject b = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation);
        b.transform.SetParent(GameObject.Find("bullets").transform);
    }

    public void SendBulllet()
    {
        NetworkManager.instance.EmitBullet(0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.tag == "Mouse" && collision.gameObject.tag == "CatBullet" || gameObject.tag == "Cat" && collision.gameObject.tag == "MouseBullet")
        {
            hp -= Bullet.damage;
            collision.gameObject.SetActive(false);
            SendHp();
        }
    }
}
