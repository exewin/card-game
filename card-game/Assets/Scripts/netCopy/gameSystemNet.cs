using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.AccessControl;
using UnityEngine.Networking;

public class gameSystemNet : NetworkBehaviour
{
	
	#region variables
	#region card.cig.vars
	[System.Serializable]
	public class card
	{
		public string cardName;
		public int falseID;
		public Texture2D portrait;
		[Range(1,10)]
		public byte level;
		[Range(0,4)]
		public byte type;
		public int hp;
		[Range(0,4)]
		public byte hpType;
		public int attack;
		[Range(0,4)]
		public byte attackType;
		public byte[] specials;
		public int[] specialID;
		public int manaCost;
		public AudioClip attackSound;
		public Texture2D attackEffect;
		[TextArea(3,5)]
		public string shortDesc;
		[TextArea(5,10)]
		public string longDesc;
		
		public byte cooldown;
		
		public Texture2D BG;
		
	}
	[Header("CARDS&SPECIALS")]
	public card[] cards;
	
	
	
	[System.Serializable]
	public class cardInGame
	{
		
		public int pos;
		public string cardName;
		public bool readyToAttack;
		public int id;
		public byte type;
		public int hp;
		public byte hpType;
		public int maxHp;
		public int attack;
		public byte attackType;
		public byte special;
		public int specialSkill;
		public AudioClip attackSound;
		public Texture2D attackEffect;
		public bool beingHit;
		public bool poisoned;
		public int owner;
		
		public void addMaxHp(int val)
		{
			maxHp+=val;
			if(maxHp<hp)
				hp=maxHp;
			
			if(hp<=0)
				id=0;
		}
		
		public void addHp(int val)
		{
			if(hp>0)
			{
				hp+=val;
					
				if(hp>maxHp)
					hp=maxHp;
				
				if(hp<=0&&id!=0)
				{
					hp=0;
					//TYPE2 SPEC ON EXIT
					gameSys.FindSpec(2,id,pos);
					id=0;
				}
			}
		}
		
		public void addDmg(int val)
		{
			if(type<3)
			{
				attack+=val;
				if(attack<1)
					attack=1;
			}
			else
			{
				attack-=val;
				if(attack>-1)
					attack=-1;
			}
		}
		
	}

	
	public cardInGame[] cardsInGame;
	#endregion
	#region specials.vars
	[System.Serializable]
	public class special
	{
		public int target;
		[Range(0,11)]
		public int type;
		public int value;
		public AudioClip specSound;
		public string name;
		[TextArea(3,5)]
		public string desc;
		public Texture2D icon;
		
		
	}
	
	public special[] specials;
	#endregion
	#region playerNow.goFaster.vars
	byte playerNow; //TURN 
	[Header("OTHER")]
	public float goFaster;
	#endregion
	#region player.vars
	[System.Serializable]
	public class player
	{
		
		public bool cpuControl;
		[Range(1,3)]
		public int ai;
		public string playerName;
		public Texture2D portrait;
		public int hp;
		public int maxHp;
		public int[] deck;
		public byte[] cardCooldown;
		public int mana;
		public int maxMana;
		public int gainMana;
		
		public void addHp(int val)
		{
			hp+=val;
			
			if(hp>maxHp)
				hp=maxHp;
			
			if(hp<=0)
			{
				matchend=true;
				paused=true;
			}
		}
		
		public void addMana(int val)
		{
			mana+=val;
			if(mana>maxMana)
				mana=maxMana;
			
			if(mana<=0)
				mana=0;
		}
		
		
		
	}

	public player[] players;
	bool[] playerHit = new bool[2];
	#endregion
	#region choose.vars
	
	//CHOOSING
	int cardChoise;
	bool cardChoose;
	float pos2;
	int pos1;
	float pos4;
	int pos3;
	int posSlot;
	
	int virtualCard;
	int realCard;
	
	int[] saveCardChoise;
	int[] savePushCard;
	#endregion
	#region proportion.vars
	//RESOLUTION_HELPER
	float realHeight;
	float heightInFloat;
	float widthInFloat;
	int fontSizer;
	float fontNormalizer;
	int superfontSizer;
	float superfontNormalizer;
	int visibleCards;
	int pushCard;
	float realWidth;
	
	//XY_FOR_CARD_SLOTS
	int x1;
	int y1;
	int moreY;
	int rk;
	
	//XY_FOR_CARDS
	int x2;
	int y2;
	int moreY2;
	int revert;
	int k;
	
	//PROPORTION
	float prop;
	int prop2;
	#endregion
	#region audio.vars
	//audio
	public AudioClip[] sounds;
	AudioSource audio;
	float volume;
	#endregion
	#region animation.vars
	bool skipping;

	//ANIMATION_CARD_AND_EFFECT
	float animaPosX;
	float animaPosY;
	float animaProgress;
	bool allowAnimation;
	int effectAnima;
	bool revertAnima;
	//CARD_SLOT_ANIMATION
	Color32 slotLightning;
	//START_GRADIENT
	bool startGradient;
	float startGradientTimer;
	#endregion
	#region textures.forFight.AI.pauseEnd.errors.modifers.cardInfo.vars
	public Texture2D[] GUITextures;
	//FOR_FIGHT
	bool allowFight;
	bool firstMove;
	bool moved;
	int p;
	float timer;
	bool pHealed;
	
	//AI
	bool anyCooldown;
	bool freePos;
	bool enoughMana;
	int freePosReturn;
	bool founded;
	int[]array;
	int skipper;
	
	//PAUSE_MENU
	static bool paused;
	public int drawCount;
	static bool matchend;

	//ERROR_CASTING
	bool errorOccupied;
	bool errorMana;
	float errorTimer;
	
	//SPECIAL MODIFERS
	int armorModifer=0;
	int casterArmorModifer=0;
	int poisonModifer=0;
	int frostModifer=0;
	int fireModifer=0;
	
	//CARD INFO BOOL
	bool cardInfo=false;
	int cardInfoId;
	
	#endregion
	#region obj.scr.vars
	public GameObject gameMen;
	public gameMenu gameMenuScript;
	public static gameSystemNet gameSys;
	#endregion
	#region styles.vars
	[Header("STYLES")]
	public GUIStyle guiStyle;
	public GUIStyle guiStyleBorder;
	public GUIStyle guiStyleSlot;
	public GUIStyle guiStyleMenu;
	public GUIStyle onlyQuoteFont;
	public GUIStyle PlayerInfo;
	public GUIStyle guiStyleLabel;
	//GUI_CARD_BUTTON_STYLE
	public GUIStyle buttonGuiStyle;
	public GUIStyle buttonGuiStyle2;
	#endregion
	#region indepIcons.vars
	[System.Serializable]
	public class indepIcon
	{
		public int posX;
		public int posY;
		public Texture2D icon;
		public float timer;
		public int scaler;
		public bool disort;
	}
	[HideInInspector]
	public indepIcon[] indepIcons;
	#endregion
	#region DEM.vars
	//DEM
	[HideInInspector]
	public bool demoState;
	[HideInInspector]
	public	string duelName;
	[HideInInspector]
	public	int[] duelCaster;
	[HideInInspector]
	public	int[] duelDeckCard;
	[HideInInspector]
	public	int[] duelPos;
	int duelCount;
	bool savedDemo;
	#endregion
	#region manaRefill;
	int[] turns;
	#endregion
	#region multiplayer
	//MULTIPLAYER
	public bool onlineGame;
	[SyncVar]public bool canOnline;
	
	public PlayerS multiplayerPlayer;
	
	[SyncVar]public int takeCard;
	[SyncVar]public int takePos;
	
	
	
	
	
	
	
	public AudioClip[] thunders;
	#endregion
	
	
	public bool tourMatch;
	
	
	#endregion
	
	#region Awake.OnEnable
	public void Awake()
	{

		duelCaster=new int[2];
		duelDeckCard=new int[1023];
		duelPos=new int[1023];
		if(!gameMen)
			gameMen=GameObject.FindWithTag("men");
		gameMenuScript=gameMen.GetComponent<gameMenu>();
		if(!onlineGame)
		{
			if(gameMen.active==true)
			{
				gameObject.SetActive(false);
			}
		}

		OnEnable();

	}
	
