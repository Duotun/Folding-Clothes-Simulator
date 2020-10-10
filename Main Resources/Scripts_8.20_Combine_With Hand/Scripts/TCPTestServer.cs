using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;
using UnityEngine.UI;
public class TCPTestServer : MonoBehaviour {  	
	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener; 
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;  	
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient;
    #endregion

    string inputstring;
    string filepath;
    server_part ss;
    GameObject server;
    string text_send;
    string text_receive;
    Text messeage;
    // Use this for initialization
    void Start () {
        // Start TcpServer background thread 		
        ss = GameObject.FindWithTag("input").GetComponent<server_part>();
        inputstring = ss.getinput();
        //saveasimage();
		tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start();
        server = GameObject.FindWithTag("server");
        messeage = GameObject.FindWithTag("server").GetComponent<Text>();
        text_send = "    Send coordinates of particles successfully!";
        text_receive = "   Received the image successfully!";
    }  	
	
	// Update is called once per frame
	void Update () { 		
		if (Input.GetKeyDown(KeyCode.S)) {             
			SendMessage();
            StartCoroutine(sometimeactive());
		} 	
	}

    IEnumerator sometimeactive()
    {
        messeage.text = text_send;
        server.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        server.SetActive(false);
    }

    void saveasimage(Byte[] bytes)
    {
        filepath = screenshot.filename;
        System.IO.File.WriteAllBytes(filepath, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filepath));
        //Debug.Log(filepath);
    }
	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests () { 		
		try { 			
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8052); 			
			tcpListener.Start();              
			Debug.Log("Server is listening");              
			Byte[] bytes = new Byte[36000];  			
			while (true) { 				
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 						
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
							var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);
                            saveasimage(bytes);
                            // Convert byte array to string message. 							
                            //string clientMessage = Encoding.ASCII.GetString(incommingData); 							
                            //Debug.Log("client message received.");
                           
						} 					
					} 				
				} 			
			} 		
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString()); 		
		}     
	}
    private void OnDisable()
    {
        int timeout = 0;
        //Debug.Log("fuck");
        tcpListenerThread.Interrupt();
        tcpListenerThread.Join(timeout);
        connectedTcpClient.Close();    //this must be closed every time to actally kill the thread for the tcp connection
        tcpListener.Stop();
     
    }
    /// <summary> 	
    /// Send message to client using socket connection. 	
    /// </summary> 	
    private void SendMessage() { 		
		if (connectedTcpClient == null) {             
			return;         
		}  		
		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = connectedTcpClient.GetStream(); 			
			if (stream.CanWrite) {
                inputstring = ss.getinput();
                string serverMessage = inputstring;
                // Convert string message to byte array.                 
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage); 				
				// Write byte array to socketConnection stream.               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
				Debug.Log("Server sent his message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
	} 
}