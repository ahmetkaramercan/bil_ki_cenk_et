using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongAnswer : MonoBehaviour
{
   void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player"){
            print("WRONGGGGG"); 
        }
        print("WRONG");
        Debug.Log("Bu script " + gameObject.name + " objesine bağlıdır."); // Ice D 

    }
}
