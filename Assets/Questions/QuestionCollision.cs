using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player"){
            print("SAFAAAAAA"); 
        }
        print("SAFA");
    }
}
