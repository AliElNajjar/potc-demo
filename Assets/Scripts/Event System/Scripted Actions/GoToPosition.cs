using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class GoToPosition : Action
{
    public SharedGameObject movingGameObject;
    public SharedTransform target;
    public State movingState = State.RunUp;
    public State finalState = State.IdleUp;

    private Transform movingTransform;
    private GameObject prevGameObject;
    private UnitOverWorldMovement player;

    public override void OnStart()
    {
        var currentGameObject = GetDefaultGameObject(movingGameObject.Value);
        if (currentGameObject != prevGameObject)
        {
            movingTransform = currentGameObject.GetComponent<Transform>();
            prevGameObject = currentGameObject;
        }

        player = currentGameObject.GetComponent<UnitOverWorldMovement>();
        player.Translate(Vector2.zero);
    }

    public override TaskStatus OnUpdate()
    {
        if (movingTransform == null)
        {
            Debug.LogWarning("Transform is null");
            return TaskStatus.Failure;
        }

        if (Vector2.SqrMagnitude(transform.position - target.Value.position) < 0.1f)
        {
            player.running = false;
            player.SetOrKeepState(finalState);
            player.Translate(Vector2.zero);
            return TaskStatus.Success;
        }

        player.Translate(GetDirection());
        player.running = true;
        return TaskStatus.Running;
    }

    private Vector2 GetDirection()
    {
        switch (movingState)
        {
            case State.RunLeft:
                player.LookingDir = Looking.Left;
                return Vector2.left;
            case State.RunRight:
                player.LookingDir = Looking.Right;
                return Vector2.right;
            case State.RunUp:
                player.LookingDir = Looking.Up;
                return Vector2.up;
            case State.RunDown:
                player.LookingDir = Looking.Down;
                return Vector2.down;
            default:
                return Vector2.zero;
        }
    }


    public override void OnReset()
    {
        movingGameObject = null;
    }
}
