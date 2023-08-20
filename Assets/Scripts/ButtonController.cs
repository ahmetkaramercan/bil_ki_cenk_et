using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public string buttonName; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow")) // Ok nesnesinin bir tagi olmalý (örneðin "Arrow")
        {
            Debug.Log("Button " + buttonName + " was shot!");
            //GameManager.Instance.Player.GetComponent<>().çrkj<
        }
    }
}
