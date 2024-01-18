using UnityEngine;

public class BreakableWall : MonoBehaviour, IMeleeInteractable
{
    [Header("DestructableWall Settings")]
    [SerializeField] private float hitPoints = 1;
    [SerializeField] private bool directional = false;
    [SerializeField, Range(-1, 1)] private int interactableDirection = 1;
    [SerializeField] private bool vertical = false;

    private Transform player;

    private int direction;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }

    public void MeleeInteract(int damage)
    {
        if (!directional)
        {
            Damage(damage);
            return;
        }

        float playerDifference;

        if (vertical) 
        { 
            playerDifference = player.position.y - transform.position.y; 
        }
        else 
        {
            playerDifference = player.position.x - transform.position.x;
        }

        if (playerDifference < 0) direction = -1;
        else direction = 1;

        if (direction == interactableDirection)
        {
            //AudioManager.Instance.Play("DestructableWall");
            Damage(damage);
        }
        else
        {
            //AudioManager.Instance.Play("Wall_Reject");
        }
    }

    private void Damage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= damage)
        {
            Destruction();
            return;
        }
    }

    private void Destruction()
    {
        if (TryGetComponent(out AudioSource audioSource))
        {
            audioSource.Play();
        }

        if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.enabled = false;
        }

        GetComponent<Collider2D>().enabled = false;
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
        
        Destroy(gameObject, 3f);
    }
}

