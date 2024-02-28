using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class GameOverPanel : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void GameOverUI()
    {
        canvasGroup.alpha = 1;              // ���İ� �÷��� ���̰� �����
        canvasGroup.blocksRaycasts = true;  // �����ɽ�Ʈ�� �ڱⰡ �ǰ� �ϱ�
    }
}
