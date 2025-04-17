using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    [Header("组件引用")]
    public TMP_Text textComponent;

    [Header("闪烁设置")]
    [Range(0, 1)] public float minAlpha = 0.3f;
    [Range(0, 1)] public float maxAlpha = 1f;
    public float blinkSpeed = 1.0f;

    void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TMP_Text>();
        }

        // 初始化验证
        ValidateAlphaRange();
    }

    void Update()
    {
        // 使用正弦波实现平滑过渡
        float sinValue = Mathf.Sin(Time.time * blinkSpeed);

        // 将正弦波输出(-1到1)映射到自定义的透明度范围
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (sinValue + 1) * 0.5f);

        // 更新颜色透明度
        UpdateTextAlpha(alpha);
    }

    void UpdateTextAlpha(float alpha)
    {
        Color color = textComponent.color;
        color.a = alpha;
        textComponent.color = color;
    }

    // 输入值验证
    void ValidateAlphaRange()
    {
        minAlpha = Mathf.Clamp01(minAlpha);
        maxAlpha = Mathf.Clamp01(maxAlpha);

        if (minAlpha > maxAlpha)
        {
            float temp = minAlpha;
            minAlpha = maxAlpha;
            maxAlpha = temp;
        }
    }

    // 在Inspector值变化时自动验证
    void OnValidate()
    {
        ValidateAlphaRange();
    }
}