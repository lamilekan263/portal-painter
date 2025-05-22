using UnityEngine;
using Oculus.Interaction;
using Meta.XR.MRUtilityKit;


public class SprayCan : MonoBehaviour
{
    [SerializeField] private ParticleSystem _sprayParticles;
    [SerializeField] private AudioSource _spraySound;

    [SerializeField] private Grabbable _grabbable;

    [SerializeField] private GameObject _nuzzlePoint;
    [SerializeField] private float _decalSize = 0.05f;

    [SerializeField] MRUKAnchor.SceneLabels labelFilter;



    [SerializeField] GameObject decalPrefab;
    [SerializeField] float spawnRate = 0.01f;

    [SerializeField] Color _paintColor;


    private float _lastSpawnTime;
    private bool _isSpraying = false;
    Vector3 _lastSpawnPosition;
    float minDistance = 0.005f;

    void Update()
    {
        if (_grabbable.SelectingPointsCount <= 0)
        {
            StopSpraying();
            return;
        }

        bool triggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.1f;

        if (triggerPressed)
        {
            if (!_isSpraying)
            {
                StartSpraying();
            }

            TrySpawnPaint();
        }
        else if (!triggerPressed && _isSpraying)
        {
            StopSpraying();
        }
    }

    void StartSpraying()
    {
        if (_sprayParticles != null)
            _sprayParticles.Play();

        if (_spraySound != null && !_spraySound.isPlaying)
            _spraySound.Play();

        TrySpawnPaint();

        _isSpraying = true;
    }

    void StopSpraying()
    {
        if (_sprayParticles != null)
            _sprayParticles.Stop();

        if (_spraySound != null && _spraySound.isPlaying)
            _spraySound.Stop();

        _isSpraying = false;
    }
    void TrySpawnPaint()
    {
        if (Time.time - _lastSpawnTime < spawnRate) return;
        Ray ray = new Ray(_nuzzlePoint.transform.position, _nuzzlePoint.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 5f, Color.red, 5f);
        MRUKRoom mRUKRoom = MRUK.Instance.GetCurrentRoom();


        if (mRUKRoom.Raycast(ray, 2f, LabelFilter.FromEnum(labelFilter), out RaycastHit hit, out MRUKAnchor anchor))
        {

            if (_lastSpawnPosition != Vector3.zero && Vector3.Distance(_lastSpawnPosition, hit.point) < minDistance)
                return;

            Vector3 spawnPos = hit.point + hit.normal * 0.001f;
            Quaternion spawnRot = Quaternion.LookRotation(-hit.normal);

            GameObject paint = Instantiate(decalPrefab, spawnPos, spawnRot);
            // float randomSize = Random.Range(0.8f, 1.2f) * _decalSize;
            paint.transform.localScale = Vector3.one * _decalSize;
            var renderer = paint.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material matInstance = renderer.material;
                matInstance.color = _paintColor;
                renderer.material = matInstance;
            }
            paint.AddComponent<OVRSpatialAnchor>();


            _lastSpawnPosition = hit.point;
            _lastSpawnTime = Time.time;
        }
    }
}