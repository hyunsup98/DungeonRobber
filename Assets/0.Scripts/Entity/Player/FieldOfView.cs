using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("시야각 적 탐지 변수")]
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float viewInnerRadius = 3f;
    [SerializeField] private float viewDistance = 22f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("시야각 쉐이더 변수")]
    [SerializeField] private Transform player;
    [SerializeField] private Material maskMaterial;

    [SerializeField] private Transform fovQuad;

    private void Start()
    {
        InitFOVShader();
    }

    private void Update()
    {
        InitFOVShader();
        SetFOVShader();
    }

    private void FixedUpdate()
    {
        FindVisibleTargets();
    }

    private void FindVisibleTargets()
    {
        Collider[] targetCol = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        foreach (var target in targetCol)
        {
            Vector3 dirToTarget = target.transform.position - transform.position;
            float angleBetween = Vector3.Angle(transform.forward, dirToTarget.normalized);

            if (angleBetween < viewAngle * 0.5f || dirToTarget.sqrMagnitude <= viewInnerRadius * viewInnerRadius)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, viewDistance, obstacleMask))
                {
                    SetVisible(target.transform, true);
                    continue;
                }
            }

            SetVisible(target.transform, false);
        }
    }

    private void SetVisible(Transform target, bool visible)
    {
        Renderer[] renderes = target.GetComponentsInChildren<Renderer>();
        foreach (var render in renderes)
        {
            render.enabled = visible;
        }
    }

    private void InitFOVShader()
    {
        maskMaterial.SetFloat("_InnerRadius", viewInnerRadius);
        maskMaterial.SetFloat("_OuterRadius", viewDistance);
        maskMaterial.SetFloat("_FOV", viewAngle);

        Vector3 playerToQuad = transform.position - fovQuad.position;
        maskMaterial.SetVector("_PlayerOffset", playerToQuad);
    }

    private void SetFOVShader()
    {
        if (player == null || maskMaterial == null) return;

        maskMaterial.SetVector("_PlayerPos", player.position);
        maskMaterial.SetVector("_PlayerForward", player.forward);
    }

    //public void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(transform.position, viewInnerRadius);
    //}
}