	public void OnEnable()
    {
		if(onlineGame)
			tourMatch=false;
		turns=new int[2];
		duelCount=0;
		gameSys=this;
		audio = GetComponent<AudioSource>();
		matchend=false;
			
		//fill 999
		RandomFill();	
		
		for(byte e=0;e<16;e++)
			cardsInGame[e].beingHit=false;
		
		players[0].mana=players[0].maxMana;
		players[1].mana=players[1].maxMana;
		
		players[0].cardCooldown=new byte[players[0].deck.Length];
		players[1].cardCooldown=new byte[players[1].deck.Length];
		
		players[0].hp=players[0].maxHp;
		players[1].hp=players[1].maxHp;
		
		playerHit[0]=false;
		playerHit[1]=false;
		
		
		playerNow=0;
		allowFight=false;
		firstMove=false;
		moved=false;
		p=0;
		timer=0;
		pHealed=false;
		
		if(tourMatch)
			gameMenuScript.result=new int[2];
		
		saveCardChoise = new int[2];
		savePushCard = new int[2];
		
		for(byte x=0;x<16;x++)
			cardsInGame[x].id=0;
		
		if(onlineGame)
			if(!isLocalPlayer)
				return;
		startGradient=true;
		startGradientTimer=1;
		paused=true;
		
		if(gameObject.activeSelf==true)
		{
			audio.PlayOneShot (sounds[3]);
		}
		
		savedDemo=false;
		//DEM
		if(!demoState)
		{
			duelCount=0;
			duelCaster=new int[2];
			duelDeckCard=new int[1023];
			duelPos=new int[1023];
		}
		Looper();
    }
	#endregion
	#region OnGUI
	public void OnGUI()
	{
		if(onlineGame)
			if(!isLocalPlayer)
				return;
		#region proportion
		SmallCardSetting(0);
		
		//CONTROL_LINES
		//GUI.DrawTexture(new Rect(Screen.width/2,0,1,Screen.height),GUITextures[9]);
		//GUI.DrawTexture(new Rect(0,Screen.height/2,Screen.width,1),GUITextures[9]);
		
		//HEIGHT_PROPORTION
		heightInFloat=Screen.height;
		widthInFloat=Screen.width;
		realHeight=heightInFloat/768;
		realWidth=widthInFloat/1024;
		visibleCards=(int)(Mathf.Ceil((widthInFloat/heightInFloat)*4));
		
		onlyQuoteFont.fontSize = 8 * fontSizer/10;
		guiStyleBorder.fontSize = 18 * fontSizer/10;
		PlayerInfo.fontSize = 15 * fontSizer/10;
		guiStyleSlot.fontSize = 15 * fontSizer/10;
		#endregion
		if(!cardInfo)
		{
			#region player_panels
				
			//ENEMY_PORTRAIT
			GUI.DrawTexture(new Rect(0,0,Screen.height/4,Screen.height/4),players[1].portrait);
			GUI.DrawTexture(new Rect(0,Screen.height/4,Screen.height/4,-Screen.height/4),GUITextures[13]);
			GUI.DrawTexture(new Rect(Screen.height/4,0,180*realHeight,78*realHeight),GUITextures[14]);
			guiStyleBorder.alignment = TextAnchor.UpperLeft;
			guiStyleBorder.normal.textColor = new Color(1,0.4f,0.4f,1);
			GUI.Box(new Rect(Screen.height/4,0,180*realHeight,78*realHeight),""+players[1].playerName+"\nHP:"+players[1].hp+"/"+players[1].maxHp+"\nMP:"+players[1].mana+"/"+players[1].maxMana,PlayerInfo);
			//ENEMY_HIT_EFFECT
			if(playerHit[1])
			{
				GUI.color = new Color(1,1,1,effectAnima);
				GUI.DrawTexture(new Rect(Screen.height/8-effectAnima*2.5f*realHeight/2,Screen.height/8-effectAnima*2.5f*realHeight/2,effectAnima*realHeight*2.5f,effectAnima*realHeight*2.5f),cardsInGame[p+playerNow*8].attackEffect);
				GUI.color=Color.white;
			}
			//SKIP TURN BUTTON
			if(players[playerNow].cpuControl==false&&playerNow==1&&!allowFight)
				if(GUI.Button(new Rect(Screen.height/4,78*realHeight,125*realHeight,30*realHeight),"[S]kip turn",guiStyleSlot))
				{
					SkipTurn();
				}
			
			
			//ALLY_PORTRAIT
			GUI.DrawTexture(new Rect(Screen.width-Screen.height/4,Screen.height-Screen.height/4,Screen.height/4,Screen.height/4),players[0].portrait);
			GUI.DrawTexture(new Rect(Screen.width,Screen.height-Screen.height/4,-Screen.height/4,Screen.height/4),GUITextures[13]);
			GUI.DrawTexture(new Rect(Screen.width-Screen.height/4-180*realHeight,Screen.height-78*realHeight,180*realHeight,78*realHeight),GUITextures[14]);
			guiStyleBorder.alignment = TextAnchor.UpperRight;
			guiStyleBorder.normal.textColor = new Color(0.4f,1,0.4f,1);
			GUI.Box(new Rect(Screen.width-Screen.height/4-180*realHeight,Screen.height-78*realHeight,180*realHeight,78*realHeight),""+players[0].playerName+"\nHP:"+players[0].hp+"/"+players[0].maxHp+"\nMP:"+players[0].mana+"/"+players[0].maxMana,PlayerInfo);
			//ALLY_HIT_EFFECT
			if(playerHit[0])
			{
				GUI.color = new Color(1,1,1,effectAnima);
				GUI.DrawTexture(new Rect(Screen.width-Screen.height/8-effectAnima*2.5f*realHeight/2,Screen.height-Screen.height/8-effectAnima*2.5f*realHeight/2,effectAnima*realHeight*2.5f,effectAnima*realHeight*2.5f),cardsInGame[p+playerNow*8].attackEffect);
				GUI.color=Color.white;
			}
			//SKIP TURN BUTTON
			if(players[playerNow].cpuControl==false&&playerNow==0&&!allowFight)
				if(GUI.Button(new Rect(Screen.width-Screen.height/4-125*realHeight,Screen.height-78*realHeight-30*realHeight,125*realHeight,30*realHeight),"[S]kip turn",guiStyleSlot))
				{
					SkipTurn();
				}
			
			
			#endregion
			#region mana_ref
			onlyQuoteFont.alignment = TextAnchor.MiddleCenter;
			onlyQuoteFont.fontSize = 17 * fontSizer/10;
			onlyQuoteFont.normal.textColor = Color.white;
			GUI.Box(new Rect(Screen.width-175*realHeight,200*realHeight,175*realHeight,150*realHeight),"",guiStyleSlot);
			GUI.Label(new Rect(Screen.width-175*realHeight,210*realHeight,175*realHeight,25*realHeight),"Mana Refill 50%",onlyQuoteFont);
			GUI.DrawTexture(new Rect(Screen.width-125*realHeight,235*realHeight,75*realHeight,75*realHeight),GUITextures[30]);
			GUI.Label(new Rect(Screen.width-175*realHeight,315*realHeight,175*realHeight,25*realHeight),"Turns left: "+(5-turns[playerNow]),onlyQuoteFont);
			
			
			//turns
			GUI.Box(new Rect(Screen.width-175*realHeight,350*realHeight,175*realHeight,50*realHeight),"",guiStyleSlot);
			GUI.Label(new Rect(Screen.width-175*realHeight,365*realHeight,175*realHeight,25*realHeight),"Moves: "+duelCount,onlyQuoteFont);
			
			//draw
			if(drawCount!=0)
			{
				GUI.Box(new Rect(Screen.width-125*realHeight,400*realHeight,125*realHeight,30*realHeight),"",guiStyleSlot);
				GUI.Label(new Rect(Screen.width-125*realHeight,405*realHeight,125*realHeight,25*realHeight),"Draw at: "+drawCount,onlyQuoteFont);
			}
			onlyQuoteFont.normal.textColor = Color.black;	
			
			#endregion
			
			#region card_slots
			if(!paused||startGradient)
			{
				
				x1=0;
				y1=0;
				moreY=0;
				revert=0;
				//CARD_SLOTS
				for(byte ca = 0; ca<16; ca++)
				{
					if(ca>11)
						revert=1;
					
					int sloPosX = (int)(Screen.width/2-180*realHeight+x1*90*realHeight);
					int sloPosY = (int)(15*realHeight+Screen.height/2+y1*90*realHeight+moreY*realHeight+20*realHeight*+(9*(revert*(-1))));
					
					//NORMAL_BOX
					GUI.Box(new Rect(sloPosX,sloPosY,90*realHeight,90*realHeight),"",guiStyleSlot);
					

					//IF_CARD_EXISTS
					if(cardsInGame[ca].id!=0)
					{
						if(cardsInGame[ca].poisoned)
						{
							GUI.color=Color.green;
						}
						//PORTRAIT
						GUI.DrawTexture(new Rect(sloPosX,sloPosY,90*realHeight,90*realHeight),cards[cardsInGame[ca].id].portrait);
						GUI.color=Color.white;
					}
					
					//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					//LIGHTING
					GUI.color=Color.white;
					if(posSlot==ca-8*playerNow && cardChoose)
					{
						GUI.color=slotLightning;
					}
					if(allowFight&&firstMove==false&&p+playerNow*8==ca&&cardsInGame[ca].id!=0 && cardsInGame[ca].readyToAttack)
					{
						GUI.color=Color.red;
					}
					GUI.DrawTexture(new Rect(sloPosX,sloPosY,90*realHeight,90*realHeight),GUITextures[16]);
					GUI.color=Color.white;
					//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
						
					//IF_CARD_EXISTS
					if(cardsInGame[ca].id!=0)
					{
						
						//STATS
						//HP
						guiStyle.fontSize = 14 * fontSizer/10;
						CheckElemental(cards[cardsInGame[ca].id].hpType,1);
						GUI.DrawTexture(new Rect(sloPosX,sloPosY+25*realHeight,25*realHeight,25*realHeight),GUITextures[3]);
						GUI.Label(new Rect(sloPosX,sloPosY+25*realHeight,25*realHeight,25*realHeight),""+cardsInGame[ca].hp,guiStyle);
						//DMG
						CheckElemental(cards[cardsInGame[ca].id].attackType,0);
						if(cardsInGame[ca].attack>0)
							GUI.DrawTexture(new Rect(sloPosX,sloPosY+50*realHeight,25*realHeight,25*realHeight),GUITextures[4]);
						else 
							GUI.DrawTexture(new Rect(sloPosX,sloPosY+50*realHeight,25*realHeight,25*realHeight),GUITextures[5]);
						GUI.Label(new Rect(sloPosX,sloPosY+50*realHeight,25*realHeight,25*realHeight),""+Mathf.Abs(cardsInGame[ca].attack),guiStyle);
						
						
						//CHECK_READY
						if(!cardsInGame[ca].readyToAttack)
						{
							GUI.color=Color.white;
							GUI.DrawTexture(new Rect(sloPosX,sloPosY,25*realHeight,25*realHeight),GUITextures[12]);
						}
						

					}
					
					
					
					
					//WAIT
					if(skipping&&ca==p+8*playerNow)
					{
						GUI.color = new Color(1,1,1,animaProgress);
						GUI.DrawTexture(new Rect(sloPosX+45*realHeight-effectAnima*realHeight/2,sloPosY+45*realHeight-effectAnima*realHeight/2,effectAnima*realHeight,effectAnima*realHeight),GUITextures[11]);
						GUI.color=Color.white;
					}
					//HIT_EFFECT
					else if(cardsInGame[ca].beingHit)
					{
						GUI.color = new Color(1,1,1,animaProgress);
						GUI.DrawTexture(new Rect(sloPosX+45*realHeight-effectAnima*realHeight/2,sloPosY+45*realHeight-effectAnima*realHeight/2,effectAnima*realHeight,effectAnima*realHeight),cardsInGame[p+playerNow*8].attackEffect);
						GUI.color=Color.white;
					}		
					
					//BUTTONS
					else if(allowFight==false)
					{
						if(GUI.Button(new Rect(sloPosX,sloPosY,90*realHeight,90*realHeight),"",guiStyle))
						{
							//CAST CARD
							Event r = Event.current;
							if (r.button == 0)
							{
								if(ca>-1+playerNow*8 && ca<8+playerNow*8 && cardsInGame[ca].id==0 && cardChoose==true)
								{
									if(posSlot==ca-8*playerNow)
									{
										CastCard();
									}
									else 
									{
										posSlot=ca-8*playerNow;
									}
								}
							}
							//CHECK CARD STATS
							if (r.button == 1)
							{
								if(cardsInGame[ca].id!=0)
								{
									cardInfoId=cardsInGame[ca].id;
									cardInfo=true;
								}
							}
						}
					}
					GUI.color=Color.white;
					x1++;
					
					//LOOP_HELP
					if(x1==4)
					{
						x1=0;
						y1++;
						if(y1==2)
						{
							moreY=-300;
						}
					}
				}
				
				
			}
			#endregion
			#region choose_buttons
			//CARD_CHOOSE_BUTTONS
			if(players[playerNow].cpuControl==false && allowFight==false)
			{
				SmallCardSetting(0);
				if(GUI.Button(new Rect(pos1-40*realHeight,Screen.height-(160*realHeight)-pos3,30*realHeight,150*realHeight),"",buttonGuiStyle))
				{
					LeftPush();
				}
					
				if(GUI.Button(new Rect(pos1+Mathf.Min(visibleCards,players[playerNow].deck.Length)*100*realHeight,Screen.height-(160*realHeight)-pos3,30*realHeight,150*realHeight),"",buttonGuiStyle2))
				{
					RightPush();
				}
			}

			#endregion
			#region hidden_cards
			//PLAYER1_HIDDEN_CARDS
			for(int l = 0; l<Mathf.Min(visibleCards,players[1].deck.Length); l++)
			{
				SmallCardSetting(l);
				if(players[1].cpuControl || playerNow==0 || allowFight)
				{
					GUI.DrawTexture(new Rect(Screen.width-(l+1)*100*realHeight-40*realHeight,10*realHeight,90*realHeight,150*realHeight),GUITextures[9]);
				}
			}
			
			//PLAYER0_HIDDEN_CARDS
			for(int p = 0; p<Mathf.Min(visibleCards,players[0].deck.Length); p++)
			{
				SmallCardSetting(p);
				if(players[0].cpuControl || playerNow==1 || allowFight)
				{
					GUI.DrawTexture(new Rect(40*realHeight+p*100*realHeight,Screen.height-(160*realHeight),90*realHeight,150*realHeight),GUITextures[9]);
				}
			}
			
			#endregion
			#region visible_deck
			//CARD CARD CARD HERE
			//IF_IAM_NOT_CPU
			if(players[playerNow].cpuControl==false && allowFight==false)
			{
				
				//CARDS
				for(k = 0; k<Mathf.Min(visibleCards,players[playerNow].deck.Length); k++)
				{
					
					
					//VIRTUAL_CARDS_FOR_REWINDING
					virtualCard=k+pushCard;
					if(virtualCard>players[playerNow].deck.Length-1)
					{
						virtualCard=virtualCard-players[playerNow].deck.Length;
					}
					else if(virtualCard<0)
					{
						virtualCard=players[playerNow].deck.Length+virtualCard;
					}
					
					
					Event e = Event.current;
					//POSITION & PROPORTION
					SmallCardSetting(k);

					//HOVER
					if(k==cardChoise)
					{
						if(cardChoose==false)
						{
							GUI.DrawTexture(new Rect(pos1-4+k*100*realHeight,Screen.height-(160*realHeight)-4-pos3,90*realHeight+8,150*realHeight+8),GUITextures[10]);
						}
					}
					//SMALL_CARD
					if(k!=cardChoise || k==cardChoise && !cardChoose)
					{
						if(GUI.Button(new Rect(pos1+rk*100*realHeight,Screen.height-(160*realHeight)-pos3,90*realHeight,150*realHeight),"",guiStyle))
						{
							if(paused==false)
							{
								if (e.button == 0)
								{
									if(cardChoise==k)
									{
										cardChoose=true;
										animaProgress=0;
										allowAnimation=true;
									}
									else
									{
										cardChoise=k;
									}
								}
								//CHECK CARD STATS
								else if (e.button == 1)
								{
									cardInfoId=cards[players[playerNow].deck[virtualCard]].falseID;
									cardInfo=true;
								}
							}
						}
					}
					//BIG_CARD
					else if(cardChoose)
					{
						
						//POSITION & PROPORTION
						BigCardSetting();
						
					}
					
					if(GUI.Button(new Rect(pos1+rk*100*realHeight,Screen.height-(160*realHeight)-pos3,90*realHeight,150*realHeight),"",guiStyle))
					{
						//CHECK CARD STATS
						if (e.button == 1)
						{
							cardInfoId=cards[players[playerNow].deck[virtualCard]].falseID;
							cardInfo=true;
						}
					}
					
					
					//BASE&NAME&PORTRAIT
					GUI.DrawTexture(new Rect(pos1+rk*100*realHeight,Screen.height-(160*realHeight)-pos3,90*realHeight,150*realHeight),GUITextures[2]);
					GUI.DrawTexture(new Rect(pos1+rk*100*realHeight+7*realHeight-1,Screen.height-(160*realHeight)+(15*realHeight)-1-pos3,(65*realHeight+2),(65*realHeight+2)),cards[players[playerNow].deck[virtualCard]].BG);
					GUI.DrawTexture(new Rect(pos1+rk*100*realHeight+7*realHeight,Screen.height-(160*realHeight)+(15*realHeight)-pos3,65*realHeight,65*realHeight),cards[players[playerNow].deck[virtualCard]].portrait);
					
					//BORDER COLOR
					if(cards[players[playerNow].deck[virtualCard]].type==0)
						GUI.color=Color.red;
					else if(cards[players[playerNow].deck[virtualCard]].type==1)
						GUI.color=Color.yellow;
					else if(cards[players[playerNow].deck[virtualCard]].type==2)
						GUI.color=Color.blue;
					else if(cards[players[playerNow].deck[virtualCard]].type==3)
						GUI.color=Color.green;
					else if(cards[players[playerNow].deck[virtualCard]].type==4)
						GUI.color=Color.magenta;
					
					GUI.DrawTexture(new Rect(pos1+rk*100*realHeight+7*realHeight-1,Screen.height-(160*realHeight)+(15*realHeight)-1-pos3,(65*realHeight+2),(65*realHeight+2)),GUITextures[17]);
					GUI.color=Color.white;
					
					guiStyle.fontSize = 11 * fontSizer/10;
					GUI.Label(new Rect(pos1+rk*100*realHeight+45*realHeight-325/10*realHeight,Screen.height-(78*realHeight)-pos3,65*realHeight,15*realHeight),cards[players[playerNow].deck[virtualCard]].cardName,guiStyle);
					
					//LEVEL_STARS
					for(int lev = 0; lev<cards[players[playerNow].deck[virtualCard]].level; lev++)
					{
						GUI.DrawTexture(new Rect(5*realHeight+pos1+rk*100*realHeight+lev*(6*realHeight),Screen.height-(155*realHeight)-pos3,7*realHeight,7*realHeight),GUITextures[8]);	
					}
					
					//HEALTH_POINTS
					CheckElemental(cards[players[playerNow].deck[virtualCard]].hpType,1);
					GUI.DrawTexture(new Rect(pos1+65*realHeight+rk*100*realHeight,5*realHeight+Screen.height-(160*realHeight)-pos3,20*realHeight,20*realHeight),GUITextures[3]);
					guiStyle.fontSize = 10 * fontSizer/10 ;
					GUI.Label(new Rect(pos1+67*realHeight+rk*100*realHeight,7*realHeight+Screen.height-(160*realHeight)-pos3,16*realHeight,16*realHeight),""+cards[players[playerNow].deck[virtualCard]].hp,guiStyle);
					guiStyle.normal.textColor = Color.black;
					
					
					//DAMAGE
					CheckElemental(cards[players[playerNow].deck[virtualCard]].attackType,0);
					if(cards[players[playerNow].deck[virtualCard]].attack>0)
						GUI.DrawTexture(new Rect(pos1+65*realHeight+rk*100*realHeight,26*realHeight+Screen.height-(160*realHeight)-pos3,20*realHeight,20*realHeight),GUITextures[4]);
					else
						GUI.DrawTexture(new Rect(pos1+65*realHeight+rk*100*realHeight,26*realHeight+Screen.height-(160*realHeight)-pos3,20*realHeight,20*realHeight),GUITextures[5]);
					guiStyle.fontSize = 10 * fontSizer/10 ;
					GUI.Label(new Rect(pos1+67*realHeight+rk*100*realHeight,28*realHeight+Screen.height-(160*realHeight)-pos3,16*realHeight,16*realHeight),""+Mathf.Abs(cards[players[playerNow].deck[virtualCard]].attack),guiStyle);
					guiStyle.normal.textColor = Color.black;
					
					
					//MANA
					GUI.color=Color.white;
					GUI.DrawTexture(new Rect(pos1+65*realHeight+rk*100*realHeight,47*realHeight+Screen.height-(160*realHeight)-pos3,20*realHeight,20*realHeight),GUITextures[6]);
					guiStyle.fontSize = 10 * fontSizer/10 ;
					GUI.Label(new Rect(pos1+67*realHeight+rk*100*realHeight,49*realHeight+Screen.height-(160*realHeight)-pos3,16*realHeight,16*realHeight),""+cards[players[playerNow].deck[virtualCard]].manaCost,guiStyle);
					guiStyle.normal.textColor = Color.black;
					
					//DESC
					GUI.DrawTexture(new Rect(pos1+rk*100*realHeight+45*realHeight-35*realHeight,100*realHeight+Screen.height-(160*realHeight)-pos3,70*realHeight,40*realHeight),GUITextures[7]);
					
					onlyQuoteFont.fontSize = 6 * fontSizer/10 ;
					GUI.Label(new Rect(pos1+rk*100*realHeight+45*realHeight-30*realHeight,105*realHeight+Screen.height-(160*realHeight)-pos3,60*realHeight,30*realHeight),"<i>"+cards[players[playerNow].deck[virtualCard]].shortDesc+"</i>",onlyQuoteFont);
					guiStyle.wordWrap = false;
					
					
					//SPECIALS
					for(int b=0;b<cards[players[playerNow].deck[virtualCard]].specials.Length;b++)
					{
						GUI.DrawTexture(new Rect(pos1+rk*100*realHeight+5*realHeight,5*realHeight+Screen.height-((153-b*14)*realHeight)-pos3,12*realHeight,12*realHeight),GUITextures[7]);
						GUI.DrawTexture(new Rect(pos1+rk*100*realHeight+5*realHeight,5*realHeight+Screen.height-((153-b*14)*realHeight)-pos3,12*realHeight,12*realHeight),GUITextures[20+cards[players[playerNow].deck[virtualCard]].specials[b]]);
					}
					
					//COOLDOWN_CARD
					if(players[playerNow].cardCooldown[virtualCard]!=0)
					{
					GUI.DrawTexture(new Rect(pos1+rk*100*realHeight,Screen.height-(160*realHeight)-pos3,90*realHeight,150*realHeight),GUITextures[18]);
					GUI.DrawTexture(new Rect(pos1+rk*100*realHeight,Screen.height-(130*realHeight)-pos3,90*realHeight,90*realHeight),GUITextures[20]);
					onlyQuoteFont.fontSize = 52 * fontSizer/10;
					onlyQuoteFont.normal.textColor = Color.white;
					GUI.Label(new Rect(pos1+rk*100*realHeight,Screen.height-(160*realHeight)-pos3,90*realHeight,150*realHeight),players[playerNow].cardCooldown[virtualCard]+"",onlyQuoteFont);
					onlyQuoteFont.normal.textColor = Color.black;
					}
					
					
					SmallCardSetting(0);

				}
			}
			//IF_IAM_CPU
			else
			{
				//?
			}
			
			#endregion
			#region independent icons
			for(int v=0;v<indepIcons.Length;v++)
			{
				if(indepIcons[v].icon!=null)
				{
					int disortionX=0;
					int disortionY=0;
					if(indepIcons[v].disort==true)
					{
						disortionX=Random.Range(-1,1);
						disortionY=Random.Range(-1,1);
					}
					GUI.color = new Color(1,1,1,1-(indepIcons[v].timer/3));
					GUI.DrawTexture(new Rect(indepIcons[v].posX-17.5f*indepIcons[v].timer*realHeight*indepIcons[v].scaler+disortionX*realHeight, indepIcons[v].posY-17.5f*indepIcons[v].timer*realHeight*indepIcons[v].scaler+disortionY*realHeight, 
					indepIcons[v].timer*realHeight*35*indepIcons[v].scaler,indepIcons[v].timer*realHeight*35*indepIcons[v].scaler),indepIcons[v].icon);
				}		
			}
			#endregion
		}
		//CARD INFO GUI
		else
		{
			
			superfontNormalizer = realWidth * 10;
			superfontSizer = (int)superfontNormalizer;
			
			//BLACK
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),GUITextures[19]);
			//BOOK
			GUI.DrawTexture(new Rect(0,0,1024*realWidth,576*realWidth),GUITextures[29],ScaleMode.ScaleToFit, true);

	
			
