using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Space
{
    Air,
    Water,
}

public enum OnWaterState
{
    Rest,
    Move,
}

public class GameManager : MonoBehaviour
{
    [Header("�÷��̾�")]
    [Tooltip("�̵� �ӵ� m/s")]
    public float MoveSpeed = 4.0f;
    [Tooltip("�޸��� �ӵ� m/s")]
    public float SprintSpeed = 6.0f;
    [Tooltip("ȸ�� �ӵ�")]
    public float RotationSpeed = 1.0f;
    [Tooltip("����")]
    public float SpeedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("���� ����")]
    public float JumpHeight = 1.2f;
    [Tooltip("�߷� ����. ���� �⺻ ������ -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("���� ��Ÿ��. ��� �����ϰ� �Ϸ��� 0���� ����")]
    public float JumpTimeout = 0.1f;
    [Tooltip("�߶� ���·� �ٲ�� ���� �ð�. ��ܿ��� ������ �� ����")]
    public float FallTimeout = 0.15f;

    [Header("�÷��̾� �� üũ")]
    [Tooltip("�÷��̾ ���� �ִ���. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Header("�÷��̾� �� üũ")]
    public bool inWater = false;
    [Tooltip("��ģ �ٴڿ� ����")]
    public float GroundedOffset = -0.14f;
    [Tooltip("�ٴ��� ���� üũ. CharacterController�� ������ �¾ƾ� ��")]
    public float GroundedRadius = 0.5f;
    [Tooltip("ĳ���Ͱ� �ٴ����� �� ���̾�")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("Cinemachine Virtual Camera�� ���� ���")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("ī�޶� �ִ� ���� ����")]
    public float TopClamp = 90.0f;
    [Tooltip("ī�޶� �ִ� ���� ����")]
    public float BottomClamp = -90.0f;

    // cinemachine
    private float _cinemachineTargetPitch;

    // �÷��̾�
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private CharacterController _controller;
    private StarterAssetsInputs _input;
    private GameObject _mainCamera;

    private const float _threshold = 0.01f;

    Collider playerCollider;

    private void Awake()
    {
        // mainCamera �ҷ�����
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        Transform child = transform.GetChild(1);
        playerCollider = child.GetComponent<Collider>();
    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();

        // timeouts �ʱ�ȭ
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        if(!inWater)
        {
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }
    }

    private void CameraRotation()
    {
        // input�� �ִٸ�
        if (_input.look.sqrMagnitude >= _threshold)
        {
            //Time.deltaTime�� ���� ���콺 ���� ����
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

            // pitch rotation ����
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            // move
            if(!inWater)inputDirection = (transform.right * _input.move.x + transform.forward * _input.move.y);
            else inputDirection = (transform.right * _input.move.x + transform.forward * _input.move.y);
        }

        // move the player
        _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void JumpAndGravity()
    {
        if(!inWater)
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
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

                // if we are not grounded, do not jump
                _input.jump = false;
            }
            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        else
        {
            if (_input.jump)
            {
                _verticalVelocity = MoveSpeed;
            }
            else _verticalVelocity = 0.0f;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = true;
            // JumpHeight = 0.4f;
            Gravity = -0.0f;
            Debug.Log("Water");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Water"))
        {
            inWater = false;
            JumpHeight = 1.2f;
            Gravity = -15.0f;
            Debug.Log("Water Out");
        }
    }
}
