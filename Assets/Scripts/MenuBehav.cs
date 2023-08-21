using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehav : MonoBehaviour
{
    // Start is called before the first frame update
    private int _playerType;
    [SerializeField]
    private SceneInfo sceneVar;
    public void ClickCharacter1()
    {
        _playerType = 0;
        StartGame();
    }
    public void ClickCharacter2()
    {
        _playerType = 1;
        StartGame();
    }
    public void ClickCharacter3()
    {
        _playerType = 2;
        StartGame();
    }
    public void ClickCharacter4()
    {
        _playerType = 3;
        StartGame();
    }

    private void StartGame()
    {
        sceneVar._playerType = _playerType;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
