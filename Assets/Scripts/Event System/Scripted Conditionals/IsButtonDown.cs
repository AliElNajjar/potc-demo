using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsButtonDown : Conditional
{
    public SharedString buttonName;

    public override TaskStatus OnUpdate()
    {
        return RewiredInputHandler.Instance.player.GetButtonDown(buttonName.Value) ? TaskStatus.Success : TaskStatus.Failure;
    }

    public override void OnReset()
    {
        buttonName = "Submit";
    }
}