
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Networking;
using System.Net.NetworkInformation;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {

    NetworkIdentity m_Identity;
    IPAddress ip;

    public string IP_address;
    public string subnetMask;
    public string defaultGateway;

	[SerializeField]
	int index;

    //public Text Ipinfo;

    public TextMeshProUGUI IPinfo;
    public TextMeshProUGUI sensorInfo;
    //SENSOR VARIABLES
	private UDP udpScript;
	Image spinner;
	Image ok;
	private RectTransform rectComponent;
	private float rotateSpeed = 200f;

	//IP Variables
	List<string> m_DropOptions = new List<string> {"1","2","3","4","5"};
	//This is the Dropdown
	//Dropdown m_Dropdown;
	public TMP_Dropdown address_Dropdown;




    // Use this for initialization
    void Start () {
		//Clear the old options of the Dropdown menu
		address_Dropdown.ClearOptions();
		//Add the options created in the List above
		address_Dropdown.AddOptions(m_DropOptions);

		ok = GameObject.Find ("OK").GetComponent<Image> ();
		spinner = GameObject.Find ("Spinner").GetComponent<Image> ();
        udpScript = GameObject.Find("UDP").GetComponent<UDP>();
        //Debug.Log("Connection : " + m_Identity.connectionToClient); // does not work
        //GetDefaultGateway();

        GetIP();
        SensorUpdate();

        Debug.Log("----- IP INFORMATION -----");
        Debug.Log("IP-adress:       " + ip);
        Debug.Log("Submask:         " + GetSubnetMask(ip));
        Debug.Log("Default Gateway: " + GetDefaultGateway());

        IP_address = GetIP();
        subnetMask = GetSubnetMask(ip).ToString();
        defaultGateway = GetDefaultGateway().ToString();
        
		UpdateIP();
        //DisplayGatewayAddresses(); works fine, but get whole list


    }
	
	// Update is called once per frame
	void Update () {
        SensorUpdate();

    }

	public string GetIP()
    {
        string strHostName = "";
        strHostName = System.Net.Dns.GetHostName();

        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
        IPAddress []addresses = ipEntry.AddressList;

		ip = addresses[index];
		//IP_address = ip.ToString ();

		return ip.ToString ();
        
        //foreach(IPAddress address in addresses)
        //{
        //    Debug.Log("Hostname: " +strHostName+"\n IP-address: "+addresses[0].ToString());
        //    //ip = address;
		//	ip = addresses[1];
        //    return address.ToString();
        //}
        //return null;
    }

    public static IPAddress GetDefaultGateway()
    {
        IPAddress result = null;

        var cards = NetworkInterface.GetAllNetworkInterfaces().ToList();
        if (cards.Any())
        {
            foreach (var card in cards)
            {
                var props = card.GetIPProperties();
                if (props == null)
                    continue;

                var gateways = props.GatewayAddresses;
                if (!gateways.Any())
                    continue;

                var gateway =
                    gateways.FirstOrDefault(g => g.Address.AddressFamily.ToString() == "InterNetwork");
                if (gateway == null)
                    continue;

                result = gateway.Address;
                break;
            };
        }

        return result;
    }

    public static void DisplayGatewayAddresses()
    {
        Console.WriteLine("Gateways");
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adapter in adapters)
        {
            IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
            GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
            if (addresses.Count > 0)
            {
                Debug.Log(adapter.Description);
                foreach (GatewayIPAddressInformation address in addresses)
                {
                    Debug.Log("  Gateway Address ......................... : {0}"+address.Address.ToString());
                }
                Console.WriteLine();
            }
        }
    }

    public static IPAddress GetSubnetMask(IPAddress address)
    {
        foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
            {
                if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (address.Equals(unicastIPAddressInformation.Address))
                    {
                        return unicastIPAddressInformation.IPv4Mask;
                    }
                }
            }
        }
        throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
    }

    public void UpdateIP()
    {
		IP_address = GetIP();
		subnetMask = GetSubnetMask(ip).ToString();
		defaultGateway = GetDefaultGateway().ToString();

		IPinfo.text = "IP-addresse: \n"+ IP_address + "\n\nSubmask: \n"+ subnetMask + "\n\nDefault Gateway: \n"+ GetDefaultGateway();
    }

    public void SensorUpdate()
    {
        if (udpScript.IsReceivingData())
        {
			spinner.enabled = false;
			ok.enabled = true;
            sensorInfo.text = "Sensor forbundet!\n\n"+udpScript.speed.ToString();
        }
        else
        {
            sensorInfo.text = "Sensor ikke forbundet...";
			spinner.enabled = true;
			ok.enabled = false;
			spinner.rectTransform.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
        }
       
    }

	public void IP_DropdownIndexUpdate()
	{
		index = address_Dropdown.value;
	}


}