			//BASE&NAME&PORTRAIT
			guiStyle.normal.textColor = Color.white;
			guiStyle.fontSize = 52 * superfontSizer/10;
			GUI.DrawTexture(new Rect(135*realWidth,55*realWidth,Screen.height/3.5f,Screen.height/3.5f),cards[cardInfoId].BG);
			GUI.DrawTexture(new Rect(135*realWidth,55*realWidth,Screen.height/3.5f,Screen.height/3.5f),cards[cardInfoId].portrait);
			
			//BORDER COLOR
			if(cards[cardInfoId].type==0)
				GUI.color=Color.red;
			else if(cards[cardInfoId].type==1)
				GUI.color=Color.yellow;
			else if(cards[cardInfoId].type==2)
				GUI.color=Color.blue;
			else if(cards[cardInfoId].type==3)
				GUI.color=Color.green;
			else if(cards[cardInfoId].type==4)
				GUI.color=Color.magenta;
			
			GUI.DrawTexture(new Rect(135*realWidth,55*realWidth,Screen.height/3.5f,Screen.height/3.5f),GUITextures[17]);
			GUI.color=Color.white;
			
			
			
			
			
			onlyQuoteFont.wordWrap = true;
			onlyQuoteFont.alignment = TextAnchor.UpperLeft;
			
			onlyQuoteFont.fontSize = 22 * superfontSizer/10;
			//LEVEL
			GUI.Label(new Rect(Screen.height/3.5f+140*realWidth,55*realWidth,Screen.width/4,35*realWidth),"Level: <color=cyan>"+cards[cardInfoId].level+"</color>",onlyQuoteFont);
			
			//MANA_COST
			GUI.Label(new Rect(Screen.height/3.5f+140*realWidth,85*realWidth,Screen.width/4,35*realWidth),"Mana Cost: <color=cyan>"+cards[cardInfoId].manaCost+"</color>",onlyQuoteFont);
			
