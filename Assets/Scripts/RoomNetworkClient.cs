using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class RoomNetworkClient :  IConnectionCallbacks, IMatchmakingCallbacks
{
    // private Room _joinedRoom;


    public RoomNetworkClient(string gameVersion)
    {

        Debug.Log("constructor");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AddCallbackTarget(this);

    }

    ~RoomNetworkClient()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public bool CreateRoom(string roomName, RoomOptions options = null, TypedLobby lobby = null, string[] blockinguserId = null)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            return PhotonNetwork.CreateRoom(roomName, options, lobby, blockinguserId);
        return false;
    }

    public bool JoinRandomRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
            return PhotonNetwork.JoinRandomRoom();
        return false;
    }

    public bool FindRoom(string roomName)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            return PhotonNetwork.JoinRoom(roomName);
        return false;
    }

    public void ListRooms()
    {
        // if (PhotonNetwork.IsConnected)
        // PhotonNetwork.GetRoomList();
    }

    public bool ReJoinRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
            return PhotonNetwork.RejoinRoom("name");
        return false;
    }

    public bool LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
            return PhotonNetwork.LeaveRoom(false);
        return false;
    }

    public bool isJoinedRoom (){
        return PhotonNetwork.InRoom;
    }
    
    public Room GetJoinedRoom() {
        return PhotonNetwork.CurrentRoom;
    }



    #region IMatchmakingCallbacks

    public void OnCreatedRoom()
    {

    }

    public void OnJoinedRoom()
    {
        Debug.Log("JoinedRoom : " + PhotonNetwork.CurrentRoom.Name);
    }
    
    public void OnJoinRandomFailed(short returnCode, string message){}
    public void OnFriendListUpdate(List<FriendInfo> friendslist) {
    }

    public void OnCreateRoomFailed(short returnCode, string message){
    }

    public void OnJoinRoomFailed(short returnCode, string message){
    }

    public void OnLeftRoom(){
    }
    
    #endregion




    #region IConnectionCallbacks

    public  void OnConnected()	
    {
         Debug.Log("PUN  OnConnected() was called by PUN");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("PUN  OnConnectedToMaster() was called by PUN");

        // Debug.Log("Server IP  : " + PhotonNetwork.ServerAddress);
        // Debug.Log("Server Type  : " + PhotonNetwork.Server.ToString());
        // Debug.Log("RoomNum : " + PhotonNetwork.CountOfRooms);
        // Debug.Log("PlayersOnMaster : " + PhotonNetwork.CountOfPlayersOnMaster);
        // Debug.Log("region : " + PhotonNetwork.CloudRegion);
        // Debug.Log("isLobby : " + PhotonNetwork.InLobby);
        // Debug.Log("AppVersion : " + PhotonNetwork.AppVersion);
        // Debug.Log("GameVersion : " + PhotonNetwork.GameVersion);
        // Debug.Log("Server IP  : " + PhotonNetwork.ServerAddress);

    }
    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN  OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public void OnRegionListReceived(RegionHandler handler){

    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> dict){

    }

    public void OnCustomAuthenticationFailed(string error){

    }

    #endregion

}
