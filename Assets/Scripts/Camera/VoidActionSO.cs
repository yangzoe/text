using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName = "Action/VoidAction")]
public class VoidActionSO : ScriptableObject
{
    public UnityAction voidAction;
    public void StartVoidAction()
    {
        voidAction.Invoke();
    }
}