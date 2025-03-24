using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private DialogData dialogData;

    void Start()
    {
        LoadDialogData();
        SendMessageToRasa("Mua sỉ giá bao nhiêu vậy shop, nói tui nghe!");
    }

    void LoadDialogData()
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, "Data_train_AI/dialog_data.json");
        if (System.IO.File.Exists(filePath))
        {
            string jsonContent = System.IO.File.ReadAllText(filePath);
            dialogData = JsonUtility.FromJson<DialogData>("{\"dialogs\":" + jsonContent + "}");
            Debug.Log("Dữ liệu hội thoại đã tải thành công!");
        }
        else
        {
            Debug.LogError("Không tìm thấy file JSON!");
        }
    }

    public void SendMessageToRasa(string message)
{
    RasaService.Instance.SendMessageToRasa(message, (response) =>
    {
        Debug.Log($"Raw AI Response: {response}");
        if (string.IsNullOrEmpty(response))
        {
            Debug.LogError("⚠️ Phản hồi từ Rasa bị rỗng!");
        }
    });
}

}
