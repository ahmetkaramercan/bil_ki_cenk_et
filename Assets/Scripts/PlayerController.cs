using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        [Header("Throwing")]
        //throwing instances
        public Transform cameraForThrowing;
        public Transform attackPointForThrow;
        public GameObject objectToThrow;
        private ParticleSystem _fireParticle;

        public int totalThrow;
        public float throwCooldown;

        public float throwForce;
        public float throwUpwardForce;

        private bool readyToThrow;
        private bool _aiming;// it is just useful for archery

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        FloatingHealthBar _healthBar;
        [SerializeField]private float _maxHealth = 100;
        [SerializeField] private float _fireDamage = 0.04f;
        [SerializeField] private float _iceDamage = 0.04f;
        [SerializeField] private float _poisionDamage = 0.04f;
        [SerializeField] private float _bombDamage = 0.04f;

        private String _playerName;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            _healthBar = GetComponentInChildren<FloatingHealthBar>();
            _playerName = this.gameObject.name;
            _aiming = false;
            if (_playerName.Equals("Archery"))
            {
                throwCooldown = 2f;
                _fireParticle = null;
                _fireDamage = 0.1f;
                _iceDamage = 0.2f;
                _poisionDamage = 0.4f;
                _bombDamage = 0.05f;

            }
            else if (_playerName.Equals("Fireman"))
            {
                _fireParticle = GetComponentInChildren<ParticleSystem>();
                _fireParticle.Stop();
                throwCooldown = 3f;
                _fireDamage = 0.05f;
                _iceDamage = 0.4f;
                _poisionDamage = 0.02f;
                _bombDamage = 0.1f;

            }
            else if (_playerName.Equals("Iceman"))
            {
                _fireParticle = GetComponentInChildren<ParticleSystem>(); 
                _fireParticle.Stop();
                throwCooldown = 2f;
                _fireDamage = 0.4f;
                _iceDamage = 0.05f;
                _poisionDamage = 0.1f;
                _bombDamage = 0.2f;

            }
            else if (_playerName.Equals("Wizard"))
            {
                throwCooldown = 0.5f;
                _fireParticle = null;
                _fireDamage = 0.2f;
                _iceDamage = 0.1f;
                _poisionDamage = 0.05f;
                _bombDamage = 0.4f;
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            AssignAnimationIDs();
            //_healthBar.UpdateHealthBar(0.5f,1f);
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            readyToThrow = true;
        }

        private void Update()
        {
            try
            {

            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            if (_playerName.Equals("Archery"))
            {
                AimForShoot();
            }
            else
            {
                _aiming = true;
            }
            Shoot();
            isDie();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public float getHealth()
        {
            return _healthBar.getHealth() * _maxHealth;
        }

        public void setHealth(float health)
        {
            _healthBar.UpdateHealthBar(health, _maxHealth);
        }
        private void isDie()
        {
            if (_healthBar.getHealth() == 0f)
            {
                //SceneManager.LoadScene("EndScene");
            }
        }

        private void AimForShoot()
        {
            if (_input.isAiming && Grounded && !_input.isSprint)
            {
                _animator.SetBool("Aiming", _input.isAiming);
                _aiming = true;
            }
            else
            {
                _animator.SetBool("Aiming", false);
                _aiming = false; ;
            }
        }

        private void Shoot()
        {
            if (_input.isShooting && _aiming && Grounded && !_input.isSprint && readyToThrow)
            {
                _animator.SetBool("Aiming", _input.isShooting);
                readyToThrow = false;
                Invoke(nameof(resetThrow), throwCooldown);
                //Debug.Log("attack point:" + attackPointForThrow.transform.position);
                //Debug.Log("camera rotation:" + cameraForThrowing.transform.rotation);
                if (totalThrow != 0)
                {

                    if (_playerName.Equals("Archery") || _playerName.Equals("Wizard"))
                    {
                        GameObject projectile = Instantiate(objectToThrow, attackPointForThrow.transform.position, Quaternion.LookRotation(cameraForThrowing.transform.forward) * Quaternion.Euler(90, 0, 0));

                        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
                        Vector3 forceToAdd = cameraForThrowing.transform.forward * throwForce + transform.up * throwUpwardForce;

                        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

                        //Debug.Log("shot!!");
                    }
                    else if(_playerName.Equals("Fireman") || _playerName.Equals("Iceman"))
                    {
                        attackPointForThrow.transform.rotation = Quaternion.LookRotation(cameraForThrowing.transform.forward);
                        _fireParticle.Play();
                    }
                    totalThrow--;
                }

            }
            else if(!_playerName.Equals("Archery"))
            {
               _animator.SetBool("Aiming", false);
            }
        }

        /*private IEnumerator Destroyer(GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(obj);
        }*/

        private void resetThrow()
        {
            readyToThrow = true;
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.isSprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.isJump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.isJump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
        void OnParticleCollision(GameObject obj)
        {
            String particalName = obj.name;

            if (particalName.Equals("EnemyFireParticles"))
            {
                _healthBar.UpdateHealthBar(_healthBar.getHealth()*_maxHealth - _fireDamage, _maxHealth);
            }
            else if (particalName.Equals("EnemyIceParticles"))
            {
                _healthBar.UpdateHealthBar(_healthBar.getHealth() * _maxHealth - _iceDamage, _maxHealth);
            }
            else if (particalName.Equals("EnemyPoisionParticles"))
            {
                _healthBar.UpdateHealthBar(_healthBar.getHealth() * _maxHealth - _poisionDamage, _maxHealth);
            }
            else if (particalName.Equals("EnemyBloodParticles"))
            {
                _healthBar.UpdateHealthBar(_healthBar.getHealth() * _maxHealth - _bombDamage, _maxHealth);
            }
        }
    }

}