using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorrectAnswer : MonoBehaviour
{
    public Text text;
    [SerializeField]
    private GameObject[] nextEnemy = new GameObject[1];
    void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "Player"){
            print("SAFAAAAAAgg");  // buraya adamın degmesi değil de okların degmesi olacak sekilde ayarla boylece sadece atılan seylerle isaretlenebilsin
        }
        nextEnemy[0].SetActive(true);
        if ( gameObject.name.Equals("Ice C"))
        {            
            GameObject targetObject0 = GameObject.Find("Ice Question");
            GameObject targetObject1 = GameObject.Find("Ice A");
            GameObject targetObject2 = GameObject.Find("Ice B");
            GameObject targetObject3 = GameObject.Find("Ice C");
            GameObject targetObject4 = GameObject.Find("Ice D");
            targetObject0.SetActive(false);
            targetObject1.SetActive(false);
            targetObject2.SetActive(false);
            targetObject3.SetActive(false);
            targetObject4.SetActive(false);
        }
        else if ( gameObject.name.Equals("Explosion D"))
        {      
            GameObject targetObject0 = GameObject.Find("Explosion Question");
            GameObject targetObject1 = GameObject.Find("Explosion A");
            GameObject targetObject2 = GameObject.Find("Explosion B");
            GameObject targetObject3 = GameObject.Find("Explosion C");
            GameObject targetObject4 = GameObject.Find("Explosion D");
            targetObject0.SetActive(false);
            targetObject1.SetActive(false);
            targetObject2.SetActive(false);
            targetObject3.SetActive(false);
            targetObject4.SetActive(false);
        }
        else if ( gameObject.name.Equals("Poison B"))
        {
            GameObject targetObject0 = GameObject.Find("Poison Question");
            GameObject targetObject1 = GameObject.Find("Poison A");
            GameObject targetObject2 = GameObject.Find("Poison B");
            GameObject targetObject3 = GameObject.Find("Poison C");
            GameObject targetObject4 = GameObject.Find("Poison D");
            targetObject0.SetActive(false);
            targetObject1.SetActive(false);
            targetObject2.SetActive(false);
            targetObject3.SetActive(false);
            targetObject4.SetActive(false);
        }
        else if ( gameObject.name.Equals("Fire A"))
        {     
            GameObject targetObject0 = GameObject.Find("Fire Question");
            GameObject targetObject1 = GameObject.Find("Fire A");
            GameObject targetObject2 = GameObject.Find("Fire B");
            GameObject targetObject3 = GameObject.Find("Fire C");
            GameObject targetObject4 = GameObject.Find("Fire D");
            targetObject0.SetActive(false);
            targetObject1.SetActive(false);
            targetObject2.SetActive(false);
            targetObject3.SetActive(false);
            targetObject4.SetActive(false);
        }

    }
}
