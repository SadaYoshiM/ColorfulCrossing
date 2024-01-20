using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockBehaviour : MonoBehaviour
{
    private GameManager gameManager;
    private int x, y;
    void Start()
    {
        x = getCoordinateX(this.transform.position.x);
        y = getCoordinateY(this.transform.position.y);
        GameObject mngr = GameObject.Find("GameManager");
        gameManager = mngr.GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
            //Effect
            gameManager.breakBlocks[y, x] = true;

            if(this.gameObject.GetComponent<Renderer>().material.color == collision.gameObject.GetComponent<Renderer>().material.color)
            {
                //ŽüˆÍ‚àÁ‚·
                if(x + 1 >= 0 && x + 1 <= 3)
                {
                    gameManager.breakBlocks[y, x + 1] = true;
                }
                if (x - 1 >= 0 && x - 1 <= 3)
                {
                    gameManager.breakBlocks[y, x - 1] = true;
                }
                if (y + 1 >= 0 && y + 1 <= 3)
                {
                    gameManager.breakBlocks[y + 1, x] = true;
                }
                if (y - 1 >= 0 && y - 1 <= 3)
                {
                    gameManager.breakBlocks[y - 1, x] = true;
                }
                FindObjectOfType<GameManager>().combo += 1;
            }
            else
            {
                FindObjectOfType<GameManager>().combo = 1;
            }
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
