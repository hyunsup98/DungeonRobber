using UnityEngine;

/// <summary>
/// Ȯ�� �޼��带 ��Ƶδ� Ŭ����
/// </summary>
public static class Extension
{
    #region Camera
    /// <summary>
    /// ���� ȭ���� ���콺 ������ ��ǥ�� ������ǥ�� ��ȯ �� ��ȯ�ϴ� �޼���
    /// </summary>
    /// <returns> ���콺 ��ǥ�� ������ǥ ���� </returns>
    public static Vector3 GetWorldPosToMouse(this Camera mainCamera, Transform transform)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.transform.position.y - transform.position.y;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
    #endregion
}
