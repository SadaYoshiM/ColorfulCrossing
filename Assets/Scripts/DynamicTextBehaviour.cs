using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTextBehaviour : MonoBehaviour
{
    private float lifeTime;
    private float maxLifeTime = 0.7f;
    private float constSpeed = 0.015f;

    void Start()
    {
        lifeTime = 0;
    }

    void Update()
    {
        lifeTime += Time.deltaTime;
        if(lifeTime > maxLifeTime)
        {
            Destroy(this.gameObject);
        }
        this.transform.position += new Vector3(0f, 1.0f, 0f) * constSpeed * Mathf.Sin(maxLifeTime - lifeTime);
    }
}
