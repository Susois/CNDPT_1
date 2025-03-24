using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class RasaMessage
{
    public string sender;
    public string message;
}

[System.Serializable]
public class RasaResponse
{
    public string recipient_id;
    public string text;
}

public class RasaChatManager : MonoBehaviour
{
    public TMP_InputField userInputField;
    public TMP_Text chatHistory;
    // private string rasaEndpoint = "http://localhost:5005/webhooks/rest/webhook";
    private string rasaEndpoint = "http://localhost:5005/webhooks/rest/webhook";


    public void SendMessageToRasa()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            AppendMessage("User: " + userMessage);
            StartCoroutine(SendRequestToRasa(userMessage));
            userInputField.text = "";
        }
    }

    private IEnumerator SendRequestToRasa(string message)
    {
        RasaMessage rasaMessage = new RasaMessage { sender = "user", message = message };
        string json = JsonUtility.ToJson(rasaMessage);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(rasaEndpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                ParseRasaResponse(responseText);
            }
            else
            {
                Debug.LogError("Error sending request: " + request.error);
                AppendMessage("Error: Could not reach Rasa server.");
            }
        }
    }

    private void ParseRasaResponse(string responseJson)
    {
        RasaResponse[] responses = JsonHelper.FromJson<RasaResponse>(responseJson);
        foreach (var response in responses)
        {
            AppendMessage("AI: " + response.text);
        }
    }

    private void AppendMessage(string message)
    {
        chatHistory.text += message + "\n";
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
