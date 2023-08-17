using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    // Start is called before the first frame update
    void Start()
    {
       // _healthBar = GetComponent<Slider>();

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateHealthBar(float curValue, float maxValue)
    {
        _healthBar.value = curValue / maxValue;
    }

    public float getHealth()
    {
        return _healthBar.value;
    }

}
