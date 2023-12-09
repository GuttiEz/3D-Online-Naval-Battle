using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;

[System.Serializable]
public class NickResponse
{
    public string Nick;
}

public class PuxaNick : MonoBehaviour
{
    public InputField inputFieldLogin;

    async void Start()
    {
        // Encontrar o objeto de persistência
        DadosJogador dadosJogador = FindObjectOfType<DadosJogador>();

        // Verificar se o objeto de persistência foi encontrado e se a variável nomeJogador não é nula
        if (dadosJogador != null && dadosJogador.nomeJogador != null)
        {
            // Chamar a API para obter o nick associado ao login
            await ObterNickDaAPI(dadosJogador.nomeJogador);
        }
    }

    async Task ObterNickDaAPI(string login)
    {
        string url = $"https://navio-api.azurewebsites.net/pega_nick?login={login}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        var asyncOperation = www.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            await Task.Delay(0); // Ponto de espera para permitir que a Unity continue a execução
        }

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Desserialize a resposta JSON usando a classe NickResponse
            NickResponse nickResponse = JsonUtility.FromJson<NickResponse>(www.downloadHandler.text);

            // Atribuir o nick obtido ao InputFieldLogin
            inputFieldLogin.text = nickResponse.Nick;
        }
        else
        {
            Debug.LogError($"Erro na solicitação da API: {www.error}");
        }
    }
}
