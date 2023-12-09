using UnityEngine;
using UnityEngine.UI;

public class SkinSelect : MonoBehaviour
{
    public int buttonValue;

    public void OnButtonPressed(int buttonValue)
    {
        Central.Instance.ButtonValue = buttonValue;
        Debug.Log("ButtonManager: Valor do botão definido como " + buttonValue);
    }
}
