using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public string buttonName; // Bu butonun adý (örneðin "ButtonA", "ButtonB" gibi)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow")) // Ok nesnesinin bir tagi olmalý (örneðin "Arrow")
        {
            // Ok butona çarptý, bu butonun adýný kullanarak iþlem yapabilirsiniz
            Debug.Log("Button " + buttonName + " was shot!");
            // Diðer gerekli iþlemleri burada gerçekleþtirin
            //GameManager.Instance.Player.GetComponent<>().çrkj<
        }
    }
}
