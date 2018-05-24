using UnityEngine;
using System.IO;
using System.ComponentModel;
using UnityEngine.Networking;

public class gameMenu : MonoBehaviour
{
	
	public string version;
	
	#region variables
	
	//RESOLUTION_HELPER
	float realHeight;
	float heightInFloat;
	int fontSizer;
	float fontNormalizer;
	
	//STYLES
	public GUIStyle guiStyleNormal;
	public GUIStyle guiStyle;
	public GUIStyle guiStyleLabel;
	public GUIStyle guiStyleBox;
	public GUIStyle guiStyleArea;
	
	public Texture2D[] menuHelpers;
	public Texture2D bg;
	public Texture2D[] interfaceImages;
	
	//OTHER
	int menu;
	int keyboardChoise;
	
	bool fullS;
	bool fpsS;
	
	public GameObject gameSys;
	public gameSystem gameSystemScript;
	public gameSystemNet gameSystemScriptNet;
	public GameObject fpsObj;
	
	int x;
	int y;
	
	int drawCounter;
	
	public bool showMenu;
	
	//CREDITS_HELP
	float scroll;
	public string[] credits;
	
	bool chosenDemo;
	public Vector2 scrollPosition = Vector2.zero;
	int asd;
	int numOMoves;
	
	//MULTIPLAYER
	public NetworkManager manager;
	bool m_ShowServer;
	public int play;
	public int chara;
	bool noConnection;
	
	//TORUNAMENT
	//TournamentText
	string realTournamentText;
	[TextArea (10,10)]
	public string tournamentText;
	float textCounter;
	
	public string customName;
	
	public string tourName;
	
	public string[] fNames;
	public string[] mNames;
	
	[HideInInspector]
	public int[] result;
	
	
	public string[] tourNames;
	public int[] tourPoints;
	public int[] tourSkill;
	public int tourXChar;
	public int curWeek;
	
	
	public int[]tourSorted;

	string faststring;
	
	bool tournamentMatch;
	
	[System.Serializable]
	public class week
	{
		public int[]schedule;
	}
	public week[] weeks;
	
	
	//PREDEFINED CHARACHTERS
	[System.Serializable]
	public class prefChar
	{
		public string playerName;
		public Texture2D portrait;
		public int hp;
		public int[] deck;
		public int mana;
		//basicMana is default non-bonus gainMana
		public int gainMana;
		public string customDeck;
	}
	public prefChar[] prefChars;
	
	float xpos;
	public int[] xchar;
	int[] aiOption;
	
	public AudioClip[] sounds;
	
	AudioSource audio;
	
	int rand;
	#endregion
	
	#region startThings
	//CHECK_PREFS
	void Awake()
	{
		showMenu=true;
		xchar = new int[2];
		aiOption = new int[2];
		gameSystemScript=gameSys.GetComponent<gameSystem>();
		if(PlayerPrefs.HasKey("fps"))
		{
			if(PlayerPrefs.GetInt("fps")==1)
			{
				fpsS=true;
				Fps(true);
			}
			else
			{
				fpsS=false;
				Fps(false);
			}
		}
		
		manager = GetComponent<NetworkManager>();
		
	}
	
	void Start()
	{
		showMenu=true;
		audio = GetComponent<AudioSource>();
		if(Screen.fullScreen)
			fullS=false;
		else
			fullS=true;
		
	}
	
	#endregion
	
