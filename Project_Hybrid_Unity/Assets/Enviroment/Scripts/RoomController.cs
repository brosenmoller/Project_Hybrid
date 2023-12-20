using UnityEngine;
using Cinemachine;

public class RoomController : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject roomContents;
    [SerializeField] private PolygonCollider2D frameCollider;
    private PolygonCollider2D loadCollider;

    private Collider2D playerCollider;
    private PlayerController playerController;

    private void Awake()
    {
       playerController = FindObjectOfType<PlayerController>();
       playerCollider = playerController.GetComponent<Collider2D>();
       loadCollider = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        //if (loadCollider.IsTouching(playerCollider)) { roomContents.SetActive(true); }
        //else { roomContents.SetActive(false); }
    }

    private void Update()
    {
        if (frameCollider.IsTouching(playerCollider))
        {
            virtualCamera.Priority = 1;
        }
        else { virtualCamera.Priority = 0; }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (player.TryGetComponent(out PlayerController playerController))
    //    {
    //        roomContents.SetActive(true);
    //        CancelInvoke(nameof(DeactivateRoom));
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (player.TryGetComponent(out PlayerController playerController))
    //    {
    //        Invoke(nameof(DeactivateRoom), 2f);
    //    }
    //}

    //private void DeactivateRoom()
    //{
    //    roomContents.SetActive(false);
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(frameCollider.bounds.center, frameCollider.bounds.size);

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (frameCollider.bounds.Contains(playerController.transform.position)) { virtualCamera.Priority = 1; }
        else { virtualCamera.Priority = 0; }
    }
}
