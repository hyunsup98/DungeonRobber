using UnityEngine;

/// <summary>
/// 확장 메서드를 관리하는 클래스
/// </summary>
public static class ExtensionManager
{
    #region Camera
    /// <summary>
    /// 스크린 상의 마우스 좌표를 월드 좌표로 반환하는 확장 메서드
    /// </summary>
    /// <param name="mainCamera"> 확장시킬 타입 </param>
    /// <param name="position"> 좌표의 x, y만 구하기 위해 높이를 통일시킬 트랜스폼 </param>
    /// <returns></returns>
    public static Vector3 GetMouseWorldPos(this Camera mainCamera)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.transform.position.y;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
    #endregion
}