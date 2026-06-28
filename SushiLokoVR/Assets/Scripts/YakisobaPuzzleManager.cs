using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class YakisobaPuzzleManager : MonoBehaviour
{
    private const string BowlBrocolisName = "BowlBrocolis";
    private const string PanelaName = "PanelaYakisoba";
    private const string BowlYakisobaName = "BowlYakisoba";
    private const string DoorName = "DoorV6 (1)";
    private const string BotaoFogaoName = "Push Button Fogao";

    [SerializeField] private float anguloDerramar = 100f;
    [SerializeField] private float tempoCozimento = 10f;
    [SerializeField] private float raioZonaPanela = 0.22f;
    [SerializeField] private float raioZonaBowl = 0.22f;

    private GameObject bowlBrocolis;
    private GameObject panela;
    private GameObject bowlYakisoba;
    private GameObject porta;

    private Outline outlineBowlBrocolis;
    private Outline outlinePanela;
    private Outline outlinePorta;
    private FogaoController fogao;
    private XRGrabInteractable grabBowlBrocolis;
    private XRGrabInteractable grabPanela;
    private XRGrabInteractable grabPorta;
    private Rigidbody rbPorta;
    private RigidbodyConstraints constraintsOriginaisPorta;

    private bool pegouBrocolis;
    private bool brocolisNaPanela;
    private bool yakisobaCozido;
    private bool pegouPanelaCozida;
    private bool puzzleCompleto;
    private float tempoLigadoComIngrediente;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CriarNaCena()
    {
        SceneManager.sceneLoaded += (_, __) => InstalarGerenciador();
        InstalarGerenciador();
    }

    private static void InstalarGerenciador()
    {
        if (FindAnyObjectByType<YakisobaPuzzleManager>() != null)
            return;

        var obj = new GameObject("YakisobaPuzzleManager");
        obj.AddComponent<YakisobaPuzzleManager>();
    }

    private void Start()
    {
        bowlBrocolis = GameObject.Find(BowlBrocolisName);
        panela = GameObject.Find(PanelaName);
        bowlYakisoba = GameObject.Find(BowlYakisobaName);
        porta = GameObject.Find(DoorName);
        fogao = BuscarFogao();

        if (!ValidarReferencias())
            return;

        outlineBowlBrocolis = PrepararOutline(bowlBrocolis, new Color(0.1f, 1f, 0.25f, 1f));
        outlinePanela = PrepararOutline(panela, new Color(0f, 0.75f, 1f, 1f));
        outlinePorta = PrepararOutline(porta, new Color(1f, 0.75f, 0.05f, 1f));

        grabBowlBrocolis = bowlBrocolis.GetComponent<XRGrabInteractable>();
        grabPanela = panela.GetComponent<XRGrabInteractable>();
        grabPorta = porta.GetComponentInChildren<XRGrabInteractable>(true);
        rbPorta = porta.GetComponentInChildren<Rigidbody>(true);

        if (grabBowlBrocolis != null)
            grabBowlBrocolis.selectEntered.AddListener(_ => pegouBrocolis = true);

        if (grabPanela != null)
            grabPanela.selectEntered.AddListener(_ =>
            {
                if (yakisobaCozido)
                {
                    pegouPanelaCozida = true;
                    SetOutline(outlinePanela, 0f);
                }
            });

        TravarPorta();
        CriarZonaDerramar(panela, raioZonaPanela, ZonaPuzzle.Panela);
        CriarZonaDerramar(bowlYakisoba, raioZonaBowl, ZonaPuzzle.BowlYakisoba);
    }

    private void Update()
    {
        if (!brocolisNaPanela || yakisobaCozido || fogao == null || !fogao.EstaLigado)
            return;

        tempoLigadoComIngrediente += Time.deltaTime;
        if (tempoLigadoComIngrediente >= tempoCozimento)
        {
            yakisobaCozido = true;
            SetOutline(outlinePanela, 5f);
            Debug.Log("Yakisoba cozido. Agora derrame a panela no BowlYakisoba.");
        }
    }

    public void EntrouNaZona(ZonaPuzzle zona)
    {
        if (zona == ZonaPuzzle.Panela && pegouBrocolis && !brocolisNaPanela)
            SetOutline(outlineBowlBrocolis, 5f);

        if (zona == ZonaPuzzle.BowlYakisoba && yakisobaCozido && pegouPanelaCozida && !puzzleCompleto)
            SetOutline(outlinePanela, 5f);
    }

    public void SaiuDaZona(ZonaPuzzle zona)
    {
        if (zona == ZonaPuzzle.Panela && !brocolisNaPanela)
            SetOutline(outlineBowlBrocolis, 0f);

        if (zona == ZonaPuzzle.BowlYakisoba && !puzzleCompleto)
            SetOutline(outlinePanela, 0f);
    }

    public void PermaneceuNaZona(ZonaPuzzle zona)
    {
        if (zona == ZonaPuzzle.Panela)
        {
            TentarDerramarBrocolis();
            return;
        }

        if (zona == ZonaPuzzle.BowlYakisoba)
            TentarFinalizarYakisoba();
    }

    private void TentarDerramarBrocolis()
    {
        if (!pegouBrocolis || brocolisNaPanela || !EstaVirado(bowlBrocolis))
            return;

        brocolisNaPanela = true;
        SetOutline(outlineBowlBrocolis, 0f);
        EsconderConteudoBrocolis();
        Debug.Log("Brocolis derramado na PanelaYakisoba.");
    }

    private void TentarFinalizarYakisoba()
    {
        if (!yakisobaCozido || !pegouPanelaCozida || puzzleCompleto || !EstaVirado(panela))
            return;

        puzzleCompleto = true;
        SetOutline(outlinePanela, 0f);
        DestravarPorta();
        Debug.Log("Yakisoba pronto. DoorV6 (1) destrancada.");
    }

    private void EsconderConteudoBrocolis()
    {
        Transform grupoBrocolis = EncontrarFilhoPorNome(bowlBrocolis.transform, "Brocolis");
        if (grupoBrocolis == null)
        {
            Debug.LogWarning("YakisobaPuzzleManager: nao encontrou o filho Brocolis dentro de BowlBrocolis.");
            return;
        }

        foreach (Transform filho in grupoBrocolis)
        {
            foreach (var renderer in filho.GetComponentsInChildren<Renderer>(true))
                renderer.enabled = false;
        }
    }

    private Transform EncontrarFilhoPorNome(Transform raiz, string nome)
    {
        foreach (Transform filho in raiz.GetComponentsInChildren<Transform>(true))
        {
            if (filho.name == nome)
                return filho;
        }

        return null;
    }

    private bool EstaVirado(GameObject obj)
    {
        if (obj == null)
            return false;

        return Vector3.Angle(obj.transform.up, Vector3.up) >= anguloDerramar;
    }

    private void TravarPorta()
    {
        if (grabPorta != null)
            grabPorta.enabled = false;

        if (rbPorta == null)
            return;

        constraintsOriginaisPorta = rbPorta.constraints;
        rbPorta.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void DestravarPorta()
    {
        if (grabPorta != null)
            grabPorta.enabled = true;

        if (rbPorta != null)
            rbPorta.constraints = constraintsOriginaisPorta;

        SetOutline(outlinePorta, 5f);
    }

    private bool ValidarReferencias()
    {
        bool valido = true;
        valido &= AvisarSeNulo(bowlBrocolis, BowlBrocolisName);
        valido &= AvisarSeNulo(panela, PanelaName);
        valido &= AvisarSeNulo(bowlYakisoba, BowlYakisobaName);
        valido &= AvisarSeNulo(porta, DoorName);
        valido &= AvisarSeNulo(fogao, nameof(FogaoController));
        return valido;
    }

    private FogaoController BuscarFogao()
    {
        var botaoFogao = GameObject.Find(BotaoFogaoName);
        if (botaoFogao != null && botaoFogao.TryGetComponent(out FogaoController controller))
            return controller;

        return FindAnyObjectByType<FogaoController>();
    }

    private bool AvisarSeNulo(Object obj, string nome)
    {
        if (obj != null)
            return true;

        Debug.LogWarning("YakisobaPuzzleManager: nao encontrou " + nome + " na cena.");
        return false;
    }

    private Outline PrepararOutline(GameObject alvo, Color cor)
    {
        var outline = alvo.GetComponent<Outline>();
        if (outline == null)
            outline = alvo.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = cor;
        outline.OutlineWidth = 0f;
        return outline;
    }

    private void SetOutline(Outline outline, float largura)
    {
        if (outline != null)
            outline.OutlineWidth = largura;
    }

    private void CriarZonaDerramar(GameObject alvo, float raio, ZonaPuzzle zona)
    {
        var zonaObj = new GameObject("ZonaDerramar_" + zona);
        zonaObj.transform.SetParent(alvo.transform, false);
        zonaObj.transform.localPosition = Vector3.up * 0.08f;

        var collider = zonaObj.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = raio;

        var monitor = zonaObj.AddComponent<YakisobaPourZone>();
        monitor.Configurar(this, zona, zona == ZonaPuzzle.Panela ? bowlBrocolis.transform : panela.transform);
    }
}

public enum ZonaPuzzle
{
    Panela,
    BowlYakisoba
}

public class YakisobaPourZone : MonoBehaviour
{
    private YakisobaPuzzleManager manager;
    private ZonaPuzzle zona;
    private Transform objetoEsperado;
    private int contatos;

    public void Configurar(YakisobaPuzzleManager puzzleManager, ZonaPuzzle tipoZona, Transform alvo)
    {
        manager = puzzleManager;
        zona = tipoZona;
        objetoEsperado = alvo;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!EhObjetoEsperado(other))
            return;

        contatos++;
        manager.EntrouNaZona(zona);
    }

    private void OnTriggerStay(Collider other)
    {
        if (EhObjetoEsperado(other))
            manager.PermaneceuNaZona(zona);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!EhObjetoEsperado(other))
            return;

        contatos = Mathf.Max(0, contatos - 1);
        if (contatos == 0)
            manager.SaiuDaZona(zona);
    }

    private bool EhObjetoEsperado(Collider other)
    {
        if (objetoEsperado == null)
            return false;

        return other.transform == objetoEsperado || other.transform.IsChildOf(objetoEsperado);
    }
}
