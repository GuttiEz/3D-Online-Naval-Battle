using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManagerOnline : MonoBehaviourPunCallbacks
{
    [Header("Ships")]
    public GameObject[] ships;
    private ShipScriptOnline shipScript;
    private List<int[]> playerShips = new List<int[]>();
    private List<int[]> ServerShips = new List<int[]>();
    private List<int[]> InputShips = new List<int[]>();
    private int shipIndex = 0;
    public List<TileScriptOnline> allTileScripts;


    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Text topText;
    public Text NextTextBtn;
    public Text playerShipText;
    public Text turnTimerText;
    public Button exit;



    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;
    public GameObject plane;
    public GameObject blood;
    public GameObject xmas;
    public GameObject halloween;



    private bool setupComplete = false;
    private bool clickable = true;
    private bool turn = false;

    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> clickedTiles = new List<GameObject>();
    private List<GameObject> clickedTiles2 = new List<GameObject>();


    public int playerShipCount = 5;

    private int _jogadoresEmJogo = 0;
    private Player _photonPlayer;
    private int _id;
    private int jogadorId = PhotonNetwork.LocalPlayer.ActorNumber;
    private List<GameManagerOnline> _jogadores;
    private bool playersReady = false;

    private int currentPlayerTurn = 0;

    private bool _jogadorInicializado = false;

    private bool bombaLancada = false;
    private bool colisaoBomba = false;

    private bool goagain = false;

    public Material novoMaterial;
    public Material novoMaterial2;
    public Material novoMaterial3;



    private Color[] originalShipColors;
    private Color[] originalShipColors2;
    private Color originalMissileColor;
    private Material originalPlaneColor;
    private Material[] originalShipMaterial;
    private Color originalMissileColor2;
    private Color originalMissileColor3;


    private AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip wrongSound;
    public AudioClip water;
    public AudioClip explosion;
    public AudioClip death;
    public GestorDeRede sair;


    public List<GameManagerOnline> Jogadores { get => _jogadores; set => _jogadores = value; }

    public static GameManagerOnline Instancia { get; set; }

    private float turnTime = 15f;
    private bool timerActive = false;


    int buttonValueEnemy = 0;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        turnTimerText.gameObject.SetActive(false);
        photonView.RPC("AdicionaJogador", RpcTarget.AllBuffered);
        _jogadores = new List<GameManagerOnline>();
        shipScript = ships[shipIndex].GetComponent<ShipScriptOnline>();
        nextBtn.onClick.AddListener(NextShipClicked);
        rotateBtn.onClick.AddListener(RotateClicked);
        exit.onClick.AddListener(Quitoupartida);
        SaveOriginalColors();
        int buttonValue = Central.Instance.ButtonValue;
        photonView.RPC("ReceiveButtonValue", RpcTarget.Others, buttonValue);
        AlterarCorBomba();

    }

    private void SaveOriginalColors()
    {
        int buttonValue = Central.Instance.ButtonValue;
        originalShipColors = new Color[ships.Length];
        originalShipMaterial = new Material[ships.Length];
        originalShipColors2 = new Color[ships.Length];


        for (int i = 0; i < ships.Length; i++)
        {
            if (buttonValue == 1)
            {
                Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                if (shipsRenderer != null)
                {
                    originalShipColors[i] = shipsRenderer.material.color;
                }
            }
            else if (buttonValue == 2)
            {
                Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                if (shipsRenderer != null)
                {
                    originalShipMaterial[i] = shipsRenderer.material;
                }
            }
            else if (buttonValue == 3)
            {
                Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                if (shipsRenderer != null)
                {
                    originalShipColors2[i] = shipsRenderer.material.color;
                }
            }

        }

        Renderer bombRenderer = missilePrefab.GetComponent<Renderer>();
        if (buttonValue == 1)
        {
            if (bombRenderer != null)
            {
                originalMissileColor = Color.black;
            }
        }

        else if (buttonValue == 2)
        {
            if (bombRenderer != null)
            {
                originalMissileColor2 = Color.black;
            }
        }
        else if (buttonValue == 3)
        {
            if (bombRenderer != null)
            {
                originalMissileColor3 = Color.black;
            }
        }

        MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
        if (planeRenderer != null)
        {
            originalPlaneColor = planeRenderer.material;
        }
        // Adicione mais lógica se necessário para outros tipos de renderers
    }


    [PunRPC]
    private void ReceiveButtonValue(int enemyButtonValue)
    {
        buttonValueEnemy = enemyButtonValue;
    }


    [PunRPC]
    private void AlterarCorBomba()
    {
        Color cor = Color.red;
        Color cor2 = Color.green;
        Color cor3 = new Color(1.0f, 0.5f, 0.0f, 1.0f);
        int buttonValue = Central.Instance.ButtonValue;
        Debug.Log("ButtonManager: Valor do botão definido como " + buttonValue);
        if (buttonValue == 1)
        {
            xmas.SetActive(false);
            halloween.SetActive(false);
            blood.SetActive(true);
            for (int i = 0; i < ships.Length; i++)
            {
                Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                if (shipsRenderer != null)
                {
                    shipsRenderer.material.color = cor;
                }
                else
                {
                    Debug.LogError("Componente Renderer não encontrado no navio!");
                }
            }

            Renderer bombRenderer = missilePrefab.GetComponent<Renderer>();
            if (bombRenderer != null)
            {
                bombRenderer.sharedMaterial.color = cor;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na bomba!");
            }
            MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
            if (planeRenderer != null)
            {
                planeRenderer.material = novoMaterial;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na dock!");
            }
        }
        else if (buttonValue == 2)
        {
            blood.SetActive(false);
            halloween.SetActive(false);
            xmas.SetActive(true);
            for (int i = 0; i < ships.Length; i++)
            {
                Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                if (shipsRenderer != null)
                {
                    shipsRenderer.material = novoMaterial2;
                }
                else
                {
                    Debug.LogError("Componente Renderer não encontrado no navio!");
                }
            }

            Renderer bombRenderer = missilePrefab.GetComponent<Renderer>();
            if (bombRenderer != null)
            {
                bombRenderer.sharedMaterial.color = cor2;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na bomba!");
            }

            MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
            if (planeRenderer != null)
            {
                planeRenderer.material = novoMaterial2;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na dock!");
            }
        }
        else if (buttonValue == 3)
        {
            blood.SetActive(false);
            xmas.SetActive(false);
            halloween.SetActive(true);
            for (int i = 0; i < ships.Length; i++)
            {
                Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                if (shipsRenderer != null)
                {
                    shipsRenderer.material.color = cor3;
                }
                else
                {
                    Debug.LogError("Componente Renderer não encontrado no navio!");
                }
            }

            Renderer bombRenderer = missilePrefab.GetComponent<Renderer>();
            if (bombRenderer != null)
            {
                bombRenderer.sharedMaterial.color = cor3;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na bomba!");
            }
            MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
            if (planeRenderer != null)
            {
                planeRenderer.material = novoMaterial3;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na dock!");
            }
        }
        else if (buttonValue == 0)
        {
            Debug.Log("ButtonManager: Mantendo cor original, pois o valor do botão é diferente de 1");
            blood.SetActive(false);
            xmas.SetActive(false);
            halloween.SetActive(false);
            RestoreOriginalColors();
        }

    }


    private void RestoreOriginalColors()
    {
        int buttonValue = Central.Instance.ButtonValue;
        for (int i = 0; i < ships.Length; i++)
        {
            Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
            if (buttonValue == 1)
            {
                if (shipsRenderer != null)
                {
                    shipsRenderer.material.color = originalShipColors[i];
                }
            }
            else if (buttonValue == 2)
            {
                shipsRenderer.material = originalShipMaterial[i];
            }
            else if (buttonValue == 3)
            {
                if (shipsRenderer != null)
                {
                    shipsRenderer.material.color = originalShipColors2[i];
                }
            }
            if (buttonValue == 0)
            {
                if (shipsRenderer != null)
                {
                    shipsRenderer.material.color = originalShipColors[i];
                }
            }
        }

        Renderer bombRenderer = missilePrefab.GetComponent<Renderer>();
        if (buttonValue == 1)
        {
            if (bombRenderer != null)
            {
                bombRenderer.sharedMaterial.color = originalMissileColor;
            }
        }
        else if (buttonValue == 2)
        {
            if (bombRenderer != null)
            {
                bombRenderer.sharedMaterial.color = originalMissileColor2;

            }
        }
        else if (buttonValue == 3)
        {
            if (bombRenderer != null)
            {
                bombRenderer.sharedMaterial.color = originalMissileColor3;
            }
        }
        else if (buttonValue == 0)
        {
            if (buttonValueEnemy == 1)
            {
                bombRenderer.sharedMaterial.color = Color.black;
            }
            else if (buttonValueEnemy == 2)
            {
                bombRenderer.sharedMaterial.color = originalMissileColor2;
            }
            else if (buttonValueEnemy == 3)
            {
                bombRenderer.sharedMaterial.color = Color.black;
            }
            else if (buttonValueEnemy == 0)
            {
                bombRenderer.sharedMaterial.color = Color.black;
            }
        }

        MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
        if (planeRenderer != null)
        {
            planeRenderer.material = originalPlaneColor;
        }
    }

    [PunRPC]
    private void AlterarCorEnemy()
    {
        Color cor = Color.red;
        Color cor2 = Color.green;
        Color cor3 = new Color(1.0f, 0.5f, 0.0f, 1.0f);
        if (buttonValueEnemy == 1)
        {
            xmas.SetActive(false);
            halloween.SetActive(false);
            blood.SetActive(true);
            int buttonValue = Central.Instance.ButtonValue;
            if (buttonValue == 1)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material.color = cor;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }
            }
            else if (buttonValue == 2)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material = novoMaterial2;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }

            }
            else if (buttonValue == 3)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material.color = cor3;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }
            }
            else if (buttonValue == 0)
            {
                Debug.Log("ButtonManager: Mantendo cor original, pois o valor do botão é diferente de 1");
                blood.SetActive(false);
                xmas.SetActive(false);
                halloween.SetActive(false);
                RestoreOriginalColors();
            }
            Renderer bombRenderer = missilePrefab.GetComponent<Renderer>();
            if (bombRenderer != null)
            {
                bombRenderer.sharedMaterial.color = cor;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na bomba!");
            }

            MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
            if (planeRenderer != null)
            {
                planeRenderer.material = novoMaterial;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na dock!");
            }

        }
        else if (buttonValueEnemy == 2)
        {
            int buttonValue = Central.Instance.ButtonValue;
            blood.SetActive(false);
            halloween.SetActive(false);
            xmas.SetActive(true);
            if (buttonValue == 1)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material.color = cor;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }

            }
            else if (buttonValue == 2)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material = novoMaterial2;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }
            }
            else if (buttonValue == 3)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material.color = cor3;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }

            }
            else if (buttonValue == 0)
            {
                Debug.Log("ButtonManager: Mantendo cor original, pois o valor do botão é diferente de 1");
                blood.SetActive(false);
                xmas.SetActive(false);
                halloween.SetActive(false);
                RestoreOriginalColors();
            }
            Renderer bombRenderer = missilePrefab.GetComponent<Renderer>();
            if (bombRenderer != null)
            {
                bombRenderer.sharedMaterial.color = cor2;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na bomba!");
            }

            MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
            if (planeRenderer != null)
            {
                planeRenderer.material = novoMaterial2;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na dock!");
            }
        }
        else if (buttonValueEnemy == 3)
        {
            blood.SetActive(false);
            xmas.SetActive(false);
            halloween.SetActive(true);
            int buttonValue = Central.Instance.ButtonValue;
            if (buttonValue == 1)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material.color = cor;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }
            }
            else if (buttonValue == 2)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material = novoMaterial2;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }

            }
            else if (buttonValue == 3)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material.color = cor3;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }
            }
            else if (buttonValue == 0)
            {
                Debug.Log("ButtonManager: Mantendo cor original, pois o valor do botão é diferente de 1");
                RestoreOriginalColors();
            }
            Renderer bombRenderer = missilePrefab.GetComponent<Renderer>();
            if (bombRenderer != null)
            {
                bombRenderer.sharedMaterial.color = cor3;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na bomba!");
            }

            MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
            if (planeRenderer != null)
            {
                planeRenderer.material = novoMaterial3;
            }
            else
            {
                Debug.LogError("Componente Renderer não encontrado na dock!");
            }

        }
        else if (buttonValueEnemy == 0)
        {
            int buttonValue = Central.Instance.ButtonValue;
            blood.SetActive(false);
            xmas.SetActive(false);
            halloween.SetActive(false);
            if (buttonValue == 1)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material.color = cor;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }

            }
            else if (buttonValue == 2)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material = novoMaterial2;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }
            }
            else if (buttonValue == 3)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    Renderer shipsRenderer = ships[i].GetComponent<Renderer>();
                    if (shipsRenderer != null)
                    {
                        shipsRenderer.material.color = cor3;
                    }
                    else
                    {
                        Debug.LogError("Componente Renderer não encontrado no navio!");
                    }
                }

            }
            else if (buttonValue == 0)
            {
                Debug.Log("ButtonManager: Mantendo cor original, pois o valor do botão é diferente de 1");
                blood.SetActive(false);
                xmas.SetActive(false);
                halloween.SetActive(false);
                RestoreOriginalColors();
            }

            Renderer bombRenderer = missilePrefab.GetComponent<Renderer>();
            if (buttonValue == 1)
            {
                if (bombRenderer != null)
                {
                    bombRenderer.sharedMaterial.color = Color.black;
                }
            }
            else if (buttonValue == 2)
            {
                if (bombRenderer != null)
                {
                    bombRenderer.sharedMaterial.color = Color.black;
                }

            }
            else if (buttonValue == 3)
            {
                if (bombRenderer != null)
                {
                    bombRenderer.sharedMaterial.color = Color.black;
                }

            }
            else if (buttonValue == 0)
            {
                if (bombRenderer != null)
                {
                    bombRenderer.sharedMaterial.color = Color.black;
                }

            }
            MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
            if (planeRenderer != null)
            {
                planeRenderer.material = originalPlaneColor;
            }

        }

    }
    void Update()
    {
        if (timerActive)
        {
            UpdateTurnTimerText();

        }
    }


    [PunRPC]
    private IEnumerator StartTurnTimer()
    {
        // O corpo do método permanece o mesmo
        while (timerActive)
        {
            yield return new WaitForSeconds(1f);
            turnTime -= 1f;
            if (turnTime <= 0)
            {
                if (currentPlayerTurn == jogadorId - 1)
                {
                    // Tempo esgotado, mude o turno automaticamente
                    timerActive = false;
                    goagain = false;
                    photonView.RPC("Colisao", RpcTarget.All);
                    photonView.RPC("LancarBomba", RpcTarget.All);
                    photonView.RPC("AlterTurn", RpcTarget.All);
                    photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
                }

            }
        }
    }

    [PunRPC]
    private void AlterTurn()
    {
        currentPlayerTurn = (currentPlayerTurn + 1) % Jogadores.Count;
    }

    private void UpdateTurnTimerText()
    {
        // Atualize o texto na tela com o tempo restante
        turnTimerText.text = Mathf.CeilToInt(turnTime).ToString();
    }

    [PunRPC]
    private void SwitchTurn(int id)
    {
        if (!AmbosJogadoresProntos())
        {
            photonView.RPC("Colisao", RpcTarget.All);
            photonView.RPC("LancarBomba", RpcTarget.All);
        }
        else
        {
            if (goagain)
            {
                turnTimerText.gameObject.SetActive(true);
                timerActive = true;
                turnTime = 15f;
                photonView.RPC("StartTurnTimer", RpcTarget.All);
                photonView.RPC("Colisaofalso", RpcTarget.All);
                photonView.RPC("LancarBombafalso", RpcTarget.All);
                turn = true;
                photonView.RPC("SetShips", RpcTarget.All, ServerShips.ToArray());
                for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
                foreach (GameObject fire in playerFires) fire.SetActive(false);
                topText.text = "Selecione onde atirar";
            }
            else
            {
                bombaLancada = false;
                colisaoBomba = false;
                if (currentPlayerTurn == jogadorId - 1)
                {
                    AlterarCorBomba();
                    turnTimerText.gameObject.SetActive(true);
                    timerActive = true;
                    turnTime = 15f;
                    photonView.RPC("StartTurnTimer", RpcTarget.All);
                    turn = true;
                    photonView.RPC("SetShips", RpcTarget.All, ServerShips.ToArray());
                    for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
                    foreach (GameObject fire in playerFires) fire.SetActive(false);
                    topText.text = "Selecione onde atirar";
                    ColorTilesByPlayer(currentPlayerTurn);

                }
                else
                {
                    AlterarCorEnemy();
                    turnTime = 15f;
                    timerActive = false;
                    turnTimerText.gameObject.SetActive(false);
                    turn = false;
                    for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
                    foreach (GameObject fire in playerFires) fire.SetActive(true);
                    topText.text = "Turno do Inimigo";
                    ColorTilesByPlayer(currentPlayerTurn);
                }
            }
        }

    }


    public void SpawnMissile(Vector3 position)
    {
        Instantiate(missilePrefab, position, Quaternion.identity);
        StartCoroutine(AguardaUmSeg(position));
    }

    private IEnumerator AguardaUmSeg(Vector3 position)
    {
        yield return new WaitForSeconds(3f);
        photonView.RPC("SpawnMissileRPC", RpcTarget.Others, position);
    }

    [PunRPC]
    public void Colisao()
    {
        colisaoBomba = true;
    }

    [PunRPC]
    public void Colisaofalso()
    {
        colisaoBomba = false;
    }


    [PunRPC]
    public void LancarBomba()
    {

        bombaLancada = true;
    }

    [PunRPC]
    public void LancarBombafalso()
    {

        bombaLancada = false;
    }

    [PunRPC]
    private bool AmbosJogadoresProntos()
    {
        return bombaLancada && colisaoBomba;
    }

    [PunRPC]
    private void SpawnMissileRPC(Vector3 position)
    {

        GameObject missile = Instantiate(missilePrefab, position, Quaternion.identity);


    }

    [PunRPC]
    private void AdicionaJogador()
    {
        _jogadoresEmJogo++;
        if (_jogadoresEmJogo == PhotonNetwork.PlayerList.Length)
        {
            if (!_jogadorInicializado)
            {
                CriaJogador();
                _jogadorInicializado = true;
            }

        }
    }


    [PunRPC]
    private void SendShips()
    {
        photonView.RPC("ReceiveOpponentShips", RpcTarget.Others, playerShips.ToArray());
    }

    [PunRPC]
    private void UpadateSetShips()
    {
        photonView.RPC("SetShips", RpcTarget.All, ServerShips.ToArray());
    }


    [PunRPC]
    private void ReceiveOpponentShips(int[][] opponentShipsArray)
    {
        ServerShips.Clear();
        foreach (int[] shipPositions in opponentShipsArray)
        {
            ServerShips.Add(shipPositions);
        }
    }

    [PunRPC]
    private void SetShips(int[][] opponentShipsArray)
    {
        InputShips.Clear();
        foreach (int[] shipPositions in opponentShipsArray)
        {
            InputShips.Add(shipPositions);
        }
    }

    [PunRPC]
    private void Inicializa(Player player)
    {
        _photonPlayer = player;
        _id = player.ActorNumber;
        GameManagerOnline.Instancia.Jogadores.Add(this);
        Debug.Log("Jogador " + _id + " inicializado.");
        Debug.Log("Jogador criado. Total de jogadores: " + Jogadores.Count);
    }

    [PunRPC]
    private void CriaJogador()
    {
        photonView.RPC("Inicializa", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    private void JogadorPronto()
    {
        playersReady = true;
    }

    public void AddPlayerShip(int[] shipPositions)
    {
        playerShips.Add(shipPositions);
    }

    private void NextShipClicked()
    {
        if (!shipScript.OnGameBoard())
        {
            topText.text = "Clique no campo para colocar o navio";
            audioSource.PlayOneShot(wrongSound);

        }
        else
        {
            if (shipIndex <= ships.Length - 2)
            {
                int[] shipPositions = shipScript.GetTileNumbers();
                AddPlayerShip(shipPositions);
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScriptOnline>();
                topText.text = "Coloque o próximo navio";
            }
            else
            {
                int[] shipPositions = shipScript.GetTileNumbers();
                AddPlayerShip(shipPositions);
                SendShips();
                photonView.RPC("JogadorPronto", RpcTarget.Others);
                clickable = false;
                if (!playersReady)
                {
                    topText.text = "Aguardando Jogador Finalizar";
                    photonView.RPC("Colisao", RpcTarget.Others);
                    photonView.RPC("LancarBomba", RpcTarget.Others);
                    photonView.RPC("JogadorPronto", RpcTarget.Others);
                    rotateBtn.gameObject.SetActive(false);
                    nextBtn.gameObject.SetActive(false);
                    setupComplete = true;

                }
                else
                {
                    rotateBtn.gameObject.SetActive(false);
                    nextBtn.gameObject.SetActive(false);
                    setupComplete = true;
                    clickable = false;
                    photonView.RPC("Colisao", RpcTarget.Others);
                    photonView.RPC("LancarBomba", RpcTarget.Others);
                    photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);

                }

            }
        }
    }


    public void TileClicked(GameObject tile)
    {
        List<GameObject> currentPlayerClickedTiles = clickedTiles;
        List<GameObject> opponentClickedTiles = clickedTiles2;

        if (setupComplete && turn)
        {
            if (!currentPlayerClickedTiles.Contains(tile) && !opponentClickedTiles.Contains(tile))
            {
                turn = false;
                Vector3 tilePos = tile.transform.position;
                tilePos.y += 15;
                if (currentPlayerTurn == jogadorId - 1)
                {
                    SpawnMissile(tilePos);
                }
                timerActive = false;
                if (jogadorId == 1)
                {
                    clickedTiles.Add(tile);
                }
                else if (jogadorId == 2)
                {
                    clickedTiles2.Add(tile);
                }
            }
        }
        else if (!setupComplete && clickable)
        {
            PlaceShip(tile);
            shipScript.SetClickedTile(tile);
        }
    }

    [PunRPC]
    private void ColorTilesByPlayer(int playerIndex)
    {
        int colorIndex = playerIndex;
        List<GameObject> currentPlayerClickedTiles = (playerIndex == 1) ? clickedTiles : clickedTiles2;

        foreach (TileScriptOnline tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScriptOnline>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);
        ships[shipIndex].transform.localPosition = newVec;


    }

    void RotateClicked()
    {
        shipScript.RotateShip();
    }

    public void CheckHit(GameObject tile, Vector3 hitPosition)
    {
        int hitCount = 0;
        if (tile.CompareTag("Ship"))
        {
            hitCount++;
            Vector3 fireOffset = new Vector3(0f, -1f, 0f);
            Vector3 firePosition = hitPosition + fireOffset;
            playerFires.Add(Instantiate(firePrefab, firePosition, Quaternion.identity));
            topText.text = "Acertou";
            audioSource.PlayOneShot(explosion);
            if (currentPlayerTurn == jogadorId - 1)
            {
                goagain = true;
            }
            else
            {
                goagain = false;

            }
            photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
            if (tile.GetComponent<ShipScriptOnline>().HitCheckSank())
            {
                if (currentPlayerTurn == jogadorId - 1)
                {
                    audioSource.PlayOneShot(death);
                    playerShipCount--;
                    playerShipText.text = playerShipCount.ToString();
                    if (playerShipCount == 0)
                    {
                        photonView.RPC("GameOver", RpcTarget.All);
                    }

                }
                else
                {
                    audioSource.PlayOneShot(hitSound);
                }
            }
        }
        else
        {
            int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
            foreach (int[] tileNumArray in InputShips)
            {
                if (tileNumArray.Contains(tileNum))
                {
                    for (int i = 0; i < tileNumArray.Length; i++)
                    {
                        if (tileNumArray[i] == tileNum)
                        {
                            tileNumArray[i] = -5;
                            hitCount++;
                        }
                        else if (tileNumArray[i] == -5)
                        {
                            hitCount++;
                        }
                    }
                    if (hitCount == tileNumArray.Length)
                    {
                        if (currentPlayerTurn == jogadorId - 1)
                        {
                            audioSource.PlayOneShot(death);
                            playerShipCount--;
                            playerShipText.text = playerShipCount.ToString();
                            topText.text = "Navio Afundado";
                            if (playerShipCount == 0)
                            {
                                photonView.RPC("GameOver", RpcTarget.All);
                            }

                        }
                        tile.GetComponent<TileScriptOnline>().SetTileColor(currentPlayerTurn, new Color32(68, 0, 0, 255));
                        tile.GetComponent<TileScriptOnline>().SwitchColors(currentPlayerTurn);
                        if (currentPlayerTurn == jogadorId - 1)
                        {
                            goagain = true;
                        }
                        else
                        {
                            goagain = false;

                        }
                        photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
                    }
                    else
                    {
                        audioSource.PlayOneShot(explosion);
                        topText.text = "Acertou";
                        tile.GetComponent<TileScriptOnline>().SetTileColor(currentPlayerTurn, new Color32(255, 0, 0, 255));
                        tile.GetComponent<TileScriptOnline>().SwitchColors(currentPlayerTurn);
                        if (currentPlayerTurn == jogadorId - 1)
                        {
                            goagain = true;
                        }
                        else
                        {
                            goagain = false;

                        }
                        photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
                    }
                    break;
                }
            }

            if (hitCount == 0)
            {
                tile.GetComponent<TileScriptOnline>().SetTileColor(currentPlayerTurn, new Color32(38, 57, 76, 255));
                tile.GetComponent<TileScriptOnline>().SwitchColors(currentPlayerTurn);
                topText.text = "Errou";
                audioSource.PlayOneShot(water);
                goagain = false;
                currentPlayerTurn = (currentPlayerTurn + 1) % Jogadores.Count;
                photonView.RPC("SwitchTurn", RpcTarget.All, jogadorId);
            }
        }
    }


    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileScriptOnline tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }


    [PunRPC]
    public void Enemywin()
    {
        SceneManager.LoadScene("YouWinbyGiveUp");
        PhotonNetwork.LeaveRoom();
        Destroy(gameObject);

    }

    [PunRPC]
    public void GameOver()
    {
        if (currentPlayerTurn == jogadorId - 1)
        {
            SceneManager.LoadScene("YouWinOnline");
            PhotonNetwork.LeaveRoom();
            Destroy(gameObject);
        }
        else
        {
            SceneManager.LoadScene("YouLoseOnline");
            PhotonNetwork.LeaveRoom();
            Destroy(gameObject);
        }
    }

    public void Quitoupartida()
    {
        SceneManager.LoadScene("YouLosebyGiveUp");
        photonView.RPC("Enemywin", RpcTarget.Others);
        PhotonNetwork.LeaveRoom();
        Destroy(gameObject);

    }


}

