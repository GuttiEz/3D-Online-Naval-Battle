using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearFields : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public InputField inputField;

    public void ClearField(){
        inputField.Select();
        inputField.text = "";
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
