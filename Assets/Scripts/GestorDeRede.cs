using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class GestorDeRede : MonoBehaviourPunCallbacks
{

    private int quickbtn = 0;
    public const int MAXPLAYERS = 2;
    public static GestorDeRede Instancia { get; private set; }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instancia = this;
    }

    private void Start()
    {
       
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conexão Bem Sucedida");
    }

    public void CriaSala(string nomeSala)
    {
        PhotonNetwork.CreateRoom(nomeSala);
    }

    public void EntraSala(string nomeSala)
    {
        PhotonNetwork.JoinRoom(nomeSala);
    }

    public void MudaNick(string nickname)
    {
        PhotonNetwork.NickName = nickname;
    }

    public string ObterListaDeJogadores()
    {
        var lista = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            lista += player.NickName + "\n";
        }
        return lista;
    }

    public bool DonoDaSala()
    {
        return PhotonNetwork.IsMasterClient;
    }
    public void SairDoLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void ComecaJogo(string nomeCena)
    {
        PhotonNetwork.LoadLevel(nomeCena);
    }


    public void OnQuickPlayButtonClicked()
    {
        quickbtn = 1;
        // Junte-se à sala existente ou crie uma nova
        PhotonNetwork.JoinRandomRoom();
    }

    // Método chamado quando a tentativa de entrar em uma sala falha
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Cria uma nova sala se falhar em se juntar a uma existente
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    // Método chamado quando o jogador entra com sucesso em uma sala
    public override void OnJoinedRoom()
    {
        // Verifica se há dois jogadores na sala antes de iniciar o jogo
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && quickbtn != 0)
        {
            photonView.RPC("ComecaJogo", RpcTarget.All, "Multiplayer");
        }
    }

}
