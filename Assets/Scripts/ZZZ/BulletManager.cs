using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public float speed;

    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Àû Å¸°Ý");
            Destroy(this.gameObject);
        }
        if(collision.gameObject.tag != "Player")
        {
            Destroy(this.gameObject);
        }
    }
    //https://m.blog.naver.com/1009unity/222252399473
}
