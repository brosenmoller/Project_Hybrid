using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; protected set; }

    private ServiceLocator serviceLocator;

    private void Awake()
    {
        serviceLocator = new ServiceLocator();
        ServiceSetup();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void ServiceSetup()
    {
        serviceLocator.Add(new InputService());
        serviceLocator.Add(new TimerService());
        serviceLocator.Add(new AudioService());
        serviceLocator.Add(new EventService());
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        serviceLocator.OnSceneLoaded();
    }

    private void FixedUpdate()
    {
        serviceLocator.FixedUpdate();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
