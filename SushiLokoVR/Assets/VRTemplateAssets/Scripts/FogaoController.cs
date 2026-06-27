using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class FogaoController : MonoBehaviour
{
    [SerializeField] private GameObject indicadorBocaLigada;

    private bool ligado = false;

    void Awake()
    {
        GetComponent<XRSimpleInteractable>().firstSelectEntered.AddListener(x => AlternarEstado());

        if (indicadorBocaLigada != null)
            indicadorBocaLigada.SetActive(false);
    }

    void AlternarEstado()
    {
        ligado = !ligado;

        if (indicadorBocaLigada != null)
            indicadorBocaLigada.SetActive(ligado);

        Debug.Log("Fogão " + (ligado ? "LIGADO" : "DESLIGADO"));
    }
}
