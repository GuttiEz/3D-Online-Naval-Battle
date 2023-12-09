using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField LoginField;
    public TMP_InputField SenhaField;
    public Button loginButton;
    public GameObject objetoInvisivel;

    [Serializable] public class LoginResponse
    {
        public string mensagem;
    }

    public async void CallAPIMethod()
    {
        string username = LoginField.text;
        string password = SenhaField.text;

        if (string.IsNullOrEmpty(username) | string.IsNullOrEmpty(password))
        {
            username = "Teste";
            password = "Teste";
        }

        string url = $"https://navio-api.azurewebsites.net/login?login={username}&senha={password}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    // Desserialize a resposta JSON manualmente usando a classe LoginResponse
                    LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseData);

                    // Obtém a mensagem da resposta
                    string mensagem = loginResponse.mensagem;
                    
                    if (mensagem == "Login Realizado com Sucesso!")
                    {
                        DadosJogador dadosJogador = FindObjectOfType<DadosJogador>();
                        dadosJogador.nomeJogador = username;
                        // Encontrar o objeto que contém o script ChangeScene
                        ChangeScene changeSceneScript = FindObjectOfType<ChangeScene>();
                        changeSceneScript.LoadScene("Menu");
                    }
                    else
                    {
                        objetoInvisivel.SetActive(true);
                    }
                }
                else
                {
                    Debug.LogError($"Erro na solicitação: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ocorreu um erro: {ex.Message}");
            }
        }
    }
}
