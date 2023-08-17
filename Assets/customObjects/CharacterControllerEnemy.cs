using System;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    //[RequireComponent(typeof(CharacterController))]
    public class CharacterControllerEnemy : MonoBehaviour
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

        public int totalThrow;
        public float throwCooldown;

        public float throwForce;
        public float throwUpwardForce;

        private bool readyToThrow;

        private Animator _animator;
        //private CharacterController _controller;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        public Transform player;
        private Rigidbody _rb;

        private bool IsCurrentDeviceMouse
        {
            get
            {
				return false;
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            //_cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            //_controller = GetComponent<CharacterController>();
            _rb = GetComponent<Rigidbody>();
            
            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            readyToThrow = true;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            //AimForShoot();
            //Shoot();
        }

        private void AimForShoot()
        {
            /*if (_input.isAiming && Grounded && !_input.isSprint)
            {
                _animator.SetBool("Aiming", _input.isAiming);
            }
            else
            {
                _animator.SetBool("Aiming", false);
            }*/
        }

        private void Shoot()
        {
            /*if (_input.isShooting && Grounded && !_input.isSprint && readyToThrow)
            {
                //_animator.SetBool("Aiming", _input.isAiming);
                readyToThrow = false;
                Debug.Log("attack point:" + attackPointForThrow.transform.position);
                Debug.Log("camera rotation:" + cameraForThrowing.transform.rotation);
                GameObject projectile = Instantiate(objectToThrow, attackPointForThrow.transform.position, cameraForThrowing.transform.rotation);
                Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

                Vector3 forceToAdd = cameraForThrowing.transform.forward * throwForce + transform.up * throwUpwardForce;

                projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

                totalThrow--;

                //Debug.Log("shot!!");
                Invoke(nameof(resetThrow), throwCooldown);
            }
            else
            {
                // _animator.SetBool("Aiming", false);
            }*/
        }

        private void resetThrow()
        {
            readyToThrow = true;
        }

        private void LateUpdate()
        {
            //CameraRotation();
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

            Debug.Log("Grounded value:\t" + Grounded);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }


        private void Move()
        {
            if (player == null)
            {
                return;
            }

            // Calculate input direction
            Vector3 inputDirection = player.position - _rb.position;
            inputDirection.y = 0f; // Ignore vertical component
            float inputMagnitude = inputDirection.magnitude;

            // Normalize input direction only if it's not too small
            if (inputMagnitude > 0.01f)
            {
                inputDirection.Normalize();

                // Calculate target rotation
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

                // Smoothly rotate to face the target direction
                float rotation = Mathf.SmoothDampAngle(_rb.rotation.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                _rb.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            // Set target speed based on move speed, sprint speed, and input magnitude
            float targetSpeed = Mathf.Lerp(0f, MoveSpeed, inputMagnitude);

            // Calculate the desired velocity based on the target speed and direction
            Vector3 desiredVelocity = inputDirection * targetSpeed;

            // Apply horizontal movement to the rigidbody only if grounded
            if (Grounded)
            {
                desiredVelocity.y = _rb.velocity.y; // Maintain the vertical velocity
                _rb.velocity = desiredVelocity;
            }

            // Update animator if available
            if (_animator != null)
            {
                _animator.SetFloat("Speed", desiredVelocity.magnitude);
                _animator.SetFloat("MotionSpeed", 1f);
            }

            // Apply gravity manually if not grounded
            if (!Grounded)
            {
                _rb.AddForce(Vector3.up * Gravity, ForceMode.Acceleration);
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
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(transform.position), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(transform.position), FootstepAudioVolume);
            }
        }
    }
}