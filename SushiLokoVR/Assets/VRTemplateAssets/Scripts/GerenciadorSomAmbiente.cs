using UnityEngine;

public class GerenciadorSomAmbiente : MonoBehaviour
{
    [Header("Chãos das Áreas")]
    public GameObject chaoRecepcao;
    public GameObject chaoCozinha;
    public GameObject chaoAreaExterna;

    [Header("AudioSources")]
    public AudioSource somRecepcao;
    public AudioSource somCozinha;
    public AudioSource somAreaExterna;

    private AudioSource somAtual;
    private Transform camTransform;

    // Bounds XZ de cada chão (estáticos — objetos não se movem)
    private Bounds boundsRecepcao;
    private Bounds boundsCozinha;
    private Bounds boundsAreaExterna;

    // Centros dos chãos usados como fallback Voronoi nas transições
    private Vector3 posRecepcao;
    private Vector3 posCozinha;
    private Vector3 posAreaExterna;

    void Start()
    {
        // Camera.main é a câmera VR real (rastreia a cabeça).
        // O campo playerCamera que existia antes apontava para o Camera Offset,
        // que não se atualiza com head-tracking nem com locomoção sem teleporte.
        camTransform = Camera.main.transform;

        boundsRecepcao    = chaoRecepcao.GetComponent<Collider>().bounds;
        boundsCozinha     = chaoCozinha.GetComponent<Collider>().bounds;
        boundsAreaExterna = chaoAreaExterna.GetComponent<Collider>().bounds;

        posRecepcao    = chaoRecepcao.transform.position;
        posCozinha     = chaoCozinha.transform.position;
        posAreaExterna = chaoAreaExterna.transform.position;

        TrocarSom(somRecepcao);
    }

    void Update()
    {
        Vector3 pos = camTransform.position;

        bool inRecepcao    = IsInBoundsXZ(pos, boundsRecepcao);
        bool inCozinha     = IsInBoundsXZ(pos, boundsCozinha);
        bool inAreaExterna = IsInBoundsXZ(pos, boundsAreaExterna);

        int inCount = (inRecepcao ? 1 : 0) + (inCozinha ? 1 : 0) + (inAreaExterna ? 1 : 0);

        AudioSource novoSom;

        if (inCount == 1)
        {
            // Dentro de exatamente uma zona: decisão direta
            if (inRecepcao)    novoSom = somRecepcao;
            else if (inCozinha) novoSom = somCozinha;
            else               novoSom = somAreaExterna;
        }
        else
        {
            // Fora de todas as zonas ou em sobreposição de bordas:
            // usa Voronoi (centro mais próximo) como fallback
            float dR = SqrDistXZ(pos, posRecepcao);
            float dC = SqrDistXZ(pos, posCozinha);
            float dE = SqrDistXZ(pos, posAreaExterna);

            if (dR <= dC && dR <= dE)  novoSom = somRecepcao;
            else if (dC <= dE)         novoSom = somCozinha;
            else                       novoSom = somAreaExterna;
        }

        if (novoSom != null && novoSom != somAtual)
            TrocarSom(novoSom);
    }

    bool IsInBoundsXZ(Vector3 point, Bounds bounds)
    {
        return point.x >= bounds.min.x && point.x <= bounds.max.x &&
               point.z >= bounds.min.z && point.z <= bounds.max.z;
    }

    float SqrDistXZ(Vector3 a, Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;
        return dx * dx + dz * dz;
    }

    void TrocarSom(AudioSource novoSom)
    {
        if (somAtual != null && somAtual.isPlaying)
            somAtual.Stop();
        somAtual = novoSom;
        somAtual.Play();
    }
}