	void OnGUI() 
	{
		
		if(!showMenu)
			return;
		
		#region proportion
		
		//HEIGHT_PROPORTION
		heightInFloat=Screen.height;
		realHeight=heightInFloat/768;
		
		int bdr = (int) (9*realHeight);
		guiStyleNormal.border=new RectOffset(bdr,bdr,bdr,bdr);
		
		fontNormalizer = realHeight * 10;
		fontSizer = (int)fontNormalizer;
		
		
		guiStyleNormal.alignment = TextAnchor.MiddleCenter;
		guiStyleLabel.alignment = TextAnchor.UpperCenter;
		
		guiStyle.fixedWidth=13*realHeight;
		guiStyle.fixedHeight=13*realHeight;
		
		#endregion
		
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),bg);
		
		//MAIN_MENU
		if(menu==0)
		{
			//ver
			guiStyleLabel.alignment = TextAnchor.LowerRight;
			guiStyleLabel.fontSize = 17 * fontSizer/10;
			GUI.Label(new Rect(Screen.width-400,Screen.height-25,400,25),version,guiStyleLabel);
			
			//logo
			GUI.DrawTexture(new Rect(45*realHeight,45*realHeight,455*realHeight,243*realHeight),interfaceImages[3]);

			
			guiStyleNormal.fontSize = 17 * fontSizer/10;
			GUI.Box(new Rect(Screen.width/2-(150*realHeight)/2,Screen.height/2-25*realHeight,150*realHeight,255*realHeight),"",guiStyleBox);
			
			

			
			//DUEL
			if(GUI.Button(new Rect(Screen.width/2-(125*realHeight)/2,Screen.height/2-(25*realHeight)/2+0*realHeight,125*realHeight,30*realHeight),new GUIContent ("Duel", "1"),guiStyleNormal))
			{
				menu=1;
				Click();
			}
			
			//TOURNAMENT
			if(GUI.Button(new Rect(Screen.width/2-(125*realHeight)/2,Screen.height/2-(25*realHeight)/2+40*realHeight,125*realHeight,30*realHeight),new GUIContent ("Tournament", "2"),guiStyleNormal))
			{
				menu=2;
				Click();
			}
			
			//MULTIPLAYER
			if(GUI.Button(new Rect(Screen.width/2-(125*realHeight)/2,Screen.height/2-(25*realHeight)/2+80*realHeight,125*realHeight,30*realHeight),new GUIContent ("Multiplayer", "3"),guiStyleNormal))
			{
				menu=3;
				Click();
			}
			
			//OPEN DEMO
			if(GUI.Button(new Rect(Screen.width/2-(125*realHeight)/2,Screen.height/2-(25*realHeight)/2+120*realHeight,125*realHeight,30*realHeight),new GUIContent ("Demo", "4"),guiStyleNormal))
			{
				menu=4;
				Click();
			}
			
			//CREDITS
			if(GUI.Button(new Rect(Screen.width/2-(125*realHeight)/2,Screen.height/2-(25*realHeight)/2+160*realHeight,125*realHeight,30*realHeight),new GUIContent ("Credits", "5"),guiStyleNormal))
			{
				scroll=0;
				menu=5;
				Click();
			}
			
			//EXIT
			if(GUI.Button(new Rect(Screen.width/2-(125*realHeight)/2,Screen.height/2-(25*realHeight)/2+200*realHeight,125*realHeight,30*realHeight),new GUIContent ("Exit", "6"),guiStyleNormal))
			{
				rand=Random.Range(0,7);
				Click();
				menu=6;
			}
			for(int f=1;f<7;f++)
				if(GUI.tooltip==""+f)
				{
					GUI.DrawTexture(new Rect(Screen.width/2-(150*realHeight)/2,Screen.height/2-175*realHeight,150*realHeight,150*realHeight),menuHelpers[f-1]);
					string tooltipText="Error text";
					if(GUI.tooltip==""+1)
						tooltipText="Play with CPU or in Hot-Seat mode with another human.";
					else if(GUI.tooltip==""+2)
						tooltipText="Take part in great tournament.";
					else if(GUI.tooltip==""+3)
						tooltipText="Play over LAN or Internet.";
					else if(GUI.tooltip==""+4)
						tooltipText="Watch duel replays.";
					else if(GUI.tooltip==""+5)
						tooltipText="Find out who created this game.";
					else if(GUI.tooltip==""+6)
						tooltipText="Leave game.";
					
					guiStyleLabel.alignment = TextAnchor.MiddleCenter;
					GUI.Label(new Rect(Screen.width/2-(300*realHeight)/2,Screen.height-175*realHeight,300*realHeight,150*realHeight),tooltipText,guiStyleLabel);	
				}
			
		}
		

		//DUEL&HOTSEAT
		else if(menu==1)
		{
			guiStyleLabel.fontSize = 22 * fontSizer/10;
			guiStyleNormal.fontSize = 17 * fontSizer/10;
			GUI.Label(new Rect(Screen.width/2-200*realHeight,5*realHeight,400*realHeight,30*realHeight),"Click on portrait to change character",guiStyleLabel);
			guiStyleLabel.fontSize = 45 * fontSizer/10;
			GUI.Label(new Rect(Screen.width/2-50*realHeight,140*realHeight,100*realHeight,100*realHeight),"VS",guiStyleLabel);
			guiStyleLabel.fontSize = 22 * fontSizer/10;
			
			for(int c=0;c<2;c++)
			{
				//HELPFUL VARIABLES
				if(c==0)
				{
					guiStyleLabel.alignment = TextAnchor.UpperRight;
					xpos = Screen.width/3-(128*realHeight)/2;
				}
				else
				{
					guiStyleLabel.alignment = TextAnchor.UpperLeft;
					xpos = Screen.width-Screen.width/3-(128*realHeight)/2;
				}
				
				//INFO
				GUI.Label(new Rect(xpos,75*realHeight,128*realHeight,30*realHeight),prefChars[xchar[c]].playerName,guiStyleLabel);
				GUI.DrawTexture(new Rect(xpos,105*realHeight,128*realHeight,128*realHeight),prefChars[xchar[c]].portrait);
				
				guiStyleLabel.wordWrap=false;
				GUI.Label(new Rect(xpos - 150*realHeight + c*300*realHeight,100*realHeight,128*realHeight,30*realHeight),"HP: "+prefChars[xchar[c]].hp,guiStyleLabel);
				GUI.Label(new Rect(xpos - 150*realHeight + c*300*realHeight,125*realHeight,128*realHeight,30*realHeight),"MP: "+prefChars[xchar[c]].mana,guiStyleLabel);
				GUI.Label(new Rect(xpos - 150*realHeight + c*300*realHeight,150*realHeight,128*realHeight,30*realHeight),"Regeneration: "+prefChars[xchar[c]].gainMana,guiStyleLabel);
				GUI.Label(new Rect(xpos - 150*realHeight + c*300*realHeight,175*realHeight,128*realHeight,30*realHeight),"Deck: "+prefChars[xchar[c]].deck.Length+"\n("+prefChars[xchar[c]].customDeck+")",guiStyleLabel);
				
				
				
				//CPU&HUMAN_BUTTONS
				int middleOffset = 300;
				if(aiOption[c]==0)
					GUI.color = new Color(1,0,1);
				if(GUI.Button(new Rect(Screen.width/2-75*realHeight/2 + c*middleOffset*realHeight + (c-1)*middleOffset*realHeight,250*realHeight,75*realHeight,30*realHeight),"Human",guiStyleNormal))
				{
					Click();
					aiOption[c]=0;
				}
				GUI.color = new Color(1,1,1);
				
				
				middleOffset = 225;
				if(aiOption[c]!=0)
					GUI.color = new Color(1,0,1);
				if(GUI.Button(new Rect(Screen.width/2-75*realHeight/2 + c*middleOffset*realHeight + (c-1)*middleOffset*realHeight,250*realHeight,75*realHeight,30*realHeight),"CPU",guiStyleNormal))
				{
					Click();
					aiOption[c]=1;
				}
				GUI.color = new Color(1,1,1);
				
				//SLIDER
				if(aiOption[c]!=0)
				{
					aiOption[c] = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(Screen.width/2-75*realHeight/2 + c*middleOffset*realHeight + (c-1)*middleOffset*realHeight,280*realHeight,75*realHeight,15*realHeight), aiOption[c], 1, 3,guiStyleNormal,guiStyle));
					GUI.Button(new Rect(Screen.width/2-75*realHeight/2 + c*middleOffset*realHeight + (c-1)*middleOffset*realHeight,280*realHeight,75*realHeight,12*realHeight),"",guiStyleLabel);
				}
				
				//DIFFICULTIES_LABEL
				if(aiOption[c]==1)
					GUI.Label(new Rect(Screen.width/2-75*realHeight/2 + c*middleOffset*realHeight + (c-1)*middleOffset*realHeight,300*realHeight,75*realHeight,30*realHeight),"Easy",guiStyleLabel);
				else if(aiOption[c]==2)
					GUI.Label(new Rect(Screen.width/2-75*realHeight/2 + c*middleOffset*realHeight + (c-1)*middleOffset*realHeight,300*realHeight,75*realHeight,30*realHeight),"Normal",guiStyleLabel);
				else if(aiOption[c]==3)
					GUI.Label(new Rect(Screen.width/2-75*realHeight/2 + c*middleOffset*realHeight + (c-1)*middleOffset*realHeight,300*realHeight,75*realHeight,30*realHeight),"Hard",guiStyleLabel);
			
			
				//PORTRAIT
				if(GUI.Button(new Rect(xpos,105*realHeight,128*realHeight,128*realHeight),"",guiStyleNormal))
				{
					Click();
					if(Input.GetMouseButtonUp(0))
					{
						xchar[c]++;
						if(xchar[c]>prefChars.Length-2)
							xchar[c]=0;
					}
					else if(Input.GetMouseButtonUp(1))
					{
						xchar[c]--;
						if(xchar[c]<0)
							xchar[c]=prefChars.Length-2;
							
					}
				}
				
			}
			
			
			//DRAWS
			guiStyleLabel.fontSize = 17 * fontSizer/10;
			GUI.Label(new Rect(Screen.width/2+235*realHeight,425*realHeight,30*realHeight,30*realHeight),"Moves limit",guiStyleLabel);
			if(drawCounter!=0)
			{
				drawCounter = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(Screen.width/2+200*realHeight,465*realHeight,125*realHeight,15*realHeight), drawCounter, 50, 250,guiStyleNormal,guiStyle));
				GUI.Button(new Rect(Screen.width/2+200*realHeight,465*realHeight,125*realHeight,12*realHeight),"",guiStyleLabel);
				GUI.Label(new Rect(Screen.width/2+235*realHeight,485*realHeight,30*realHeight,30*realHeight),""+drawCounter,guiStyleLabel);
				
				if(GUI.Button(new Rect(Screen.width/2+200*realHeight,425*realHeight,30*realHeight,30*realHeight),"X",guiStyleNormal))
				{
					Click();
					drawCounter=0;
				}
			}
			else
			{
				if(GUI.Button(new Rect(Screen.width/2+200*realHeight,425*realHeight,30*realHeight,30*realHeight),"",guiStyleNormal))
				{
					Click();
					drawCounter=100;
				}
			}
			
			
			//CONFIRM
			if(GUI.Button(new Rect(Screen.width/2-(100*realHeight)/2,375*realHeight,100*realHeight,30*realHeight),"Confirm",guiStyleNormal))
			{
				tournamentMatch=false;
				Click();
				Confirm();
				Swap();
			}
		}

		//NEW TOURNAMENT&MULTIPLAYER
		else if(menu==2||menu==3)
		{
			guiStyleLabel.fontSize = 22 * fontSizer/10;
			guiStyleNormal.fontSize = 17 * fontSizer/10;
			GUI.Label(new Rect(Screen.width/2-200*realHeight,5*realHeight,400*realHeight,30*realHeight),"Click on portrait to change character",guiStyleLabel);
			guiStyleArea.fontSize = 17 * fontSizer/10;
				
				//INFO
				GUI.Label(new Rect(Screen.width/2-64*realHeight,75*realHeight,128*realHeight,30*realHeight),prefChars[xchar[0]].playerName,guiStyleLabel);
				GUI.DrawTexture(new Rect(Screen.width/2-64*realHeight,105*realHeight,128*realHeight,128*realHeight),prefChars[xchar[0]].portrait);
				
				guiStyleLabel.wordWrap=false;
				GUI.Label(new Rect(Screen.width/2-64*realHeight,240*realHeight,128*realHeight,30*realHeight),"Max health points: "+prefChars[xchar[0]].hp,guiStyleLabel);
				GUI.Label(new Rect(Screen.width/2-64*realHeight,265*realHeight,128*realHeight,30*realHeight),"Max mana points: "+prefChars[xchar[0]].mana,guiStyleLabel);
				GUI.Label(new Rect(Screen.width/2-64*realHeight,290*realHeight,128*realHeight,30*realHeight),"Mana per round: "+prefChars[xchar[0]].gainMana,guiStyleLabel);
				GUI.Label(new Rect(Screen.width/2-64*realHeight,315*realHeight,128*realHeight,30*realHeight),"Deck: "+prefChars[xchar[0]].customDeck+" cards",guiStyleLabel);
				
				GUI.Label(new Rect(Screen.width/2-128*realHeight,365*realHeight,128*realHeight,30*realHeight),"Enter your name: ",guiStyleLabel);
				customName = GUI.TextField(new Rect(Screen.width/2+18*realHeight,365*realHeight,155*realHeight,30*realHeight), customName,10,guiStyleArea);
			
			
				//PORTRAIT
				if(GUI.Button(new Rect(Screen.width/2-64*realHeight,105*realHeight,128*realHeight,128*realHeight),"",guiStyleNormal))
				{
					Click();
					if(Input.GetMouseButtonUp(0))
					{
						xchar[0]++;
						if(xchar[0]>prefChars.Length-2)
							xchar[0]=0;
					}
					else if(Input.GetMouseButtonUp(1))
					{
						xchar[0]--;
						if(xchar[0]<0)
							xchar[0]=prefChars.Length-2;
							
					}
				}
				
			
			
			//CONFIRM
			if(menu==2)
			{		
				//SAVED GAMES
				if(GUI.Button(new Rect(5*realHeight,Screen.height-105*realHeight,100*realHeight,100*realHeight),interfaceImages[4],guiStyleNormal))
				{
					Click();
					menu=13;
				}
			
			
			
				if(GUI.Button(new Rect(Screen.width/2-(100*realHeight)/2,420*realHeight,100*realHeight,30*realHeight),"Confirm",guiStyleNormal))
				{
					Click();
					menu=7;
				}
			}
		}
		
		//MULTIPLAYER
		if(menu==3)
		{
			
			GUI.Box(new Rect(Screen.width/2-380*realHeight, 485*realHeight,370*realHeight,250*realHeight),"",guiStyleBox);
			GUI.Box(new Rect(Screen.width/2+10*realHeight, 485*realHeight,340*realHeight,120*realHeight),"",guiStyleBox);
			GUI.Box(new Rect(Screen.width/2+10*realHeight, 615*realHeight,340*realHeight,120*realHeight),"",guiStyleBox);
			if (GUI.Button(new Rect(Screen.width/2+10*realHeight, 615*realHeight,340*realHeight,120*realHeight), "",guiStyleNormal))
			{
				Click();
				manager.StartMatchMaker();
			}
			if (GUI.Button(new Rect(Screen.width/2+10*realHeight, 485*realHeight,340*realHeight,120*realHeight), "",guiStyleNormal))
			{
				Click();
				manager.StopMatchMaker();
			}
			
			
			GUI.DrawTexture(new Rect(Screen.width/2+35*realHeight, 505*realHeight,150*realHeight,84*realHeight),interfaceImages[0]);
			GUI.DrawTexture(new Rect(Screen.width/2+185*realHeight, 505*realHeight,150*realHeight,84*realHeight),interfaceImages[0]);
			
			GUI.DrawTexture(new Rect(Screen.width/2+35*realHeight, 635*realHeight,150*realHeight,84*realHeight),interfaceImages[0]);
			GUI.DrawTexture(new Rect(Screen.width/2+185*realHeight, 635*realHeight,150*realHeight,84*realHeight),interfaceImages[1]);

			
			GUI.Label(new Rect(Screen.width/2+10*realHeight, 435*realHeight,370*realHeight,250*realHeight),"Connection Type",guiStyleLabel);
		
            noConnection = (manager.client == null || manager.client.connection == null || manager.client.connection.connectionId == -1);
			
            if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
            {
                if (noConnection)
                {
					

                    if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                    {
						GUI.Label(new Rect(Screen.width/2-380*realHeight, 435*realHeight,370*realHeight,250*realHeight),"LAN Options",guiStyleLabel);
						
						GUI.Label(new Rect(Screen.width/2-380*realHeight, 510*realHeight,370*realHeight,250*realHeight),"Your IP Address: "+Network.player.ipAddress,guiStyleLabel);
                        if (GUI.Button(new Rect(Screen.width/2-265*realHeight, 550*realHeight, 150*realHeight, 30*realHeight), "Start server",guiStyleNormal))
                        {
							Click();
							SwapOnline(0);
                            manager.StartHost();
							
                        }
                    }

                    if (GUI.Button(new Rect(Screen.width/2-265*realHeight, 680*realHeight, 150*realHeight, 30*realHeight), "Join server",guiStyleNormal))
                    {
						Click();
						SwapOnline(1);
                        manager.StartClient();
						
                    }
					
					GUI.Label(new Rect(Screen.width/2-350*realHeight, 630*realHeight, 150*realHeight, 30*realHeight),"Enter address:",guiStyleLabel);
                    manager.networkAddress = GUI.TextField(new Rect(Screen.width/2-200*realHeight, 630*realHeight, 150*realHeight, 30*realHeight), manager.networkAddress,guiStyleArea);
					
                }
                else
                {
                    GUI.Label(new Rect(5*realHeight, 5*realHeight, 200*realHeight, 30*realHeight), "Connecting to " + manager.networkAddress + ":" + manager.networkPort,guiStyleNormal);


                    if (GUI.Button(new Rect(5*realHeight, 30*realHeight, 200*realHeight, 30*realHeight), "Cancel Connection Attempt",guiStyleNormal))
                    {
						Click();
                        manager.StopClient();
                    }
                }
            }
			else
			{
				GUI.Label(new Rect(Screen.width/2-380*realHeight, 510*realHeight,370*realHeight,250*realHeight),"",guiStyleLabel);
			}

            if (manager.IsClientConnected() && !ClientScene.ready)
            {
                if (GUI.Button(new Rect(5*realHeight, 5*realHeight, 200*realHeight, 30*realHeight), "Client Ready",guiStyleNormal))
                {
					Click();
                    ClientScene.Ready(manager.client.connection);

                    if (ClientScene.localPlayers.Count == 0)
                    {
                        ClientScene.AddPlayer(0);
                    }
                }
            }

            if (!NetworkServer.active && !manager.IsClientConnected() && noConnection)
            {
                if (manager.matchMaker != null)
                {
					GUI.Label(new Rect(Screen.width/2-380*realHeight, 435*realHeight,370*realHeight,250*realHeight),"Match Maker Options",guiStyleLabel);
                    if (manager.matchInfo == null)
                    {
                        if (manager.matches == null)
                        {
                            if (GUI.Button(new Rect(Screen.width/2-265*realHeight, 680*realHeight, 150*realHeight, 30*realHeight), "Create Match",guiStyleNormal))
                            {
								Click();
                                manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
								
								SwapOnline(0);
								
                            }

                            GUI.Label(new Rect(Screen.width/2-350*realHeight, 630*realHeight, 150*realHeight, 30*realHeight), "Room Name:",guiStyleLabel);
                            manager.matchName = GUI.TextField(new Rect(Screen.width/2-200*realHeight, 630*realHeight, 150*realHeight, 30*realHeight), manager.matchName,guiStyleArea);

                            if (GUI.Button(new Rect(Screen.width/2-265*realHeight, 550*realHeight, 150*realHeight, 30*realHeight), "Find Match",guiStyleNormal))
                            {
								Click();
                                manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, manager.OnMatchList);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < manager.matches.Count; i++)
                            {
                                var match = manager.matches[i];
                                if (GUI.Button(new Rect(Screen.width/2-265*realHeight, 550*realHeight+i*35*realHeight, 150*realHeight, 30*realHeight), "Join Match:" + match.name,guiStyleNormal))
                                {
									Click();
                                    manager.matchName = match.name;
                                    manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
									
									SwapOnline(1);
									
                                }
                            }

                            if (GUI.Button(new Rect(Screen.width/2+125*realHeight, 550*realHeight, 150*realHeight, 30*realHeight), "Back to Match Menu",guiStyleNormal))
                            {
								Click();
                                manager.matches = null;
                            }
                        }
                    }
                }
            }
        }

		//OPEN DEMO
		else if(menu==4)
		{
			DirectoryInfo dir = new DirectoryInfo("./");
			FileInfo[] info = dir.GetFiles("*.DEM");
			
			guiStyleNormal.fontSize = 17 * fontSizer/10;
			guiStyleLabel.fontSize = 17 * fontSizer/10;
			GUI.Label(new Rect(Screen.width/2-150*realHeight,5*realHeight,300*realHeight,30*realHeight),"Click on file, to see the details.",guiStyleLabel);
			guiStyleLabel.alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(5*realHeight,5*realHeight,300*realHeight,30*realHeight),"Path: ("+dir.FullName+")",guiStyleLabel);
			
			GUI.Box(new Rect(5*realHeight,30*realHeight,450*realHeight,Screen.height-50*realHeight),"",guiStyleNormal);
			int bajdas=0;
			scrollPosition = GUI.BeginScrollView(new Rect(5*realHeight,30*realHeight,480*realHeight,Screen.height-55*realHeight), scrollPosition, new Rect(0, 0, 450*realHeight, 5*realHeight+asd*30*realHeight));
			foreach (FileInfo f in info) 
			{
				if(GUI.Button(new Rect(5*realHeight,5*realHeight+(bajdas*30*realHeight),440*realHeight,30*realHeight),""+f.Name,guiStyleNormal))
				{
					Click();
					chosenDemo=true;
					#region DemoThings
					SaveLoad.Load(f.Name);
					gameSystemScript.demoState=true;
					gameSystemScript.duelName=SaveData.duelName;
					gameSystemScript.duelCaster=SaveData.duelCaster;
					gameSystemScript.duelDeckCard=SaveData.duelDeckCard;
					gameSystemScript.duelPos=SaveData.duelPos;
					gameSystemScript.players[0].cpuControl=true;
					gameSystemScript.players[1].cpuControl=true;
					gameSystemScript.players[0].deck=SaveData.duelDeck0;
					gameSystemScript.players[1].deck=SaveData.duelDeck1;
					gameSystemScript.drawCount=SaveData.dc;
					for(int a=0;a<2;a++)
					{
						gameSystemScript.players[a].playerName=prefChars[SaveData.duelCaster[a]].playerName;
						gameSystemScript.players[a].portrait=prefChars[SaveData.duelCaster[a]].portrait;
						gameSystemScript.players[a].maxHp=prefChars[SaveData.duelCaster[a]].hp;
						gameSystemScript.players[a].maxMana=prefChars[SaveData.duelCaster[a]].mana;
						gameSystemScript.players[a].gainMana=prefChars[SaveData.duelCaster[a]].gainMana;
					}
					numOMoves=0;
					for(int a=0;a<1024;a++)
					{
						if(SaveData.duelDeckCard[a]==777)
							break;
						else
							numOMoves++;
						
					}
					#endregion
				}
				bajdas++;
				asd=bajdas;
			}
			GUI.EndScrollView();
			if(bajdas==0)
			{
				GUI.Label(new Rect(10*realHeight,35*realHeight,340*realHeight,30*realHeight),"No demos yet.",guiStyleLabel);
			}
				
			if(chosenDemo)
			{
				
				//INFO
				guiStyleLabel.fontSize = 22 * fontSizer/10;
				GUI.Box(new Rect(Screen.width/2,105*realHeight,132*realHeight,132*realHeight),"",guiStyleBox);
				GUI.Label(new Rect(Screen.width/2,75*realHeight,128*realHeight,30*realHeight),prefChars[SaveData.duelCaster[0]].playerName,guiStyleLabel);
				GUI.DrawTexture(new Rect(Screen.width/2+2*realHeight,103*realHeight,128*realHeight,128*realHeight),prefChars[SaveData.duelCaster[0]].portrait);
				if(SaveData.winner==0)
					GUI.DrawTexture(new Rect(Screen.width/2+2*realHeight,103*realHeight,44*realHeight,44*realHeight),interfaceImages[2]);
					
				
				guiStyleLabel.fontSize = 45 * fontSizer/10;
				GUI.Label(new Rect(Screen.width/2+178*realHeight,140*realHeight,100*realHeight,100*realHeight),"VS",guiStyleLabel);
				
				
				guiStyleLabel.fontSize = 22 * fontSizer/10;
				GUI.Box(new Rect(Screen.width/2+272*realHeight,105*realHeight,132*realHeight,132*realHeight),"",guiStyleBox);
				GUI.Label(new Rect(Screen.width/2+272*realHeight,75*realHeight,128*realHeight,30*realHeight),prefChars[SaveData.duelCaster[1]].playerName,guiStyleLabel);
				GUI.DrawTexture(new Rect(Screen.width/2+274*realHeight,103*realHeight,128*realHeight,128*realHeight),prefChars[SaveData.duelCaster[1]].portrait);
				if(SaveData.winner==1)
					GUI.DrawTexture(new Rect(Screen.width/2+274*realHeight,103*realHeight,44*realHeight,44*realHeight),interfaceImages[2]);
				
				
				GUI.Box(new Rect(Screen.width/2,300*realHeight,400*realHeight,266*realHeight),"",guiStyleBox);
				
				guiStyleLabel.fontSize = 19 * fontSizer/10;
				guiStyleLabel.alignment = TextAnchor.UpperCenter;
				GUI.Label(new Rect(Screen.width/2+10*realHeight,310*realHeight,400*realHeight,266*realHeight),"Details",guiStyleLabel);
				guiStyleLabel.alignment = TextAnchor.UpperLeft;
				guiStyleLabel.fontSize = 17 * fontSizer/10;
				GUI.Label(new Rect(Screen.width/2+10*realHeight,330*realHeight,400*realHeight,266*realHeight),"game version: "+SaveData.duelVersion,guiStyleLabel);
				if(SaveData.duelVersion==version)
					GUI.Label(new Rect(Screen.width/2+10*realHeight,350*realHeight,400*realHeight,266*realHeight),"<color=green>Compatible</color>",guiStyleLabel);
				else
					GUI.Label(new Rect(Screen.width/2+10*realHeight,350*realHeight,400*realHeight,266*realHeight),"<color=yellow>Warning, demo can be played, but may cause errors</color>",guiStyleLabel);
				
				GUI.Label(new Rect(Screen.width/2+10*realHeight,370*realHeight,400*realHeight,266*realHeight),"opponents: "+prefChars[SaveData.duelCaster[0]].playerName+" vs "+prefChars[SaveData.duelCaster[1]].playerName,guiStyleLabel);
				if(SaveData.winner<2)
					GUI.Label(new Rect(Screen.width/2+10*realHeight,390*realHeight,400*realHeight,266*realHeight),"winner: "+prefChars[SaveData.duelCaster[SaveData.winner]].playerName+" (Player "+(SaveData.winner+1)+")",guiStyleLabel);
				else
					GUI.Label(new Rect(Screen.width/2+10*realHeight,390*realHeight,400*realHeight,266*realHeight),"winner: Draw",guiStyleLabel);
				
				GUI.Label(new Rect(Screen.width/2+10*realHeight,410*realHeight,400*realHeight,266*realHeight),"Number of moves: "+(numOMoves-1),guiStyleLabel);
				
				if(GUI.Button(new Rect(Screen.width/2+136*realHeight,Screen.height/2+(75*realHeight)/2+200*realHeight,125*realHeight,30*realHeight),"Play!",guiStyleNormal))
				{
					Swap();
				}
			}
			
		}
		
		//CREDITS
		else if(menu==5)
		{
			guiStyleLabel.alignment = TextAnchor.MiddleLeft;
			guiStyleLabel.fontSize=24 * fontSizer/10;
			for(int a=0;a<credits.Length;a++)
			{
				GUI.Label(new Rect(5*realHeight,a*50*realHeight+Screen.height-scroll*realHeight,1024*realHeight,80*realHeight),credits[a],guiStyleLabel);
			}
			
			
		}
		
		//EXIT MENU
		else if(menu==6)
		{
			guiStyleLabel.wordWrap=true;
			guiStyleLabel.fontSize=18 * fontSizer/10;

			GUI.Box(new Rect(Screen.width/2-(200*realHeight),Screen.height/2-100*realHeight,400*realHeight,200*realHeight),"",guiStyleBox);
			string etext="Error message";
			
			if(rand==0)
				etext="Ganon says:\nDon't leave and I will make your face the greatest in Koridai!";
			else if(rand==1)
				etext="Jamal says:\nEvery 60 seconds, a minute passes. Don't leave so we can stop this!";
			else if(rand==2)
				etext="Bronjahm says:\nStay or I will sever your soul from your body!";
			else if(rand==3)
				etext="Reaper says:\nI'm waiting for you on your desktop.";
			else if(rand==4)
				etext="Jigsaw says:\nI want you to play a game.";
			else if(rand==5)
				etext="Elsa says:\nLeave and I will freeze your system.";
			else if(rand==6)
				etext="Gandalf says:\nFellowship needs you. Don't be fool.";
			
			GUI.Label(new Rect(Screen.width/2-(175*realHeight),Screen.height/2-50*realHeight,350*realHeight,200*realHeight),etext,guiStyleLabel);
			GUI.DrawTexture(new Rect(Screen.width/2-(150*realHeight)/2,Screen.height/2-250*realHeight,150*realHeight,150*realHeight),menuHelpers[6+rand]);
			
			//EXIT
			if(GUI.Button(new Rect(Screen.width/2-(175*realHeight),Screen.height/2+55*realHeight,125*realHeight,30*realHeight),"Exit",guiStyleNormal))
			{
				Click();
				Application.Quit();
			}

			//BACK
			if(GUI.Button(new Rect(Screen.width/2+(50*realHeight),Screen.height/2+55*realHeight,125*realHeight,30*realHeight),"Return",guiStyleNormal))
			{
				Click();
				menu=0;
			}
			
			
		}
		
		//TOURNAMENT PART2
		else if(menu==7)
		{
			
			guiStyleLabel.alignment = TextAnchor.UpperLeft;
			guiStyleLabel.wordWrap=true;
			realTournamentText = GUI.TextField(new Rect(Screen.width,Screen.width,0,0), tournamentText,(int)textCounter,guiStyleLabel);
			GUI.Label(new Rect(55*realHeight,55*realHeight,Screen.width-110*realHeight,500*realHeight), realTournamentText,guiStyleLabel);
			
			//CONFIRM
			if(realTournamentText==tournamentText)
			{
				if(GUI.Button(new Rect(5*realHeight,Screen.height-35*realHeight,100*realHeight,30*realHeight),"Continue",guiStyleNormal))
				{
					Click();
					StartTournament();
					TournamentSort();
					menu=8;
				}
			}
			else
			{
				if(GUI.Button(new Rect(5*realHeight,Screen.height-35*realHeight,100*realHeight,30*realHeight),"Skip",guiStyleNormal))
				{
					Click();
					textCounter=65535;
				}
			}
			
			//BACK
			if(GUI.Button(new Rect(Screen.width-105*realHeight,Screen.height-35*realHeight,100*realHeight,30*realHeight),"Cancel",guiStyleNormal))
			{
				Click();
				menu=0;
			}
			
			
		}
		//Tournament part 3
		else if (menu==8)
		{
				
			//LEADERSCORE
			guiStyleNormal.fontSize = 17 * fontSizer/10;
			GUI.Box(new Rect(50*realHeight,50*realHeight,300*realHeight,325*realHeight),"",guiStyleBox);
			guiStyleLabel.alignment = TextAnchor.UpperLeft;
			guiStyleLabel.fontSize = 22 * fontSizer/10;
			GUI.Label(new Rect(60*realHeight,27*realHeight,200*realHeight,30*realHeight),"Leaderboard",guiStyleLabel);
			for(int a=0;a<9;a++)
			{
				if(a!=0)
				{
					
					if(tourXChar==marked[a-1])
						guiStyleNormal.normal.textColor = new Color(0.4f,1f,0.4f,1);
					else
						guiStyleNormal.normal.textColor = Color.white;
					
					
					GUI.Label(new Rect(60*realHeight,60*realHeight+a*35*realHeight,200*realHeight,30*realHeight),prefChars[marked[a-1]].playerName+" "+tourNames[marked[a-1]],guiStyleNormal);
					
					
					GUI.Label(new Rect(260*realHeight,60*realHeight+a*35*realHeight,80*realHeight,30*realHeight),""+tourPoints[marked[a-1]],guiStyleNormal);
					guiStyleNormal.normal.textColor = Color.white;
				}
				else
				{
					GUI.Label(new Rect(60*realHeight,60*realHeight,200*realHeight,30*realHeight),"Magician",guiStyleLabel);
					GUI.Label(new Rect(260*realHeight,60*realHeight,80*realHeight,30*realHeight),"Points",guiStyleLabel);
				}
			}
			
			if(curWeek!=11&&curWeek!=12)
			{
				GUI.Label(new Rect(Screen.width/2-150*realHeight,5*realHeight,300*realHeight,30*realHeight),"Tournament menu",guiStyleLabel);
					
				//CALENDAR
				GUI.Label(new Rect(Screen.width-340*realHeight,27*realHeight,280*realHeight,35*realHeight),"Incoming duels (Week "+(curWeek+1)+")",guiStyleLabel);
				GUI.Box(new Rect(Screen.width-350*realHeight,50*realHeight,300*realHeight,150*realHeight),"",guiStyleBox);
				
				if(curWeek<7)
				{
					for(int a=0;a<8;a+=2)
					{
						
						if(tourXChar==weeks[curWeek].schedule[a]||tourXChar==weeks[curWeek].schedule[a+1])
							guiStyleNormal.normal.textColor = new Color(0.4f,1f,0.4f,1);
						else 
							guiStyleNormal.normal.textColor = Color.white;
						
						GUI.Label(new Rect(Screen.width-340*realHeight,60*realHeight+a*17*realHeight,280*realHeight,30*realHeight),
						prefChars[weeks[curWeek].schedule[a]].playerName+" "+tourNames[weeks[curWeek].schedule[a]]+
						" vs "
						+prefChars[weeks[curWeek].schedule[a+1]].playerName+" "+tourNames[weeks[curWeek].schedule[a+1]],
						guiStyleNormal);
						
						guiStyleNormal.normal.textColor = Color.white;
					}
				}
				else if(tourPoints[marked[0]] <= tourPoints[tourXChar])
				{
					guiStyleNormal.normal.textColor = new Color(0.4f,1f,0.4f,1);
					GUI.Label(new Rect(Screen.width-340*realHeight,60*realHeight,280*realHeight,30*realHeight), prefChars[tourXChar].playerName+" "+tourNames[tourXChar]+" vs Champion" ,guiStyleNormal);
					guiStyleNormal.normal.textColor = Color.white;
				}
				else 
				{
					GUI.Label(new Rect(Screen.width-340*realHeight,60*realHeight,280*realHeight,30*realHeight), prefChars[marked[0]].playerName+" "+tourNames[marked[0]]+" vs Champion" ,guiStyleNormal);
				}
				
				
				//NEXT MATCH BUTTON
				if(curWeek!=7)
				{
					if(GUI.Button(new Rect(Screen.width/2-(100*realHeight)/2,420*realHeight,100*realHeight,30*realHeight),"Duel",guiStyleNormal))
					{
						Click();
						if(curWeek<7)
						{
							RandomPoints();
						}
						Confirm();
						Swap();
					}
				}
				else if(tourPoints[marked[0]] <= tourPoints[tourXChar])
				{
					if(GUI.Button(new Rect(Screen.width/2-(100*realHeight)/2,420*realHeight,100*realHeight,30*realHeight),"Duel",guiStyleNormal))
					{
						Click();
						Confirm();
						Swap();
					}
				}
				else 
				{
					guiStyleLabel.wordWrap=true;
					GUI.Label(new Rect(50*realHeight,420*realHeight,300*realHeight,30*realHeight),"Unfortunately, you don't have enough points to challenge champion. Leave in shame.",guiStyleLabel);
				}
				
				//EXIT TO MENU
				if(GUI.Button(new Rect(Screen.width-105*realHeight,Screen.height-35*realHeight,100*realHeight,30*realHeight),"Exit",guiStyleNormal))
				{
					Click();
					menu=0;
				}
				
			}
			//ASDFISFHDSUHSEUAHFJSDHF
			else 
			{
				GUI.Label(new Rect(Screen.width/2-150*realHeight,5*realHeight,300*realHeight,30*realHeight),"Tournament results",guiStyleLabel);
				guiStyleLabel.wordWrap=true;
				if(curWeek==12)
				{
					GUI.Label(new Rect(50*realHeight,420*realHeight,800*realHeight,30*realHeight),"Congratulations! Now everyone will call you tournament champion. Many years later, legends said that "+tourNames[tourXChar]+" have never lost a single match since his memorable victory in final match against Archmage "+faststring+". He became the most powerful "+prefChars[tourXChar].playerName+" that ever existed.",guiStyleLabel);
					
					guiStyleLabel.fontSize = 16 * fontSizer/10;
					GUI.Box(new Rect(Screen.width/2,105*realHeight,132*realHeight,132*realHeight),"",guiStyleBox);
					GUI.Label(new Rect(Screen.width/2,75*realHeight,199*realHeight,30*realHeight),"Winner: "+prefChars[tourXChar].playerName+" "+tourNames[tourXChar],guiStyleLabel);
					GUI.DrawTexture(new Rect(Screen.width/2+2*realHeight,103*realHeight,128*realHeight,128*realHeight),prefChars[tourXChar].portrait);
					//CREDITS
					if(GUI.Button(new Rect(Screen.width-105*realHeight,Screen.height-35*realHeight,100*realHeight,30*realHeight),"Credits",guiStyleNormal))
					{
						scroll=0;
						Click();
						menu=5;
					}
				}
				else 
				{
					GUI.Label(new Rect(50*realHeight,420*realHeight,300*realHeight,30*realHeight),"It's not shame to lose against master. Archmage "+faststring+" is still the champion. Better luck next time magician.",guiStyleLabel);
					
					guiStyleLabel.fontSize = 16 * fontSizer/10;
					GUI.Box(new Rect(Screen.width/2,105*realHeight,132*realHeight,132*realHeight),"",guiStyleBox);
					GUI.Label(new Rect(Screen.width/2,75*realHeight,199*realHeight,30*realHeight),"Winner: "+prefChars[8].playerName+" "+faststring,guiStyleLabel);
					GUI.DrawTexture(new Rect(Screen.width/2+2*realHeight,103*realHeight,128*realHeight,128*realHeight),prefChars[8].portrait);
					
					//EXIT
					if(GUI.Button(new Rect(Screen.width-105*realHeight,Screen.height-35*realHeight,100*realHeight,30*realHeight),"Exit",guiStyleNormal))
					{
						Click();
						menu=0;
					}
				}
			}
		}
		//TOUR SAVES
		else if(menu==13)
		{
			DirectoryInfo dir = new DirectoryInfo("./");
			FileInfo[] info = dir.GetFiles("*.SAV");
			
			guiStyleNormal.fontSize = 17 * fontSizer/10;
			guiStyleLabel.fontSize = 17 * fontSizer/10;
			GUI.Label(new Rect(Screen.width/2-150*realHeight,5*realHeight,300*realHeight,30*realHeight),"Saved Tournaments",guiStyleLabel);
			guiStyleLabel.alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(5*realHeight,5*realHeight,300*realHeight,30*realHeight),"Path: ("+dir.FullName+")",guiStyleLabel);
			
			GUI.Box(new Rect(5*realHeight,30*realHeight,450*realHeight,Screen.height-50*realHeight),"",guiStyleNormal);
			int bajdala=0;
			scrollPosition = GUI.BeginScrollView(new Rect(5*realHeight,30*realHeight,480*realHeight,Screen.height-55*realHeight), scrollPosition, new Rect(0, 0, 450*realHeight, 5*realHeight+asd*30*realHeight));
			foreach (FileInfo f in info) 
			{
				if(GUI.Button(new Rect(5*realHeight,5*realHeight+(bajdala*30*realHeight),440*realHeight,30*realHeight),""+f.Name,guiStyleNormal))
				{
					Click();
					LocalLoadTour(f.Name);
				}
				bajdala++;
				asd=bajdala;
			}
			GUI.EndScrollView();
			if(bajdala==0)
			{
				GUI.Label(new Rect(10*realHeight,35*realHeight,340*realHeight,30*realHeight),"No tournament saves yet.",guiStyleLabel);
			}
			
		}
		
		//BACK
		if(menu!=0&&menu!=6&&menu!=7&&menu!=8)
		{
				guiStyleNormal.fontSize=18 * fontSizer/10;
				guiStyleNormal.alignment = TextAnchor.MiddleCenter;
				if(GUI.Button(new Rect(Screen.width-105*realHeight,Screen.height-35*realHeight,100*realHeight,30*realHeight),"Back",guiStyleNormal))
				{
					Click();
					menu=0;
				}
			}
		
		
	}
	
	#region voids
	void Fps(bool a)
	{
		fpsObj.SetActive (a);
	}
	
	void Swap()
	{
		gameSys.SetActive (true);
		gameObject.SetActive (false);
	}
	
	void LateUpdate()
	{
		if(menu==5)
		{
			scroll+=Time.deltaTime*30;
			if(Input.GetMouseButton(0)||Input.GetMouseButton(1))
			{
				scroll+=Time.deltaTime*60;
			}
		}
		
		else if(menu==7)
		{
			textCounter+=Time.deltaTime*18;
		}
		else
		{
			textCounter=0;
		}
		
	}
	
	//SinglePlayer
	void Confirm()
	{
		if(curWeek==7)
		{
			drawCounter=0;
			xchar[0]=tourXChar;
			gameSystemScript.players[0].playerName=prefChars[xchar[0]].playerName+" "+tourNames[tourXChar];
			xchar[1]=8;
			faststring=mNames[Random.Range(0,30)];
			gameSystemScript.players[1].playerName="Archmage "+faststring;
			aiOption[0]=0;
			aiOption[1]=3;
			tournamentMatch=true;
		}
		for(int a=0;a<2;a++)
		{
			if(!tournamentMatch)
				gameSystemScript.players[a].playerName=prefChars[xchar[a]].playerName;
			gameSystemScript.players[a].portrait=prefChars[xchar[a]].portrait;
			gameSystemScript.players[a].maxHp=prefChars[xchar[a]].hp;
			gameSystemScript.players[a].maxMana=prefChars[xchar[a]].mana;
			gameSystemScript.players[a].gainMana=prefChars[xchar[a]].gainMana;
			gameSystemScript.players[a].deck=new int[prefChars[xchar[a]].deck.Length];
			for(int z=0;z<prefChars[xchar[a]].deck.Length;z++)
			{
				gameSystemScript.players[a].deck[z]=prefChars[xchar[a]].deck[z];
			}
			
			if(aiOption[a]==0)
				gameSystemScript.players[a].cpuControl=false;
			else
			{
				gameSystemScript.players[a].cpuControl=true;
				gameSystemScript.players[a].ai=aiOption[a];
			}
		}
		gameSystemScript.drawCount=drawCounter;
		gameSystemScript.demoState=false;
		if(tournamentMatch)
			gameSystemScript.tourMatch=true;
		
	}	
	
	void Click()
	{
		audio.PlayOneShot (sounds[0]);
	}

	public void SwapOnline(byte playa)
	{
		audio.Stop();
		chara=xchar[0];
		play=playa;
		showMenu=false;
	}
	
	void RandomPoints()
	{
		for(int a=0;a<8;a+=2)
		{
			if(tourXChar==weeks[curWeek].schedule[a]||tourXChar==weeks[curWeek].schedule[a+1])
			{
				xchar[0]=weeks[curWeek].schedule[a];
				xchar[1]=weeks[curWeek].schedule[a+1];
				
				gameSystemScript.players[0].playerName=prefChars[xchar[0]].playerName+" "+tourNames[weeks[curWeek].schedule[a]];
				gameSystemScript.players[1].playerName=prefChars[xchar[1]].playerName+" "+tourNames[weeks[curWeek].schedule[a+1]];
				drawCounter=50;
				
				for(int b=0;b<2;b++)
				{
					if(tourXChar==weeks[curWeek].schedule[b])
						aiOption[b]=0;
					else
						aiOption[b]=tourSkill[weeks[curWeek].schedule[a+b]];
				}
				tournamentMatch=true;
			}
			else 
			{
				int rand=Random.Range(1,3);
				int[] points;
				if(rand==0)
					points=new int[2]{3,0};
				else if(rand==1)
					points=new int[2]{0,3};
				else 
					points=new int[2]{1,1};
				
				tourPoints[weeks[curWeek].schedule[a]]+=points[0];
				tourPoints[weeks[curWeek].schedule[a+1]]+=points[1];
			}
		}
		
	}
	
	void StartTournament()
	{
		curWeek=0;
		result=new int[2];
		tourXChar=xchar[0];
		tourPoints=new int[8];
		for(int a=0;a<8;a++)
			tourPoints[a]=0;
		
		tourNames=new string[8];
		for(int a=0;a<8;a++)
		{
			if(a!=tourXChar)
			{
				if(a==3||a==7)
					tourNames[a]=fNames[Random.Range(0,fNames.Length)];
				else
					tourNames[a]=mNames[Random.Range(0,mNames.Length)];
			}
			else
				tourNames[a]=customName;
		}
		tourSkill=new int[8];
		for(int a=0;a<8;a++)
		{
			if(a!=tourXChar)
				tourSkill[a]=Random.Range(1,4);
		}
		
	}
	int[] marked;
	
	public void TournamentSort()
	{
		
		tourSorted=new int[8];
		marked=new int[8] {9,9,9,9,9,9,9,9};
		for(int b=0;b<8;b++)
		{
			int max=0;
			for(int a=0;a<8;a++)
			{
				if(max<=tourPoints[a])
				{
					bool can=true;
					for(int z=0;z<8;z++)
					{
						if(marked[z]==a)
						{
							can=false;
							break;
						}
					}
					if(can)
					{
						max=tourPoints[a];
						marked[b]=a;
					}
				}
				if(a==7)
				{
					tourSorted[b]=max;
				}
			}
		}
		LocalSaveTour();
	}
	
	public void GiveResult()
	{
		if(curWeek!=7)
		{
			for(int a=0;a<8;a+=2)
			{
				if(tourXChar==weeks[curWeek].schedule[a]||tourXChar==weeks[curWeek].schedule[a+1])
				{
					tourPoints[weeks[curWeek].schedule[a]]+=result[0];
					tourPoints[weeks[curWeek].schedule[a+1]]+=result[1];
				}
			}
			curWeek++;
			TournamentSort();
		}
		else
		{
		if(result[0]==0)
			curWeek=11;
		else 
			curWeek=12;
		}
	}
	
	public void LocalSaveTour()
	{
		tourName = "Tournament "+prefChars[tourXChar].playerName+" "+tourNames[tourXChar];
		LoadTour.currentFilePaths=this.tourName+".SAV";
		SaveTour.tourNames=this.tourNames;
		SaveTour.curWeek=this.curWeek;
		SaveTour.tourPoints=this.tourPoints;
		SaveTour.tourSkill=this.tourSkill;
		SaveTour.tourXChar=this.tourXChar;
		
		LoadTour.Save();
	}
	
	public void LocalLoadTour(string file)
	{
		LoadTour.Load(file);
		tourName=SaveTour.tourName;
		tourNames=SaveTour.tourNames;
		curWeek=SaveTour.curWeek;
		tourPoints=SaveTour.tourPoints;
		tourSkill=SaveTour.tourSkill;
		tourXChar=SaveTour.tourXChar;
		menu=8;
		TournamentSort();
	}
	
	public void StopOnline()
	{
		audio.Play();
		Click();
		showMenu=true;
		manager.StopHost();
	}
	
	#endregion
	
}