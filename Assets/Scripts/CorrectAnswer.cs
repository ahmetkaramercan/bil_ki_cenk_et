using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorrectAnswer : MonoBehaviour
{
    public Text text;
    [SerializeField]
    private GameObject[] nextText = new GameObject[5];
    void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "Player"){
            print("SAFAAAAAAgg"); 
        }
        print("SAFA");

        if ( gameObject.name.Equals("Ice C"))
        {
            print("SAFA ICE C");
           /* GameObject targetObject = GameObject.Find("Ice Question (1)"); // Değişken ismini hedef objenin adıyla değiştirin
            if (targetObject != null)
            {
                print("SAFA ICE CCCC");
                text=targetObject.GetComponent<Text>();
                text.color= new Color(0.0f, 0.372549f, 1.0f, 1.0f); // "SetVisibility" metodu hedef objede çağrılacaktır
                
            }*/

            nextText[0].SetActive(true);
            nextText[1].SetActive(true);
            nextText[2].SetActive(true);
            nextText[3].SetActive(true);
            nextText[4].SetActive(true);
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

    }
}
