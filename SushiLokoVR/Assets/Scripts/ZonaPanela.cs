using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class ZonaPanela : MonoBehaviour
{
    private PuzzleCozinha puzzle;

    // Brócolis que estão fisicamente dentro do trigger
    private readonly HashSet<GameObject> naZona = new();
    // Brócolis confirmados como depositados (não segurados)
    private readonly HashSet<GameObject> depositados = new();
    private bool bowlBrocolisDerramado;

    public void SetPuzzle(PuzzleCozinha p) => puzzle = p;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (EhBowlBrocolis(other.gameObject))
        {
            VerificarDerramamentoBowl(other);
            return;
        }

        if (!EhBrocoli(other.gameObject)) return;

        naZona.Add(other.gameObject);

        // Aguarda o jogador soltar o brócolis antes de contar
        var grab = other.GetComponentInParent<XRGrabInteractable>()
                ?? other.GetComponent<XRGrabInteractable>();
        if (grab != null && grab.isSelected) return;

        if (depositados.Add(other.gameObject))
            puzzle?.BrocoliEntrou();
    }

    void OnTriggerExit(Collider other)
    {
        if (EhBowlBrocolis(other.gameObject)) return;

        if (!EhBrocoli(other.gameObject)) return;

        naZona.Remove(other.gameObject);
        if (depositados.Remove(other.gameObject))
            puzzle?.BrocoliSaiu();
    }

    static bool EhBrocoli(GameObject go) =>
        go.name.ToLower().Contains("brocoli");

    void VerificarDerramamentoBowl(Collider other)
    {
        if (bowlBrocolisDerramado) return;

        var grab = other.GetComponentInParent<XRGrabInteractable>()
                ?? other.GetComponent<XRGrabInteractable>();
        if (grab == null || !grab.isSelected) return;

        bool inclinado = Vector3.Dot(grab.transform.up, Vector3.down) > 0f;
        if (!inclinado) return;

        bowlBrocolisDerramado = true;
        puzzle?.BrocolisDerramadosDoBowl();
    }

    static bool EhBowlBrocolis(GameObject go)
    {
        var atual = go.transform;
        while (atual != null)
        {
            var nome = atual.name.ToLower();
            if (nome.Contains("bowl") && nome.Contains("brocoli"))
                return true;

            atual = atual.parent;
        }

        return false;
    }
}
