using UnityEngine;
using TMPro;

public class BillboardText : MonoBehaviour
{
    public NPC_state state;
    public TMP_Text textComponent;

    void Start() {
    }

    void Update()
    {
        if (Camera.main != null)
        {
            // Match the camera's rotation to keep text flat and facing forward
            transform.rotation = Camera.main.transform.rotation;
        }

        if (state.NPC_currentState is InvestigarState) {
            textComponent.text = "?";
        }
        else if (state.NPC_currentState is PerseguirState) {
            textComponent.text = "!";
        }

        else if (state.NPC_currentState is CargarState && GameManager.Instance.timer_on) {
            textComponent.text = "...";
        }

        else {
            textComponent.text = "";
        }
    }

}