			//TYPE
			if(cards[cardInfoId].type==0)
				GUI.Label(new Rect(Screen.height/3.5f+140*realWidth,115*realWidth,Screen.width/8,35*realWidth),"Type: <color=red>Infantry</color>",onlyQuoteFont);
			else if(cards[cardInfoId].type==1)
				GUI.Label(new Rect(Screen.height/3.5f+140*realWidth,115*realWidth,Screen.width/8,35*realWidth),"Type: <color=yellow>Ranged</color>",onlyQuoteFont);
			else if(cards[cardInfoId].type==2)
				GUI.Label(new Rect(Screen.height/3.5f+140*realWidth,115*realWidth,Screen.width/8,35*realWidth),"Type: <color=blue>Group Damage</color>",onlyQuoteFont);
			else if(cards[cardInfoId].type==3)
				GUI.Label(new Rect(Screen.height/3.5f+140*realWidth,115*realWidth,Screen.width/8,35*realWidth),"Type: <color=green>Healer</color>",onlyQuoteFont);
			else if(cards[cardInfoId].type==4)
				GUI.Label(new Rect(Screen.height/3.5f+140*realWidth,115*realWidth,Screen.width/8,35*realWidth),"Type: <color=magenta>Group Healer</color>",onlyQuoteFont);


			
			//DESC
			onlyQuoteFont.fontSize = 32 * superfontSizer/10;
			GUI.Label(new Rect(135*realWidth,Screen.height/3.5f+60*realWidth,Screen.width/3,Screen.height/3),""+cards[cardInfoId].cardName+"",onlyQuoteFont);
			onlyQuoteFont.fontSize = 18 * superfontSizer/10;
			GUI.Label(new Rect(135*realWidth,Screen.height/3.5f+100*realWidth,Screen.width/3,Screen.height/3),"	"+cards[cardInfoId].longDesc+"",onlyQuoteFont);
			
	
	
			
	
	
			onlyQuoteFont.fontSize = 18 * superfontSizer/10;
			//HEALTH
			GUI.Label(new Rect(Screen.width/2+25*realWidth,65*realWidth,Screen.width/4,35*realWidth),"Health: <color=cyan>"+cards[cardInfoId].hp+"</color>",onlyQuoteFont);
			//HEALTH_TYPE
			if(cards[cardInfoId].hpType==0)
			{
				GUI.Label(new Rect(Screen.width/2+25*realWidth,95*realWidth,Screen.width/4,35*realWidth),"Health Type: <color=white>Normal</color>",onlyQuoteFont);
				onlyQuoteFont.fontSize = 14 * superfontSizer/10;
				GUI.Label(new Rect(Screen.width/2+25*realWidth,125*realWidth,Screen.width/3,35*realWidth),"no bonus",onlyQuoteFont);
			}
			else if(cards[cardInfoId].hpType==1)
			{
				GUI.Label(new Rect(Screen.width/2+25*realWidth,95*realWidth,Screen.width/4,35*realWidth),"Health Type: <color=grey>Undead</color>",onlyQuoteFont);
				onlyQuoteFont.fontSize = 14 * superfontSizer/10;
				GUI.Label(new Rect(Screen.width/2+25*realWidth,125*realWidth,Screen.width/3,35*realWidth),"<color=green>poison</color> effect resistance, <color=cyan>-1</color><color=white> normal</color> damage",onlyQuoteFont);
			}
			else if(cards[cardInfoId].hpType==2)
			{
				GUI.Label(new Rect(Screen.width/2+25*realWidth,95*realWidth,Screen.width/4,35*realWidth),"Health Type: <color=blue>Magic</color>",onlyQuoteFont);
				onlyQuoteFont.fontSize = 14 * superfontSizer/10;
				GUI.Label(new Rect(Screen.width/2+25*realWidth,125*realWidth,Screen.width/3,35*realWidth),"<color=green>poison</color>, <color=blue>frost</color> and <color=red>fire</color> effects resistance",onlyQuoteFont);
				
			}
			else if(cards[cardInfoId].hpType==3)
			{
				GUI.Label(new Rect(Screen.width/2+25*realWidth,95*realWidth,Screen.width/4,35*realWidth),"Health Type: <color=green>Nature</color>",onlyQuoteFont);
				onlyQuoteFont.fontSize = 14 * superfontSizer/10;
				GUI.Label(new Rect(Screen.width/2+25*realWidth,125*realWidth,Screen.width/3,35*realWidth),"<color=green>poison</color> effect resistance, <color=cyan>-1</color><color=green> poison</color> damage, <color=cyan>+1</color> healing",onlyQuoteFont);
			}

			
			
			onlyQuoteFont.fontSize = 18 * superfontSizer/10;
			//DAMAGE
			if(cards[cardInfoId].attack>0)
			{
				GUI.Label(new Rect(Screen.width/2+25*realWidth,165*realWidth,Screen.width/4,35*realWidth),"Damage: <color=cyan>"+cards[cardInfoId].attack+"</color>",onlyQuoteFont);
				//DAMAGE_TYPE
				if(cards[cardInfoId].attackType==0)
				{
					GUI.Label(new Rect(Screen.width/2+25*realWidth,195*realWidth,Screen.width/4,35*realWidth),"Damage Type: <color=white>Normal</color>",onlyQuoteFont);
					onlyQuoteFont.fontSize = 14 * superfontSizer/10;
					GUI.Label(new Rect(Screen.width/2+25*realWidth,225*realWidth,Screen.width/3,35*realWidth),"no bonus",onlyQuoteFont);
				}
				else if(cards[cardInfoId].attackType==1)
				{
					GUI.Label(new Rect(Screen.width/2+25*realWidth,195*realWidth,Screen.width/4,35*realWidth),"Damage Type: <color=red>Fire</color>",onlyQuoteFont);
					onlyQuoteFont.fontSize = 14 * superfontSizer/10;
					GUI.Label(new Rect(Screen.width/2+25*realWidth,225*realWidth,Screen.width/3,35*realWidth),"ignores armor, <color=cyan>+1 </color>damage to<color=green> nature</color>",onlyQuoteFont);
				}
				else if(cards[cardInfoId].attackType==2)
				{
					GUI.Label(new Rect(Screen.width/2+25*realWidth,195*realWidth,Screen.width/4,35*realWidth),"Damage Type: <color=blue>Ice</color>",onlyQuoteFont);
					onlyQuoteFont.fontSize = 14 * superfontSizer/10;
					GUI.Label(new Rect(Screen.width/2+25*realWidth,225*realWidth,Screen.width/3,35*realWidth),"victim looses max health aswell",onlyQuoteFont);
					
				}
				else if(cards[cardInfoId].attackType==3)
				{
					GUI.Label(new Rect(Screen.width/2+25*realWidth,195*realWidth,Screen.width/4,35*realWidth),"Damage Type: <color=green>Poison</color>",onlyQuoteFont);
					onlyQuoteFont.fontSize = 14 * superfontSizer/10;
					GUI.Label(new Rect(Screen.width/2+25*realWidth,225*realWidth,Screen.width/3,35*realWidth),"causes <color=green>poison</color> effect (<color=cyan>-1</color> damage at next attack)",onlyQuoteFont);
				}
				else if(cards[cardInfoId].attackType==4)
				{
					GUI.Label(new Rect(Screen.width/2+25*realWidth,195*realWidth,Screen.width/4,35*realWidth),"Damage Type: <color=yellow>Holy</color>",onlyQuoteFont);
					onlyQuoteFont.fontSize = 14 * superfontSizer/10;
					GUI.Label(new Rect(Screen.width/2+25*realWidth,225*realWidth,Screen.width/3,35*realWidth),"<color=cyan>+2</color> damage to<color=grey> undead</color>",onlyQuoteFont);
				}
			}
			//HEAL
			else
			{
				GUI.Label(new Rect(Screen.width/2+25*realWidth,165*realWidth,Screen.width/4,35*realWidth),"Heal: <color=cyan>"+Mathf.Abs(cards[cardInfoId].attack)+"</color>",onlyQuoteFont);
				GUI.Label(new Rect(Screen.width/2+25*realWidth,195*realWidth,Screen.width/4,35*realWidth),"Heal Type: <color=magenta>Life</color>",onlyQuoteFont);
				onlyQuoteFont.fontSize = 14 * superfontSizer/10;
				GUI.Label(new Rect(Screen.width/2+25*realWidth,225*realWidth,Screen.width/3,35*realWidth),"can't heal<color=grey> undead</color>",onlyQuoteFont);
			}

				

			onlyQuoteFont.fontSize = 18 * superfontSizer/10;
			//SPECIALS
			GUI.Label(new Rect(Screen.width/2+25*realWidth,255*realWidth,Screen.width/4,35*realWidth),"Special Skills:",onlyQuoteFont);
			for(int b=0;b<cards[cardInfoId].specials.Length;b++)
			{
				GUI.DrawTexture(new Rect(Screen.width/2+25*realWidth,290*realWidth+b*65*realWidth,35*realWidth,35*realWidth),GUITextures[20+cards[cardInfoId].specials[b]]);
				onlyQuoteFont.alignment = TextAnchor.MiddleCenter;
				onlyQuoteFont.fontSize = 14 * superfontSizer/10;
				if(cards[cardInfoId].specials[b]==1)
					GUI.Label(new Rect(Screen.width/2+60*realWidth,290*realWidth+b*65*realWidth,80*realWidth,50*realWidth),"(On Entrance)",onlyQuoteFont);
				else if(cards[cardInfoId].specials[b]==2)
					GUI.Label(new Rect(Screen.width/2+60*realWidth,290*realWidth+b*65*realWidth,80*realWidth,50*realWidth),"(On Exit/Death)",onlyQuoteFont);
				else if(cards[cardInfoId].specials[b]==3)
					GUI.Label(new Rect(Screen.width/2+60*realWidth,290*realWidth+b*65*realWidth,80*realWidth,50*realWidth),"(Every Attack)",onlyQuoteFont);
				else if(cards[cardInfoId].specials[b]==4 || cards[cardInfoId].specials[b]==8)
					GUI.Label(new Rect(Screen.width/2+60*realWidth,290*realWidth+b*65*realWidth,80*realWidth,50*realWidth),"(Every Injury)",onlyQuoteFont);
				else if(cards[cardInfoId].specials[b]==5)
					GUI.Label(new Rect(Screen.width/2+60*realWidth,290*realWidth+b*65*realWidth,80*realWidth,50*realWidth),"(On Kill)",onlyQuoteFont);
				else if(cards[cardInfoId].specials[b]==6)
					GUI.Label(new Rect(Screen.width/2+60*realWidth,290*realWidth+b*65*realWidth,80*realWidth,50*realWidth),"(On Caster's Injury)",onlyQuoteFont);
				else if(cards[cardInfoId].specials[b]==7)
					GUI.Label(new Rect(Screen.width/2+60*realWidth,290*realWidth+b*65*realWidth,80*realWidth,50*realWidth),"(On Anyone Death)",onlyQuoteFont);
				
				GUI.color = new Color(1,1,1,0.5f);
				GUI.DrawTexture(new Rect(Screen.width/2+167*realWidth,290*realWidth+b*65*realWidth,46*realWidth,46*realWidth),specials[cards[cardInfoId].specialID[b]].icon);
				GUI.color=Color.white;
				
				GUI.Label(new Rect(Screen.width/2+140*realWidth,290*realWidth+b*65*realWidth,100*realWidth,50*realWidth),""+specials[cards[cardInfoId].specialID[b]].name+"",onlyQuoteFont);
				GUI.Label(new Rect(Screen.width/2+240*realWidth,290*realWidth+b*65*realWidth,150*realWidth,50*realWidth),"-"+specials[cards[cardInfoId].specialID[b]].desc+"",onlyQuoteFont);
			}
			
			
			//BACK
			if(GUI.Button(new Rect(Screen.width-105*realWidth,Screen.height-35*realWidth,100*realWidth,30*realWidth),"Back",guiStyleSlot))
			{
				cardInfo=false;
			}
			
