using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class RegistroScript : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField senha;
    public TMP_InputField login;
    public TMP_InputField user;
    public TMP_InputField datanasc;
    public Button RegistrarBtn;
    public GameObject objetoInvisivelSucess;
    public GameObject objetoInvisivelFail;

    private string nick;
    private string username;
    private string password;
    private string userEmail;
    private string userBirthdate;

    private const string url = "https://navio-api.azurewebsites.net/cria_usuario";

    [Serializable]
    private class RegistrationRequest
    {
        public string nick;
        public string login;
        public string senha;
        public string email;
        public string data_nasc;
    }

    public async void CallRegistroAPIMethod()
    {
        nick = user.text;
        username = login.text;
        password = senha.text;
        userEmail = email.text;
        userBirthdate = datanasc.text;

        if (string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userBirthdate))
        {
            // Ativar objeto de falha
            objetoInvisivelFail.SetActive(true);
        }

        await SendRegistrationRequest();
    }

    private async Task SendRegistrationRequest()
    {
        try
        {
            // Construir o objeto que será serializado para JSON
            var requestBody = new RegistrationRequest
            {
                nick = nick,
                login = username,
                senha = password,
                email = userEmail,
                data_nasc = userBirthdate
            };

            // Serializar o objeto para JSON usando JsonUtility
            string jsonBody = JsonUtility.ToJson(requestBody);

            using (HttpClient client = new HttpClient())
            using (HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json"))
            {
                // Enviar a solicitação POST
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    // Verificar se a resposta contém a chave "mensagem"
                    if (responseData.Contains("\"mensagem\""))
                    {
                        
                        // Ativar objeto de sucesso
                        objetoInvisivelSucess.SetActive(true);
                    }
                    else
                    {
                        
                        // Ativar objeto de falha
                        objetoInvisivelFail.SetActive(true);
                    }
                }
                else
                {
                    
                    
                    // Ativar objeto de falha
                    objetoInvisivelFail.SetActive(true);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ocorreu um erro: {ex.Message}");
        }
    }
}
