using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
 * 座標注意(左上から、(-3, -3),(-3,-1),(-3,1),(-3,3),(-1,-3),...)
 * y↑ □□□□
 *     □□□□
 *     □□□□
 *     □□□□
 *     　　　→x
 * xy座標系上の(x,y)=行列での添え字(y,x)
 */

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
            gameManager.breakBlocks[y, x] = true;

            if(this.gameObject.GetComponent<Renderer>().material.color == collision.gameObject.GetComponent<Renderer>().material.color)
            {
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
                FindObjectOfType<GameManager>().comboIncrement();
            }
            else
            {
                FindObjectOfType<GameManager>().comboReset();
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
