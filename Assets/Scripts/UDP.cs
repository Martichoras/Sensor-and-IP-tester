/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using System.Globalization; // Includes Cultureinfo

public class UDP : MonoBehaviour
{

    // receiving Thread
    Thread receiveThread;

    // udpclient object
    UdpClient client;

    // public
    // public string IP = "127.0.0.1"; default local
    public int port; // define > init

    // infos
    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = ""; // clean up this from time to time!
    public int speed;

    // start from shell
    public static void Main()
    {
        UDP receiveObj = new UDP();
        receiveObj.init();

        string text = "";
        do
        {
            text = Console.ReadLine();
        }
        while (!text.Equals("exit"));
    }
    // start from unity3d
    public void Start()
    {

        init();
    }

    // init
    private void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define port
        port = 5000;

        // status
        print("Sending to 127.0.0.1 : " + port);
        print("Test-Sending to this Port: nc -u 127.0.0.1  " + port + "");


        // ----------------------------
        // Abhören
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread für den Empfang eingehender Nachrichten erstellen.
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(port);
        while (true)
        {

            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);



                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);


                //string input = "12ACD";

                // ORIGINAL, USED TO WORK!
                //int.TryParse(text, out speed); 

                //This works in 2017
                int.TryParse(text, System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat, out speed);


                //int.TryParse(text, System.Globalization.CultureInfo.CreateSpecificCulture("en-US") CultureInfo.CurrentCulture.NumberFormat, out speed);

                //speed = int.Parse(text);

                lastReceivedUDPPacket = text;


                allReceivedUDPPackets = allReceivedUDPPackets + text;

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        //int.TryParse(lastReceivedUDPPacket, System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat, out speed);
        return lastReceivedUDPPacket;
    }

    void OnApplicationQuit()
    { // from https://answers.unity.com/questions/293734/threading-with-udpclient-editor-freezing.html
        receiveThread.Abort();
        if (client != null)
            client.Close();
        Debug.Log("Closing networking client...");
    }

    public bool IsReceivingData()
    {
        Debug.Log("Button pressed. Awaiting client:");

        if (String.IsNullOrEmpty(lastReceivedUDPPacket))
        {
            Debug.Log("Sensor is not connected");
            return false;
            
        }
        else {
        return true;
        }
    }

}

