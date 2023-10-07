using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip shotgunShoot;
    public AudioClip bulletHit;
    public AudioClip arShoot;
    public AudioClip pistolShoot;
    public AudioClip jumpSound;
    public AudioClip movingSound;
    public AudioClip buildingSound;
    public AudioClip destroyedBuildingSound;
    public AudioClip weaponSwitch;

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
        switch (soundName)
        {
            case "Shotgun":
                audioSource.PlayOneShot(shotgunShoot);
                break;
            case "BulletHit":
                audioSource.PlayOneShot(bulletHit);
                break;
            case "AR":
                audioSource.PlayOneShot(arShoot);
                break;
            case "Pistol":
                audioSource.PlayOneShot(pistolShoot);
                break;
            case "Jump":
                audioSource.PlayOneShot(jumpSound);
                break;
            case "Moving":
                audioSource.PlayOneShot(movingSound);
                break;
            case "Building":
                audioSource.PlayOneShot(buildingSound);
                break;
            case "DestroyedBuilding":
                audioSource.PlayOneShot(destroyedBuildingSound);
                break;
            case "WeaponSwitch":
                audioSource.PlayOneShot(weaponSwitch);
                break;
            default:
                Debug.LogWarning("Sound name not found!");
                break;
        }
    }
}
