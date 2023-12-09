using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net.Http;
using System.Threading.Tasks;


[System.Serializable]
public class SenhaResponse
{
    public string SenhaNova;
}


public class RecuperaSenha : MonoBehaviour
{
    public TMP_InputField user;
    public GameObject objetoInvisivelSucess;
    public GameObject objetoInvisivelFail;
    public Text textoSenha;
    


    private const string baseUrl = "https://navio-api.azurewebsites.net/recupera_senha?Login=";

    public async void CallRecuperaSenhaAPIMethod()
    {
        string nick = user.text;

        if (string.IsNullOrEmpty(nick))
        {
            // Trate o caso em que o campo está vazio
            Debug.LogError("Campo de usuário vazio.");
            return;
        }

        await SendRecuperaSenhaRequest(nick);
    }

    private async Task SendRecuperaSenhaRequest(string nick)
    {
        try
        {
            string url = baseUrl + Uri.EscapeDataString(nick);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    
                    if (responseData.Contains("Login informado errado!"))
                    {   
                        objetoInvisivelFail.SetActive(true);
                    }
                    else
                    {
                        SenhaResponse senhaResponse = JsonUtility.FromJson<SenhaResponse>(responseData);

                        textoSenha.text = "Senha Nova: " + senhaResponse.SenhaNova;

                        objetoInvisivelSucess.SetActive(true);
                    }
                    
                }
                else
                {
                    Debug.LogError($"Erro na solicitação: {response.StatusCode}");
                    // Ativar objeto de sucesso
                    
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ocorreu um erro: {ex.Message}");
        }
    }
}
