using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ڷ�ƾ���� ���� ����ϴ� yield return �������� ����� �͵��� �̸� ĳ���ؼ� �����ϴ� Ŭ����
/// </summary>
public static class CoroutineManager
{
    private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>();
    private static readonly Dictionary<float, WaitForSecondsRealtime> _waitForSecondsRealtime = new Dictionary<float, WaitForSecondsRealtime>();

    public static WaitForSeconds waitForSeconds(float seconds)
    {
        //ĳ���� WaitForSeconds�߰� => ���ڷ� ���� seconds�� _waitForSeconds�� ���ٸ� �߰�����
        if(!_waitForSeconds.TryGetValue(seconds, out var _seconds))
        {
            _waitForSeconds.Add(seconds, _seconds = new WaitForSeconds(seconds));
        }

        return _seconds;
    }

    public static WaitForSecondsRealtime waitForSecondsRealtime(float seconds)
    {
        if(!_waitForSecondsRealtime.TryGetValue(seconds, out var _seconds))
        {
            _waitForSecondsRealtime.Add(seconds, _seconds = new WaitForSecondsRealtime(seconds));
        }

        return _seconds;
    }
}
