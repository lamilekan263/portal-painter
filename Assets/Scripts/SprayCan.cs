using UnityEngine;

public class SprayCan : MonoBehaviour
{
    public ParticleSystem sprayParticles;
    public AudioSource spraySound;

    private bool isSpraying = false;

    public GameObject decalPrefab; // Assign a prefab with a spray decal
    public float spawnRate = 0.05f; // Seconds between decal spawns
    private float lastSpawnTime;

    void Update()
    {
        bool triggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.1f;

        if (triggerPressed && !isSpraying)
        {
            StartSpraying();

        }
        else if (!triggerPressed && isSpraying)
        {
            StopSpraying();
        }
    }

    void StartSpraying()
    {
        sprayParticles.Play();

        if (!spraySound.isPlaying)
            spraySound.Play();
        // TrySpawnDecal();
        isSpraying = true;
    }

    void StopSpraying()
    {
        sprayParticles.Stop();
        spraySound.Stop();
        isSpraying = false;
    }



    void TrySpawnDecal()
    {
        if (Time.time - lastSpawnTime < spawnRate) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f))
        {
            GameObject decal = Instantiate(decalPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
            decal.transform.SetParent(hit.transform); // Attach to surface
        }

        lastSpawnTime = Time.time;
    }
}
