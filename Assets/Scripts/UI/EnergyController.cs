using System.Collections;
using UnityEngine;

public class EnergyController : MonoBehaviour
{
    [Header("Settings")]
    public float fullWaitTime = 2f;    // ����������ȴ�ʱ��
    public float resetDuration = 8f;   // ������ʱ��

    private int currentBars = 0;
    private GameObject[] energyBars;
    private bool isResetting;
    private Player player;

    void Start()
    {
        InitializeBars();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void InitializeBars()
    {
        energyBars = new GameObject[15];
        for (int i = 0; i < 15; i++)
        {
            energyBars[i] = GameObject.Find($"Energy ({i + 1})");
            if (energyBars[i] == null)
            {
                Debug.LogError($"������ Energy ({i + 1}) δ�ҵ���");
                continue;
            }
            energyBars[i].SetActive(false);
        }
    }

    public void AddEnergyBar()
    {
        if (currentBars >= 15 || isResetting) return;

        energyBars[currentBars].SetActive(true);
        currentBars++;

        if (currentBars == 15)
        {
            StartCoroutine(ResetProcess());
        }
    }

    IEnumerator ResetProcess()
    {
        isResetting = true;

        // 1. ֹͣPlayer���Զ��л�Э��
        if (player.autoSwitchToEatingCoroutine != null)
        {
            player.StopCoroutine(player.autoSwitchToEatingCoroutine);
            player.autoSwitchToEatingCoroutine = null;
        }

        // 2. ����Full״̬������Energy
        player.currentState = Player.State.Full;
        Debug.Log($"����Full״̬��ʱ�䣺{Time.time}");

        // 3. �ȴ���������ӳ�ʱ��
        yield return new WaitForSeconds(fullWaitTime);

        // 4. ִ����������ʧ����
        float interval = resetDuration / 15;
        for (int i = 14; i >= 0; i--)
        {
            energyBars[i].SetActive(false);
            yield return new WaitForSeconds(interval);
        }

        // 5. ǿ������Energy���л���Eating
        player.Energy = player.minEnergy; // �ؼ������Energy
        player.currentState = Player.State.Eating;
        Debug.Log($"�˳�Full״̬��ʱ�䣺{Time.time}");

        // 6. ���ÿ�����״̬
        currentBars = 0;
        isResetting = false;
    }

    public void ForceReset()
    {
        StopAllCoroutines();
        foreach (var bar in energyBars) bar.SetActive(false);
        currentBars = 0;
        isResetting = false;
    }
}