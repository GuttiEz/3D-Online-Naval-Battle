using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Coin : MonoBehaviour
{
    public Button coinBtn;
    public Button coinBtn1;
    public Button coinBtn2;
    public Text coinText;

    public GameObject Skin1;
    public GameObject Skin2;
    public GameObject Skin3;

    public GameObject BuyBtn;
    public GameObject BuyBtn1;
    public GameObject BuyBtn2;



    // Start is called before the first frame update
    void Start()
    {
        coinBtn.onClick.AddListener(BoughtCoin);
        coinBtn1.onClick.AddListener(BoughtCoin1);
        coinBtn2.onClick.AddListener(BoughtCoin2);

        coinText.text = PlayerMoney.Instance.CurrentMoney.ToString();
    }

    private void BoughtCoin()
    {
        FindObjectOfType<PlayerMoney>().AddMoney(1000);
        Debug.Log("1000 Coins compradas");
        coinText.text = PlayerMoney.Instance.CurrentMoney.ToString();

    }

    private void BoughtCoin1()
    {
        FindObjectOfType<PlayerMoney>().AddMoney(5000);
        Debug.Log("5000 Coins compradas");
        coinText.text = PlayerMoney.Instance.CurrentMoney.ToString();
    }

    private void BoughtCoin2()
    {
        FindObjectOfType<PlayerMoney>().AddMoney(10000);
        Debug.Log("10000 Coins compradas");
        coinText.text = PlayerMoney.Instance.CurrentMoney.ToString();
    }

    public void OnSkinButtonPressed()
    {
        if (FindObjectOfType<PlayerMoney>().TryRemoveMoney(1000))
        {
            Debug.Log("1000 gastas");
            coinText.text = PlayerMoney.Instance.CurrentMoney.ToString();
            Skin1.gameObject.SetActive(true);
            BuyBtn.gameObject.SetActive(false);
        }
    }

    public void OnSkinButtonPressed1()
    {
        if (FindObjectOfType<PlayerMoney>().TryRemoveMoney(5000))
        {
            Debug.Log("5000 gastas");
            coinText.text = PlayerMoney.Instance.CurrentMoney.ToString();
            Skin2.gameObject.SetActive(true);
            BuyBtn1.gameObject.SetActive(false);

        }
    }

    public void OnSkinButtonPressed2()
    {
        if (FindObjectOfType<PlayerMoney>().TryRemoveMoney(10000))
        {
            Debug.Log("10000 gastas");
            coinText.text = PlayerMoney.Instance.CurrentMoney.ToString();
            Skin3.gameObject.SetActive(true);
            BuyBtn2.gameObject.SetActive(false);

        }
    }


}
