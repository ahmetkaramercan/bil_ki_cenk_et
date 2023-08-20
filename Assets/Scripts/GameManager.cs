using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField]
    private GameObject[] player = new GameObject[4];
    private int _activePlayer;

    private float _curTime;
    private bool _gameOver;

    [SerializeField]
    private TextMeshProUGUI _timeText;

    public GameObject Archery
    {
        get { return player[0]; }
        set { player[0] = value; }
    }
    public GameObject Wizard
    {
        get { return player[1]; }
        set { player[1] = value; }
    }
    public GameObject Iceman
    {
        get { return player[2]; }
        set { player[2] = value; }
    }
    public GameObject Fireman
    {
        get { return player[3]; }
        set { player[3] = value; }
    }

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    instance = singletonObject.AddComponent<GameManager>();
                }
            }

            return instance;
        }
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        StartCoroutine(StartTime());
        _gameOver = false;
        _curTime = 0f;

        player[0].SetActive(true);
        _activePlayer = 0;
    }

    IEnumerator StartTime()
    {
        while (!_gameOver)
        {
            _curTime += 0.1f;
            _timeText.SetText("Time:" + _curTime.ToString("N2") + "second");
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        setThePlayer();
        
    }


    private void setThePlayer()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            setActivePlayer(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            setActivePlayer(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            setActivePlayer(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            setActivePlayer(3);
        }
    }

    private void setActivePlayer(int activePlayer)
    {
        Quaternion oldRot = player[_activePlayer].transform.rotation;
        Vector3 oldPos = player[_activePlayer].transform.position;
        switch (activePlayer)
        {
            case 0:
                player[0].SetActive(true);
                player[_activePlayer].SetActive(false);
                player[0].transform.rotation = oldRot;
                player[0].transform.position = oldPos;
                _activePlayer = 0;
                break;
            case 1:
                player[1].SetActive(true);
                player[_activePlayer].SetActive(false);
                player[1].transform.rotation = oldRot;
                player[1].transform.position = oldPos;
                _activePlayer = 1;
                break;
            case 2:
                player[2].SetActive(true);
                player[_activePlayer].SetActive(false);
                player[2].transform.rotation = oldRot;
                player[2].transform.position = oldPos;
                _activePlayer = 2;
                break;
            case 3:
                player[3].SetActive(true);
                player[_activePlayer].SetActive(false);
                player[3].transform.rotation = oldRot;
                player[3].transform.position = oldPos;
                _activePlayer = 3;
                break;
            case 4:
                player[0].SetActive(false);
                player[1].SetActive(false);
                player[2].SetActive(false);
                player[3].SetActive(false);
                //_activePlayer = -1;
                break;
        }
    }

}
