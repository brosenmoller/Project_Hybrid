using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputHandlingMethod inputHandlingMethod;

    public static GameManager Instance { get; protected set; }

    private ServiceLocator serviceLocator;

    public event Action OnSceneChange;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            serviceLocator = new ServiceLocator();
            ServiceSetup();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ServiceSetup()
    {
        serviceLocator.Add(new InputService(inputHandlingMethod));
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
        serviceLocator?.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        serviceLocator.OnSceneLoaded();
    }

    private void Update()
    {
        serviceLocator.Update();
    }

    public void ReloadScene()
    {
        OnSceneChange?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        OnSceneChange?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
