using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    
    public Rigidbody2D rb;
    Animator animator;
    
    private Vector2 _movement;
    private const float MoveSpeed = 5f;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
   
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
    
        // Sprite boyutunu al
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        collider.size = sprite.bounds.size;
    }
    /*[RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class AutoCollider : MonoBehaviour {
        void Reset() {
            UpdateCollider();
        }

        void OnValidate() {
            UpdateCollider();
        }

        void UpdateCollider() {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null) return;

            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.size = sr.sprite.bounds.size;
            col.offset = sr.sprite.bounds.center;
        }
    }*/
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
    
        animator.SetFloat("moveX", moveX);
        animator.SetBool("isMoving", Mathf.Abs(moveX) > 0.1f);
        _movement = GameManager.Instance.ProcessMovement();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + _movement.normalized * (MoveSpeed * Time.fixedDeltaTime));
    }
}