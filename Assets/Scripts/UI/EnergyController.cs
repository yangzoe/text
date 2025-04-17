using System.Collections;
using UnityEngine;

public class EnergyController : MonoBehaviour
{
    [Header("Settings")]
    public float fullWaitTime = 2f;    // 能量条满后等待时间
    public float resetDuration = 8f;   // 总重置时间

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
                Debug.LogError($"能量条 Energy ({i + 1}) 未找到！");
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

        // 1. 停止Player的自动切换协程
        if (player.autoSwitchToEatingCoroutine != null)
        {
            player.StopCoroutine(player.autoSwitchToEatingCoroutine);
            player.autoSwitchToEatingCoroutine = null;
        }

        // 2. 进入Full状态并冻结Energy
        player.currentState = Player.State.Full;
        Debug.Log($"进入Full状态，时间：{Time.time}");

        // 3. 等待填满后的延迟时间
        yield return new WaitForSeconds(fullWaitTime);

        // 4. 执行能量条消失动画
        float interval = resetDuration / 15;
        for (int i = 14; i >= 0; i--)
        {
            energyBars[i].SetActive(false);
            yield return new WaitForSeconds(interval);
        }

        // 5. 强制重置Energy并切换回Eating
        player.Energy = player.minEnergy; // 关键：清空Energy
        player.currentState = Player.State.Eating;
        Debug.Log($"退出Full状态，时间：{Time.time}");

        // 6. 重置控制器状态
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