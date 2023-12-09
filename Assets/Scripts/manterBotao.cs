using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class manterBotao : MonoBehaviour
{
    public static manterBotao instance;
    public Button Btn_ControlaSom;

    private void Awake()
    {
        // Verifica se já existe uma instância
        if (instance == null)
        {
            // Se não existir, esta instância se torna a instância única
            instance = this;
            DontDestroyOnLoad(this.transform.parent.gameObject); // Mantém o pai do botão (provavelmente o Canvas)
        }
        else
        {
            // Se já existir uma instância, destrói esta instância
            Destroy(this.gameObject);
        }
    }

    public void SetButton(Button button)
    {
        Btn_ControlaSom = button;
    }
}
