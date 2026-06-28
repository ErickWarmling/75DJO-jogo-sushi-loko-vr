using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabLineOfSightFilter : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
{
    private const string ObstacleLayerName = "Obstaculos";

    [SerializeField] private LayerMask blockingLayers;
    [SerializeField] private float originOffset = 0.03f;

    public bool canProcess => isActiveAndEnabled;

    private XRGrabInteractable grabInteractable;
    private bool selectFilterRegistered;
    private bool hoverFilterRegistered;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InstallInScene()
    {
        foreach (var grab in FindObjectsByType<XRGrabInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (grab.GetComponent<GrabLineOfSightFilter>() == null)
                grab.gameObject.AddComponent<GrabLineOfSightFilter>();
        }
    }

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        int obstacleLayer = LayerMask.NameToLayer(ObstacleLayerName);
        if (obstacleLayer >= 0 && blockingLayers.value == 0)
            blockingLayers = 1 << obstacleLayer;
    }

    private void OnEnable()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null && !selectFilterRegistered)
        {
            grabInteractable.selectFilters.Add(this);
            selectFilterRegistered = true;
        }

        if (grabInteractable != null && !hoverFilterRegistered)
        {
            grabInteractable.hoverFilters.Add(this);
            hoverFilterRegistered = true;
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectFilters.Remove(this);
            grabInteractable.hoverFilters.Remove(this);
            selectFilterRegistered = false;
            hoverFilterRegistered = false;
        }
    }

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        return HasClearPath(interactor, interactable);
    }

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        return HasClearPath(interactor, interactable);
    }

    private bool HasClearPath(object interactor, object interactable)
    {
        if (blockingLayers.value == 0)
            return true;

        Transform origin = GetInteractorTransform(interactor);
        Transform target = GetInteractableTransform(interactable);

        if (origin == null || target == null)
            return true;

        Vector3 start = origin.position;
        Vector3 end = GetTargetPoint(target);
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        if (distance <= Mathf.Epsilon)
            return true;

        direction /= distance;
        start += direction * originOffset;
        float rayDistance = distance - originOffset;

        if (rayDistance <= 0f)
            return true;

        if (!Physics.Raycast(start, direction, out RaycastHit hit, rayDistance, blockingLayers, QueryTriggerInteraction.Ignore))
            return true;

        return HitBelongsToInteractable(hit.transform, target);
    }

    private Transform GetInteractorTransform(object interactor)
    {
        if (interactor is Component component)
            return component.transform;

        return null;
    }

    private Transform GetInteractableTransform(object interactable)
    {
        if (interactable is Component component)
            return component.transform;

        return null;
    }

    private Vector3 GetTargetPoint(Transform target)
    {
        var collider = target.GetComponentInChildren<Collider>();
        if (collider != null)
            return collider.bounds.center;

        var renderer = target.GetComponentInChildren<Renderer>();
        if (renderer != null)
            return renderer.bounds.center;

        return target.position;
    }

    private bool HitBelongsToInteractable(Transform hit, Transform target)
    {
        if (hit == target || hit.IsChildOf(target) || target.IsChildOf(hit))
            return true;

        if (BelongsToConnectedBody(hit, target))
            return true;

        return IsDoorLike(hit) && IsDoorLike(target) && hit.root == target.root;
    }

    private bool BelongsToConnectedBody(Transform hit, Transform target)
    {
        foreach (var joint in target.GetComponentsInChildren<Joint>(true))
        {
            if (joint.connectedBody == null)
                continue;

            Transform connected = joint.connectedBody.transform;
            if (hit == connected || hit.IsChildOf(connected) || connected.IsChildOf(hit))
                return true;
        }

        return false;
    }

    private bool IsDoorLike(Transform transform)
    {
        string name = GetHierarchyName(transform);
        return name.Contains("Door")
            || name.Contains("Porta")
            || name.Contains("Trinco")
            || name.Contains("Espelho")
            || name.Contains("Hanger")
            || name.Contains("BarnDoor");
    }

    private string GetHierarchyName(Transform transform)
    {
        string name = transform.name;
        Transform parent = transform.parent;

        while (parent != null)
        {
            name = parent.name + "/" + name;
            parent = parent.parent;
        }

        return name;
    }
}
