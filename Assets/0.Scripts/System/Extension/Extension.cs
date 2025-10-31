using UnityEngine;

/// <summary>
/// 확장 메서드를 모아두는 클래스
/// </summary>
public static class Extension
{
    #region Camera
    /// <summary>
    /// 게임 화면의 마우스 포인터 좌표를 월드좌표로 변환 후 반환하는 메서드
    /// </summary>
    /// <returns> 마우스 좌표의 월드좌표 벡터 </returns>
    public static Vector3 GetWorldPosToMouse(this Camera mainCamera, Transform transform)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.transform.position.y - transform.position.y;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
    #endregion
}