			onlyQuoteFont.alignment = TextAnchor.MiddleCenter;
			onlyQuoteFont.normal.textColor = Color.black;
			
			
		}
		#region startGradient&pause_menu
		//START_GRADIENT
		if(startGradient)
		{
			GUI.color = new Color(1,1,1,startGradientTimer);
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),GUITextures[19]);
		}
		//PAUSE_MENU
		else if(paused&&!matchend)
		{
			GUI.color=Color.white;
			guiStyleSlot.fontSize = 15 * fontSizer/10;
			guiStyleMenu.fontSize = 15 * fontSizer/10;
			guiStyleLabel.fixedWidth=25*realHeight;
			guiStyleLabel.fixedHeight=25*realHeight;
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),GUITextures[18]);
			GUI.Box(new Rect(Screen.width/2-(200*realHeight),Screen.height/2-100*realHeight,400*realHeight,200*realHeight),"",guiStyleSlot);

			//EXIT TO MENU
			if(GUI.Button(new Rect(Screen.width/2+(50*realHeight),Screen.height/2+60*realHeight,125*realHeight,25*realHeight),"Exit to Menu",guiStyleSlot))
			{
				if(onlineGame)
					gameMenuScript.StopOnline();
				if(tourMatch)
				{
					if(players[0].cpuControl==true)
					{
						gameMenuScript.result[0]=3;
						gameMenuScript.result[1]=0;
					}
					else
					{
						gameMenuScript.result[0]=0;
						gameMenuScript.result[1]=3;
					}
					gameMenuScript.GiveResult();
				}
				gameMen.SetActive(true);
				gameObject.SetActive(false);
			}
			
			//BACK TO GAME
			if(GUI.Button(new Rect(Screen.width/2-(175*realHeight),Screen.height/2+60*realHeight,125*realHeight,25*realHeight),"Back to Game",guiStyleSlot))
			{
				paused=false;
			}
			
			GUI.Label(new Rect(Screen.width/2-(175*realHeight),Screen.height/2-20*realHeight,125*realHeight,25*realHeight),"Game Speed:",guiStyleMenu);
			goFaster = GUI.HorizontalSlider(new Rect(Screen.width/2-(50*realHeight),Screen.height/2-20*realHeight,200*realHeight,25*realHeight), goFaster, 1, 5,guiStyleSlot,guiStyleLabel);

			
			GUI.Label(new Rect(Screen.width/2-(175*realHeight),Screen.height/2-60*realHeight,125*realHeight,25*realHeight),"Volume:",guiStyleMenu);
			audio.volume = GUI.HorizontalSlider(new Rect(Screen.width/2-(50*realHeight),Screen.height/2-60*realHeight,200*realHeight,25*realHeight), audio.volume, 0, 1,guiStyleSlot,guiStyleLabel);
			
			if(tourMatch)
			{
				GUI.Label(new Rect(Screen.width/2-(175*realHeight),Screen.height/2+20*realHeight,300*realHeight,25*realHeight),"In tournament, quiting match means defeat.",guiStyleMenu);
			}
			
			
		}
		else if (matchend)
		{
			GUI.color=Color.white;
			guiStyleSlot.fontSize = 15 * fontSizer/10;
			guiStyleMenu.fontSize = 15 * fontSizer/10;
			GUI.Box(new Rect(Screen.width/2-(200*realHeight),Screen.height/2-100*realHeight,400*realHeight,200*realHeight),"",guiStyleSlot);
			if(!demoState)
				GUI.Label(new Rect(Screen.width/2-60*realHeight,Screen.height/2-90*realHeight,120*realHeight,25*realHeight),"Duel Ended",guiStyleMenu);
			else
				GUI.Label(new Rect(Screen.width/2-60*realHeight,Screen.height/2-90*realHeight,120*realHeight,25*realHeight),"Demo Ended",guiStyleMenu);
			
			if(players[0].hp<=0)
				GUI.Label(new Rect(Screen.width/2-100*realHeight,Screen.height/2-50*realHeight,200*realHeight,25*realHeight),players[1].playerName+" is a winner.",guiStyleMenu);
			else if(players[1].hp<=0)
				GUI.Label(new Rect(Screen.width/2-100*realHeight,Screen.height/2-50*realHeight,200*realHeight,25*realHeight),players[0].playerName+" is a winner.",guiStyleMenu);
			else 
				GUI.Label(new Rect(Screen.width/2-100*realHeight,Screen.height/2-50*realHeight,200*realHeight,25*realHeight),"Draw",guiStyleMenu);
			//EXIT TO MENU
			if(GUI.Button(new Rect(Screen.width/2+(50*realHeight),Screen.height/2+60*realHeight,125*realHeight,25*realHeight),"Exit to Menu",guiStyleSlot))
			{
				if(onlineGame)
					gameMenuScript.StopOnline();
				if(tourMatch)
				{
					if(players[0].hp<=0)
					{
						gameMenuScript.result[0]=0;
						gameMenuScript.result[1]=3;
					}
					else if(players[1].hp<=0)
					{
						gameMenuScript.result[0]=3;
						gameMenuScript.result[1]=0;
					}
					else 
					{
						gameMenuScript.result[0]=1;
						gameMenuScript.result[1]=1;
					}
					gameMenuScript.GiveResult();
				}
				gameMen.SetActive(true);
				gameObject.SetActive(false);
			}
			
			
			
			//SAVE DEMO
			if(!savedDemo)
			{
				if(!demoState)
					if(GUI.Button(new Rect(Screen.width/2-(175*realHeight),Screen.height/2+60*realHeight,125*realHeight,25*realHeight),"Save Demo",guiStyleSlot))
					{
						
						duelName = System.DateTime.Now.ToString("yyyy.MM.dd HH.mm")+" - "+players[0].playerName+" vs "+players[1].playerName;
						duelCaster[0]=gameMenuScript.xchar[0];
						duelCaster[1]=gameMenuScript.xchar[1];

						SaveLoad.currentFilePath=this.duelName+".DEM";
						SaveData.duelVersion=gameMenuScript.version;
						SaveData.duelName=this.duelName;
						SaveData.duelCaster=this.duelCaster;
						if(players[0].hp<=0)
							SaveData.winner=1;
						else if(players[1].hp<=0)
							SaveData.winner=0;
						else 
							SaveData.winner=2;
						SaveData.duelDeckCard=this.duelDeckCard;
						SaveData.duelDeck0=this.players[0].deck;
						SaveData.duelDeck1=this.players[1].deck;
						SaveData.duelPos=this.duelPos;
						SaveData.dc=drawCount;
						duelCount++;
						duelDeckCard[duelCount]=777;
						SaveLoad.Save();
						savedDemo=true;
					}
			}
			else
			{
				GUI.Label(new Rect(Screen.width/2-60*realHeight,Screen.height/2-10*realHeight,120*realHeight,25*realHeight),"Demo Saved!",guiStyleMenu);
			}
		}
		#endregion
	}
	#endregion
	#region Update
	public void Update()
	{
		if(onlineGame)
			if(!isLocalPlayer)
				return;
		if(!matchend)
		{
			//CARD INFO CANCELLER
			if(cardInfo)
			{
				if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Tab))
					cardInfo=false;
			}
			else if(!players[playerNow].cpuControl&&!allowFight)
				if(Input.GetKeyDown(KeyCode.Tab))
				{
					cardInfo=true;
					if(cardChoise+pushCard<players[playerNow].deck.Length)
						cardInfoId=cards[players[playerNow].deck[cardChoise+pushCard]].falseID;
					else 
						cardInfoId=cards[players[playerNow].deck[cardChoise+pushCard-players[playerNow].deck.Length]].falseID;
				}
			
			//START_GRADIENT
			if(startGradient)
			{
				startGradientTimer-=Time.deltaTime*1;
				if(startGradientTimer<=0)
				{
					startGradient=false;
					paused=false;
				}
			}
			
			//PAUSE
			if(Input.GetKeyDown(KeyCode.Escape)&&paused&&!cardInfo)
			{
				paused=false;
			}
			else if(Input.GetKeyDown(KeyCode.Escape)&&!paused&&cardInfo)
			{
				cardInfo=false;
			}
			
			//UNPAUSED
			else if(paused==false)
			{
						
				for(int v=0;v<indepIcons.Length;v++)
				{
					if(indepIcons[v].icon)
					{
						indepIcons[v].timer+=Time.deltaTime*1.5f*goFaster;
						if(indepIcons[v].timer>=3)
						{
							indepIcons[v].icon=null;
							indepIcons[v].timer=0;
						}
					}
				}
				
				#region choosing
				//CHOOSE_CARD
				if(!players[playerNow].cpuControl)
				{
					if(cardChoose==false && allowAnimation==false)
					{
						if(Input.GetKeyDown(KeyCode.LeftArrow))
						{
							LeftPush();
						}
						if(Input.GetKeyDown(KeyCode.RightArrow))
						{
							RightPush();
						}
						//CLICK_CHOOSEN_CARD
						if(Input.GetKeyDown(KeyCode.Return))
						{
							animaProgress=0;
							allowAnimation=true;
							cardChoose=true;
						}
					}
					//CHOOSE_POSITION
					else
					{
						if(Input.GetKeyDown(KeyCode.LeftArrow))
						{
							posSlot--;
							if(posSlot<0)
								posSlot=0;
						}
						if(Input.GetKeyDown(KeyCode.RightArrow))
						{
							posSlot++;
							if(posSlot>7)
								posSlot=7;
						}
						if(playerNow==0)
						{
							if(Input.GetKeyDown(KeyCode.UpArrow))
							{
								if(posSlot>3)
								posSlot-=4;
							}
							if(Input.GetKeyDown(KeyCode.DownArrow))
							{
								if(posSlot<4)
								posSlot+=4;
							}
						}
						else if (playerNow==1)
						{
							if(Input.GetKeyDown(KeyCode.DownArrow))
							{
								if(posSlot>3)
								posSlot-=4;
							}
							if(Input.GetKeyDown(KeyCode.UpArrow))
							{
								if(posSlot<4)
								posSlot+=4;
							}
						}
						//CONFIRM_POSITION
						if(Input.GetKeyDown(KeyCode.Return))
						{
							CastCard();
						}
						
						if(errorOccupied)
						{
							if(Time.time%0.4<0.2)
								slotLightning= new Color(1,0,0);
							else
								slotLightning=Color.yellow;
						}
						else
						{
						if(Time.time%2<1)
							slotLightning= new Color(0,0.7f,0);
						else
							slotLightning= new Color(0,0.7f,0.7f);
						}
					}
				}
				//CANCEL_CHOOSEN_CARD
				if(Input.GetKeyDown(KeyCode.Escape))
				{
					if(cardChoose==true)
					{
						allowAnimation=false;
						cardChoose=false;
					}
					else
					{
						paused=true;
					}
				}
				#endregion
				
				//SKIP_TURN
				if(Input.GetKeyDown(KeyCode.S) && !allowFight && !players[playerNow].cpuControl)
				{
					SkipTurn();
				}
				
				#region animations
				
				//CARD_ANIMATION
				if(allowAnimation && !allowFight && !errorMana)
				{
						
					//FASTEN_ANIMATION
					if(Input.GetKeyDown(KeyCode.Return) && animaProgress!=0 )
					{
						animaProgress=1;
					}
					
					if(playerNow==0)
					{
						animaPosY=Mathf.Lerp(1*realHeight, 165*realHeight,animaProgress);
						animaPosX=Mathf.Lerp(40*realHeight + cardChoise *100*realHeight, 10*realHeight*(prop/100/2),animaProgress);
					}
					else
					{
						animaPosY=Mathf.Lerp(600*realHeight, 165*realHeight,animaProgress);
						animaPosX=Mathf.Lerp(Screen.width-visibleCards*100*realHeight-40*realHeight + cardChoise *100*realHeight, 10*realHeight*(prop/100/2),animaProgress);
					}
					prop2=(int)Mathf.Lerp(100f,250f,animaProgress);
					animaProgress+=Time.deltaTime*1.5f;
					
					//END ANIMATION
					if(animaProgress>=1)
					{
						allowAnimation=false;
					}
						
				}

				//EFFECT_ANIMATION
				if(allowAnimation && allowFight || allowAnimation && errorMana)
				{
					if(revertAnima)
					{
						animaProgress-=Time.deltaTime*3f*goFaster;
						effectAnima=(int)Mathf.Lerp(0f,75f,animaProgress);
					}
					else if(animaProgress<1 && revertAnima==false)
					{
						revertAnima=false;
						animaProgress+=Time.deltaTime*1.5f*goFaster;
						effectAnima=(int)Mathf.Lerp(0f,75f,animaProgress);
					}
					else
					{
						revertAnima=true;
					}

				}
				
				#endregion
				
				#region updateFight
				//IF_CARDS_FIGHT
				if (allowFight)
				{
					
					//WAIT_1_ROUND
					if(p==-2)
					{
						timer = 0;
						p++;
					}
					else if(p>=0)
					{
						//SKIP_EMPTY
						if(cardsInGame[p+playerNow*8].id==0)
						{
							if(!firstMove)
								timer = 33;
						}
						//CARD_MOVE
						else if(p<8 && moved==false)
						{
							AttackPos(p,p+playerNow*8,playerNow*16);
						}
					}
					
					
					timer += Time.deltaTime*1*goFaster;
					if(timer>1.4)
					{
						ClearHit();
					}
					if(timer>1.5)
					{
						firstMove=false;
						timer=0;
						moved=false;
						p++;
					}
					
					//END_FIGHT
					EndFight();

				}
				
				#endregion
		
				#region AI
				//AI
				
				if(!allowFight)
				if(players[playerNow].cpuControl)
				{
					if(!demoState&&!onlineGame)
					{
						allowFight = false;
						freePos = false;
						enoughMana = false;
						anyCooldown = false;
						
						//CHECK_FOR_FREE_POSITIONS
						if(!freePos)
						{
							FreePos(0,8);
						}
						//CHECK_IF_GOT_MANA
						if(!enoughMana&&freePos)
						{
							for(int z = 0;z<players[playerNow].deck.Length-1;z++)
							{
								if(cards[players[playerNow].deck[z]].manaCost <= players[playerNow].mana)
								{
									enoughMana=true;
									//CHECK_IF_ANY_CARD_IS_NOT_COOLDOWN
									if(players[playerNow].cardCooldown[z]==0)
									{
										anyCooldown=true;
										break;
									}
								}
							}
						}
						

						
						//SKIP_TURN_IF_CANT_CAST
						if(!freePos||!enoughMana||!anyCooldown||skipper==10*players[playerNow].ai)
						{
							SkipTurn();
						}
						
						//DUMB_AI
						////////////////////////////////////////////
						//cast rndC on rndP
						else if(players[playerNow].ai==0)
						{
							AI0();
						}
						////////////////////////////////////////////
						
						//EASY_AI
						////////////////////////////////////////////
						//cast rndC on opt50P
						//allow ranged on line 1
						else if(players[playerNow].ai==1)
						{
							AI1();
						}
						////////////////////////////////////////////
						
						//NORMAL_AI
						////////////////////////////////////////////
						//cast optC on opt100P & no random skips
						else if(players[playerNow].ai==2)
						{
							AI2();
						}
						////////////////////////////////////////////
						
						//HARD_AI
						////////////////////////////////////////////
						//cast optC on opt90P & no random skips & check opposition
						//allow archer and wyvern on line 1
						else if(players[playerNow].ai==3)
						{
							AI3();
						}
						////////////////////////////////////////////
						skipper++;
						
					}
					else if (demoState) //DEMO BOT
					{
						if(duelDeckCard[duelCount]!=999)
						{
							realCard=duelDeckCard[duelCount];
							posSlot=duelPos[duelCount];
							CastCard();
						}
						else
						{
							SkipTurn();
						}
					}
					else if (onlineGame&&canOnline) //MULTIPLAYER BOT
					{
						if(takeCard!=999)
						{
							realCard=takeCard;
							posSlot=takePos;
							CastCard();
							CmdCanOnline(false);
						}
						else
						{
							SkipTurn();
							CmdCanOnline(false);
						}
					}
				}
				
				#endregion
				
				//ERROR_TIMER
				if(errorTimer>0)
				{
					errorTimer-=Time.deltaTime*1;
					if(errorTimer<=0)
					{
						errorMana=false;
						errorOccupied=false;
					}
				}
				
			}
		}
	}
	#endregion
	#region voids
	#region checkEle.and.Animations
	public void CheckElemental(byte type,byte what)
	{
		//ATK
		if(what==0)
		{
			//normal
			if(type==0)
			{
				GUI.color=Color.white;
			}
			//fire
			else if(type==1)
			{
				GUI.color=new Color(1,0.2f,0.2f,1);
			}
			//ice
			else if(type==2)
			{
				GUI.color=new Color(0.2f,0.2f,1,1);
			}
			//poison
			else if(type==3)
			{
				GUI.color=new Color(0.2f,1,0.2f,1);
			}
			//holy
			else if(type==4)
			{
				GUI.color=new Color(1, 0.92f, 0.016f, 1);
			}
		}
		//HP
		else if(what==1)
		{
			//normal
			if(type==0)
			{
				GUI.color=Color.white;
			}
			//undead
			else if(type==1)
			{
				GUI.color=new Color(0.5f,0.5f,0.5f,1);
			}
			//magic
			else if(type==2)
			{
				GUI.color=new Color(0.4f,0.4f,1,1);
			}
			//nature
			else if(type==3)
			{
				GUI.color=new Color(0.4f,1,0.4f,1);
			}
			//infernal
			else if(type==4)
			{
				GUI.color=new Color(1,0.4f,0.4f,1);
			}
		}
	}

	//SMALL_CARD_SETTINGS
	public void SmallCardSetting(int key)
	{
		if(playerNow==0)
		{
			pos4 = 1*realHeight;
			pos2 = 40*realHeight;
		}
		else if(playerNow==1)
		{
			pos4 = 600*realHeight;
			pos2 = Screen.width-Mathf.Min(visibleCards,players[1].deck.Length)*100*realHeight-40*realHeight;
		}
		prop = 100;
		realHeight=heightInFloat/768;
		rk=key;
		pos1 = (int)pos2;
		pos3 = (int)pos4;
		fontNormalizer = realHeight * 10;
		fontSizer = (int)fontNormalizer;
	}
	
	//BIG_CARD_SETTINGS
	public void BigCardSetting()
	{
		prop=prop2;
		realHeight=heightInFloat/768;
		rk=0;
		pos2 = animaPosX;
		pos1 = (int)pos2;
		pos4 = animaPosY;
		pos3 = (int)pos4;
		realHeight *= prop/100;
		fontNormalizer = realHeight * 10 + (prop-100) * 1/100;
		fontSizer = (int)fontNormalizer;
	}
	
	public void SkipAttack()
	{
		ClearAnimation();
		allowAnimation=true;
		skipping=true;
	}
	
	//CLEAR_EFFECTS
	public void ClearHit()
	{
		for(int z=0;z<16;z++)
		{
		cardsInGame[z].beingHit=false;
		}
		//HOURGLASS
		playerHit[0]=false;
		playerHit[1]=false;
		skipping=false;
	}
	
	public void ClearAnimation()
	{
		revertAnima=false;
		allowAnimation=false;
		cardChoose=false;
		animaProgress=0;
		effectAnima=0;
	}
	#endregion
	#region castingCard
	public void TakeAbilities()
	{
		int k=posSlot+playerNow*8;
		int j=players[playerNow].deck[realCard];
		cardsInGame[k].pos=k;
		cardsInGame[k].cardName=cards[j].cardName;
		cardsInGame[k].id=j;
		cardsInGame[k].type=cards[j].type;
		cardsInGame[k].hp=cards[j].hp;
		cardsInGame[k].maxHp=cards[j].hp;
		cardsInGame[k].hpType=cards[j].hpType;
		cardsInGame[k].attack=cards[j].attack;
		cardsInGame[k].attackType=cards[j].attackType;
		cardsInGame[k].attackSound=cards[j].attackSound;
		cardsInGame[k].attackEffect=cards[j].attackEffect;
		cardsInGame[k].poisoned=false;
		cardsInGame[k].readyToAttack=false;
		FindSpec(1,cards[j].falseID,k);
	}
	
	public void CastCard()
	{
		
		//DEM
		if(!demoState)
			if(onlineGame&&!canOnline||!onlineGame)
				realCard=cardChoise+pushCard;
		
		if(paused==false)
		{
			if(realCard>players[playerNow].deck.Length-1&&!demoState)
			{
				realCard-=players[playerNow].deck.Length;
			}
			
			//CHECK_IF_POSSIBLE
			if(cards[players[playerNow].deck[realCard]].manaCost<=players[playerNow].mana && cardsInGame[posSlot+playerNow*8].id==0 && players[playerNow].cardCooldown[realCard]==0)
			{
				players[playerNow].mana-=cards[players[playerNow].deck[realCard]].manaCost;
				players[playerNow].cardCooldown[realCard]=cards[players[playerNow].deck[realCard]].cooldown;
				//DEM
				if(!demoState)
				{
					duelDeckCard[duelCount]=realCard;
					duelPos[duelCount]=posSlot;
				}
				if(onlineGame && players[playerNow].cpuControl==false)
				{
					multiplayerPlayer.sendCard=realCard;
					multiplayerPlayer.sendPos=posSlot;
					multiplayerPlayer.bul=true;
					multiplayerPlayer.doStuff=true;
				}
				TakeAbilities();
				ClearAnimation();
				StartFight();
			}
			//SLOT_NOT_EMPTY
			else if(cardsInGame[posSlot+playerNow*8].id!=0)
			{
				if(players[playerNow].cpuControl==false)
				{
					//ERROR SOUND
					audio.PlayOneShot (sounds[0]);
					errorOccupied=true;
					errorTimer=1.6f;
				}
			}
			//MANA_NOT_ENOUGH
			else if(cards[players[playerNow].deck[realCard]].manaCost>players[playerNow].mana)
			{
				if(players[playerNow].cpuControl==false)
				{
					//LOW MANA SOUND
					audio.PlayOneShot (sounds[1]);
					errorMana=true;
					errorTimer=1.6f;
				}
			}
			//COOLDOWN_NOT_READY
			else if(players[playerNow].cardCooldown[realCard]!=0)
			{
				if(players[playerNow].cpuControl==false)
				{
					//COOLDOWN_ERROR_SOUND
					audio.PlayOneShot (sounds[2]);
				}
			}
		}
	}
	#endregion
	#region start.end.skip.switch
	public void SwitchPlayer()
	{
		if(drawCount!=0)
			if(duelCount==drawCount)
			{
				paused=true;
				matchend=true;
			}
		if(playerNow==0)
		{
			playerNow=1;
		}
		else if(playerNow==1)
		{
			playerNow=0;
		}
		turns[playerNow]++;
		if(turns[playerNow]==5)
		{
			players[playerNow].addMana(players[playerNow].maxMana/2);
			audio.PlayOneShot (sounds[4]);
			CastIndep(GUITextures[30],34,0);
			turns[playerNow]=0;
		}
		
		cardChoise = saveCardChoise[playerNow];
		pushCard = savePushCard[playerNow];
		
		allowAnimation=false;
		cardChoose=false;
		
		
		//REDUCE_COOLDOWN
		for(byte w = 0; w<players[playerNow].deck.Length; w++)
		{
			if(players[playerNow].cardCooldown[w]>0)
			players[playerNow].cardCooldown[w]-=1;
		}
		
		
		//GAIN_MANA
		players[playerNow].addMana(players[playerNow].gainMana);
	}
	public void StartFight()
	{
		saveCardChoise[playerNow] = cardChoise;
		savePushCard[playerNow] = pushCard;
		firstMove=true;
		timer=0;
		p=-2;
		allowFight=true;
		//DEM
		//if(!demoState)
			duelCount++;
	}
	public void SkipTurn()
	{
		skipper=0;
		duelDeckCard[duelCount]=999;
		if(onlineGame&&!canOnline)
		{
			multiplayerPlayer.bul=true;
			multiplayerPlayer.sendCard=999;
			multiplayerPlayer.sendPos=0;
			multiplayerPlayer.doStuff=true;
		}
		StartFight();
	}
	public void EndFight()
	{
		if(p==8)
		{
			SwitchPlayer();
			p=0;
			allowFight=false;
			allowAnimation=false;
		}
	}
	#endregion
	#region Push
	//ARROWS & BUTTONS
	public void LeftPush()
	{
		if(paused==false)
		{
			if(cardChoise>0)
			{
				cardChoise--;
			}
			else if(pushCard>0)
			{
				pushCard--;
			}
			else if(cardChoise==0 && pushCard<=0)
			{
				pushCard--;
			}
			if(pushCard<0)
			{
				pushCard=players[playerNow].deck.Length-1;
			}
		}
		
	}
	public void RightPush()
	{
		if(paused==false)
		{
			if(cardChoise>Mathf.Min(visibleCards-2,players[playerNow].deck.Length-2))
			{
				pushCard++;
			}
			else
			{
				cardChoise++;
			}
			if(pushCard==players[playerNow].deck.Length)
			{
				pushCard=0;
			}
		}
		
	}
	#endregion
	#region Attack
	//ATTACK POSITIONS
	public void AttackPos(int p, int cig, int pNow)
	{

			if(cardsInGame[cig].readyToAttack)
			{
				//TYPES:
				//0-INFANTRY
				//1-RANGED
				//2-MAGIC
				//3-HEALER
				//4-GROUP_HEALER
				
				//CURRENT_CARD
				if(cardsInGame[cig].id!=0)
				{
					
					//TYPE3 SPEC (ATTACK)
					FindSpec(3,cards[cardsInGame[cig].id].falseID,cig);
					
					//CARDS_0,1,2,3_AND_8,9,10,11
					if(cig<4||cig>7&&cig<12)
					{
						//CHECK_TYPE_OF_CURRENT_CARD
						//INFANTRY_OR_RANGED_ON_LINE_1
						if(cardsInGame[cig].type==0 || cardsInGame[cig].type==1)
						{
							//CHECK_IF_ENEMY_INFANTRY_EXIST
							if(cardsInGame[Mathf.Abs((cig+8)-pNow)].id!=0)
							{
								Attack(cig,Mathf.Abs((cig+8)-pNow));
							}
							//CHECK_IF_RANGED_ENEMY_EXIST
							else if(cardsInGame[Mathf.Abs((cig+12)-pNow)].id!=0)
							{
								Attack(cig,Mathf.Abs((cig+12)-pNow));
							}
							//ATTACK_ENEMY_PLAYER
							else
							{
								Attack(cig,33);
							}
						}
						//CHECK_TYPE_OF_CURRENT_CARD
						//MAGIC_ON_LINE_1
						else if(cardsInGame[cig].type==2)
						{
							Attack(cig,88);
						}
						//CHECK_TYPE_OF_CURRENT_CARD
						//HEALER_ON_LINE_1
						else if(cardsInGame[cig].type==3)
						{
							Attack(cig,cig);
						}
						//CHECK_TYPE_OF_CURRENT_CARD
						//GHEALER_ON_LINE_1
						else if(cardsInGame[cig].type==4)
						{
							Attack(cig,88);
						}
					}
					//CARDS_4,5,6,7_AND_12,13,14,15
					else if(cig<8&&cig>3||cig>11&&cig<16)
					{
						
						//CHECK_TYPE_OF_CURRENT_CARD
						//INFANTRY_ON_LINE_2
						if(cardsInGame[cig].type==0)
						{
							//CHECK_IF_LINE_1_IS_EMPTY
							if(cardsInGame[cig-4].id==0)
							{
								
								//CHECK_IF_ENEMY_INFANTRY_EXIST
								if(cardsInGame[Mathf.Abs((cig+4)-pNow)].id!=0)
								{
									Attack(cig,Mathf.Abs((cig+4)-pNow));
								}
								//CHECK_IF_RANGED_ENEMY_EXIST
								else if(cardsInGame[Mathf.Abs((cig+8)-pNow)].id!=0)
								{
									Attack(cig,Mathf.Abs((cig+8)-pNow));
								}
								//ATTACK_ENEMY_PLAYER
								else
								{
									Attack(cig,33);
								}
							}
							//SKIP_ATTACK
							else
							{
								SkipAttack();
							}
						}
						//CHECK_TYPE_OF_CURRENT_CARD
						//RANGED_ON_LINE_2
						else if(cardsInGame[cig].type==1)
						{
							//CHECK_IF_ENEMY_INFANTRY_EXIST
							if(cardsInGame[Mathf.Abs((cig+4)-pNow)].id!=0)
							{
								Attack(cig,Mathf.Abs((cig+4)-pNow));
							}
							//CHECK_IF_RANGED_ENEMY_EXIST
							else if(cardsInGame[Mathf.Abs((cig+8)-pNow)].id!=0)
							{
								Attack(cig,Mathf.Abs((cig+8)-pNow));
							}
							//ATTACK_ENEMY_PLAYER
							else
							{
								Attack(cig,33);
							}
						}
						//CHECK_TYPE_OF_CURRENT_CARD
						//MAGIC_ON_LINE_2
						else if(cardsInGame[cig].type==2 )
						{
							Attack(cig,88);
						}
						//CHECK_TYPE_OF_CURRENT_CARD
						//HEALER_ON_LINE_2
						else if(cardsInGame[cig].type==3)
						{
							if(cardsInGame[cig-4].id!=0)
								Attack(cig,cig-4);
							else
								Attack(cig,cig);
						}	
						//CHECK_TYPE_OF_CURRENT_CARD
						//GHEALER_ON_LINE_2
						else if(cardsInGame[cig].type==4 )
						{
							Attack(cig,88);
						}
					}
				}
				
				moved=true;
			}
			else
			{
				timer = 33;
				cardsInGame[cig].readyToAttack=true;
				moved=true;
			}
			
		
	}
	
	//FINDOUT ATTACKER TYPE AND PERFORM ACTION
	public void Attack(int attacker, int defender)
	{
		int pNow;
		
		if(attacker<=7)
			pNow=0;
		else 
			pNow=1;
		
		if(defender<16)
		{
			//CHECK_HEALING_CONDITIONS
			if(cardsInGame[attacker].type==3 || cardsInGame[attacker].type==4 && defender != 88)
			{
				
				//HEAL_ALLY 
				if(attacker!=defender)
				{
					//CANT HEAL UNDEAD
					if(cardsInGame[defender].hpType!=1)
					{
						//CHECK_ALLY_HP
						if(cardsInGame[defender].hp!=cardsInGame[defender].maxHp)
						{
							pHealed = true;
							//EFFECTS
							CastIndep(cardsInGame[attacker].attackEffect,defender,1);
							audio.PlayOneShot (cardsInGame[attacker].attackSound);
							TypeComparasion(attacker, defender);
						}
						else if(cardsInGame[attacker].type!=4)
						{
							Attack(attacker,attacker);
						}
					}
					else if(cardsInGame[attacker].type==3)
					{
						Attack(attacker,attacker);
					}
				}
				//HEAL_YOURSELF
				else
				{
					//CHECK_MY_HP
					if(cardsInGame[defender].hp!=cardsInGame[defender].maxHp)
					{
						pHealed = true;
						//EFFECTS
						CastIndep(cardsInGame[attacker].attackEffect,defender,0);
						audio.PlayOneShot (cardsInGame[attacker].attackSound);
						TypeComparasion(attacker, defender);
					}
					else if(cardsInGame[attacker].type==3)
					{
						SkipAttack();
					}
				}
			}
			//ATTACK
			else if(cardsInGame[attacker].type<3)
			{
				CastIndep(cardsInGame[attacker].attackEffect,defender,1);
				audio.PlayOneShot (cardsInGame[attacker].attackSound);
				TypeComparasion(attacker, defender);

				
				//CHECK_IF_DEAD
				if(cardsInGame[defender].hp<1)
				{
					//TYPE5 SPEC ON KILL
					FindSpec(5,cards[cardsInGame[attacker].id].falseID,attacker);
					Research(0,0);
				}
				//TYPE8 SPEC (AFTER HIT)
				else
					FindSpec(8,cards[cardsInGame[defender].id].falseID,defender);
			}
		}
		//ATTACK_PLAYER
		else if(defender==33)
		{
			
			if(attacker<=7)
				CastIndep(cardsInGame[attacker].attackEffect,32,1);
			else
				CastIndep(cardsInGame[attacker].attackEffect,33,1);
			
			
			audio.PlayOneShot (cardsInGame[attacker].attackSound);
			int attag=cardsInGame[attacker].attack;
			//POISON?
			if(cardsInGame[attacker].poisoned)
				attag=cardsInGame[attacker].attack-1;
			Research(2,attacker);
				attag-=casterArmorModifer;
			if(attag<1)
				attag=1;
			players[Mathf.Abs(pNow-1)].addHp(-attag);
			cardsInGame[attacker].poisoned=false;
			casterArmorModifer=0;
			
			Research(1,attacker);
		}
		
		//HEAL_PLAYER
		else if(defender==66)
		{
			
			if(attacker<=7)
				CastIndep(cardsInGame[attacker].attackEffect,33,0);
			else
				CastIndep(cardsInGame[attacker].attackEffect,32,0);
			
			pHealed = true;
			
			audio.PlayOneShot (cardsInGame[attacker].attackSound);
			
			int healz=cardsInGame[attacker].attack;
			//POISON?
			if(cardsInGame[attacker].poisoned)
				healz=cardsInGame[attacker].attack+1;
			if(healz>0)
				healz=-1;
			
			players[pNow].addHp(-healz);
		}
		
		//ATTACK_ALL
		else if(defender==88 && cardsInGame[attacker].type==2)
		{
			for(int q=0;q<8;q++)
			{	
				if(cardsInGame[q+Mathf.Abs(pNow-1)*8].id!=0)
				{
					Attack(attacker,q+Mathf.Abs(pNow-1)*8);
				}
				if(q==0)
				{
					Attack(attacker,33);
				}
			}
		}		
		
		//HEAL_ALL
		else if(defender==88 && cardsInGame[attacker].type==4)
		{
			pHealed = false;
			for(int f=0;f<8;f++)
			{	
				if(cardsInGame[f+pNow*8].id!=0)
				{
					Attack(attacker,f+pNow*8);
				}
				if(f==0 && players[pNow].hp!=players[pNow].maxHp)
				{
					Attack(attacker,66);
				}
			}
			if(pHealed==false)
			{
				SkipAttack();
			}
		}	
	}
	//CHECK_ATTACK_BONUSES&DECREASES
	public void TypeComparasion(int attacker, int defender)
	{
		
		
		//MAGIC
		if(cardsInGame[defender].hpType==2)
		{
			finalAttack(0,attacker, defender); //nobonus
		}
		//IF NOT MAGIC
		else 
		{
			//UNDEAD
			if(cardsInGame[defender].hpType==1)
			{
				//VS NORMAL
				if(cardsInGame[attacker].attackType==0)
				{
					finalAttack(-1,attacker, defender); //-1dmg
				}
				//VS HOLY
				else if(cardsInGame[attacker].attackType==4)
				{
					finalAttack(2,attacker, defender); //+2dmg
				}
				//VS OTHER
				else
				{
					finalAttack(0,attacker, defender); //nobonus
				}
				
			}
			//NATURE
			else if(cardsInGame[defender].hpType==3)
			{
				//VS POISON
				if(cardsInGame[attacker].attackType==3)
				{
					finalAttack(-1,attacker, defender); //-1dmg
				}
				//VS FIRE
				else if(cardsInGame[attacker].attackType==1)
				{
					finalAttack(1,attacker, defender); //+1dmg
				}
				//VS OTHER
				else
				{
					if(cardsInGame[attacker].attack>0)
						finalAttack(0,attacker, defender); //nobonus
					//HEAL NATURE
					else
						finalAttack(-1,attacker, defender); //+1hp bonus
				}
				
			}
			
			
			//NORMAL
			else if(cardsInGame[defender].hpType==0)
			{
				finalAttack(0,attacker, defender); //nobonus
			}
			
					
				
			
		}
		
		
	}	
	//FIND MODIFERS AND PERFORM AN ATTACK
	public void finalAttack(int modifer,int attacker, int defender)
	{
		//DMG UNITS
		if(cardsInGame[attacker].type<3)
		{
			//TYPE4 SPEC (RESISTANCE)
				FindSpec(4,cards[cardsInGame[defender].id].falseID,defender);
			if(cardsInGame[attacker].poisoned)
				modifer-=1;
				
				//ARMOR MODIFER - IGNORE IF FIRE ATTACK
				if(cardsInGame[attacker].attackType!=1)
					modifer-=armorModifer;
				
				//POISON MODIFER
				if(cardsInGame[attacker].attackType==3)
					modifer-=poisonModifer;
				
				//FROST MODIFER
				if(cardsInGame[attacker].attackType==3)
					modifer-=frostModifer;
				
			if(cardsInGame[attacker].attack+modifer>=1)
				cardsInGame[defender].addHp(-(cardsInGame[attacker].attack+modifer));
			else
				cardsInGame[defender].addHp(-1);
			
			//CLEAR
			armorModifer=0;
			poisonModifer=0;
			frostModifer=0;
		}
		//HEALERS all day all day domino dancing
		else
		{
			if(cardsInGame[attacker].poisoned&&cardsInGame[attacker].attack<0)
				modifer+=1;
			if(cardsInGame[attacker].attack+modifer<0)
				cardsInGame[defender].addHp(-(cardsInGame[attacker].attack+modifer));
			else
				cardsInGame[defender].addHp(-1);
		}
		
		//REMOVE POISON
		if(cardsInGame[attacker].poisoned)
			cardsInGame[attacker].poisoned=false;
		
		//ICE ABILITY = MAXHP-=ATTACK
		if(cardsInGame[attacker].attackType==2)
			if(cardsInGame[defender].hpType!=2)
				cardsInGame[defender].addMaxHp(-(cardsInGame[attacker].attack+modifer));

		//POISON ABILITY
		if(cardsInGame[attacker].attackType==3)
			if(cardsInGame[defender].hpType==0 || cardsInGame[defender].hpType==4)
				cardsInGame[defender].poisoned=true;
		
	}
	#endregion
	#region AIfunctions
	
	//HARD AI
	public void AI3()
	{
		AI2();
	}
	//NORMAL AI
	public void AI2()
	{
		array=new int[3]; //SUPER USEFUL CARDS
		founded=false;
		array[0]=13;//alchemist
		array[1]=20;//fairy
		array[2]=32;//rogue
		FindUsefulCard(array);
		if(!founded)
		{
			array=new int[7]; //USEFUL CARDS
			array[0]=18;//snake
			array[1]=22;//lich
			array[2]=25;//chieftain
			array[3]=29;//guard
			array[4]=34;//hexer
			array[5]=37;//witchdoctor
			array[6]=40;//inventor
			FindUsefulCard(array);
			if(!founded)
			{
				AI1(); //ACT LIKE EASY
				if(!founded)
				{
					SkipTurn();
				}
			}
		}
		
		if(cards[players[playerNow].deck[cardChoise]].type==0)
		{
			FreePos(0,4);
			if(freePos)
				posSlot=freePosReturn;
		}
		else if(cards[players[playerNow].deck[cardChoise]].type!=0)
		{
			FreePos(4,8);
			if(freePos)
				posSlot=freePosReturn;
		}
		
		//RESTRICTIONS:
		//ALLOW INFANTRY ON LINE 2?
		//NO
		if(cards[players[playerNow].deck[cardChoise]].type==0 && posSlot>=4)
		{
			SkipTurn();
		}
		else 
		{
			CastCard();
		}
		//ALLOW NON-INFANTRY ON LINE 1?
		//YES
	}
	//EASY AI
	public void AI1()
	{
		cardChoise=Random.Range(0,players[playerNow].deck.Length);
		if(cards[players[playerNow].deck[cardChoise]].manaCost>players[playerNow].mana)
		{
			//virtual_break
		}
		else if(players[playerNow].cardCooldown[cardChoise]!=0)
		{
			//virtual_break
		}
		else if(cards[players[playerNow].deck[cardChoise]].type==0)
		{
			FreePos(0,4);
			if(freePos)
				posSlot=freePosReturn;
			
			
		}
		else if(cards[players[playerNow].deck[cardChoise]].type!=0)
		{
			FreePos(4,8);
			if(freePos)
				posSlot=freePosReturn;
		}
		
		//RESTRICTIONS:
		//ALLOW INFANTRY ON LINE 2?
		//NO
		if(cards[players[playerNow].deck[cardChoise]].type==0 && posSlot>=4)
		{
			SkipTurn();
		}
		else 
		{
			CastCard();
		}
		//ALLOW NON-INFANTRY ON LINE 1?
		//YES
	}
	//DUMB AI
	public void AI0()
	{
		cardChoise=Random.Range(0,players[playerNow].deck.Length);
		posSlot=Random.Range(0,8);
		CastCard();
	}
	
	
	//FIND FREE POSITION WITHIN RANGE
	public void FreePos(int min,int max)
	{
		freePos=false;
		for(int x = min;x<max;x++)
		{
			if(cardsInGame[x+playerNow*8].id==0)
			{
				freePos=true;
				freePosReturn=Random.Range(min,max);
				break;
			}
		}
	}
	
	public void FindUsefulCard(int[] concreteCards)
	{
		for(int z = 0;z<concreteCards.Length;z++)
		{
			for(int x = 0;x<players[playerNow].deck.Length;x++)
			{
				if(cards[players[playerNow].deck[x]].falseID==concreteCards[z])
				{
					if(cards[players[playerNow].deck[x]].manaCost<=players[playerNow].mana)
					{
						if(players[playerNow].cardCooldown[x]<1)
						{
							if(cards[players[playerNow].deck[x]].type==0)
							{
								FreePos(0,4);
							}
							else if(cards[players[playerNow].deck[x]].type!=0)
							{
								FreePos(4,8);
							}
							if(freePos)
							{
								founded=true;
								cardChoise=x;
								break;
							}
						}
					}
				}
			}
		}
	}
	
	public void FindAnyCard()
	{
		for(int x = 0;x<players[playerNow].deck.Length;x++)
		{
			if(cards[players[playerNow].deck[x]].manaCost<=players[playerNow].mana)
			{
				if(players[playerNow].cardCooldown[x]<1)
				{
					founded=true;
					cardChoise=x;
					break;
				}
			}
		}
	}
	
	
	
	#endregion
	#region Specials
	public void DoSpecial(int q,int id,int cig)
	{
		if(specials[q].specSound)
			audio.PlayOneShot (specials[q].specSound);
		
		timer=0;

		byte casterTarget=0;
		byte teamTarget=0;
		//self buffs
		if(specials[q].target==0)
		{
			if(specials[q].type==2)
				cardsInGame[cig].addHp(specials[q].value);
			else if(specials[q].type==3)
				armorModifer+=specials[q].value;
			else if(specials[q].type==4)
				casterArmorModifer+=specials[q].value;
			else if(specials[q].type==5)
				cardsInGame[cig].attack+=specials[q].value;
			else if(specials[q].type==6)
				poisonModifer+=specials[q].value;
			else if(specials[q].type==7)
				frostModifer+=specials[q].value;
			else if(specials[q].type==9)
			{
				cardsInGame[cig].addMaxHp(specials[q].value);
				cardsInGame[cig].addHp(specials[q].value);
			}
			
			
		}
		//teams boosts
		else if(specials[q].target==1||specials[q].target==2)
		{
			
			//ally team
			if(specials[q].target==1)
			{
				if(cig<=7)
					teamTarget=0;
				else
					teamTarget=1;
			}
			//enemy team
			else if(specials[q].target==2)
			{
				if(cig<=7)
					teamTarget=1;
				else
					teamTarget=0;
			}
			
			if(specials[q].type==2)
			{
				for(int x=0;x<8;x++)
				{
					cardsInGame[teamTarget*8+x].addHp(specials[q].value);
					CastIndep(specials[q].icon,teamTarget*8+x,0);
				}
			}
			else if(specials[q].type==5)
			{
				for(int x=0;x<8;x++)
				{
					cardsInGame[teamTarget*8+x].addDmg(specials[q].value);
					CastIndep(specials[q].icon,teamTarget*8+x,0);
				}
			}
			else if(specials[q].type==9)
			{
				for(int x=0;x<8;x++)
				{
					cardsInGame[teamTarget*8+x].addMaxHp(specials[q].value);
					cardsInGame[teamTarget*8+x].addHp(specials[q].value);
					CastIndep(specials[q].icon,teamTarget*8+x,0);
				}
			}
		}
		//caster
		else if(specials[q].target==3)
		{
			if(cig<=7)
				casterTarget=0;
			else
				casterTarget=1;
			
			if(specials[q].type==0)
				players[casterTarget].addMana(specials[q].value);
			if(specials[q].type==2)
				players[casterTarget].addHp(specials[q].value);
			
			CastIndep(specials[q].icon,33-casterTarget,0);
		}
		//enemy caster
		else if(specials[q].target==4)
		{
			//znajdz dziada
			if(cig<=7)
				casterTarget=1;
			else
				casterTarget=0;
			
			if(specials[q].type==0)
				players[casterTarget].addMana(specials[q].value);
			if(specials[q].type==2)
				players[casterTarget].addHp(specials[q].value);
			
			CastIndep(specials[q].icon,33-casterTarget,0);
		}
		//static target 
		else if(specials[q].target==999)
		{
			if(specials[q].type==8)
				//counterattack
				{
					int s;
					int spos;
					if(cig<=7)
					{
						s=0;
						spos=cig-8;
					}
					else
					{
						s=16;
						spos=cig;
					}
					
					AttackPos(spos, cig, s);
				}
			else if(specials[q].type==10)
				//pierce
				{
					
				}
			else if(specials[q].type==11)
				//slashing
				{
					
				}
		}
		
	}
	public void CastIndep(Texture2D icon, int cig, byte gowno)
	{
		int sloPosX=0;
		int sloPosY=0;
			if(cig<17)
			{
				int moreYs=0;
				int x1s=0;
				int y1s=0;
				
				for(int a=0;a<cig;a++)
				{
						x1s++;
					
					//LOOP_HELP
					if(x1s==4)
					{
						x1s=0;
						y1s++;
						if(y1s==2)
						{
							moreYs=-300;
						}
					}
				}
				
				int reverts=0;;
				if(cig>11)
					reverts=1;
				
				sloPosX = (int)(Screen.width/2-180*realHeight+x1s*90*realHeight+45*realHeight);
				sloPosY = (int)(15*realHeight+Screen.height/2+y1s*90*realHeight+45*realHeight+moreYs*realHeight+20*realHeight*+(9*(reverts*(-1))));
			}
			else if(cig==32)
			{
				sloPosX = (int)(Screen.height/8);
				sloPosY = sloPosX;
				
			}
			else if(cig==33)
			{
				sloPosX = (int)(Screen.width-Screen.height/8);
				sloPosY = (int)(Screen.height-Screen.height/8);
			}
			else if(cig==34)
			{
				sloPosX = (int)(Screen.width-87*realHeight);
				sloPosY = (int)(272*realHeight);
			}
			
			
		
		for(int v=0;v<indepIcons.Length;v++)
		{
			if(indepIcons[v].icon==null)
			{
				if(gowno==1)
					indepIcons[v].disort=true;
				else
					indepIcons[v].disort=false;
				
				if(cig==32||cig==33)
					indepIcons[v].scaler=2;
				else if(cig==34)
					indepIcons[v].scaler=3;
				else
					indepIcons[v].scaler=1;
				
				
				indepIcons[v].posX=sloPosX;
				indepIcons[v].posY=sloPosY;
				indepIcons[v].icon=icon;
				break;
			}
		}
	}
	public void Research(int type,int pNow)
	{
		//0-anyone death
		//1-revenge (after hit)
		//2-armor caster (before hit)
		if(pNow<=7)
			pNow=0;
		else 
			pNow=1;
		
		if(type==0)
		{
			for(int x=0;x<16;x++)
			{
				FindSpec(7,cards[cardsInGame[x].id].falseID,x);
			}
		}
		else if(type==1)
		{
			for(int y=0;y<8;y++)
			{
				FindSpec(9,cards[cardsInGame[y+Mathf.Abs((pNow-1))*8].id].falseID,y+Mathf.Abs((pNow-1))*8);
			}
		}
		else if(type==2)
		{
			for(int z=0;z<8;z++)
			{
				FindSpec(6,cards[cardsInGame[z+Mathf.Abs((pNow-1))*8].id].falseID,z+Mathf.Abs((pNow-1))*8);
			}
		}
	}
	public void FindSpec(int type,int card,int cig)
	{
		for(int x=0;x<cards[card].specials.Length;x++)
		{
			if(cards[card].specials[x]==type)
			{
				DoSpecial(cards[card].specialID[x],card,cig);
			}
		}
	}	

	
	#endregion
	#region RandomFill/Looper
	void RandomFill()
	{
		//FILL_999_ID_CARD_RANDOMLY
		//2 players
		for(byte q = 0; q<2; q++)
		{
			//player deck length
			byte w=0;
			for(w = 0; w<players[q].deck.Length; w++)
			{
				//if card==999 go random
				if(players[q].deck[w]==999)
				{
					players[q].deck[w]=Random.Range(1,cards.Length);
					//check if such a card already exists
					for(byte e = 0; e<players[q].deck.Length; e++)
					{
						if(w!=e)
							if(players[q].deck[w] == players[q].deck[e])
							{
								players[q].deck[w]=999;
								w--;
								break;
							}
					}
				}
			}
		}
	}
	
	void Looper()
	{	
		if(gameObject.activeSelf==true)
		{
			StartCoroutine(Example());
		}
	}

    IEnumerator Example()
    {
			yield return new WaitForSeconds(Random.Range(5,22));
			audio.PlayOneShot(thunders[Random.Range(0,thunders.Length)]);
			Looper();
	}
	#endregion
	#region Multiplayersvoids
	[Command]
	public void CmdCanOnline(bool f)
	{
		canOnline=f;
	}
	
	[ClientRpc]
	public void RpcUpStats(int xchar, string customName, int[] importedDeck)
	{
		if(!isLocalPlayer)
			return;
		
		int play=0;
		for(int k=0;k<2;k++)
		{
			if(!players[k].cpuControl)
			{
				play=k;
				break;
			}
		}
		
	
		players[Mathf.Abs(play-1)].playerName=customName;
		players[Mathf.Abs(play-1)].portrait=gameMenuScript.prefChars[xchar].portrait;
		players[Mathf.Abs(play-1)].maxHp=gameMenuScript.prefChars[xchar].hp;
		players[Mathf.Abs(play-1)].hp=gameMenuScript.prefChars[xchar].hp;
		players[Mathf.Abs(play-1)].maxMana=gameMenuScript.prefChars[xchar].mana;
		players[Mathf.Abs(play-1)].mana=gameMenuScript.prefChars[xchar].mana;
		players[Mathf.Abs(play-1)].gainMana=gameMenuScript.prefChars[xchar].gainMana;
		players[Mathf.Abs(play-1)].deck=new int[importedDeck.Length];
		players[Mathf.Abs(play-1)].cardCooldown=new byte[importedDeck.Length];
		players[Mathf.Abs(play-1)].cpuControl=true;
		for(int z=0;z<importedDeck.Length;z++)
		{
			players[Mathf.Abs(play-1)].deck[z]=importedDeck[z];
		}
	}

	#endregion
	#endregion
}