using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 은신 효과 버프 - 플레이어를 투명화하고 적에게 안 보이게 함
/// </summary>
public class StealthBuff : BaseBuff
{
    private Renderer[] renderers;
    private GameObject playerObject;

    public override void OnActivate(List<BaseBuff> list, BaseStat stat)
    {
        // 플레이어 오브젝트 찾기
        if (playerObject == null)
        {
            // Player_Controller 찾기
            Player_Controller player = Player_Controller.Instance;
            if (player == null)
                player = FindObjectOfType<Player_Controller>();
            
            if (player != null)
            {
                playerObject = player.gameObject;
            }
        }

        if (playerObject != null)
        {
            renderers = playerObject.GetComponentsInChildren<Renderer>();
            StartCoroutine(StealthCoroutine(list, stat));
        }
        
        base.OnActivate(list, stat);
    }

    private IEnumerator StealthCoroutine(List<BaseBuff> list, BaseStat stat)
    {
        // 투명화 적용
        foreach (var renderer in renderers)
        {
            if (renderer != null && renderer.material != null)
            {
                Material mat = renderer.material;
                Color color = mat.color;
                color.a = 0.3f; // 반투명
                mat.color = color;
            }
        }

        // duration 만큼 대기
        yield return CoroutineManager.waitForSeconds(duration);

        // 투명화 해제
        foreach (var renderer in renderers)
        {
            if (renderer != null && renderer.material != null)
            {
                Material mat = renderer.material;
                Color color = mat.color;
                color.a = 1f; // 불투명
                mat.color = color;
            }
        }
    }

    public override void OnDeActivate(List<BaseBuff> list, BaseStat stat)
    {
        base.OnDeActivate(list, stat);
    }
}

