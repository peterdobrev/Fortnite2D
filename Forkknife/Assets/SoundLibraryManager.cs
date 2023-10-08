using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Player")]
    public AudioClip[] die;
    [Header("Weapons")]
    public AudioClip[] weaponSwitch;
    public AudioClip[] shotgunShoot;
    public AudioClip[] bulletHit;
    public AudioClip[] arShoot;
    public AudioClip[] pistolShoot;
    [Header("Moving")]
    public AudioClip[] jumpSound;
    public AudioClip[] movingSound;
    [Header("Buildings")]
    public AudioClip[] buildingSound;
    public AudioClip[] destroyedBuildingSound;
    [Header("Healing and Shields")]
    public AudioClip[] bigShield;
    public AudioClip[] miniShield;
    public AudioClip[] bandages;
    public AudioClip[] medkit;



    // Singleton Instance
    public static SoundManager instance;

    private void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(string soundName)
    {
        audioSource.volume = 1f;
        switch (soundName)
        {
            case "Die":
                audioSource.volume = 0.5f;
                audioSource.PlayOneShot(GetRandomClip(die));
                break;
            case "Shotgun":
                audioSource.PlayOneShot(GetRandomClip(shotgunShoot));
                break;
            case "BulletHit":
                audioSource.PlayOneShot(GetRandomClip(bulletHit));
                break;
            case "AR":
                audioSource.PlayOneShot(GetRandomClip(arShoot));
                break;
            case "Pistol":
                audioSource.PlayOneShot(GetRandomClip(pistolShoot));
                break;
            case "Jump":
                audioSource.PlayOneShot(GetRandomClip(jumpSound));
                break;
            case "Moving":
                audioSource.PlayOneShot(GetRandomClip(movingSound));
                break;
            case "Building":
                audioSource.PlayOneShot(GetRandomClip(buildingSound));
                break;
            case "DestroyedBuilding":
                audioSource.PlayOneShot(GetRandomClip(destroyedBuildingSound));
                break;
            case "WeaponSwitch":
                audioSource.volume = 0.25f;
                audioSource.PlayOneShot(GetRandomClip(weaponSwitch));
                break;
            case "MiniShield":
                audioSource.volume = 0.5f;
                audioSource.PlayOneShot(GetRandomClip(miniShield));
                break;
            case "BigShield":
                audioSource.volume = 0.5f;
                audioSource.PlayOneShot(GetRandomClip(bigShield));
                break;
            case "Medkit":
                audioSource.volume = 0.5f;
                audioSource.PlayOneShot(GetRandomClip(medkit));
                break;
            case "Bandages":
                audioSource.volume = 0.5f;
                audioSource.PlayOneShot(GetRandomClip(bandages));
                break;
            default:
                Debug.LogWarning("Sound name not found!");
                break;
        }
    }
    private AudioClip GetRandomClip(AudioClip[] clipArray)
    {
        if (clipArray == null || clipArray.Length == 0) return null;
        return clipArray[Random.Range(0, clipArray.Length)];
    }
}
