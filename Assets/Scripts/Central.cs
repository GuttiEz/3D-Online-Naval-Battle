using UnityEngine;

public class Central : MonoBehaviour
{
    private static Central instance;

    // Adicione qualquer dado global que você deseja compartilhar entre cenas
    private int buttonValue = 0;

    public static Central Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManagerCentral").AddComponent<Central>();
            }
            return instance;
        }
    }

    public int ButtonValue
    {
        get { return buttonValue; }
        set { buttonValue = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
