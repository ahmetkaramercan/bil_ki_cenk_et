using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongAnswer : MonoBehaviour
{
   void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player"){
            print("WRONGGGGG"); // bunu da ayni sekilde sadece atilan oklarla falan olacak sekilde ayarla
        }
        print("WRONG");
        //Debug.Log("Bu script " + gameObject.name + " objesine bağlıdır.");

    }
}
