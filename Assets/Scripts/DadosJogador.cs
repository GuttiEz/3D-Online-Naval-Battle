using UnityEngine;

public class DadosJogador : MonoBehaviour
{
    // Variáveis que serão persistidas entre cenas
    public string nomeJogador;

    // Método para evitar destruição ao mudar de cena
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}