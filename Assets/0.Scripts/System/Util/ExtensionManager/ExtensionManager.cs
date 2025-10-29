using UnityEngine;

/// <summary>
/// Ȯ�� �޼��带 �����ϴ� Ŭ����
/// </summary>
public static class ExtensionManager
{
    #region Camera
    /// <summary>
    /// ��ũ�� ���� ���콺 ��ǥ�� ���� ��ǥ�� ��ȯ�ϴ� Ȯ�� �޼���
    /// </summary>
    /// <param name="mainCamera"> Ȯ���ų Ÿ�� </param>
    /// <param name="position"> ��ǥ�� x, y�� ���ϱ� ���� ���̸� ���Ͻ�ų Ʈ������ </param>
    /// <returns></returns>
    public static Vector3 GetMouseWorldPos(this Camera mainCamera)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.transform.position.y;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
    #endregion
}