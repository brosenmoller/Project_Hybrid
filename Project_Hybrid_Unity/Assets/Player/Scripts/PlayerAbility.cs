using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerController))]
public abstract class PlayerAbility : MonoBehaviour
{
    public Rigidbody2D RigidBody { get; private set; }
    public PlayerController Controller { get; private set; }
    [SerializeField] protected Transform SpriteHolder;

    protected InputService InputService { get; private set; }

    private void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        InputService = ServiceLocator.Instance.Get<InputService>();
        Initialize();
    }

    protected virtual void Initialize() { }
}

