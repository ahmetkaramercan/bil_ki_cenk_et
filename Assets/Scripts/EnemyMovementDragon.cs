using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovementDragon : MonoBehaviour
{
    private Transform _player;
    private FloatingHealthBar _healthBar;
    [SerializeField]
    private float MAX_DISTANCE_FOR_AWAKE = 10f;
    [SerializeField]
    private float MAX_DISTANCE_FOR_FIRE = 5f;
    private bool _closeEnoughToFire;

    private ParticleSystem _fireParticle;
    private Rigidbody _rb;
    private Animator _animator;
    private bool _hasAnimator;
    private int _animIDAwake;
    private int _animIDCloseEnough;
    private int _animIDDie;
    [SerializeField]
    private float RotationSmoothTime = 0.12f;
    private float _rotationVelocity;
    [SerializeField]
    private float MoveSpeed = -15f;

    [SerializeField] private float _maxHealth = 100f;
    private float _fireDamage;
    private float _iceDamage;
    private float _poisionDamage;
    private float _bombDamage;
    // Start is called before the first frame update

    [SerializeField]
    private GameObject[] nextText = new GameObject[1];

    private void OnEnable()
    {
        _player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        _hasAnimator = TryGetComponent(out _animator);
        _healthBar = GetComponentInChildren<FloatingHealthBar>();
        _rb = GetComponent<Rigidbody>();
        _closeEnoughToFire = false;


        String _playerName = this.gameObject.name;
        if (_playerName.Equals("BloodThirstyEnemy"))
        {
            _fireParticle = GetComponentInChildren<ParticleSystem>();
            _fireDamage = 0.1f;
            _iceDamage = 0.2f;
            _poisionDamage = 0.4f;
            _bombDamage = 0.05f;

        }
        else if (_playerName.Equals("EnemyDragon"))
        {
            _fireParticle = GetComponentInChildren<ParticleSystem>();
            _fireParticle.Stop();
            _fireDamage = 0.05f;
            _iceDamage = 0.4f;
            _poisionDamage = 0.2f;
            _bombDamage = 0.1f;

        }
        else if (_playerName.Equals("IceEnemy"))
        {
            _fireParticle = GetComponentInChildren<ParticleSystem>();
            _fireParticle.Stop();
            _fireDamage = 0.4f;
            _iceDamage = 0.05f;
            _poisionDamage = 0.1f;
            _bombDamage = 0.2f;

        }
        else if (_playerName.Equals("PoisionEnemy"))
        {
            _fireParticle = GetComponentInChildren<ParticleSystem>();
            _fireParticle.Stop();
            _fireDamage = 0.2f;
            _iceDamage = 0.1f;
            _poisionDamage = 0.05f;
            _bombDamage = 0.4f;
        }
    }
    void Start()
    {
        _animIDAwake = Animator.StringToHash("Awake");
        _animIDCloseEnough = Animator.StringToHash("CloseEnough");
        _animIDDie = Animator.StringToHash("Die"); 

    }

    // Update is called once per frame
    void Update()
    {
        _player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        if (!isDie() && _player != null)
        {
            SetAnimationAndParticles();
            if (_animator.GetBool(_animIDAwake) && !_animator.GetBool(_animIDCloseEnough))
            {
                //Debug.Log("Try to Move!!");
                MoveAndRotate();
            }
            else if(_animator.GetBool(_animIDCloseEnough))
            {
                Rotate();
            }
        }
        else if(isDie()){
            nextText[0].SetActive(true);
        }
        
        
    }

    private float distanceToPlayer()
    {
        return (this.transform.position - _player.transform.position).magnitude;
    }

    private void SetAnimationAndParticles()
    {
        float playerDistance = distanceToPlayer();
        bool isAwake = playerDistance < MAX_DISTANCE_FOR_AWAKE;
        bool closeEnoughToFire = playerDistance < MAX_DISTANCE_FOR_FIRE;
        if (closeEnoughToFire)
        {
            Debug.Log("closeEnough" + playerDistance);
            _fireParticle.Play();
        }
        else
        {
            Debug.Log("notcloseEnough" + playerDistance);
            _fireParticle.Stop();

        }

        if (_hasAnimator)
        {
            _animator.SetBool(_animIDAwake, isAwake);
            _animator.SetBool(_animIDCloseEnough, closeEnoughToFire);
            _animator.SetBool(_animIDDie, _healthBar.getHealth() <= 0f);

        }
    }

    private void MoveAndRotate()
    {
        // Calculate input direction
        Vector3 inputDirection = _player.position - _rb.position;
        inputDirection.y = 0f; // Ignore vertical component
        float inputMagnitude = inputDirection.magnitude;

        // Normalize input direction only if it's not too small
        if (inputMagnitude > 0.01f)
        {
            inputDirection.Normalize();

            // Calculate target rotation
            float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

            // Smoothly rotate to face the target direction
            float rotation = Mathf.SmoothDampAngle(_rb.rotation.eulerAngles.y, targetRotation, ref _rotationVelocity, RotationSmoothTime);
            _rb.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // Set target speed based on move speed, sprint speed, and input magnitude
        float targetSpeed = Mathf.Lerp(0f, MoveSpeed, inputMagnitude);

        // Calculate the desired velocity based on the target speed and direction
        Vector3 desiredVelocity = inputDirection * targetSpeed;

        // Apply horizontal movement to the rigidbody only if grounded
        desiredVelocity.y = _rb.velocity.y; // Maintain the vertical velocity
        _rb.velocity = desiredVelocity;
    }

    void Rotate()
    {
        Vector3 inputDirection = _player.position - _rb.position;
        inputDirection.y = 0f; // Ignore vertical component
        float inputMagnitude = inputDirection.magnitude;

        // Normalize input direction only if it's not too small
        if (inputMagnitude > 0.01f)
        {
            inputDirection.Normalize();

            // Calculate target rotation
            float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

            // Smoothly rotate to face the target direction
            float rotation = Mathf.SmoothDampAngle(_rb.rotation.eulerAngles.y, targetRotation, ref _rotationVelocity, RotationSmoothTime);
            _rb.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
     
    }

    void OnParticleCollision(GameObject obj)
    {
        String particalTag= obj.tag;
        if (particalTag.Equals("FireParticle"))
        {
            _healthBar.UpdateHealthBar(_healthBar.getHealth() * _maxHealth - _fireDamage, _maxHealth);
        }
        else if (particalTag.Equals("IceParticle"))
        {
            _healthBar.UpdateHealthBar(_healthBar.getHealth() * _maxHealth - _iceDamage, _maxHealth);
        }
        else if (particalTag.Equals("PoisionParticle"))
        {
            _healthBar.UpdateHealthBar(_healthBar.getHealth() * _maxHealth - _poisionDamage, _maxHealth);
        }
        else if (particalTag.Equals("BloodParticle"))
        {
            _healthBar.UpdateHealthBar(_healthBar.getHealth() * _maxHealth - _bombDamage, _maxHealth);
        }
    }

    private bool isDie()
    {
        if (_healthBar.getHealth() == 0f)
        {
            Invoke("DestroyYourself", 3);
            _animator.SetBool(_animIDDie, true);
            return true;
        }
        return false;
    }

    void DestroyYourself()
    {
        Destroy(gameObject);
    }
}
