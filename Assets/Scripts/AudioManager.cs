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

    [Header("UI")]
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Game")]
    [SerializeField] private AudioClip finishSound;

    private void Awake()
    {
        // Only keep one AudioManager between scenes
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
        //Debug.Log("AudioManager started");

        if (musicSource == null)
        {
           // Debug.LogWarning("MusicSource is not assigned!");
            return;
        }

        if (backgroundMusic == null)
        {
           // Debug.LogWarning("Background music is not assigned!");
            return;
        }

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;

        //Debug.Log("Starting music: " + backgroundMusic.name);

        musicSource.Play();

      //  Debug.Log("Music isPlaying: " + musicSource.isPlaying);
    }

    private void PlaySound(AudioClip clip)
    {
        if (sfxSource == null)
        {
         //   Debug.LogWarning("SFXSource is not assigned!");
            return;
        }

        if (clip == null)
        {
        //    Debug.LogWarning("Tried to play a sound, but no AudioClip was assigned.");
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

    public void PlayButtonHover()
    {
        PlaySound(buttonHoverSound);
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
    }

    public void PlayFinish()
    {
        PlaySound(finishSound);
    }
}