using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject PoisonEffect;
    public Transform spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyYourself", 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(PoisonEffect, spawnPoint.position, Quaternion.identity);
            Debug.Log("Particle Collision Enter!!");
        }
        //Debug.Log("Collision Enter!!");
        DestroyYourself();
    }

    void DestroyYourself()
    {
        Destroy(gameObject);
    }
}
