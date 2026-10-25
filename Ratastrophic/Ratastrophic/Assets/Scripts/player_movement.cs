using UnityEngine;
using UnityEngine.InputSystem;

public class player_movemet : MonoBehaviour
{
    public InputAction moveAction;

    [Header("Settings")]
    public float moveSpeed = 5.0f;
    public float maxSpeed = 600f;
    public float rotationSpeed = 15.0f;
    public bool hasPotion = false;

    private Rigidbody _rigidbody;
    private Vector3 _moveDirection;

    [Header("Audio Settings")]
    public AudioSource walkingAudioSource;
    public AudioClip walk_soundClip;
    public GameObject audio_area;

    [Header("Animation")]
    public Animator animatorRat;

    private void OnEnable()
    {
        if(animatorRat != null)
            animatorRat.SetBool("isDead", false);

        moveAction.Enable();
    }

    private void OnDisable()
    {
        if(animatorRat != null)
            animatorRat.SetBool("isDead", true);
        moveAction.Disable();
    }

    private void Start()
    {
        if (walkingAudioSource == null) {
            walkingAudioSource = gameObject.AddComponent<AudioSource>();
        }

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }

    private void Update()
    {
        if(walkingAudioSource.isPlaying){
            audio_area.SetActive(true);
        }
        else
        {
            audio_area.SetActive(false);
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        _moveDirection = new Vector3(input.x, 0f, input.y).normalized;

        if(input.x != 0 || input.y != 0)
        {
            if(animatorRat != null)
                animatorRat.SetBool("isWalking", true);
        }
        else 
            if(animatorRat != null)
                animatorRat.SetBool("isWalking", false);

        if (input.sqrMagnitude > 0.01f)
        {
            if (!walkingAudioSource.isPlaying)
            {
                walkingAudioSource.clip = walk_soundClip;
                walkingAudioSource.loop = true;
                walkingAudioSource.Play();
            }
        }
        else
        {
            if (walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        Vector3 targetVelocity = _moveDirection * moveSpeed;
        _rigidbody.linearVelocity = new Vector3(targetVelocity.x, 0, targetVelocity.z);

        /*Vector3 currentHorizontalVel = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
        
        if (_moveDirection.sqrMagnitude > 0.01f) {
            _rigidbody.AddForce(_moveDirection * moveSpeed, ForceMode.Force);
            if (currentHorizontalVel.magnitude > maxSpeed)
            {
                Vector3 clampedVel = currentHorizontalVel.normalized * maxSpeed;
                _rigidbody.linearVelocity = new Vector3(clampedVel.x, _rigidbody.linearVelocity.y, clampedVel.z);
            }
        }
        else
        {
            Vector3 dampedVel = Vector3.Lerp(currentHorizontalVel, Vector3.zero, 15 * Time.fixedDeltaTime);
            _rigidbody.linearVelocity = new Vector3(dampedVel.x, _rigidbody.linearVelocity.y, dampedVel.z);
        }*/
        
    }

    private void RotatePlayer()
    {
        if (_moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
            _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}