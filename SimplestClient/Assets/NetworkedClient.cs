using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491; //socket being used
    byte error;
    bool isConnected = false;
    int ourClientID;

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
            SendMessageToHost("Hello from client"); //press S to get Msg

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error); //Passes in HostID, connectionID, ChannelID,datasize and error as referance. Takes in recbuffer and buffersize normally

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig(); //adding channel to config and returning unique ID of the channel
            reliableChannelID = config.AddChannel(QosType.Reliable);//garenteed to be delivered, order not garenteed
            unreliableChannelID = config.AddChannel(QosType.Unreliable); //nothing garenteed
            HostTopology topology = new HostTopology(config, maxConnections); //last step in creating a connection, letting the servert know what configuration will be used (how many default connections, what special connections)
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID); //Adds host based on Networking.HostTopology. Returns the Host ID.


            connectionID = NetworkTransport.Connect(hostID, "192.168.2.39", socketPort, 0, out error); // server is local on network (I did my local network)

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }
    
    public void Disconnect() //disconnects connection
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToHost(string msg) //sends mdg to host
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error); //sends msg as a bugger of bytes.  msg.Length * sizeof(char) = the number of chars * the bytes each char takes
    }

    private void ProcessRecievedMsg(string msg, int id) //processes msg from buffer to string
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
    }

    public bool IsConnected()
    {
        return isConnected;
    }


}
public static class ClientToServerSignifiers 
{
    public const int CreateAccount = 1;
    public const int LoginAccount = 2;

}
public static class ServerToClientSignifiers
{
    public const int LoginComplete = 1;
    public const int LoginFailed = 2;

    public const int AccountCreationComplete = 3;
    public const int AccountCreationFailed = 4;

}
