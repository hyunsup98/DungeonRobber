using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("시야각 적 탐지 변수")]
    [SerializeField] private float viewAngle = 90f;         //시야각
    [SerializeField] private float viewInnerRadius = 3f;    //플레이어 주변 시야 범위
    [SerializeField] private float viewDistance = 22f;      //부채꼴 시야각의 최대 시야 거리
    [SerializeField] private LayerMask targetMask;          //적(Enemy) 레이어마스크
    [SerializeField] private LayerMask obstacleMask;        //장애물, 벽 레이어마스크

    [Header("시야각 쉐이더 변수")]
    [SerializeField] private Transform player;              //플레이어 위치 정보
    [SerializeField] private Material maskMaterial;         //시야 메테리얼
    [Range(1f, 15f)]
    [SerializeField] private float fovRotateSpeed = 10f;    //마우스 포인터를 돌릴 때 보간 수치

    [Header("Quad(Panel 트랜스폼)")]
    [SerializeField] private Transform fovQuad;             //카메라에 붙어있는 쿼드(판넬) 위치 정보

    Vector3 lookDir = Vector3.zero;                         //시야각이 바라보고 있는 방향 벡터

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
            Vector3 dirToMousePoint = Camera.main.GetWorldPosToMouse() - transform.position;
            Vector3 dirToTarget = target.transform.position - transform.position;
            float angleBetween = Vector3.Angle(dirToMousePoint.normalized, dirToTarget.normalized);

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

        lookDir = Vector3.Slerp(lookDir, (Camera.main.GetWorldPosToMouse() - transform.position).normalized, fovRotateSpeed * Time.deltaTime);

        maskMaterial.SetVector("_PlayerPos", player.position);
        maskMaterial.SetVector("_PlayerForward", lookDir);
    }
}
