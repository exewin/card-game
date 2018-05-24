using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerS : NetworkBehaviour 
{
    [SyncVar]
	public GameObject messagePrefab;
	

	
	[SyncVar]
	public bool bul;
	
	[SyncVar]
	public int sendPos;
	[SyncVar]
	public int sendCard;
	[SyncVar]
	public bool doStuff;
	[SyncVar]
	public bool doStuff2;
	
	[SyncVar]
	public bool receiver;
	
	[SyncVar]
	public int play;
	[SyncVar]
	public int chara;
	
	public gameSystemNet gameSystemScript;
	public gameMenu gameMenuScript;
	
	public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
    }
	
	public void Start()
	{
		
		gameSystemScript=GetComponent<gameSystemNet>();
		gameSystemScript.multiplayerPlayer=this;
		gameMenuScript=gameSystemScript.gameMenuScript;
		play=gameMenuScript.play;
		chara=gameMenuScript.chara;
		gameSystemScript.players[play].playerName=gameMenuScript.customName+" The "+gameMenuScript.prefChars[chara].playerName;
		gameSystemScript.players[play].portrait=gameMenuScript.prefChars[chara].portrait;
		gameSystemScript.players[play].maxHp=gameMenuScript.prefChars[chara].hp;
		gameSystemScript.players[play].maxMana=gameMenuScript.prefChars[chara].mana;
		gameSystemScript.players[play].gainMana=gameMenuScript.prefChars[chara].gainMana;
		gameSystemScript.players[play].deck=new int[gameMenuScript.prefChars[chara].deck.Length];
		gameSystemScript.players[play].cpuControl=false;
		for(int z=0;z<gameMenuScript.prefChars[chara].deck.Length;z++)
			{
				gameSystemScript.players[play].deck[z]=gameMenuScript.prefChars[chara].deck[z];
			}	
		gameSystemScript.demoState=false;
		gameSystemScript.onlineGame=true;
		gameSystemScript.OnEnable();
		gameObject.SetActive(true);
		

		if(play==1)
		{
			doStuff2=true;
			receiver=true;
		}
		
	}
	
    public void TakeCard(int p, int c, bool b)
	{
		if(!isServer)
			return;
		
		gameSystemScript.takePos=p;
		gameSystemScript.takeCard=c;
		gameSystemScript.CmdCanOnline(b);
    }
	
	
	
	public void TakePlayerInfo(int xchar, string customName, int[] importedDeck)
	{
		if(!isServer)
			return;
		
		gameSystemScript.RpcUpStats(xchar, customName, importedDeck);
		

		
		if(!receiver&&play!=1)
		{
			doStuff2=true;
			receiver=true;
			Debug.Log("info got");
		}
	}
	
	
	[Command]
    public void CmdFire(int p, int c, bool b)
    {
       // This [Command] code is run on the server!

       // create the message object locally
		var message = (GameObject)Instantiate(messagePrefab, transform.position - transform.forward, Quaternion.identity);
		message.GetComponent<Message>().Stats(p,c,b);
		
		message.GetComponent<Rigidbody>().velocity = -transform.forward*4;
       
       // spawn the message on the clients
       NetworkServer.Spawn(message);
       
       // when the message is destroyed on the server it will automaticaly be destroyed on clients
       Destroy(message, 2.0f);
    }
	
	[Command]
    public void CmdStartInfo(int a, string b, int[] c)
    {
       // This [Command] code is run on the server!

       // create the message object locally
		var message = (GameObject)Instantiate(messagePrefab, transform.position - transform.forward, Quaternion.identity);
		message.GetComponent<Message>().StartInfo(a,b,c);
		
		message.GetComponent<Rigidbody>().velocity = -transform.forward*4;
       
       // spawn the message on the clients
       NetworkServer.Spawn(message);
       
       // when the message is destroyed on the server it will automaticaly be destroyed on clients
       Destroy(message, 2.0f);
    }


	void Update()
    {
		
		if (!isLocalPlayer)
            return;
		
		if(doStuff)
		{
			CmdFire(sendPos,sendCard,bul);
			doStuff=false;
		}
		if(doStuff2)
		{
			CmdStartInfo(chara,gameSystemScript.players[play].playerName,gameSystemScript.players[play].deck);
			doStuff2=false;
		}


	}
	
	
}