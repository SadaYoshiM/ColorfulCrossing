using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    private int x, y;
    // Start is called before the first frame update
    void Start()
    {
        x = getCoordinateX(this.transform.position.x);
        y = getCoordinateY(this.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
            //Effect
            GameManager gameManeger;
            GameObject obj = GameObject.Find("GameManager");
            gameManeger = obj.GetComponent<GameManager>();
            gameManeger.breakBlocks[x, y] = true;
            Destroy(this.gameObject);
        }
        if(collision.gameObject.tag == "Block")
        {
            Destroy(this.gameObject);
        }
    }

    private int getCoordinateX(float N)
    {
        return ((int)N + 3) / 2;
    }

    private int getCoordinateY(float N)
    {
        return (3 - (int)N) / 2;
    }
}
