using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class LoadingPage : MonoBehaviour
{
    public Image blackScreen;
    public Text loadingIcon;

    public float duration = 1f;
    private Tweener breatheTween;
    void StartBreatheEffect()
    {
        float duration = 0.5f;
        // 通过DoTween的DOFade来实现呼吸效果
        breatheTween = loadingIcon.DOFade(0, duration)
            .SetLoops(-1, LoopType.Yoyo) // 设置无限循环，Yoyo模式表示往复运动
            .SetUpdate(true); // 使用真实时间，这样当Time.timeScale = 0时动画仍可执行
    }

    void StopBreatheEffect()
    {
        // 如果Tween存在，停止它
        if (breatheTween != null)
        {
            breatheTween.Kill();
            breatheTween = null;
        }
        // 让Loading图标立即变为不透明
        loadingIcon.color = new Color(loadingIcon.color.r, loadingIcon.color.g, loadingIcon.color.b, 1f);
    }
    void FadeIn()
    {
        // 停止前一个补间
        blackScreen.DOKill();
        // 开始新的补间
        blackScreen.DOFade(1, duration).SetUpdate(true);
    }

    void FadeOut()
    {
        // 停止前一个补间
        blackScreen.DOKill();
        // 开始新的补间
        blackScreen.DOFade(0, duration).SetUpdate(true).
        OnComplete(() => blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1f));

    }
    public void EnterLoading()
    {
        loadingIcon.gameObject.SetActive(true);
        blackScreen.gameObject.SetActive(true);
        FadeIn();
        StartBreatheEffect();
    }
    public void ExitLoading()
    {
        loadingIcon.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);
        FadeOut();
        StopBreatheEffect();
    }
    private void OnEnable()
    {
        EnterLoading();
    }
    private void OnDisable()
    {
        ExitLoading();
    }
}
