using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PuzzleCozinha : MonoBehaviour
{
    [Header("Referências do Puzzle")]
    public FogaoController fogaoController;
    public ZonaPanela zonaPanela;
    public Transform bowlBrocolis;
    public Transform panelaYakisoba;
    public Transform bowlYakisoba;
    public GameObject conteudoBowl;
    public GameObject portaTrancada;

    [Header("Configurações")]
    [Tooltip("Quantidade total de brócolis (usado apenas pela detecção via ZonaPanela)")]
    public int totalBrocolis = 3;
    [Tooltip("Tempo contínuo (segundos) que o fogão deve ficar ligado para cozinhar")]
    public float tempoCozimento = 10f;
    [Tooltip("Distância máxima (XZ) entre BowlBrocolis e PanelaYakisoba para detectar o derramamento dos brócolis")]
    public float raioDerramoBowl = 0.5f;
    [Tooltip("Distância máxima (XZ) entre PanelaYakisoba e BowlYakisoba para detectar o derramamento final")]
    public float raioDerramo = 0.8f;

    [Header("Outline da Porta")]
    public Color corOutline = Color.yellow;
    public float larguraOutline = 5f;

    // --- estado interno ---
    private bool brocolisNaPanela;
    private int brocolisContados;
    private float tempoCozinhando;
    private bool cozimentoConcluido;
    private bool puzzleConcluido;

    // bowl physics
    private Rigidbody bowlRigidbody;
    private XRGrabInteractable bowlGrab;
    private bool bowlOriginalUseGravity;
    private RigidbodyConstraints bowlOriginalConstraints;
    private bool bowlPhysicsLiberada;

    void Start()
    {
        if (zonaPanela != null) zonaPanela.SetPuzzle(this);

        if (portaTrancada != null)
        {
            var grab = portaTrancada.GetComponentInChildren<XRGrabInteractable>(true);
            if (grab != null)
                grab.enabled = false;
            else
                Debug.LogWarning("PuzzleCozinha: nenhum XRGrabInteractable encontrado em portaTrancada.");
        }

        if (bowlBrocolis != null)
        {
            bowlRigidbody = bowlBrocolis.GetComponent<Rigidbody>();
            bowlGrab = bowlBrocolis.GetComponent<XRGrabInteractable>();

            if (bowlRigidbody != null && bowlGrab != null)
            {
                PrepararBowlBrocolisInicial();
                bowlGrab.selectEntered.AddListener(LiberarFisicaBowlBrocolis);
            }
        }
    }

    void OnDestroy()
    {
        if (bowlGrab != null)
            bowlGrab.selectEntered.RemoveListener(LiberarFisicaBowlBrocolis);
    }

    void PrepararBowlBrocolisInicial()
    {
        bowlOriginalUseGravity = bowlRigidbody.useGravity;
        bowlOriginalConstraints = bowlRigidbody.constraints;

        bowlRigidbody.linearVelocity = Vector3.zero;
        bowlRigidbody.angularVelocity = Vector3.zero;
        bowlRigidbody.useGravity = false;
        bowlRigidbody.isKinematic = true;
        bowlRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void LiberarFisicaBowlBrocolis(SelectEnterEventArgs _)
    {
        if (bowlPhysicsLiberada || bowlRigidbody == null) return;

        bowlPhysicsLiberada = true;
        bowlRigidbody.constraints = bowlOriginalConstraints;
        bowlRigidbody.isKinematic = false;
        bowlRigidbody.useGravity = bowlOriginalUseGravity;
        bowlRigidbody.linearVelocity = Vector3.zero;
        bowlRigidbody.angularVelocity = Vector3.zero;
    }

    void Update()
    {
        if (puzzleConcluido) return;

        AtualizarCozimento();

        if (cozimentoConcluido)
            VerificarDerramamento();
    }

    // ─── Etapas 2 e 3: cozimento ─────────────────────────────────────────────

    void AtualizarCozimento()
    {
        if (cozimentoConcluido) return;

        bool temIngredientes = brocolisNaPanela || brocolisContados >= totalBrocolis;

        if (temIngredientes && fogaoController.EstaLigado)
        {
            tempoCozinhando += Time.deltaTime;
            if (tempoCozinhando >= tempoCozimento)
            {
                cozimentoConcluido = true;
                Debug.Log("PuzzleCozinha: cozimento concluído! Leve a panela ao balcão e despeje no bowl.");
            }
        }
        else
        {
            tempoCozinhando = 0f;
        }
    }

    // ─── Etapas 4 e 5: derramar PanelaYakisoba no BowlYakisoba ──────────────

    void VerificarDerramamento()
    {
        bool inclinada = Vector3.Dot(panelaYakisoba.up, Vector3.down) > 0f;
        float distXZ = new Vector2(
            panelaYakisoba.position.x - bowlYakisoba.position.x,
            panelaYakisoba.position.z - bowlYakisoba.position.z).magnitude;

        if (inclinada && distXZ < raioDerramo)
            Concluir();
    }

    // ─── Etapa 6: conclusão ───────────────────────────────────────────────────

    void Concluir()
    {
        puzzleConcluido = true;

        if (conteudoBowl != null)
        {
            var mr = conteudoBowl.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = true;
        }

        if (portaTrancada != null)
        {
            var grab = portaTrancada.GetComponentInChildren<XRGrabInteractable>(true);
            if (grab != null) grab.enabled = true;

            var outline = portaTrancada.GetComponent<Outline>()
                       ?? portaTrancada.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = corOutline;
            outline.OutlineWidth = larguraOutline;
        }

        Debug.Log("PuzzleCozinha: puzzle concluído! Porta destrancada.");
    }

    // ─── Chamados por ZonaPanela (fallback — colocar brócolis individualmente) ─

    public void BrocoliEntrou()
    {
        brocolisContados = Mathf.Min(brocolisContados + 1, totalBrocolis);
        Debug.Log($"PuzzleCozinha: brócolis na panela {brocolisContados}/{totalBrocolis}");
    }

    public void BrocoliSaiu()
    {
        brocolisContados = Mathf.Max(brocolisContados - 1, 0);
        if (!cozimentoConcluido) tempoCozinhando = 0f;
        Debug.Log($"PuzzleCozinha: brócolis na panela {brocolisContados}/{totalBrocolis}");
    }

    public void BrocolisDerramadosDoBowl()
    {
        if (brocolisNaPanela) return;

        brocolisNaPanela = true;
        brocolisContados = totalBrocolis;
        Debug.Log("PuzzleCozinha: brócolis despejados na panela! Ligue o fogão.");
    }
}
