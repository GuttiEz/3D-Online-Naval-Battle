using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadScene(string cena)
    {

        SceneManager.LoadScene(cena); // Carrega a cena especificada
    }
}
