using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson {
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour {
        [SerializeField]
        private bool m_NoClip;

        [SerializeField]
        private float m_InitialAcceleration;

        [SerializeField]
        private float m_FlySpeed;

        [SerializeField]
        private float m_WalkSpeed;

        [SerializeField]
        private float m_RunSpeed;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_RunstepLenghten;

        [SerializeField]
        private float m_JumpSpeed;

        [SerializeField]
        private float m_StickToGroundForce;

        [SerializeField]
        private float m_GravityMultiplier;

        [SerializeField]
        private MouseLook m_MouseLook;

        [SerializeField]
        private bool m_UseFovKick;

        [SerializeField]
        private FOVKick m_FovKick = new FOVKick();

        [SerializeField]
        private bool m_UseHeadBob;

        [SerializeField]
        private CurveControlledBob m_HeadBob = new CurveControlledBob();

        [SerializeField]
        private LerpControlledBob m_JumpBob = new LerpControlledBob();

        [SerializeField]
        private float m_StepInterval;

        [SerializeField]
        private AudioClip[] m_FootstepSounds;

        [SerializeField]
        private AudioClip m_JumpSound;

        [SerializeField]
        private AudioClip m_LandSound;

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private bool m_IsWalking;
        private AudioSource m_AudioSource;
        private float m_Acceleration;

        public bool NoClip {
            get { return m_NoClip; }
            set { m_NoClip = value; }
        }

        public Vector3 MoveDirection {
            get { return m_MoveDir; }
        }

        public Vector3 Velocity {
            get { return m_CharacterController.velocity; }
        }

        public float Acceleration {
            get { return m_Acceleration; }
        }

        private void Start() {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_Acceleration = m_InitialAcceleration;
            m_MouseLook.Init(transform, m_Camera.transform);
        }


        private void Update() {
            RotateView();

            // TODO: Make use of "CrossPlatformInputManager"
            if (Input.GetKeyDown(KeyCode.Tab)) {
                m_NoClip = !m_NoClip;
            }

            if (!m_Jump) {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded) {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
                m_Jump = false;
            }

            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded) {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        private void PlayLandingSound() {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        private void FixedUpdate() {
            if (m_NoClip) {
                float forwardMovement = Input.GetAxis("Vertical") * m_FlySpeed * Time.deltaTime;
                float horizontalMovement = Input.GetAxis("Horizontal") * m_FlySpeed * Time.deltaTime;
                Vector3 movementDelta = new Vector3(horizontalMovement, 0, 0);
                transform.position += m_Camera.transform.forward * forwardMovement + transform.TransformDirection(movementDelta);
                return;
            }

            float speed;
            GetInput(out speed);

            // Always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // Get a normal for the surface that is being touched to move along it
            /*RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            Vector3 projectedDesiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;*/

            // Air strafe
            Vector3 horizontalVelocity = new Vector3(m_CharacterController.velocity.x, 0.0f, m_CharacterController.velocity.z).normalized;
            Vector3 horizontalDirection = new Vector3(m_Camera.transform.forward.x, 0.0f, m_Camera.transform.forward.z).normalized;
            float angleBetweenVelocityAndDirection = Vector3.Angle(horizontalVelocity, horizontalDirection);

            Vector3 leftLimit = Quaternion.Euler(0, -45, 0) * horizontalVelocity;
            Vector3 rightLimit = Quaternion.Euler(0, 45, 0) * horizontalVelocity;
            float angleBetweenDirectionAndLeftLimit = Vector3.Angle(leftLimit, horizontalDirection);
            float angleBetweenDirectionAndRightLimit = Vector3.Angle(rightLimit, horizontalDirection);

            /*Debug.DrawRay(transform.position, leftLimit, Color.red);
            Debug.DrawRay(transform.position, rightLimit, Color.red);
            Debug.DrawRay(transform.position, horizontalDirection, Color.blue);
            if (angleBetweenVelocityAndDirection != 90) {
                Debug.Log(angleBetweenDirectionAndLeftLimit + " <- " + angleBetweenVelocityAndDirection + " -> " + angleBetweenDirectionAndRightLimit);
            }*/

            /*if (angleBetweenDirectionAndRightLimit > 90) {
                Debug.Log("TOO FAR LEFT");
            } else if (angleBetweenDirectionAndLeftLimit > 90) {
                Debug.Log("TOO FAR RIGHT");
            } else {
                Debug.Log("Looking good");
            }*/

            // Acceleration logic TODO: Take player aiming direction into consideration
            float accelerationRate = 20.0f;
            if (m_CharacterController.isGrounded) {
                if (desiredMove.x < 0 && m_CharacterController.velocity.x > 0 || desiredMove.x > 0 && m_CharacterController.velocity.x < 0) {
                    m_Acceleration = Mathf.Clamp(m_Acceleration - Time.deltaTime * accelerationRate, m_InitialAcceleration, 1.0f);
                } else if (desiredMove.x > 0 && m_CharacterController.velocity.x > 0 || desiredMove.x < 0 && m_CharacterController.velocity.x < 0) {
                    m_Acceleration = Mathf.Clamp(m_Acceleration + Time.deltaTime * accelerationRate, m_InitialAcceleration, 1.0f);
                }

                if (desiredMove.z < 0 && m_CharacterController.velocity.z > 0 || desiredMove.z > 0 && m_CharacterController.velocity.z < 0) {
                    m_Acceleration = Mathf.Clamp(m_Acceleration - Time.deltaTime * accelerationRate, m_InitialAcceleration, 1.0f);
                } else if (desiredMove.z > 0 && m_CharacterController.velocity.z > 0 || desiredMove.z < 0 && m_CharacterController.velocity.z < 0) {
                    m_Acceleration = Mathf.Clamp(m_Acceleration + Time.deltaTime * accelerationRate, m_InitialAcceleration, 1.0f);
                }

                if (desiredMove.x == 0.0f && desiredMove.z == 0.0f) {
                    m_Acceleration = m_InitialAcceleration;
                }

                m_MoveDir.x = desiredMove.x * speed * m_Acceleration;
                m_MoveDir.z = desiredMove.z * speed * m_Acceleration;
            } else {
                m_MoveDir = m_CharacterController.velocity;
            }

            if (m_CharacterController.isGrounded) {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump) {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            } else {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound() {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed) {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0)) {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) * Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep)) {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio() {
            if (!m_CharacterController.isGrounded) {
                return;
            }

            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed) {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob) {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded) {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            } else {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed) {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1) {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0) {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView() {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit) {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below) {
                return;
            }

            if (body == null || body.isKinematic) {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
