using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LoginManager : MonoBehaviourPunCallbacks
{

    [SerializeField] TextMeshProUGUI waitText = null;
    [SerializeField] TMP_InputField p_name = null;
    [SerializeField] Button playBtn = null;

    private bool isConnecting = false;

    private const int maxPlayersinRoom = 2;
    private const string GameVersion = "0.1";

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        CheckName();
        playBtn.interactable = false;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CheckName()
    {
        playBtn.interactable = !string.IsNullOrEmpty(p_name.text);
       
    }

    public void FindOpponent()
    {
        PlayerPrefs.SetString("playername", p_name.text);
        PhotonNetwork.NickName = p_name.text;
        Debug.Log(PhotonNetwork.NickName);
        isConnecting = true;
        waitText.text = "Searching...";
        playBtn.interactable = false;
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        if (isConnecting)
            PhotonNetwork.JoinRandomRoom();

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected due to " + cause);
        playBtn.interactable = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersinRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined success");

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerCount != maxPlayersinRoom)
        {
            waitText.text = "Waiting for opponent";
            playBtn.interactable = false;
        }
        else
        {
            Debug.Log("Begin");
            waitText.text = "Opponent found";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersinRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            PhotonNetwork.LoadLevel("Game");

        }
    }

}
