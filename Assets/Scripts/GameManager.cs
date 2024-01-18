using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] SpawnPoint;
    [SerializeField] Material[] Materials;
    public GameObject Ball;
    public GameObject Block;

    private const int spawnPointSize = 17;
    private const int colorSize = 4;
    private const int listSize = 5;
    private float ballSpeed = 30.0f;

    public bool[,] breakBlocks = new bool[(spawnPointSize - 1) / 4, (spawnPointSize - 1) / 4];
    private int[,] arrangeBlocks = new int[(spawnPointSize - 1) / 4, (spawnPointSize - 1) / 4];
    private Queue<int> colorlist = new Queue<int>();
    private float parameterY = 0.15f;
    private float parameterZ = -0.32f;
    private Vector3 shootDirModifier;
    void Start()
    {
        //ブロックの生成
        initArrangeList();
        generateBlocks();

        //ボールのリスト生成
        for(int i = 0; i < listSize; i++)
        {
            appendColorlist();
        }

        shootDirModifier = new Vector3(0f, parameterY, parameterZ);

    }

    // Update is called once per frame
    void Update()
    {
        //マウスクリックで弾を発射
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 spawnBallPos = SpawnPoint[16].transform.position + new Vector3(0f, 0.2f, 0.5f);
            GameObject ball = Instantiate(Ball, spawnBallPos, SpawnPoint[16].transform.rotation);
            ball.GetComponent<Renderer>().material = Materials[colorlist.Dequeue()];
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 dir = ray.direction + shootDirModifier;
            ball.GetComponent<BallBehaviour>().shoot(dir.normalized * ballSpeed);
            appendColorlist();
            //DelayCoroutine();
        }

        //リセット処理
        if (Input.GetKeyDown(KeyCode.Space))
        {
            destroyAllBlocks();
            destroyAllBalls();
            initArrangeList();
            generateBlocks();
        }

    }

    void generateBlocks()
    {
        for (int i = 0; i < (spawnPointSize - 1) / 4; i++)
        {
            for(int j = 0; j < (spawnPointSize - 1) / 4; j++)
            {
                GameObject block = Instantiate(Block, SpawnPoint[i * ((spawnPointSize - 1) / 4) + j].transform.position, SpawnPoint[i * ((spawnPointSize - 1) / 4) + j].transform.rotation);
                block.GetComponent<MeshRenderer>().material = Materials[arrangeBlocks[i, j]];
            }
        }
    }

    void initArrangeList()
    {
        for (int i = 0; i < (spawnPointSize - 1) / 4; i++)
        {
            for (int j = 0; j < (spawnPointSize - 1) / 4; j++)
            {
                arrangeBlocks[i, j] = Random.Range(0, colorSize);
                breakBlocks[i, j] = false;
            }
        }
    }

    void destroyAllBlocks()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach(GameObject block in blocks)
        {
            Destroy(block, 0f);
        }
    }

    void destroyAllBalls()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in balls)
        {
            Destroy(ball, 0f);
        }
    }

    void appendColorlist()
    {
        colorlist.Enqueue(Random.Range(0, colorSize));
    }

    IEnumerator DelayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
