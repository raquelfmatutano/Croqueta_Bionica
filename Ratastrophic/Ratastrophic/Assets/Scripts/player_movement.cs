using UnityEngine;
using UnityEngine.InputSystem;

public class player_movemet : MonoBehaviour
{
    public InputAction moveAction;

    [Header("Settings")]
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 15.0f;

    private Rigidbody _rigidbody;
    private Vector3 _moveDirection;

    [Header("Audio Settings")]
    public AudioSource walkingAudioSource;
    public AudioClip walk_soundClip;
    public GameObject audio_area;

    private void OnEnable() => moveAction.Enable();
    private void OnDisable() => moveAction.Disable();

    private void Start()
    {
        if (walkingAudioSource == null) {
            walkingAudioSource = gameObject.AddComponent<AudioSource>();
        }

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
        _rigidbody.linearVelocity = new Vector3(targetVelocity.x, _rigidbody.linearVelocity.y, targetVelocity.z);
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