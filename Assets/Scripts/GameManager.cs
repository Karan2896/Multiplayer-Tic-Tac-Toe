using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPun,IPunObservable
{
    [SerializeField]
    int[,] matrix = new int[3, 3] { { -5, -5, -5 }, { -5, -5, -5 }, { -5, -5, -5 } };
    bool p1turn = true, p2turn = false;
    [SerializeField]
    int value = 1;
    int winnerValue;
    [SerializeField]
    string sign = "X";
    [SerializeField]
    TextMeshProUGUI winnerText,turnText,player1Name,player2Name;
    [SerializeField]
    int tileCount;
    [SerializeField]
    public bool isMyturn;
    [SerializeField]
    GameObject blockPanel;
    bool startTimer = false;
    float timer = 5f;
    string p1="", p2="";
    //PhotonView photnview;

    private void Start()
    {
        //photnview = GetComponent<PhotonView>()


        if (PhotonNetwork.IsMasterClient)
        {
            isMyturn = true;
           // Debug.Log(gameObject.GetComponent<PhotonView>().Owner.NickName);
            Debug.Log(PhotonNetwork.NickName);
            player1Name.text = PhotonNetwork.NickName + " -- X";
            player2Name.text = PhotonNetwork.PlayerList[1].NickName + " -- O";
            p1 = PhotonNetwork.NickName;
            p2 = PhotonNetwork.PlayerList[1].NickName;
        }
        else
        {
            isMyturn = false;
            Debug.Log(PhotonNetwork.NickName);
            //Debug.Log(gameObject.GetComponent<PhotonView>().Owner.NickName);
            player2Name.text = PhotonNetwork.NickName+" -- O";
            player1Name.text = PhotonNetwork.PlayerList[0].NickName + " -- X";
            p2 = PhotonNetwork.NickName;
            p1 = PhotonNetwork.PlayerList[0].NickName;
        }


        for (int i = 0; i < transform.childCount; i++)
        {
            int x = i;
            transform.GetChild(i).GetComponent<Button>().onClick.AddListener(()=>ChangeMatrixValue(x));
            transform.GetChild(i).GetComponent<Button>().onClick.AddListener(()=> ChangeinOtherClients(x));
            transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        TurnToggle(isMyturn);


    }


    
    [PunRPC]
    private void TurnToggle(bool myturn)
    {
        //Debug.Log(myturn);
        if (myturn == false)
        {
            blockPanel.SetActive(true);
            turnText.text = "";
        }
        else
        {
            blockPanel.SetActive(false);
            turnText.text = "Your Turn";
        }
    }

    void ChangeinOtherClients(int index)
    {
        photonView.RPC("ChangeMatrixValue", RpcTarget.Others, index);
        photonView.RPC("TurnToggle", RpcTarget.Others, true);
        blockPanel.SetActive(true);
        turnText.text = "";
    }

    [PunRPC]
    public void ChangeMatrixValue(int index)
    {
        tileCount++;
        // Debug.Log(EventSystem.current.currentSelectedGameObject.tag);
        
        TextMeshProUGUI child = transform.GetChild(index).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (p1turn)
        {
            value = 1;
            sign = "X";
            p2turn = true;
            p1turn = false;
            child.color = new Color32(255, 255 ,255, 255);
        }
        else if (p2turn)
        {
            value = 2;
            sign = "O";
            p2turn = false;
            p1turn = true;
            child.color = new Color32(231, 76, 60, 255);
        }
        switch (transform.GetChild(index).tag)
        {
            case "00":
                matrix[0, 0] = value;
                break;
            case "01":
                matrix[0, 1] = value;
                break;
            case "02":
                matrix[0, 2] = value;
                break;
            case "10":
                matrix[1, 0] = value;
                break;
            case "11":
                matrix[1, 1] = value;
                break;
            case "12":
                matrix[1, 2] = value;
                break;
            case "20":
                matrix[2, 0] = value;
                break;
            case "21":
                matrix[2, 1] = value;
                break;
            case "22":
                matrix[2, 2] = value;
                break;

        }
        transform.GetChild(index).GetComponent<Button>().interactable = false;
        GameEvaluate();
        child.text = sign;
       

    }

    void GameEvaluate()
    {
        if (rowEvaluate() || columnEvaluate() || diagonalEvaluate())
        {
            if (winnerValue == 3)
            {
                //Debug.Log("Player 1 is Winner-------X");
                winnerText.text = p1+" is Winner";
                startTimer = true;
                StartCoroutine(RestartGame());

            }
            if (winnerValue == 6)
            {
                //Debug.Log("Player 2 is Winner-------O");
                winnerText.text = p2+" is Winner";
                startTimer = true;
                StartCoroutine(RestartGame());
            }
            //turnText.text = "";
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Button>().interactable = false;

            }
        }
        else if (tileCount == 9)
        {
            winnerText.text = "Game Draw";
            startTimer = true;
            StartCoroutine(RestartGame());
        }
        

    }

    private void Update()
    {
        if (startTimer)
        {
            timer -= Time.deltaTime;
            turnText.text = "New Game in " + (int)timer;
        }
            
    }

    IEnumerator RestartGame()
    {
       
        yield return new WaitForSeconds(5f);
        timer = 5f;
        startTimer = false;
        tileCount = 0;
        winnerValue = 0;
        turnText.text = "";
        winnerText.text = "";

        int[,] matrix2 = new int[3, 3] { { -5, -5, -5 }, { -5, -5, -5 }, { -5, -5, -5 } };
        matrix = matrix2;
        isMyturn = !isMyturn;
        TurnToggle(isMyturn);
        for (int i = 0; i < 9; i++)
        {
            transform.GetChild(i).GetComponent<Button>().interactable = true;
            transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

    }

    bool rowEvaluate()
    {
        int value = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                value += matrix[i, j];

            }
            if (value == 3 || value == 6)
            {
                winnerValue = value;
                Debug.Log("Returned from here");
                return true;
            }
            value = 0;
        }
        return false;
    }
    bool columnEvaluate()
    {
        int value = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                value += matrix[j, i];

            }
            if (value == 3 || value == 6)
            {
                winnerValue = value;
                Debug.Log("Returned from here");
                return true;
            }
            value = 0;
        }

        return false;
    }
    bool diagonalEvaluate()
    {
        int value = 0;

        value += matrix[0, 0] + matrix[1, 1] + matrix[2, 2];
        if (value == 3 || value == 6)
        {
            winnerValue = value;
            Debug.Log("Returned from here");
            return true;
        }
        value = 0;
        value += matrix[0, 2] + matrix[1, 1] + matrix[2, 0];
        if (value == 3 || value == 6)
        {
            winnerValue = value;
            Debug.Log("Returned from here");
            return true;
        }

        return false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        //if (stream.IsWriting)
        //{
        //    stream.SendNext(PhotonNetwork.NickName);
        //    //isMyturn = false;
        //    //turnText.text = "";
        //}
        //else
        //{
        //    Debug.Log((string)stream.ReceiveNext());
        //    if (string.IsNullOrEmpty(player1Name.text))
        //    {
        //        Debug.Log((string)stream.ReceiveNext());
        //        player1Name.text = (string)stream.ReceiveNext() + " -- O";
        //        p2 = (string)stream.ReceiveNext();
        //    }
        //    else if (string.IsNullOrEmpty(player2Name.text))
        //    {
        //        Debug.Log((string)stream.ReceiveNext());
        //        player2Name.text = (string)stream.ReceiveNext() + " -- X";
        //        p1 = (string)stream.ReceiveNext();
        //    }

        //}

    }

}
