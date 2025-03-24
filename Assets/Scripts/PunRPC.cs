using Photon.Pun;
using UnityEngine;


public class MultiplayerAI : MonoBehaviourPunCallbacks
{
    public void SendAIResponse(string response)
    {
        photonView.RPC("SyncAIResponse", RpcTarget.All, response);
    }

    [PunRPC]
    void SyncAIResponse(string response)
    {
        Debug.Log("AI Response: " + response);
    }
}
