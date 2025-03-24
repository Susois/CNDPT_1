using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public InputField userInput;
    public Text chatDisplay;

    public void SendMessageToAI()
    {
        string message = userInput.text;
        chatDisplay.text += "\nPlayer: " + message;
        userInput.text = "";

        RasaService.Instance.SendMessageToRasa(message, (response) =>
        {
            chatDisplay.text += "\nAI: " + response;
        });
    }
}
