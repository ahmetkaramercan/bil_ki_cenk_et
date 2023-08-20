using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Arrow : MonoBehaviour
{
    public GameObject PoisonEffect;
    public Transform spawnPoint;
    private GameObject _particle;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyYourself", 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _particle = Instantiate(PoisonEffect, spawnPoint.position, Quaternion.identity);
            Invoke("DestroyParticle", 5);
            //Debug.Log("Particle Collision Enter!!");
        }
        //Debug.Log("Collision Enter!!");
        DestroyYourself();
    }

    void DestroyYourself()
    {
        Destroy(gameObject);
    }

    void DestroyParticle()
    {
        Destroy(_particle);
    }
}
