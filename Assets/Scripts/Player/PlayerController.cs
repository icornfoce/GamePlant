using UnityEngine;

namespace DimensionGame.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f;
        public float turnSmoothTime = 0.1f;
        private float _turnSmoothVelocity;

        public Animator animator;
        public Transform cam;

        [Header("Audio Settings")]
        public AudioSource audioSource;
        public AudioClip[] footstepSounds;     // เสียงกลุ่มที่ 1 (เดินช่วงแรก)
        public AudioClip[] continuousSounds;   // เสียงกลุ่มที่ 2 (เมื่อเดินค้างเกิน 1 วิ)
        public AudioClip jumpSound; 
        public float footstepStep = 0.5f;      // จังหวะการก้าวเท้า
        public float soundSwitchDelay = 1.0f;  // เวลาที่ต้องกดค้างก่อนเปลี่ยนเสียง
        
        private float _moveInputTimer = 0f;    // ตัวนับเวลาการกดปุ่มค้าง
        private float _footstepTimer;
        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;

        private AudioSource _skillAudioSource; // เพิ่มตัวแปรใหม่

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            
            // หา AudioSource ตัวแรกสำหรับเสียงเท้า (อันเดิม)
            if (audioSource == null) audioSource = GetComponent<AudioSource>();

            // สร้างหรือหา AudioSource ตัวที่สองสำหรับสกิล
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1) {
                _skillAudioSource = sources[1];
            } else {
                _skillAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            _isGrounded = _controller.isGrounded;
            if (_isGrounded && _velocity.y < 0) _velocity.y = -2f;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            // 1. ระบบกระโดด (Physics Only)
            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (audioSource && jumpSound) audioSource.PlayOneShot(jumpSound);
            }

            // 2. การเคลื่อนที่และตรรกะเสียง
            if (direction.magnitude >= 0.1f)
            {
                // นับเวลาการกดค้าง
                _moveInputTimer += Time.deltaTime;

                // หมุนและเคลื่อนที่
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _controller.Move(moveDir.normalized * (moveSpeed * Time.deltaTime));

                if (animator) animator.SetBool("ForwardWalk", true);

                // จัดการเสียงเท้า
                if (_isGrounded)
                {
                    _footstepTimer -= Time.deltaTime;
                    if (_footstepTimer <= 0)
                    {
                        // ถ้ากดค้างเกินเวลาที่กำหนด จะส่งค่า true เพื่อเปลี่ยนกลุ่มเสียง
                        PlayFootstep(_moveInputTimer >= soundSwitchDelay);
                        _footstepTimer = footstepStep;
                    }
                }
            }
            else
            {
                // หยุดเดิน: รีเซ็ตเวลาและหยุดเสียงทันที
                _moveInputTimer = 0f;
                if (animator) animator.SetBool("ForwardWalk", false);
                _footstepTimer = 0f;
                if (audioSource.isPlaying) audioSource.Stop();
            }

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void PlayFootstep(bool useSecondSound)
        {
            // เลือกใช้ Array ตามเงื่อนไขเวลา
            AudioClip[] targetArray = useSecondSound ? continuousSounds : footstepSounds;

            if (targetArray == null || targetArray.Length == 0) return;

            int n = Random.Range(0, targetArray.Length);
            audioSource.clip = targetArray[n];
            audioSource.Play();
        }
    }
}