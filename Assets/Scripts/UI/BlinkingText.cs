using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    [Header("�������")]
    public TMP_Text textComponent;

    [Header("��˸����")]
    [Range(0, 1)] public float minAlpha = 0.3f;
    [Range(0, 1)] public float maxAlpha = 1f;
    public float blinkSpeed = 1.0f;

    void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TMP_Text>();
        }

        // ��ʼ����֤
        ValidateAlphaRange();
    }

    void Update()
    {
        // ʹ�����Ҳ�ʵ��ƽ������
        float sinValue = Mathf.Sin(Time.time * blinkSpeed);

        // �����Ҳ����(-1��1)ӳ�䵽�Զ����͸���ȷ�Χ
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (sinValue + 1) * 0.5f);

        // ������ɫ͸����
        UpdateTextAlpha(alpha);
    }

    void UpdateTextAlpha(float alpha)
    {
        Color color = textComponent.color;
        color.a = alpha;
        textComponent.color = color;
    }

    // ����ֵ��֤
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

    // ��Inspectorֵ�仯ʱ�Զ���֤
    void OnValidate()
    {
        ValidateAlphaRange();
    }
}