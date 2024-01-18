using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private float lifeTime = 0f;
    private float maxLifeTime = 3.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if(lifeTime > maxLifeTime)
        {
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
        GetComponent<Rigidbody>().velocity = dir;
    }
}
