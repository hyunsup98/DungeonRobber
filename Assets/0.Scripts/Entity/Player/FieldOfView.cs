using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("�þ߰� �� Ž�� ����")]
    [SerializeField] private float viewAngle = 90f;         //�þ߰�
    [SerializeField] private float viewInnerRadius = 3f;    //�÷��̾� �ֺ� �þ� ����
    [SerializeField] private float viewDistance = 22f;      //��ä�� �þ߰��� �ִ� �þ� �Ÿ�
    [SerializeField] private LayerMask targetMask;          //��(Enemy) ���̾��ũ
    [SerializeField] private LayerMask obstacleMask;        //��ֹ�, �� ���̾��ũ

    [Header("�þ߰� ���̴� ����")]
    [SerializeField] private Transform player;              //�÷��̾� ��ġ ����
    [SerializeField] private Material maskMaterial;         //�þ� ���׸���
    [Range(1f, 15f)]
    [SerializeField] private float fovRotateSpeed = 10f;    //���콺 �����͸� ���� �� ���� ��ġ

    [Header("Quad(Panel Ʈ������)")]
    [SerializeField] private Transform fovQuad;             //ī�޶� �پ��ִ� ����(�ǳ�) ��ġ ����

    Vector3 lookDir = Vector3.zero;                         //�þ߰��� �ٶ󺸰� �ִ� ���� ����

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

        lookDir = Vector3.Slerp(lookDir, (CameraController.Instance.GetMousePos() - transform.position).normalized, fovRotateSpeed * Time.deltaTime);

        maskMaterial.SetVector("_PlayerPos", player.position);
        maskMaterial.SetVector("_PlayerForward", lookDir);
    }
}
