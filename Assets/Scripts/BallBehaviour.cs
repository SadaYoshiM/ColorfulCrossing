using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private float shootSpeed = 20.0f;

    void Start()
    {

    }

    void Update()
    {
        if(this.transform.position.z > 2)
        {
            FindObjectOfType<GameManager>().combo = 0;
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Block")
        {
            //Effect
            Destroy(this.gameObject, 0f);
        }
    }

    public void shoot(Vector3 dir)
    {
        GetComponent<Rigidbody>().velocity = dir * shootSpeed;
    }
}
