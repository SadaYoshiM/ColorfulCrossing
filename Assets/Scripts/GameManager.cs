using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // public
    public enum GameState
    {
        Opening,
        Playing,
        Over
    }
    [SerializeField] GameObject[] SpawnPoint;
    [SerializeField] Material[] Materials;
    [SerializeField] AudioSource[] Audios;
    public GameObject MainPanel;
    public GameObject OneningHTP;
    public GameObject PnayingHTP;
    public GameObject Ball;
    public GameObject Block;
    public GameObject Canvas;
    public GameObject DynamicText;
    public GameObject BreakBlast;
    public Text BallQueueText;
    public Text TimerText;
    public Text ComboText;
    public int score;
    public int combo = 0;
    public bool[,] breakBlocks = new bool[(spawnPointSize - 1) / 4, (spawnPointSize - 1) / 4];

    // private
    private GameState currentState = GameState.Opening;
    private GameObject[,] blocks = new GameObject[(spawnPointSize - 1) / 4, (spawnPointSize - 1) / 4];
    private Queue<int> colorlist = new Queue<int>();
    private Transform mainText;
    private const int spawnPointSize = 17;
    private const int colorSize = 4;
    private const int colorlistSize = 5;
    private float timer;
    private float gameTime = 60f;
    private int[,] arrangeBlocks = new int[(spawnPointSize - 1) / 4, (spawnPointSize - 1) / 4];
    private Vector3 shootDirModifier = new Vector3(0f, 0.15f, -0.32f);

    void Start()
    {
        GameOpening();
    }

    void Update()
    {
        if(currentState == GameState.Opening && Input.GetKeyDown(KeyCode.Space))
        {
            MainPanel.SetActive(false);
            OneningHTP.SetActive(false);
            PnayingHTP.SetActive(true);
            Initialize();
            dispatch(GameState.Playing);
        }
        if(currentState == GameState.Playing)
        {
            if(timer > gameTime)
            {
                dispatch(GameState.Over);
            }
            else
            {
                GamePlaying();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    public void GameOpening()
    {
        currentState = GameState.Opening;
        Initialize();
        FindObjectOfType<Score>().Initialize();
        MainPanel.SetActive(true);
        OneningHTP.SetActive(true);
        PnayingHTP.SetActive(false);
        mainText.gameObject.GetComponent<Text>().text = "Press SPACE to Start";
        mainText.gameObject.GetComponent<Text>().color = Color.white;
    }

    public void GamePlaying()
    {
        if (!Audios[0].isPlaying)
        {
            Audios[0].Play();
        }
        currentState = GameState.Playing;
        score = 0;
        timer += Time.deltaTime;
        TimerText.text = "Time : " + (gameTime - (int)timer).ToString();

        if (Input.GetMouseButtonDown(0) && colorlist.Count != 0)
        {
            playSE(Audios[1]);
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
            FindObjectOfType<Score>().AddScore(score, combo);
            if (combo > 0 && combo % 10 == 0)
            {
                generateDynamicText("COMBO BONUS +" + (combo * 10).ToString(), SpawnPoint[9].transform.position + new Vector3(-6.5f, 2f, 0f));
                playSE(Audios[3]);
            }
            ComboText.text = "Combo : " + combo.ToString();

        }

        if (colorlist.Count <= 0)
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
            if (balls.Length <= 0)
            {
                checkScore();
                reload();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || checkScore())
        {
            playSE(Audios[5]);
            reload();
        }
    }
    public void GameOver()
    {
        MainPanel.SetActive(true);
        PnayingHTP.SetActive(false);
        mainText.gameObject.GetComponent<Text>().text = "Finish!\nYour Score : " + FindObjectOfType<Score>().getScore();
        if (FindObjectOfType<Score>().scoreCompare()) 
        {
            mainText.gameObject.GetComponent<Text>().text += "\nHigh Score !!";
            playSE(Audios[4]);
        }
        mainText.gameObject.GetComponent<Text>().color = Color.white;
        FindObjectOfType<Score>().Save();

        Invoke("GameOpening", 5f);
    }

    public void dispatch(GameState state)
    {
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
    public void playSE(AudioSource audio)
    {
        audio.Play();
    }

    public void comboReset()
    {
        combo = 0;
        ComboText.text = "Combo : 0";
    }

    public void comboIncrement()
    {
        combo++;
    }

    void Initialize()
    {
        initArrangeList();
        generateBlocks();
        setBalls();
        displayBallQueueText();
        timer = 0;
        combo = 0;
        mainText = MainPanel.transform.GetChild(0);
        FindObjectOfType<Score>().setScore(0);
        ComboText.text = "Combo : " + combo.ToString();
    }

    void reload()
    {
        destroyAllBlocks();
        destroyAllBalls();
        initArrangeList();
        generateBlocks();
        setBalls();
        displayBallQueueText();
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
        
        for (int i = 0; i < colorlistSize; i++)
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
            BallQueueText.text += "<color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(Materials[indexC].color) + ">●</color>";
        }

        for(int i = 0; i < colorlistSize - colorlist.Count; i++)
        {
            BallQueueText.text += "<color=#C8C8C8>●</color>";
        }
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
                    GameObject blast = Instantiate(BreakBlast, SpawnPoint[i * ((spawnPointSize - 1) / 4) + j].transform.position + new Vector3((1.5f - j) / 4.0f, 0.5f, -1.1f), SpawnPoint[i * ((spawnPointSize - 1) / 4) + j].transform.rotation);
                    blast.GetComponent<ParticleSystem>().Play();
                    playSE(Audios[2]);
                    generateDynamicText("+10", SpawnPoint[i * ((spawnPointSize - 1) / 4) + j].transform.position + new Vector3((1.5f - j) / 4.0f, 0.5f, 0f));
                    Destroy(blocks[i, j]);
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
            generateDynamicText("PERFECT BONUS +100", SpawnPoint[9].transform.position + new Vector3(-6f, 1f, 0f));
            FindObjectOfType<Score>().AddScore(100, 0);
            playSE(Audios[3]);
            return true;
        }
        return false;
    }

    void generateDynamicText(string text, Vector3 pos)
    {
        GameObject dynamicText = Instantiate(DynamicText);
        dynamicText.transform.SetParent(Canvas.transform, false);
        dynamicText.GetComponent<Text>().text = "<b><color=#ffa500ff>" + text + "</color></b>";
        dynamicText.GetComponent<Text>().transform.position += pos;
    }

}
