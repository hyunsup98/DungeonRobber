using TMPro;
using UnityEngine;

public class UI_InteractiveMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text textInteractive;  //�ٲ��� Text ������Ʈ

    public void OnInteractiveMessage(string message)
    {
        textInteractive.gameObject.SetActive(true);
        textInteractive.text = message;
    }

    public void OffInteractiveMessage()
    {
        textInteractive.gameObject.SetActive(false);
    }
}
