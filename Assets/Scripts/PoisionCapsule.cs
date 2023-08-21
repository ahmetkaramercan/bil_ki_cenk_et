using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisionCapsule : MonoBehaviour
{
    public GameObject PoisonEffect;
    private GameObject _particle;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyYourself", 8);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _particle = Instantiate(PoisonEffect, this.transform.position, Quaternion.identity);
            Invoke("DestroyParticle", 3);
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
