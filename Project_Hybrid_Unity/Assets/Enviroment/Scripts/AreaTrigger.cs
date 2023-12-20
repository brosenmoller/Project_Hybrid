using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class AreaTrigger : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent OnEntered;
    [SerializeField] private UnityEvent OnExited;

    [Header("Settings")]
    [SerializeField] private bool disableAfterActivation = true;

    private bool disabled = false;

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
        if (disabled) { return; }

        if (other.gameObject.TryGetComponent(out PlayerController playerController))
        {
            OnEntered?.Invoke();

            if (disableAfterActivation) { disabled = true; }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (disabled) { return; }

        if (other.gameObject.TryGetComponent(out PlayerController playerController))
        {
            OnExited?.Invoke();
        }
    }
}
