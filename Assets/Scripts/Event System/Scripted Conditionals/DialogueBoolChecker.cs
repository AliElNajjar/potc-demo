using BehaviorDesigner.Runtime.Tasks;
using Scripts.Managers;

public class DialogueBoolChecker : Conditional
{
    public string boolName;
    
    public override TaskStatus OnUpdate()
    {
        var trigger = DialogueController.Instance.GetBool(boolName);
        if (trigger)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}
