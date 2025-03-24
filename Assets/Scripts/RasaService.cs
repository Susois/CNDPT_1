using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
// using System.Text.Json;
using System.Collections;
using Newtonsoft.Json;

public class RasaService : MonoBehaviour
{
    private static RasaService instance;
    private string rasaEndpoint = "http://localhost:5005/webhooks/rest/webhook";

    public static RasaService Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("RasaService");
                instance = obj.AddComponent<RasaService>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    public void SendMessageToRasa(string message, Action<string> callback)
    {
        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log($"📤 Gửi tin nhắn đến Rasa: {message}");
            Instance.StartCoroutine(Instance.SendRequestToRasa(message, callback));
        }
        else
        {
            Debug.LogError("⚠️ Tin nhắn rỗng, không gửi đi!");
            callback?.Invoke("Lỗi: Tin nhắn rỗng.");
        }
    }

    private IEnumerator SendRequestToRasa(string message, Action<string> callback)
    {
        var rasaMessage = new { sender = "user", message = message };
        string json = JsonConvert.SerializeObject(rasaMessage);
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
                Debug.Log($"✅ Phản hồi từ Rasa: {responseText}");

                // Kiểm tra phản hồi có dữ liệu hợp lệ không
                if (string.IsNullOrEmpty(responseText) || responseText == "[]")
                {
                    Debug.LogError("⚠️ Rasa phản hồi rỗng!");
                    callback?.Invoke("Lỗi: Phản hồi từ AI rỗng.");
                }
                else
                {
                    callback?.Invoke(responseText);
                }
            }
            else
            {
                Debug.LogError($"❌ Lỗi khi gửi request: {request.error}");
                callback?.Invoke($"Lỗi: Không thể kết nối đến server Rasa. ({request.error})");
            }
        }
    }
}
