using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public string buttonName; // Bu butonun ad� (�rne�in "ButtonA", "ButtonB" gibi)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow")) // Ok nesnesinin bir tagi olmal� (�rne�in "Arrow")
        {
            // Ok butona �arpt�, bu butonun ad�n� kullanarak i�lem yapabilirsiniz
            Debug.Log("Button " + buttonName + " was shot!");
            // Di�er gerekli i�lemleri burada ger�ekle�tirin
            //GameManager.Instance.Player.GetComponent<>().�rkj<
        }
    }
}
