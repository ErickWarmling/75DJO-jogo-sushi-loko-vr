using UnityEngine;

public class GerenciadorSomAmbiente : MonoBehaviour
{
    [Header("Referências")]
    public Transform playerCamera;
    public float maxDistance = 5f;
    public LayerMask layerChao;

    [Header("Chãos das Áreas")]
    public GameObject chaoRecepcao;
    public GameObject chaoCozinha;
    public GameObject chaoAreaExterna;

    [Header("AudioSources")]
    public AudioSource somRecepcao;
    public AudioSource somCozinha;
    public AudioSource somAreaExterna;

    private AudioSource somAtual;
    private Collider colliderRecepcao;
    private Collider colliderCozinha;
    private Collider colliderAreaExterna;

    void Start()
    {
        colliderRecepcao  = chaoRecepcao.GetComponent<Collider>();
        colliderCozinha   = chaoCozinha.GetComponent<Collider>();
        colliderAreaExterna = chaoAreaExterna.GetComponent<Collider>();

        TrocarSom(somRecepcao);
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.position, Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerChao))
            return;

        AudioSource novoSom = null;

        if (hit.collider == colliderRecepcao)
            novoSom = somRecepcao;
        else if (hit.collider == colliderCozinha)
            novoSom = somCozinha;
        else if (hit.collider == colliderAreaExterna)
            novoSom = somAreaExterna;

        if (novoSom != null && novoSom != somAtual)
            TrocarSom(novoSom);
    }

    void TrocarSom(AudioSource novoSom)
    {
        if (somAtual != null && somAtual.isPlaying)
            somAtual.Stop();

        somAtual = novoSom;
        somAtual.Play();
    }
}
