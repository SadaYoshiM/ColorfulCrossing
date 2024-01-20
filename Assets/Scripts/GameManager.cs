using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Opening,
        Playing,
        Over
    }
    public GameState currentState = GameState.Opening;

    [SerializeField] GameObject[] SpawnPoint;
    [SerializeField] Material[] Materials;
    public GameObject Ball;
    public GameObject Block;
    public Text BallQueueText;
    public Text TimerText;
    public int score;
    public int combo = 0;

    private const int spawnPointSize = 17;
    private const int colorSize = 4;
    private const int listSize = 5;
    //private float ballSpeed = 25.0f;
    private float flashSpeed = 3.0f;
    private float timer;

    private GameObject[,] blocks = new GameObject[(spawnPointSize - 1) / 4, (spawnPointSize - 1) / 4];
    public bool[,] breakBlocks = new bool[(spawnPointSize - 1) / 4, (spawnPointSize - 1) / 4];
    private int[,] arrangeBlocks = new int[(spawnPointSize - 1) / 4, (spawnPointSize - 1) / 4];
    private Queue<int> colorlist = new Queue<int>();
    private float parameterY = 0.15f;
    private float parameterZ = -0.32f;
    private Vector3 shootDirModifier;

    void Start()
    {
        Initialize();
        timer = 0;
    }

    void Update()
    {
        if(currentState == GameState.Opening && Input.GetKeyDown(KeyCode.Space))
        {
            dispatch(GameState.Playing);
        }
        if(currentState == GameState.Playing)
        {
            if(timer > 30)
            {
                dispatch(GameState.Over);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitMenu();
        }

        score = 0;
        timer += Time.deltaTime;
        TimerText.text = "Time : " + (60 - (int)timer).ToString();

        //マウスクリックで弾を発射
        if (Input.GetMouseButtonDown(0) && colorlist.Count != 0)
        {
            Vector3 spawnBallPos = SpawnPoint[16].transform.position + new Vector3(0f, 0.2f, 0.5f);
            GameObject ball = Instantiate(Ball, spawnBallPos, SpawnPoint[16].transform.rotation);
            ball.GetComponent<Renderer>().material = Materials[colorlist.Dequeue()];
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 dir = ray.direction + shootDirModifier;
            ball.GetComponent<BallBehaviour>().shoot(dir.normalized);
            displayBallQueueText();
        }

        checkBlocks();
        if (score > 0)
        {
            Debug.Log("combo : " + combo + " score : " + score);
            FindObjectOfType<Score>().AddScore(score * combo);
        }

        if(colorlist.Count <= 0)
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
            if(balls.Length <= 0)
            {
                checkScore();
                reload();
            }
        }


        //リセット処理
        if (Input.GetKeyDown(KeyCode.Space) || checkScore())
        {
            reload();
        }

    }

    void Initialize()
    {
        initArrangeList();
        generateBlocks();
        setBalls();
        displayBallQueueText();
        shootDirModifier = new Vector3(0f, parameterY, parameterZ);
    }

    void reload()
    {
        destroyAllBlocks();
        destroyAllBalls();
        Initialize();
    }

    void QuitMenu()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Application.Quit();
        }
    }
    public void GameOpening()
    {

    }
    public void GamePlaying()
    {

    }
    public void GameOver()
    {

    }

    public void dispatch(GameState state)
    {
        GameState oldState = currentState;
        currentState = state;
        switch (state)
        {
            case GameState.Opening:
                GameOpening();
                break;
            case GameState.Playing:
                GamePlaying();
                break;
            case GameState.Over:
                GameOver();
                break;
        }
    }

    void generateBlocks()
    {
        for (int i = 0; i < (spawnPointSize - 1) / 4; i++)
        {
            for(int j = 0; j < (spawnPointSize - 1) / 4; j++)
            {
                blocks[i, j] = Instantiate(Block, SpawnPoint[i * ((spawnPointSize - 1) / 4) + j].transform.position, SpawnPoint[i * ((spawnPointSize - 1) / 4) + j].transform.rotation);
                blocks[i, j].GetComponent<MeshRenderer>().material = Materials[arrangeBlocks[i, j]];
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

    void setBalls()
    {
        colorlist.Clear();
        //ボールのリスト生成
        for (int i = 0; i < listSize; i++)
        {
            appendColorlist();
        }
    }

    void displayBallQueueText()
    {
        BallQueueText.text = "";
        for(int i = 0; i < colorlist.Count; i++)
        {
            int indexC = colorlist.Dequeue();
            colorlist.Enqueue(indexC);
            string colorcode = UnityEngine.ColorUtility.ToHtmlStringRGB(Materials[indexC].color);
            BallQueueText.text += "<color=#" + colorcode + ">●</color>";
        }

        for(int i = 0; i < listSize - colorlist.Count; i++)
        {
            BallQueueText.text += "<color=#C8C8C8>●</color>";
        }

        if(colorlist.Count <= 0)
        {
            BallQueueText.color = flashColor(BallQueueText.color);
        }
    } 

    Color flashColor(Color color)
    {
        float time = 0;
        time += Time.deltaTime * flashSpeed;
        color.a = Mathf.Sin(time);

        return color;
    }

    void checkBlocks()
    {
        for(int i = 0; i < (spawnPointSize - 1) / 4; i++)
        {
            for (int j = 0; j < (spawnPointSize - 1) / 4; j++)
            {
                if (breakBlocks[i, j] && blocks[i, j] != null)
                {
                    score += 10;
                    //
                    blocks[i, j].SetActive(false);
                    blocks[i, j] = null;
                }
            }
        }
    }
    bool checkScore()
    {
        int count = 0;
        for (int i = 0; i < (spawnPointSize - 1) / 4; i++)
        {
            for (int j = 0; j < (spawnPointSize - 1) / 4; j++)
            {
                if (breakBlocks[i, j])
                {
                    count += 1;
                }
            }
        }
        if(count == spawnPointSize - 1)
        {
            Debug.Log("Perfect!");
            FindObjectOfType<Score>().AddScore(100);
            return true;
        }
        return false;
    }


    IEnumerator DelayCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
