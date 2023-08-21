using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Experimental.GraphView.GraphView;
using StarterAssets;

public class CorrectAnswer : MonoBehaviour
{
    public Text text;
    [SerializeField]
    private GameObject[] nextEnemy = new GameObject[1];
    private GameObject _player;

    [SerializeField]
    private TextMeshProUGUI _correctText;
    
    void Start(){
        _correctText.SetText("");
    }
    private void Update()
    {
        _player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "Arrow"){
            _player.GetComponent<PlayerController>().increaseBulletAmount(5);
            _correctText.SetText("Correct !!!");
            Invoke("Start",2);
            nextEnemy[0].SetActive(true);
            if ( gameObject.name.Equals("Ice C"))
            {            
                GameObject targetObject = GameObject.Find("Ice Question");
                targetObject.SetActive(false);
               
            }
            else if ( gameObject.name.Equals("Explosion D"))
            {      
                GameObject targetObject = GameObject.Find("Explosion Question");
                targetObject.SetActive(false);
                
            }
            else if ( gameObject.name.Equals("Poison B"))
            {
                GameObject targetObject = GameObject.Find("Poison Question");
                targetObject.SetActive(false);
            }
            else if ( gameObject.name.Equals("Fire A"))
            {     
                GameObject targetObject = GameObject.Find("Fire Question");
                targetObject.SetActive(false);
            }
        }
        

    }
    
}
