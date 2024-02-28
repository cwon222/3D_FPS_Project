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
        canvasGroup.alpha = 1;              // 알파값 올려서 보이게 만들기
        canvasGroup.blocksRaycasts = true;  // 레이케스트를 자기가 되게 하기
    }
}
