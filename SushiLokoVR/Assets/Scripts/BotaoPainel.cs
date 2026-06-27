using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BotaoPainel : MonoBehaviour
{
    public GameObject tela;

    void Start()
    {
        if (tela != null)
            tela.SetActive(false);

        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => AlternarPainel());
    }

    public void AlternarPainel()
    {
        if (tela != null)
            tela.SetActive(!tela.activeSelf);
    }
}
