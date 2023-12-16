using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeathTrigger : MonoBehaviour
{
#if UNITY_EDITOR
    private void Awake()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (!collider.isTrigger)
        {
            Debug.LogWarning("The GameObject: \"" + gameObject.name + "\" has an area trigger component but doesn't have a trigger collider");
        }
    }
#endif

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController player))
        {
            GameManager.Instance.ReloadScene();
        }
    }
}
