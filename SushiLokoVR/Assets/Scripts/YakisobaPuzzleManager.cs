using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class YakisobaPuzzleManager : MonoBehaviour
{
    private const string BowlBrocolisName = "BowlBrocolis";
    private const string PanelaName = "PanelaYakisoba";
    private const string BowlYakisobaName = "BowlYakisoba";
    private const string DoorName = "DoorV6 (1)";
    private const string BotaoFogaoName = "Push Button Fogao";
    private const string MesaPedidoName = "MesaPedido";
    private const string VisualPanelaInicialName = "YakisobaPanelaInicial";
    private const string VisualPanelaComBrocolisName = "YakisobaPanelaComBrocolis";
    private const string VisualBowlFinalName = "YakisobaBowlFinal";

    [Header("Regras do puzzle")]
    [SerializeField] private float anguloDerramar = 100f;
    [SerializeField] private float tempoCozimento = 10f;
    [SerializeField] private float raioZonaPanela = 0.22f;
    [SerializeField] private float raioZonaBowl = 0.22f;
    [SerializeField] private float tempoAntesDeEncerrar = 3f;

    [Header("Visuais editaveis")]
    [SerializeField] private GameObject yakisobaPanelaInicial;
    [SerializeField] private GameObject yakisobaPanelaComBrocolis;
    [SerializeField] private GameObject yakisobaBowlFinal;
    [SerializeField] private bool criarVisualPadraoSeNaoConfigurar = true;

    [Header("Fallback visual gerado por codigo")]
    [SerializeField] private float alturaConteudoPanela = 0.09f;
    [SerializeField] private float alturaConteudoBowl = 0.08f;

    private GameObject bowlBrocolis;
    private GameObject panela;
    private GameObject bowlYakisoba;
    private GameObject porta;
    private GameObject mesaPedido;
    private GameObject mensagemSucesso;
    private GameObject conteudoPanela;
    private GameObject brocolisNaPanelaVisual;
    private GameObject conteudoBowlYakisoba;

    private Outline outlineBowlBrocolis;
    private Outline outlinePanela;
    private Outline outlinePorta;
    private Material materialCaldo;
    private Material materialMacarrao;
    private FogaoController fogao;
    private XRGrabInteractable grabBowlBrocolis;
    private XRGrabInteractable grabPanela;
    private XRGrabInteractable grabBowlYakisoba;
    private XRGrabInteractable grabPorta;
    private Rigidbody rbPorta;
    private RigidbodyConstraints constraintsOriginaisPorta;

    private bool pegouBrocolis;
    private bool brocolisNaPanela;
    private bool yakisobaCozido;
    private bool pegouPanelaCozida;
    private bool puzzleCompleto;
    private bool pedidoEntregue;
    private bool segurandoBowlYakisoba;
    private bool bowlYakisobaNaMesaPedido;
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
        mesaPedido = GameObject.Find(MesaPedidoName);
        fogao = BuscarFogao();

        if (!ValidarReferencias())
            return;

        outlineBowlBrocolis = PrepararOutline(bowlBrocolis, new Color(0.1f, 1f, 0.25f, 1f));
        outlinePanela = PrepararOutline(panela, new Color(0f, 0.75f, 1f, 1f));
        outlinePorta = PrepararOutline(porta, new Color(1f, 0.75f, 0.05f, 1f));
        ConfigurarConteudosVisuais();

        grabBowlBrocolis = bowlBrocolis.GetComponent<XRGrabInteractable>();
        grabPanela = panela.GetComponent<XRGrabInteractable>();
        grabBowlYakisoba = bowlYakisoba.GetComponent<XRGrabInteractable>();
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

        if (grabBowlYakisoba != null)
        {
            grabBowlYakisoba.selectEntered.AddListener(_ => segurandoBowlYakisoba = true);
            grabBowlYakisoba.selectExited.AddListener(_ => TentarEntregarPedidoAposSoltar());
        }

        TravarPorta();
        CriarZonaDerramar(panela, raioZonaPanela, ZonaPuzzle.Panela);
        CriarZonaDerramar(bowlYakisoba, raioZonaBowl, ZonaPuzzle.BowlYakisoba);
        CriarZonaEntregaPedido();
        CriarMensagemSucesso();
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
        MostrarBrocolisNaPanela();
        Debug.Log("Brocolis derramado na PanelaYakisoba.");
    }

    private void TentarFinalizarYakisoba()
    {
        if (!yakisobaCozido || !pegouPanelaCozida || puzzleCompleto || !EstaVirado(panela))
            return;

        puzzleCompleto = true;
        SetOutline(outlinePanela, 0f);
        TransferirConteudoParaBowlYakisoba();
        DestravarPorta();
        Debug.Log("Yakisoba pronto. DoorV6 (1) destrancada.");
    }

    public bool EhBowlYakisoba(Collider outro)
    {
        return EhColliderDoObjeto(outro, bowlYakisoba.transform);
    }

    public void DefinirBowlYakisobaNaMesaPedido(bool estaNaMesa)
    {
        bowlYakisobaNaMesaPedido = estaNaMesa;
    }

    public void TentarEntregarPedidoNaMesa(Collider outro)
    {
        if (!EhBowlYakisoba(outro))
            return;

        TentarEntregarPedidoAposSoltar();
    }

    private void TentarEntregarPedidoAposSoltar()
    {
        segurandoBowlYakisoba = grabBowlYakisoba != null && grabBowlYakisoba.isSelected;

        if (pedidoEntregue || !puzzleCompleto || !bowlYakisobaNaMesaPedido || segurandoBowlYakisoba)
            return;

        pedidoEntregue = true;
        Debug.Log("Pedido entregue com sucesso.");
        StartCoroutine(EncerrarJogo());
    }

    private IEnumerator EncerrarJogo()
    {
        if (mensagemSucesso != null)
            mensagemSucesso.SetActive(true);

        yield return new WaitForSeconds(tempoAntesDeEncerrar);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ConfigurarConteudosVisuais()
    {
        yakisobaPanelaInicial = ResolverVisualEditavel(yakisobaPanelaInicial, VisualPanelaInicialName);
        yakisobaPanelaComBrocolis = ResolverVisualEditavel(yakisobaPanelaComBrocolis, VisualPanelaComBrocolisName);
        yakisobaBowlFinal = ResolverVisualEditavel(yakisobaBowlFinal, VisualBowlFinalName);

        bool temVisualEditavel = yakisobaPanelaInicial != null
            || yakisobaPanelaComBrocolis != null
            || yakisobaBowlFinal != null;

        if (!temVisualEditavel && criarVisualPadraoSeNaoConfigurar)
        {
            CriarConteudosVisuaisPadrao();
            return;
        }

        SetActiveSeExiste(yakisobaPanelaInicial, true);
        SetActiveSeExiste(yakisobaPanelaComBrocolis, false);
        SetActiveSeExiste(yakisobaBowlFinal, false);
    }

    private GameObject ResolverVisualEditavel(GameObject visual, string nomePadrao)
    {
        if (visual != null)
            return visual;

        return BuscarObjetoNaCenaPorNome(nomePadrao);
    }

    private GameObject BuscarObjetoNaCenaPorNome(string nome)
    {
        foreach (var raiz in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var transformEncontrado in raiz.GetComponentsInChildren<Transform>(true))
            {
                if (transformEncontrado.name == nome)
                    return transformEncontrado.gameObject;
            }
        }

        return null;
    }

    private void SetActiveSeExiste(GameObject obj, bool ativo)
    {
        if (obj != null)
            obj.SetActive(ativo);
    }

    private void CriarConteudosVisuaisPadrao()
    {
        materialCaldo = CriarMaterial("MaterialCaldoYakisoba", new Color(0.92f, 0.48f, 0.16f, 0.95f));
        materialMacarrao = CriarMaterial("MaterialMacarraoYakisoba", new Color(1f, 0.82f, 0.42f, 1f));

        conteudoPanela = CriarGrupoConteudo(panela.transform, "ConteudoVisualPanelaYakisoba");
        CriarCaldo(conteudoPanela.transform, "CaldoPanela", alturaConteudoPanela, 0.2f);
        CriarMacarrao(conteudoPanela.transform, alturaConteudoPanela + 0.01f, 0.2f);

        brocolisNaPanelaVisual = new GameObject("BrocolisVisualPanela");
        brocolisNaPanelaVisual.transform.SetParent(conteudoPanela.transform, false);
        CriarClonesBrocolis(brocolisNaPanelaVisual.transform, alturaConteudoPanela + 0.025f);
        brocolisNaPanelaVisual.SetActive(false);

        conteudoBowlYakisoba = CriarGrupoConteudo(bowlYakisoba.transform, "ConteudoVisualBowlYakisoba");
        CriarCaldo(conteudoBowlYakisoba.transform, "CaldoBowlYakisoba", alturaConteudoBowl, 0.18f);
        CriarMacarrao(conteudoBowlYakisoba.transform, alturaConteudoBowl + 0.01f, 0.18f);
        CriarClonesBrocolis(conteudoBowlYakisoba.transform, alturaConteudoBowl + 0.025f);
        conteudoBowlYakisoba.SetActive(false);
    }

    private GameObject CriarGrupoConteudo(Transform pai, string nome)
    {
        var grupo = new GameObject(nome);
        grupo.transform.SetParent(pai, false);
        grupo.transform.localPosition = Vector3.zero;
        grupo.transform.localRotation = Quaternion.identity;
        grupo.transform.localScale = Vector3.one;
        return grupo;
    }

    private Material CriarMaterial(string nome, Color cor)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        var material = new Material(shader);
        material.name = nome;
        material.color = cor;
        return material;
    }

    private void CriarCaldo(Transform pai, string nome, float altura, float raio)
    {
        var caldo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        caldo.name = nome;
        caldo.transform.SetParent(pai, false);
        caldo.transform.localPosition = Vector3.up * altura;
        caldo.transform.localRotation = Quaternion.identity;
        caldo.transform.localScale = new Vector3(raio, 0.01f, raio);

        var collider = caldo.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);

        caldo.GetComponent<Renderer>().material = materialCaldo;
    }

    private void CriarMacarrao(Transform pai, float altura, float raio)
    {
        for (int i = 0; i < 6; i++)
        {
            var fio = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fio.name = "MacarraoVisual";
            fio.transform.SetParent(pai, false);

            float angulo = i * 60f;
            float distancia = i % 2 == 0 ? raio * 0.22f : raio * 0.42f;
            fio.transform.localPosition = new Vector3(
                Mathf.Cos(angulo * Mathf.Deg2Rad) * distancia,
                altura,
                Mathf.Sin(angulo * Mathf.Deg2Rad) * distancia);
            fio.transform.localRotation = Quaternion.Euler(0f, angulo + 20f, 0f);
            fio.transform.localScale = new Vector3(raio * 0.55f, 0.006f, raio * 0.04f);

            var collider = fio.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            fio.GetComponent<Renderer>().material = materialMacarrao;
        }
    }

    private void CriarClonesBrocolis(Transform pai, float altura)
    {
        Transform grupoBrocolis = EncontrarFilhoPorNome(bowlBrocolis.transform, "Brocolis");
        if (grupoBrocolis == null)
            return;

        int index = 0;
        foreach (Transform filho in grupoBrocolis)
        {
            var clone = Instantiate(filho.gameObject, pai);
            clone.name = filho.name + "_visual";
            clone.transform.localPosition = PosicaoBrocolisVisual(index, altura);
            clone.transform.localRotation = Quaternion.Euler(0f, index * 73f, 0f);
            clone.transform.localScale = filho.localScale;
            RemoverComponentesInterativos(clone);
            index++;
        }
    }

    private Vector3 PosicaoBrocolisVisual(int index, float altura)
    {
        Vector2[] posicoes =
        {
            new Vector2(-0.045f, 0.025f),
            new Vector2(0.04f, 0.035f),
            new Vector2(-0.01f, -0.035f),
            new Vector2(0.055f, -0.025f),
            new Vector2(-0.055f, -0.01f)
        };

        Vector2 posicao = posicoes[index % posicoes.Length];
        return new Vector3(posicao.x, altura, posicao.y);
    }

    private void RemoverComponentesInterativos(GameObject obj)
    {
        foreach (var collider in obj.GetComponentsInChildren<Collider>(true))
            Destroy(collider);

        foreach (var rigidbody in obj.GetComponentsInChildren<Rigidbody>(true))
            Destroy(rigidbody);

        foreach (var interactable in obj.GetComponentsInChildren<XRBaseInteractable>(true))
            Destroy(interactable);

        foreach (var outline in obj.GetComponentsInChildren<Outline>(true))
            Destroy(outline);
    }

    private void MostrarBrocolisNaPanela()
    {
        if (yakisobaPanelaComBrocolis != null)
        {
            SetActiveSeExiste(yakisobaPanelaInicial, false);
            yakisobaPanelaComBrocolis.SetActive(true);
            return;
        }

        if (brocolisNaPanelaVisual != null)
            brocolisNaPanelaVisual.SetActive(true);
    }

    private void TransferirConteudoParaBowlYakisoba()
    {
        SetActiveSeExiste(yakisobaPanelaInicial, false);
        SetActiveSeExiste(yakisobaPanelaComBrocolis, false);
        SetActiveSeExiste(yakisobaBowlFinal, true);

        if (conteudoPanela != null)
            conteudoPanela.SetActive(false);

        if (conteudoBowlYakisoba != null && yakisobaBowlFinal == null)
            conteudoBowlYakisoba.SetActive(true);
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
        valido &= AvisarSeNulo(mesaPedido, MesaPedidoName);
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

    private void CriarZonaEntregaPedido()
    {
        Bounds bounds = CalcularBounds(mesaPedido);
        var zonaObj = new GameObject("ZonaEntregaPedido");
        zonaObj.transform.position = new Vector3(bounds.center.x, bounds.max.y + 0.12f, bounds.center.z);
        zonaObj.transform.rotation = Quaternion.identity;

        var collider = zonaObj.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(
            Mathf.Max(bounds.size.x + 0.25f, 0.6f),
            0.24f,
            Mathf.Max(bounds.size.z + 0.25f, 0.6f));

        var monitor = zonaObj.AddComponent<YakisobaDeliveryZone>();
        monitor.Configurar(this);
    }

    private Bounds CalcularBounds(GameObject alvo)
    {
        var renderers = alvo.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
            return new Bounds(alvo.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return bounds;
    }

    private void CriarMensagemSucesso()
    {
        Transform cameraTransform = Camera.main != null ? Camera.main.transform : null;
        if (cameraTransform == null)
            return;

        var canvasObj = new GameObject("MensagemSucessoPedido");
        canvasObj.transform.SetParent(cameraTransform, false);
        canvasObj.transform.localPosition = new Vector3(0f, 0f, 0.55f);
        canvasObj.transform.localRotation = Quaternion.identity;
        canvasObj.transform.localScale = Vector3.one * 0.0008f;

        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;
        canvasObj.AddComponent<GraphicRaycaster>();

        var rect = canvas.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(900f, 260f);

        var fundoObj = new GameObject("Fundo");
        fundoObj.transform.SetParent(canvasObj.transform, false);
        var fundo = fundoObj.AddComponent<Image>();
        fundo.color = new Color(0.02f, 0.08f, 0.05f, 0.88f);
        var fundoRect = fundo.GetComponent<RectTransform>();
        fundoRect.anchorMin = new Vector2(0.5f, 0.5f);
        fundoRect.anchorMax = new Vector2(0.5f, 0.5f);
        fundoRect.anchoredPosition = Vector2.zero;
        fundoRect.sizeDelta = new Vector2(900f, 260f);

        var textoObj = new GameObject("Texto");
        textoObj.transform.SetParent(canvasObj.transform, false);
        var texto = textoObj.AddComponent<TextMeshProUGUI>();
        texto.text = "Pedido entregue!\nYakisoba finalizado com sucesso.";
        texto.fontSize = 54f;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = Color.white;
        var textoRect = texto.GetComponent<RectTransform>();
        textoRect.anchorMin = new Vector2(0.5f, 0.5f);
        textoRect.anchorMax = new Vector2(0.5f, 0.5f);
        textoRect.anchoredPosition = Vector2.zero;
        textoRect.sizeDelta = new Vector2(820f, 200f);

        mensagemSucesso = canvasObj;
        mensagemSucesso.SetActive(false);
    }

    private bool EhColliderDoObjeto(Collider collider, Transform objeto)
    {
        if (collider == null || objeto == null)
            return false;

        return collider.transform == objeto || collider.transform.IsChildOf(objeto);
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

public class YakisobaDeliveryZone : MonoBehaviour
{
    private YakisobaPuzzleManager manager;
    private int contatosBowlYakisoba;

    public void Configurar(YakisobaPuzzleManager puzzleManager)
    {
        manager = puzzleManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!manager.EhBowlYakisoba(other))
            return;

        contatosBowlYakisoba++;
        manager.DefinirBowlYakisobaNaMesaPedido(true);
        manager.TentarEntregarPedidoNaMesa(other);
    }

    private void OnTriggerStay(Collider other)
    {
        manager.TentarEntregarPedidoNaMesa(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!manager.EhBowlYakisoba(other))
            return;

        contatosBowlYakisoba = Mathf.Max(0, contatosBowlYakisoba - 1);
        if (contatosBowlYakisoba == 0)
            manager.DefinirBowlYakisobaNaMesaPedido(false);
    }
}
