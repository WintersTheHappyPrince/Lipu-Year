using UnityEngine;

public class StateMachine
{
    public PlayerState currentState;

    public void SetState(PlayerState newState)
    {
        if (currentState != null)
        {
            Debug.Log("Exit " + currentState);
            currentState.Exit();
        }

        if(newState != currentState)
        {
            currentState = newState;
            currentState.Enter();
        }
        Debug.Log("Enter " + currentState);
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }

        //Debug.Log("currentState " + currentState);
    }
}
