using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WrongAnswer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _wrongText;

    void Start(){
        _wrongText.SetText("");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Arrow"){
            _wrongText.SetText("Wrong !!!");
            Invoke("Start",2);
            print("WRONG"); // bunu da ayni sekilde sadece atilan oklarla falan olacak sekilde ayarla
        }
        print("WRONG" + other.gameObject.tag);
        //Debug.Log("Bu script " + gameObject.name + " objesine bağlıdır.");

    }
}
