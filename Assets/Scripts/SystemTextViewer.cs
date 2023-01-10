using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum SystemType {  Money = 0, Build }

public class SystemTextViewer : MonoBehaviour
{
    private TextMeshProUGUI textSystem;
    private TMPAlpha tmpAlpha;

    private void Awake()
    {
        textSystem = GetComponent<TextMeshProUGUI>();
        tmpAlpha = GetComponent<TMPAlpha>();
    }

    public void PrintText(SystemType type)
    {
        switch (type)
        {
            case SystemType.Money:
                textSystem.text = "System : Not enough point...";
                break;
            case SystemType.Build:
                textSystem.text = "System : Invalid buid tower...";
                break;
        }

        tmpAlpha.FadeOut(); // 시스템 텍스트가 출력되었다가 서서히 사라지도록
    }
}
