using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SomAmbiente : MonoBehaviour
{
    public Transform playerCamera;
    public float maxDistance = 3f;

    private AudioSource audioSource;
    private bool playerNaArea = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.position, Vector3.down);
        bool estaNaArea = Physics.Raycast(ray, out RaycastHit hit, maxDistance)
                          && hit.collider.gameObject == gameObject;

        if (estaNaArea && !playerNaArea)
        {
            audioSource.Play();
            playerNaArea = true;
        }
        else if (!estaNaArea && playerNaArea)
        {
            audioSource.Stop();
            playerNaArea = false;
        }
    }
}
