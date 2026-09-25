using UnityEngine;
using TMPro;

public class BillboardText : MonoBehaviour
{
    public NPC_state state;
    public TMP_Text textComponent;

    void Start() {
        //textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (Camera.main != null)
        {
            // Match the camera's rotation to keep text flat and facing forward
            transform.rotation = Camera.main.transform.rotation;
        }

        if (state.NPC_currentState == RobotState.INVESTIGACION) {
            textComponent.text = "?";
        }
        else if (state.NPC_currentState == RobotState.PERSECUCION) {
            textComponent.text = "!";
        }

        else {
            textComponent.text = "";
        }
    }

}
