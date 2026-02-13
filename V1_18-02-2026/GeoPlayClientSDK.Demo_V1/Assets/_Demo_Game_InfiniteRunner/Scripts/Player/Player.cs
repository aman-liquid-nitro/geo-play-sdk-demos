using System;
using System.ComponentModel;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class Player : MonoBehaviour, IPausable
    {
        [SerializeField] private Animator animController;
        [SerializeField] private PlayerAnimationEvents animationEvents;

        [Header("References")]
        [SerializeField] private StoreItemsDB storeItemsDB;
        [SerializeField] private SpriteRenderer mountPrefab;
        [SerializeField] private Transform[] mountContainers;

        [Header("Attributes")]
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private float manualFallForce = 20f;
        [SerializeField] private ParticleSystem dustRockParticles;
        [SerializeField] private ParticleSystem chairDustParticles;

        [Header("Input Settings")]
        [SerializeField] private bool useMobileInput = false;
        [SerializeField] private float swipeThreshold = 50f;
        [SerializeField] private float mobileDuckDuration = 1.5f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody2D rb;
        private CapsuleCollider2D col;
        private bool isGrounded;
        private bool wasGrounded;
        private bool isPaused;
        private Vector2 pausedVelocity;
        private IInputHandler inputHandler;

        private const string ANIM_PROPERTY_JUMP = "jump";
        private const string ANIM_PROPERTY_DUCK = "duck";

        [Header("Collider Offset")]
        [SerializeField] private Vector2 colliderOffset = new Vector2(-4.283311f, 2.562742f);
        [SerializeField] private Vector2 colliderOffsetDuck = new Vector2(-3.89f, 1.814f);
        [Header("Collider Size")]
        [SerializeField] private Vector2 colliderSize = new Vector2(2.365459f, 3.868692f);
        [SerializeField] private Vector2 colliderSizeDuck = new Vector2(4.3f, 2.37f);

        public static event Action<Obstacle> OnObstacleCollision;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CapsuleCollider2D>();

            // Initialize input handler based on platform or setting
            InitializeInputHandler();
        }

        private void Start()
        {
            InitPlayerSkin();
        }

        private void InitializeInputHandler()
        {
#if UNITY_ANDROID || UNITY_IOS
            inputHandler = new MobileInputHandler(swipeThreshold, mobileDuckDuration);
#else
            if (useMobileInput)
                inputHandler = new MobileInputHandler(swipeThreshold, mobileDuckDuration);
            else
                inputHandler = new KeyboardInputHandler();
#endif

            // Subscribe to input events
            inputHandler.OnJumpInput += Jump;
            inputHandler.OnDuckInput += StartDuck;
            inputHandler.OnDuckRelease += StopDuck;
            inputHandler.OnIdleInput += OnIdle;
        }

        private void OnDestroy()
        {
            // Unsubscribe from input events
            if (inputHandler != null)
            {
                inputHandler.OnJumpInput -= Jump;
                inputHandler.OnDuckInput -= StartDuck;
                inputHandler.OnDuckRelease -= StopDuck;
                inputHandler.OnIdleInput -= OnIdle;
            }
        }

        private void Update()
        {
            if (isPaused) return;

            // Check ground status
            CheckGroundStatus();

            // Process input through the handler
            inputHandler?.Update();
        }

        private void CheckGroundStatus()
        {
            wasGrounded = isGrounded;

            // Perform ground check using overlap circle
            if (groundCheckPoint != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
            }
            else
            {
                // Fallback: check slightly below the player
                Vector2 checkPosition = (Vector2)transform.position + Vector2.down * 0.1f;
                isGrounded = Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer);
            }

            // Trigger landing event when player just landed
            if (!wasGrounded && isGrounded)
            {
                OnLanded();
            }
        }

        private void OnLanded()
        {
            // Called when player touches ground after being in air
            animController.SetBool(ANIM_PROPERTY_JUMP, false);
        }

        private void Jump()
        {
            if (!isGrounded) return;

            if (rb != null)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            animController.SetBool(ANIM_PROPERTY_JUMP, true);
            isGrounded = false;
            ToggleChairParticles(false);
            animationEvents.OnJump();
        }

        private void StartDuck()
        {
            animController.SetBool(ANIM_PROPERTY_DUCK, true);
            SetColliderSize(isDuck: true);
            ToggleDuckParticles(true);

            if (!isGrounded)
            {
                if (rb != null)
                    rb.AddForce(Vector2.down * manualFallForce, ForceMode2D.Impulse);
            }
            else
            {
                ToggleChairParticles(false);
            }
        }

        private void StopDuck()
        {
            if (!isGrounded) return;

            animController.SetBool(ANIM_PROPERTY_DUCK, false);
            SetColliderSize(isDuck: false);
            ToggleDuckParticles(false);
            ToggleChairParticles(true);
        }

        private void OnIdle()
        {
            if (!isGrounded) return;

            animController.SetBool(ANIM_PROPERTY_JUMP, false);
            animController.SetBool(ANIM_PROPERTY_DUCK, false);

            SetColliderSize(isDuck: false);
            ToggleDuckParticles(false);
            ToggleChairParticles(true);
        }

        public void TogglePause(bool isPaused)
        {
            this.isPaused = isPaused;
            animController.speed = isPaused ? 0 : 1;

            if (isPaused)
            {
                pausedVelocity = rb.velocity;
                rb.velocity = Vector2.zero;
            }
            else
                rb.velocity = pausedVelocity;
            rb.isKinematic = isPaused;
        }

        private void SetColliderSize(bool isDuck)
        {
            col.offset = isDuck ? colliderOffsetDuck : colliderOffset;
            col.size = isDuck ? colliderSizeDuck : colliderSize;
            col.direction = isDuck ? CapsuleDirection2D.Horizontal : CapsuleDirection2D.Vertical;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == null) return;
            Obstacle obstacle = collision.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                OnObstacleCollision?.Invoke(obstacle);
            }
        }

        private void ToggleChairParticles(bool isActive)
        {
            if (chairDustParticles)
                chairDustParticles.gameObject.SetActive(isActive);
        }

        private void ToggleDuckParticles(bool isActive)
        {
            if (dustRockParticles)
                dustRockParticles.gameObject.SetActive(isActive);
        }

        void InitPlayerSkin()
        {
            int itemId = StoreManager.GetSelectedItemId();
            if (itemId <= 0) return;

            for (int i = 0; i < mountContainers.Length; i++) 
            {
                var mount = Instantiate(mountPrefab, mountContainers[i]);
                StoreItemSO itemSO = storeItemsDB.GetItem(itemId);
                if (itemSO != null)
                {
                    mount.sprite = itemSO.sprite;
                    mount.transform.localPosition = itemSO.localPosition;
                    mount.transform.localScale = Vector3.one * itemSO.localScale;
                    mount.transform.localEulerAngles = Vector3.forward * itemSO.zRotation;
                }
                mount.enabled = false;

                if (i == 0)
                    animationEvents.SetMountMove0(mount);
                else if (i == 1)
                    animationEvents.SetMountMove1(mount);
                else if (i == 2)
                    animationEvents.SetMountDuck(mount);
            }
        }

        // Visualize ground check in editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 checkPos = groundCheckPoint != null ? groundCheckPoint.position : transform.position + Vector3.down * 0.1f;
            Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
        }
    }
}