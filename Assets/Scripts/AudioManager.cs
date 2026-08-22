using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Movement")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip wallJumpSound;

    [Header("Abilities")]
    [SerializeField] private AudioClip abilitySelectSound;
    [SerializeField] private AudioClip gravityFlipSound;
    [SerializeField] private AudioClip timeStopSound;

    [Header("Game")]
    [SerializeField] private AudioClip finishSound;

    private void Awake()
    {
        // Keep only one AudioManager between scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;

            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayJump()
    {
        PlaySound(jumpSound);
    }

    public void PlayLand()
    {
        PlaySound(landSound);
    }

    public void PlayDash()
    {
        PlaySound(dashSound);
    }

    public void PlayWallJump()
    {
        PlaySound(wallJumpSound);
    }

    public void PlayAbilitySelect()
    {
        PlaySound(abilitySelectSound);
    }

    public void PlayGravityFlip()
    {
        PlaySound(gravityFlipSound);
    }

    public void PlayTimeStop()
    {
        PlaySound(timeStopSound);
    }

    public void PlayFinish()
    {
        PlaySound(finishSound);
    }
}