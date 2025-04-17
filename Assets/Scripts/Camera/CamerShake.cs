using Cinemachine;
using UnityEngine;
public class CameraShake : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;
    public VoidActionSO voidActionSO;
    private void OnEnable()
    {
        voidActionSO.voidAction += impulse;
    }
    public void impulse()
    {
        impulseSource.GenerateImpulse();
    }
}