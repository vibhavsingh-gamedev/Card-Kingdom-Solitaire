using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioManager handles all game audio.
/// Singleton that persists across scenes via DontDestroyOnLoad.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip cardClickSFX;
    [SerializeField] private AudioClip cardPlaceSFX;
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip invalidMoveSFX;
    [SerializeField] private AudioClip winSFX;
    [SerializeField] private AudioClip loseSFX;

    private Dictionary<string, AudioClip> sfxDictionary;

    private void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSFXDictionary();
            Debug.Log("AudioManager initialized and set to persist");
        }
        else
        {
            Debug.Log("Duplicate AudioManager destroyed");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Auto-play music on start
        PlayMusic();
    }

    private void InitializeSFXDictionary()
    {
        sfxDictionary = new Dictionary<string, AudioClip>
        {
            { "CardClick", cardClickSFX },
            { "CardPlace", cardPlaceSFX },
            { "ButtonClick", buttonClickSFX },
            { "InvalidMove", invalidMoveSFX },
            { "Win", winSFX },
            { "Lose", loseSFX }
        };

        Debug.Log($"SFX Dictionary initialized with {sfxDictionary.Count} sounds");
    }

    public void PlayMusic()
    {
        if (musicSource == null)
        {
            Debug.LogError("Music Source is NULL! Assign it in Inspector!");
            return;
        }

        if (backgroundMusic == null)
        {
            Debug.LogError("Background Music clip is NULL! Assign it in Inspector!");
            return;
        }

        if (musicSource.isPlaying && musicSource.clip == backgroundMusic)
        {
            Debug.Log("Music already playing");
            return;
        }

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
        Debug.Log("Background music started playing");
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlaySFX(string sfxName)
    {
        if (sfxSource == null)
        {
            Debug.LogError($"SFX Source is NULL when trying to play '{sfxName}'!");
            return;
        }

        if (sfxDictionary == null)
        {
            Debug.LogError("SFX Dictionary is NULL!");
            return;
        }

        if (sfxDictionary.ContainsKey(sfxName))
        {
            AudioClip clip = sfxDictionary[sfxName];
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
                Debug.Log($"Playing SFX: {sfxName}");
            }
            else
            {
                Debug.LogWarning($"SFX clip is NULL for: {sfxName}");
            }
        }
        else
        {
            Debug.LogWarning($"SFX not found in dictionary: {sfxName}");
        }
    }
}