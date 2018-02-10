using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct GZ_Player
{
    public int Index;
    public int TileScore;
    public int ComboScore;
    public Color PColor;
}

public class GZ_GameSystem: MonoBehaviour
{
    public static GZ_GameSystem Instance { get; private set; }

    public int NumPlayer;
    public Color[] PlayerColors;
    private GZ_Player[] _players;
    public static int[] TotalScores;

    public int RowCount;
    public int ColCount;
    public  int TotalTileCount;
    public int OccupiedTileCount;

    public static GZ_Player CurrentPlayer;

    private int _currentPlayerIndex = -1;
    private bool _IsIgnoreInput;

    public GameObject[] SwitchPlayerImage;
    public AnimationCurve MainUIEnableCurve;

    public RectTransform[] ScoreBoardTransforms;
    public Text[] Scoreboard;
    public float[] ScoreBoardPositionX;

    public GameObject[] GameOverImages;

    public GameObject LogoObject;
    public GameObject RecordObject;
    

    public Transform Barrier;
    bool _isBarrierOpen = false;

    private void Awake()
    {
        Instance = this;
        TotalScores = new int[2];
    }

    private void Start()
    {
        StartGame();
    }

    public void SetScoreBoard(int playerIndex, int score)
    {
        Scoreboard[playerIndex].text = score.ToString();
    }

    public void EnableRecordSession(bool enabled)
    {
       
        RecordObject.SetActive(enabled);

        if (enabled)
        {
           

            StartCoroutine(GZ_Math.PlayAnim_CR(0.5f,
             (progress) =>
             {
                 EnableScoreBoard(1f - progress);
                 Barrier.transform.localScale = Vector3.one * Mathf.Lerp(0.2f, 0, 1 - progress);
             },
             () =>
             {

             }));
        }else
        {
            StartGame();
        }
    }

    public void EnableBarrier(bool enabled)
    {
        //LogoObject.SetActive(!enabled);

        if (enabled)
        {
            _IsIgnoreInput = false;
        }
        else
        {
            _isBarrierOpen = true;
            _IsIgnoreInput = true;
            HideAllUI();
            //GZ_Crowd.Instance.DestroyCrowd();
            //GZ_CircleTable.Instance.ClearTable();

            StartCoroutine(GZ_Math.PlayAnim_CR(0.5f,
                 (progress) =>
                 {
                     EnableScoreBoard(1f - progress);
                     Barrier.transform.localScale = Vector3.one * Mathf.Lerp(0.2f, 0, 1 - progress);
                 },
                 () =>
                 {

                 }));
        }
    }


    public void RestartGame()
    {
        _IsIgnoreInput = true;
        _isBarrierOpen = false;

        GZ_CircleTable.Instance.ClearTable();

        StartCoroutine(GZ_Math.PlayAnim_CR(0.5f, 
            (progress) => 
            {
                EnableScoreBoard(1f - progress);
                Barrier.transform.localScale = Vector3.one * Mathf.Lerp(0.2f, 0, 1 -progress);
            }, 
            () => 
            {
                StartGame();
            }));

    }

    void HideAllUI()
    {
        SwitchPlayerImage[0].SetActive(false);
        SwitchPlayerImage[1].SetActive(false);

        for (int i =0; i < GameOverImages.Length; i++)
            GameOverImages[i].SetActive(false);
    }

    void EnableScoreBoard(float percentage)
    {
        var board1Pos = ScoreBoardTransforms[0].anchoredPosition;
        board1Pos.x = Mathf.Lerp(ScoreBoardPositionX[0], ScoreBoardPositionX[1], percentage);

        ScoreBoardTransforms[0].anchoredPosition = board1Pos;

        var board2Pos = ScoreBoardTransforms[1].anchoredPosition;
        board2Pos.x = Mathf.Lerp(ScoreBoardPositionX[1], ScoreBoardPositionX[0], percentage);
        ScoreBoardTransforms[1].anchoredPosition = board2Pos;
    }

    void ResestUIIMAge()
    {
        for(int i =0; i < GameOverImages.Length; i++)
            GameOverImages[i].SetActive(false);
    }

