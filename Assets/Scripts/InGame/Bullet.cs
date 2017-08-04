﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    float speed = 2000.0f;
    public const int damage = 25;
    float livedTime = 0.0f;
    // Use this for initialization


    Rigidbody ri;

    void Awake() {
        ri = GetComponent<Rigidbody>();
    }

	void Start () {
        ri.AddForce(transform.forward * speed);
	}
	
	// Update is called once per frame
	void Update () {
        livedTime += Time.deltaTime;
        if (livedTime >= 2.0f)
            gameObject.SetActive(false);
        if (!gameObject.activeSelf)
            ri.velocity = Vector3.zero;
	}
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Invincible")
        {
            gameObject.SetActive(false);
        }
    }
}
