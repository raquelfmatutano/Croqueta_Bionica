using UnityEngine;

public enum RobotState {PATRULLA, INVESTIGACION, PERSECUCION, NONE};

public class NPC_state : MonoBehaviour
{
    public RobotState NPC_currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NPC_currentState = RobotState.PATRULLA;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
