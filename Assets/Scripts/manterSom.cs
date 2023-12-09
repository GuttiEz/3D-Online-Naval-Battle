using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manterSom : MonoBehaviour
{
    public static manterSom instance;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
