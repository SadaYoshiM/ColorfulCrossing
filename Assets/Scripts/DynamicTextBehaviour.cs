using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTextBehaviour : MonoBehaviour
{
    private float lifeTime;
    private float maxLifeTime = 0.7f;
    private float constSpeed = 0.15f;
    // Start is called before the first frame update
    void Start()
    {
        lifeTime = 0;
    }

    // Update is called once per frame
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
