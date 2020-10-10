using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class TCPTestClient : MonoBehaviour {  	
	#region private members 	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread;
    #endregion

    string inputstring;
    string receivedmessage;
    public static int[] numberint = new int[8];
    server_part ss;
    GameObject client;
    string text_send;
    string text_receive;
    Text messeage;
    GameObject server; string filepath;
    // Use this for initialization 	
    void Start () {
        ss = GameObject.FindWithTag("input").GetComponent<server_part>();
        inputstring = ss.getinput();
        ConnectToTcpServer(); 
        client = GameObject.FindWithTag("client"); server = GameObject.FindWithTag("server");
        messeage = GameObject.FindWithTag("server").GetComponent<Text>();
        text_send = "    Send coordinates of particles successfully!";
        text_receive = "           Received the image successfully!";
    }  	
	// Update is called once per frame
	void Update () {         
		if (Input.GetKeyDown(KeyCode.C)) {   
			SendMessage();
            StartCoroutine(sometimeactive());
           
        }     
	}  	
    IEnumerator sometimeactive()
    {

        client.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        client.SetActive(false);
        StartCoroutine(sometimeactive_server_receive());
    }
    IEnumerator sometimeactive_server_receive()
    {
        filepath = screenshot.filename;
        messeage.text = text_receive + "\n Path:    " + filepath;
        server.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        server.SetActive(false);
    }
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer () { 		
		try {  			
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();  		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}  	
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData() { 		
		try { 			
			socketConnection = new TcpClient("localhost", 8052);  			
			Byte[] bytes = new Byte[2048];  //for receiving instructions             
			while (true) { 				
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream()) { 					
					int length; 					
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 		
						var incommingData = new byte[length]; 						
						Array.Copy(bytes, 0, incommingData, 0, length); 						
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
                        receivedmessage = serverMessage;
                        transformfromstringtoint();
						Debug.Log("server message received as: " + serverMessage); 					
					} 				
				} 			
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}  	
    void transformfromstringtoint()
    {
        string[] fournumber = receivedmessage.Split(' ');
        for (int i = 0; i < fournumber.Length; i++)
        {
            numberint[i] = Convert.ToInt32(fournumber[i]);
           // Debug.Log(numberint[i]);
        }
    }

    private void OnDisable()
    {
        int timeout = 1;
        clientReceiveThread.Interrupt();
        clientReceiveThread.Join(timeout);
        socketConnection.Close();
       // clientReceiveThread.Join();
      
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    private void SendMessage() {         
		if (socketConnection == null) {             
			return;         
		}  		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {
                inputstring = ss.getinput();
                string clientMessage = inputstring;
                // Debug.Log(server_part.inputstring);
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = screenshot.bytes;			
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	} 
}

/* sending origiaal format

  // Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {
                inputstring = ss.getinput();
                string clientMessage = inputstring;
                Debug.Log(server_part.inputstring);
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent his message - should be received by server");     
 */
