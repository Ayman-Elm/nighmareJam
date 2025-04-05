using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("FMOD Settings")]
    [SerializeField] private string masterBusPath = "bus:/";

    private Bus masterBus;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeBus();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeBus()
    {
        masterBus = RuntimeManager.GetBus(masterBusPath);
    }

    public void PlayOneShot(EventReference eventReference, Vector3 position = default)
    {
        RuntimeManager.PlayOneShot(eventReference, position);
    }

    public void PlayOneShotAttached(EventReference eventReference, GameObject gameObject)
    {
        RuntimeManager.PlayOneShotAttached(eventReference, gameObject);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        return RuntimeManager.CreateInstance(eventReference);
    }

    public void SetMasterVolume(float volume)
    {
        masterBus.setVolume(volume);
    }

    public float GetMasterVolume()
    {
        masterBus.getVolume(out float volume);
        return volume;
    }
} 