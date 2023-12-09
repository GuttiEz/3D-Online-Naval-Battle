using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    [SerializeField] private int currentMoney;

    private static PlayerMoney instance;


    // Adicione qualquer dado global que você deseja compartilhar entre cenas

    public static PlayerMoney Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManagerCentral").AddComponent<PlayerMoney>();
            }
            return instance;
        }
    }


    public int CurrentMoney
    {
        get { return currentMoney; }
        set { currentMoney = value; }
    }

    private void Awake()
    {
        currentMoney = PlayerPrefs.GetInt("prefMoney");
    }

    public void AddMoney(int moneyAmounttoAdd)
    {
        currentMoney += moneyAmounttoAdd;
        PlayerPrefs.SetInt("prefMoney", currentMoney);
    }

    public bool TryRemoveMoney(int moneyToRemove)
    {
        if(currentMoney >= moneyToRemove)
        {
            currentMoney -= moneyToRemove;
            PlayerPrefs.SetInt("prefMoney", currentMoney);
            return true;
        }
        else
        {
            return false;
        }
    }
}
