using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisionCapsule : MonoBehaviour
{
    public GameObject PoisonEffect;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyYourself", 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(PoisonEffect, this.transform.position, Quaternion.identity);
            //Debug.Log("Particle Collision Enter!!");
        }
        //Debug.Log("Collision Enter!!");
        DestroyYourself();
    }

    void DestroyYourself()
    {
        Destroy(gameObject);
    }
}
