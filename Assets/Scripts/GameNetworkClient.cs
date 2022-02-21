using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public enum FLOWSTATE
{
    NOTJOIN, PAUSING, STAND_BY, INGAME
}

public enum INGAMESTATE
{
    READY, PLAYING, DROPPED, FINISHED, RESULT, TERMINATED
}

public enum EVENTCODE
{
    STARTGAME, SYNC, GENERATESYNCOBJ
}

public class GameNetworkClient : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region private
    private RoomNetworkClient _roomClient;
    private FLOWSTATE _gameFlowState = FLOWSTATE.NOTJOIN;
    private INGAMESTATE _inGameState = INGAMESTATE.READY;

    [SerializeField] private List<GameObject> _synchableObjectList;

    #endregion




    void Awake()
    {
        _roomClient = new RoomNetworkClient("testroom");
        SetUserName(System.Guid.NewGuid().ToString());
    }
    void OnDestroy()
    {
        _roomClient.LeaveRoom();
    }



    public void JoinGameRoom()
    {
        if (_gameFlowState == FLOWSTATE.NOTJOIN)
            _roomClient.JoinRandomRoom();
    }
    public void JoinorCreateGameRoom()
    {
        if (_gameFlowState == FLOWSTATE.NOTJOIN)
            _roomClient.CreateRoom("unityroom");
    }
    public void GetRoomName()
    {
        if (_gameFlowState != FLOWSTATE.NOTJOIN)
        {
            Debug.Log("RoomName is " + _roomClient.GetJoinedRoom().Name);
        }
    }

    public void CreateSynchronizedTransformObject(GameObject obj)
    {
        GenerateOwnedSyncTransformObjectWithTag(0);
    }


    public void StartGame()
    {
        StartGameInRoom();
    }



    void Update()
    {
        if (!_roomClient.isJoinedRoom())
            return;

        // SendSyncMessage("1");
    }


    public void SetUserName(string username)
    {
        PhotonNetwork.NickName = username;
    }

    public void GenerateOwnedSyncTransformObjectWithTag(int objectid)
    {
        GameObject obj = _synchableObjectList[objectid];
        GameObject copy = Instantiate<GameObject>(obj, obj.transform.position, obj.transform.rotation, obj.transform.parent);
        PhotonView photonView = copy.GetComponent<PhotonView>();


        if (PhotonNetwork.AllocateViewID(photonView))
        {
            object[] data = new object[]{
                objectid, copy.transform.position, copy.transform.rotation, photonView.ViewID 
            };
            
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions{
                Receivers = ReceiverGroup.Others
            };
            PhotonNetwork.RaiseEvent((byte)EVENTCODE.GENERATESYNCOBJ, data, raiseEventOptions, SendOptions.SendReliable);

            copy.SetActive(true);
        }
        else
        {
            Debug.LogError("Failed to allocate a ViewId.");
            Destroy(copy);
        }

    }



    public void SendSyncMessage(string message)
    {
        PhotonNetwork.RaiseEvent((byte)EVENTCODE.SYNC, message, null, SendOptions.SendReliable);
    }

    public void StartGameInRoom()
    {
        PhotonNetwork.RaiseEvent((byte)EVENTCODE.STARTGAME, null, null, SendOptions.SendReliable);
    }



    #region CustomCallbacks
    private void OnReadyStartGame()
    {
        _gameFlowState = FLOWSTATE.INGAME;
        _inGameState = INGAMESTATE.READY;
    }

    private void OnGetSyncMessage(ExitGames.Client.Photon.EventData photonEvent)
    {
        Debug.Log("test :" + photonEvent.CustomData);
    }

    private void OnGenerateSyncedObject(EventData photonEvent) {
        object[] data = (object[]) photonEvent.CustomData;
        GameObject obj =_synchableObjectList[(int)data[0]];
        GameObject copy = (GameObject) Instantiate( obj, (Vector3) data[1], (Quaternion) data[2], obj.transform.parent);
        PhotonView photonView = copy.GetComponent<PhotonView>();
        photonView.ViewID = (int) data[3];
        copy.SetActive(true);
    }

    #endregion 


    #region MonoBehaviourPunCallbacks Callbacks
    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        Debug.Log("CallBack  : " + photonEvent.Code);
        switch (photonEvent.Code)
        {
            case (byte)EVENTCODE.STARTGAME:
                OnReadyStartGame();
                break;
            case (byte)EVENTCODE.SYNC:
                OnGetSyncMessage(photonEvent);
                break;
            case (byte)EVENTCODE.GENERATESYNCOBJ:
                OnGenerateSyncedObject(photonEvent);
                break;
        }
    }

    public override void OnJoinedRoom()
    {
        _gameFlowState = FLOWSTATE.PAUSING;
        Debug.Log("JoinedRoom : " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName}が参加しました");
        foreach (var u in PhotonNetwork.PlayerList)
            Debug.Log(u.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName}が退出しました");
        foreach (var u in PhotonNetwork.PlayerList)
            Debug.Log(u.NickName);
    }

    #endregion MonoBehaviourPunCallbacks Callbacks
}