    void StartGame()
    {
        _IsIgnoreInput = true;
        GZ_Crowd.Instance.CreateCrowd();



        StartCoroutine(GZ_Math.PlayAnim_CR(0.5f,
            (progress) =>
            {
                Barrier.transform.localScale = Vector3.one * Mathf.Lerp(0.2f, 0, progress);
            },
            () =>
            {

                Scoreboard[0].text = Scoreboard[1].text = "0";
                TotalScores[0] = TotalScores[1] = 0;
                
                _currentPlayerIndex = -1;
                _players = new GZ_Player[NumPlayer];

                for (int i = 0; i < NumPlayer; i++)
                {
                    _players[i].Index = i;
                    _players[i].TileScore = _players[i].ComboScore = 0;
                    _players[i].PColor = PlayerColors[i];
                }

                StartCoroutine(GZ_Math.PlayAnim_CR(1f,
                    (percentage) =>
                    {
                        EnableScoreBoard(percentage);
                    },
                    () =>
                    {
                        GZ_RadioEmitter.Instance.Bang();
                        _IsIgnoreInput = false;

                        GZ_CircleTable.Instance.SpawnTable(RowCount, ColCount);

                        TotalTileCount = RowCount * ColCount;
                        OccupiedTileCount = 0;

                        SwitchPlayer();
                    }));
            }));
    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.R))
        {
            ResestUIIMAge();
            GZ_RecordModule.Instance.HideAll();
            RestartGame();
        }

        if (Input.GetKeyUp(KeyCode.Z)) //player 1
        {
            GZ_RecordModule.Instance.SetUpPlayer(0);
        }

        if (Input.GetKeyUp(KeyCode.X)) //player 2
        {
            GZ_RecordModule.Instance.SetUpPlayer(1);
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            Debug.Log("dddd");
            EnableBarrier(false);
            HideAllUI();
        }


        //Debug.Log(_IsIgnoreInput);
        if (_IsIgnoreInput || _isBarrierOpen) return;


        if (Input.GetMouseButtonUp(0))
        {
            bool res = GZ_CircleTable.Instance.SelectTile(ref _players[_currentPlayerIndex]);
            if (res)
            {
                CalculateScore();
                SwitchPlayer();
                GZ_Crowd.Instance.ApplyColor();
              


                TotalScores[0] = _players[0].TileScore + _players[0].ComboScore * 2;
                TotalScores[1] = _players[1].TileScore + _players[1].ComboScore * 2;
                Debug.LogFormat("PlayerScore: 0: {0}, 1: {1}", TotalScores[0], TotalScores[1]);
            }
                
        }

        if (TotalTileCount == OccupiedTileCount)
        {
            if(TotalScores[0] > TotalScores[1])
            {
                GameOverImages[0].SetActive(true);
            }
            else if(TotalScores[0] < TotalScores[1])
            {
                GameOverImages[1].SetActive(true);
            }
            else //Draw
            {
                GameOverImages[2].SetActive(true);
            }

            return;
        }
    }

    void CalculateScore()
    {
        var tileTable = GZ_CircleTable.Instance.GetTileArray();

        /*
        for(int rr = 0; rr < tileTable.GetLength(0); rr++)
        {
            string dd = "";
            for(int cc =0; cc < tileTable.GetLength(1); cc++)
            {
                dd += tileTable[rr, cc].AcquireIndex + ",";
            }

            Debug.Log(dd);
        }
        Debug.Log("-------------------");
        */
        _players[0].ComboScore = _players[1].ComboScore = 0;


        //check row
        for (int r = 0; r < tileTable.GetLength(0); r++)
        {
            bool result = true;
            int currentPlayerAcquireIndex = tileTable[r, 0].AcquireIndex;
            
            for(int cr = 0; cr < tileTable.GetLength(1); cr++)
            {
                //Debug.LogFormat("R: {0}, C:{1}, P: {1}", r, cr, tileTable[r, cr].AcquireIndex);
                if (tileTable[r, cr].AcquireIndex == -1 
                    || 
                    tileTable[r, cr].AcquireIndex != currentPlayerAcquireIndex)
                {
                    result = false;
                    break;
                }                    
            }

            if (result)
            {
                _players[currentPlayerAcquireIndex].ComboScore++;
                Debug.Log("row line:" + r + "by " + currentPlayerAcquireIndex);
            }
        }

        //check col
        for (int c = 0; c < tileTable.GetLength(1); c++)
        {
            bool result = true;
            int currentPlayerAcquireIndex = tileTable[0, c].AcquireIndex;

            for (int rc = 0; rc < tileTable.GetLength(0); rc++)
            {
                //Debug.LogFormat("R: {0}, C:{1}, P: {1}", r, cr, tileTable[r, cr].AcquireIndex);
                if (tileTable[rc, c].AcquireIndex == -1
                    || tileTable[rc, c].AcquireIndex != currentPlayerAcquireIndex)
                {
                    result = false;
                    break;
                }
            }

            if (result)
            {
                _players[currentPlayerAcquireIndex].ComboScore++;
                Debug.Log("col line:" + c + "by " + currentPlayerAcquireIndex);
            }
        }

    }

    void SwitchPlayer()
    {
        _IsIgnoreInput = true;

        GZ_AudioModule.Instance.Play(2);
        _currentPlayerIndex = ++_currentPlayerIndex % NumPlayer;

        SwitchPlayerImage[_currentPlayerIndex].SetActive(true);

        GZ_RadioEmitter.Instance.Bang();

        StartCoroutine(
            GZ_Math.PlayAnim_CR(
            0.5f,
            (percentage) => // scale up
            {
                var value =  MainUIEnableCurve.Evaluate(percentage);
                SwitchPlayerImage[_currentPlayerIndex].transform.localScale = Vector3.one * value;
                
            },
            () =>  /* done callback */
            {
                //Switch Player
                StartCoroutine(GZ_Math.WaitFor_CR(1f,
                () =>
                {
                    StartCoroutine(GZ_Math.PlayAnim_CR(0.5f, //Scale down
                        (percent) => 
                        {
                            var value = MainUIEnableCurve.Evaluate(1f - percent);
                            SwitchPlayerImage[_currentPlayerIndex].transform.localScale = Vector3.one * value;
                        }, 
                        () => 
                        {
                            _IsIgnoreInput = false;
                            SwitchPlayerImage[_currentPlayerIndex].SetActive(false);
                        }));
                }));
            }));
    }
}