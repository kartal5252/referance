using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using UnityEngine;

namespace HurnCheat
{
	// Token: 0x02000002 RID: 2
	public class Main : MonoBehaviour
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private void Start()
		{
			this.mRes = new ResourceManager("HurnCheat.Lang.L", typeof(Main).Assembly);
			this.StringsLang();
			this.BioPos = Utils.Vec3Set(-3569.93f, 186.24f, -2844.98f);
			this.BioPos1 = Utils.Vec3Set(-2090.874f, 187.77f, 1142.586f);
			this.BioPos2 = Utils.Vec3Set(-804.98f, 209.38f, -2456.39f);
			this.BioPos3 = Utils.Vec3Set(1165.82f, 191.78f, -2709.02f);
			this.MenuPos = new Rect(20f, 32f, 225f, 180f);
			this.sMenuPos = new Rect(this.MenuPos.xMin + this.MenuPos.width + 5f, this.MenuPos.yMin, 200f, 250f);
			this.SvMenu = new Rect((float)(Screen.width - 150), 80f, 140f, 280f);
			this.SpamText = "HurnCheat updated 3.4.2! Download";
			this.sendsmax = 16;
			this.pConsole.AppendLine("null");
			if (this.dBug[0] == 0)
			{
				this.pConsole.AppendLine("-H Test [1] OK");
				this.dBug[0] = 1;
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000021B4 File Offset: 0x000003B4
		private void StringsLang()
		{
			this.sMenu[0] = "ESP";
			this.sMenu[1] = this.Lang("menu_text");
			this.sMenu[2] = "SPAMCHAT";
			this.BiosName[0] = "SPAWN";
			this.BiosName[1] = this.Lang("b_1");
			this.BiosName[2] = this.Lang("b_2");
			this.BiosName[3] = this.Lang("b_3");
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002234 File Offset: 0x00000434
		private string Lang(string var)
		{
			string result = "erro";
			if (this.mRes.GetString(var, this.L) != null)
			{
				result = this.mRes.GetString(var, this.L);
			}
			return result;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002270 File Offset: 0x00000470
		private void Update()
		{
			this.StringsLang();
			if (this.iLang == 0)
			{
				this.L = CultureInfo.CreateSpecificCulture("pt");
			}
			else if (this.iLang == 1)
			{
				this.L = CultureInfo.CreateSpecificCulture("en");
			}
			if (Input.GetKeyDown(127))
			{
				this.HK1 = !this.HK1;
			}
			if (Input.GetKeyDown(286))
			{
				this.HK2 = !this.HK2;
			}
			if (Input.GetKeyDown(287))
			{
				this.HK3 = !this.HK3;
			}
			if (Input.GetKeyDown(288))
			{
				this.HK4 = !this.HK4;
			}
			if (Input.GetKeyDown(289))
			{
				this.HK5 = !this.HK5;
			}
			if (Input.GetKeyDown(290))
			{
				this.HK6 = !this.HK6;
			}
			if (Input.GetKeyDown(277))
			{
				this.House = this.pLocalPlayer.transform.position;
				this.MyHouse(1);
			}
			if (this.HK11)
			{
				if (Input.GetKeyDown(275))
				{
					foreach (StructureManager structureManager in Resources.FindObjectsOfTypeAll(typeof(StructureManager)))
					{
						if (structureManager != null)
						{
							structureManager.gameObject.SetActive(false);
						}
					}
				}
				if (Input.GetKeyUp(275))
				{
					foreach (StructureManager structureManager2 in Resources.FindObjectsOfTypeAll(typeof(StructureManager)))
					{
						if (structureManager2 != null)
						{
							structureManager2.gameObject.SetActive(true);
						}
					}
				}
			}
			if (this.HK10)
			{
				this.pLight.Cycle.Hour = 12f;
			}
			if (this.dBug[1] == 0)
			{
				this.pConsole.AppendLine("-H Test [2] OK");
				this.dBug[1] = 1;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002464 File Offset: 0x00000664
		private void OnGUI()
		{
			if (this.IMsg && this.pLocalPlayer != null)
			{
				this.pChat.AppendLine("<color=#E00000>HurnCheat online!</color>");
				this.IMsg = false;
			}
			if (!this.IMsg && this.pLocalPlayer == null)
			{
				this.IMsg = true;
			}
			if (this.HK10 && this.pLocalPlayer != null)
			{
				GUI.Box(new Rect((float)(Screen.width - 170), 75f, 160f, 30f), " ");
				Utils.DrawLabelS(new Rect((float)(Screen.width - 160), 80f, 200f, 50f), this.Lang("set_dm"), Color.yellow, 15);
			}
			if (this.pLocalPlayer == null)
			{
				this.SvMenu = GUI.Window(99, this.SvMenu, new GUI.WindowFunction(this.Menu), this.Lang("sv_l"));
			}
			if (this.HK2 && this.pLocalPlayer != null)
			{
				Vector3 position = this.pLocalPlayer.transform.position;
				Utils.DrawLabelS(new Rect(20f, 380f, 450f, 450f), string.Format("Pos: [X {0} Y {1} Z {2}]", position.x, position.y, position.z), this.MainColor, 14);
			}
			if (this.HK3 && this.pLocalPlayer != null)
			{
				GUIStyle guistyle = new GUIStyle(GUI.skin.GetStyle("label"));
				guistyle.fontSize = 21;
				float num = Vector3.Distance(this.pLocalPlayer.transform.position, this.House);
				int num2 = (int)num;
				Utils.DrawLabelS(new Rect(20f, 400f, 450f, 450f), string.Format(string.Concat(new object[]
				{
					this.Lang("c_p"),
					": [X {0} Y {1} Z {2}] ",
					this.Lang("d"),
					": ",
					num2,
					"m"
				}), this.House.x, this.House.y, this.House.z), this.MainColor, 14);
				Utils.DrawLabel(this.House, this.Lang("c"), Color.magenta, 21);
			}
			if (this.LoadHouse)
			{
				this.LoadHouse = false;
				this.MyHouse(0);
			}
			if (this.HK8 && this.pLocalPlayer != null)
			{
				this.ShowBIO();
			}
			if (this.HK1)
			{
				GUIStyle guistyle2 = new GUIStyle(GUI.skin.GetStyle("window"));
				guistyle2.margin.top = 20;
				string @string = Encoding.ASCII.GetString(Utils.cData);
				Utils.DrawLabelS(new Rect(this.MenuPos.xMin + 10f, this.MenuPos.yMin - 29f, this.MenuPos.width, 34f), @string, Color.green, 23);
				this.pPlayert = true;
				GUI.Box(new Rect(this.MenuPos.xMin + this.MenuPos.width / 3f - 5f, this.MenuPos.y + this.MenuPos.height, 90f, 40f), "");
				this.iLang = GUI.SelectionGrid(new Rect(this.MenuPos.xMin + this.MenuPos.width / 3f, this.MenuPos.y + this.MenuPos.height + 5f, 80f, 30f), this.iLang, this.sLang, 2);
				this.MenuPos = GUI.Window(1, this.MenuPos, new GUI.WindowFunction(this.Menu), " ", guistyle2);
				if (this.iMenu == 0)
				{
					this.sMenuPos = GUI.Window(101, this.sMenuPos, new GUI.WindowFunction(this.Menu), "ESP");
				}
				if (this.iMenu == 1)
				{
					this.sMenuPos = GUI.Window(102, this.sMenuPos, new GUI.WindowFunction(this.Menu), this.Lang("menu_text"));
				}
				if (this.iMenu == 2)
				{
					this.sMenuPos = GUI.Window(103, this.sMenuPos, new GUI.WindowFunction(this.Menu), "SPAMCHAT");
				}
			}
			if (this.HK9 && this.pLocalPlayer != null)
			{
				this.SpamChat();
			}
			if (!this.HK9 && this.d > 0)
			{
				this.d = 0;
			}
			if (this.dBug[2] == 0)
			{
				this.pConsole.AppendLine("-H Test [3] OK");
				this.dBug[2] = 1;
			}
			this.ESP();
			if (this.dBug[3] == 0)
			{
				this.pConsole.AppendLine("HurnCheat 3.4.2 r1343 Loaded!");
				this.dBug[3] = 1;
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000029AC File Offset: 0x00000BAC
		private void Menu(int windowID)
		{
			GUI.color = this.MainColor;
			if (windowID == 1)
			{
				this.iMenu = GUI.SelectionGrid(new Rect(20f, 20f, 180f, 150f), this.iMenu, this.sMenu, 1);
			}
			GUIStyle guistyle = new GUIStyle(GUI.skin.GetStyle("toggle"));
			guistyle.fontSize = 15;
			if (windowID == 101)
			{
				this.HK4 = GUI.Toggle(new Rect(10f, 20f, 160f, 20f), this.HK4, string.Format(this.Lang("e_m1") + " [{0}m]", Utils.IntBar(this.esplmt)), guistyle);
				this.esplmt = GUI.HorizontalSlider(new Rect(20f, 45f, 160f, 20f), this.esplmt, 1f, 800f);
				this.HK5 = GUI.Toggle(new Rect(10f, 60f, 160f, 20f), this.HK5, string.Format(this.Lang("e_m2") + " [{0}m]", Utils.IntBar(this.esplmt2)), guistyle);
				this.esplmt2 = GUI.HorizontalSlider(new Rect(20f, 85f, 160f, 20f), this.esplmt2, 1f, 800f);
				this.HK7 = GUI.Toggle(new Rect(10f, 100f, 160f, 20f), this.HK7, string.Format(this.Lang("e_m3") + " [{0}m]", Utils.IntBar(this.esplmt4)), guistyle);
				this.esplmt4 = GUI.HorizontalSlider(new Rect(20f, 125f, 160f, 20f), this.esplmt4, 1f, 800f);
				this.HK6 = GUI.Toggle(new Rect(10f, 140f, 160f, 20f), this.HK6, string.Format(this.Lang("e_m4") + " [{0}m]", Utils.IntBar(this.esplmt3)), guistyle);
				this.esplmt3 = GUI.HorizontalSlider(new Rect(20f, 165f, 160f, 20f), this.esplmt3, 1f, 800f);
				this.HK12 = GUI.Toggle(new Rect(10f, 180f, 260f, 20f), this.HK12, string.Format(this.Lang("e_m5") + " [{0}m]", Utils.IntBar(this.esplmt5)), guistyle);
				this.esplmt5 = GUI.HorizontalSlider(new Rect(20f, 205f, 160f, 20f), this.esplmt5, 1f, 800f);
				this.HK3 = GUI.Toggle(new Rect(10f, 220f, 160f, 20f), this.HK3, this.Lang("e_m6") + " [Insert]", guistyle);
				if (GUI.Button(new Rect(2f, 2f, 30f, 15f), "X "))
				{
					this.iMenu = 99;
				}
			}
			if (windowID == 102)
			{
				this.HK2 = GUI.Toggle(new Rect(10f, 20f, 160f, 20f), this.HK2, this.Lang("o_m1"), guistyle);
				this.HK8 = GUI.Toggle(new Rect(10f, 40f, 160f, 20f), this.HK8, this.Lang("o_m2"), guistyle);
				this.HK10 = GUI.Toggle(new Rect(10f, 60f, 160f, 20f), this.HK10, this.Lang("o_m3"), guistyle);
				this.HK11 = GUI.Toggle(new Rect(10f, 80f, 260f, 20f), this.HK11, this.Lang("o_m4"), guistyle);
				Utils.DrawLabelS(new Rect(10f, 100f, 260f, 30f), "[" + this.Lang("m4_k") + "] ↑↑", this.MainColor, 15);
				this.sp = GUI.Toggle(new Rect(10f, 190f, 260f, 20f), this.sp, string.Format("Loot Point [{0}m]", Utils.IntBar(this.esplmt6)), guistyle);
				this.esplmt6 = GUI.HorizontalSlider(new Rect(20f, 215f, 160f, 20f), this.esplmt6, 1f, 800f);
				if (GUI.Button(new Rect(2f, 2f, 30f, 15f), "X "))
				{
					this.iMenu = 99;
				}
			}
			if (windowID == 103)
			{
				GUI.Label(new Rect(70f, 20f, 100f, 20f), this.Lang("s_t1"));
				this.SpamText = GUI.TextField(new Rect(20f, 40f, 160f, 20f), this.SpamText);
				GUI.Label(new Rect(90f, 60f, 100f, 20f), this.Lang("s_t2"));
				this.Ale = GUI.Toggle(new Rect(20f, 80f, 160f, 20f), this.Ale, this.Lang("s_t3"), guistyle);
				GUI.Label(new Rect(20f, 100f, 100f, 20f), Utils.IntBar(this.chatdelay));
				this.chatdelay = GUI.HorizontalSlider(new Rect(20f, 115f, 160f, 20f), this.chatdelay, 1f, 500f);
				GUI.Label(new Rect(20f, 140f, 100f, 20f), this.d.ToString() + "/" + this.sendsmax.ToString());
				if (GUI.Button(new Rect(20f, 160f, 160f, 40f), string.Format("{0}", this.HK9 ? this.Lang("s_t4") : this.Lang("s_t5"))))
				{
					this.HK9 = !this.HK9;
				}
				if (GUI.Button(new Rect(2f, 2f, 30f, 15f), "X "))
				{
					this.iMenu = 99;
				}
			}
			if (!this.pPlayert && this.HK1)
			{
				Loader.UnLoad();
			}
			if (windowID == 99)
			{
				GUI.color = Color.white;
				this.svID = GUI.SelectionGrid(new Rect(10f, 20f, 120f, 250f), this.svID, this.Servers, 1);
				GUI.color = this.MainColor;
				if (this.svID != 99)
				{
					this.pConsole.ExecuteCommand("connect " + this.ServersIP[this.svID]);
					this.svID = 99;
				}
			}
			GUI.DragWindow(new Rect(0f, 0f, (float)Screen.height, (float)Screen.width));
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00003170 File Offset: 0x00001370
		private void SpamChat()
		{
			int num = (int)this.chatdelay;
			if (this.d < num)
			{
				this.d++;
				return;
			}
			if (this.send < this.sendsmax)
			{
				if (this.Ale)
				{
					this.SpamText = Utils.RandomString(10);
				}
				this.pChat.SendChatClient(this.SpamText);
				this.send++;
				this.d = 0;
				return;
			}
			this.d = 0;
			this.send = 0;
			this.HK9 = false;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000031FC File Offset: 0x000013FC
		private void Awake()
		{
			base.StartCoroutine(this.cacheProxies());
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00003440 File Offset: 0x00001640
		private IEnumerator cacheProxies()
		{
			for (;;)
			{
				if (this.pLight == null)
				{
					this.pLight = (TOD_Sky)Object.FindObjectOfType(typeof(TOD_Sky));
				}
				if (this.pLocalPlayer == null)
				{
					this.pLocalPlayer = Object.FindObjectOfType<NetworkEntityManagerPlayerOwner>();
				}
				this.pPlayers = (Object.FindObjectsOfType(typeof(NetworkEntityManagerPlayerProxy)) as NetworkEntityManagerPlayerProxy[]);
				if (this.HK6)
				{
					this.ItensProxy = (Object.FindObjectsOfType(typeof(NetworkEntityManagerWorldItemProxy)) as NetworkEntityManagerWorldItemProxy[]);
				}
				if (this.HK5)
				{
					this.Creatures = (Object.FindObjectsOfType(typeof(MeleeCreatureAttackBehavior)) as MeleeCreatureAttackBehavior[]);
				}
				if (this.HK12)
				{
					this.Grow = (Object.FindObjectsOfType(typeof(GrowingPlantClient)) as GrowingPlantClient[]);
				}
				if (this.sp)
				{
					this.Sp = (Object.FindObjectsOfType(typeof(LootSpawnPoint)) as LootSpawnPoint[]);
				}
				if (this.HK7)
				{
					this.Rocks = (Resources.FindObjectsOfTypeAll(typeof(RockResourceNode)) as RockResourceNode[]);
					this.Minig = (Object.FindObjectsOfType(typeof(ExplodableMiningRock)) as ExplodableMiningRock[]);
				}
				if (this.pChat == null)
				{
					this.pChat = Object.FindObjectOfType<ChatManagerClient>();
				}
				if (this.pConsole == null)
				{
					this.pConsole = Object.FindObjectOfType<ConsoleManager>();
				}
				yield return new WaitForSeconds(1f);
			}
			yield break;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000345C File Offset: 0x0000165C
		private void MyHouse(int Type)
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			if (Type == 1)
			{
				string[] array = new string[]
				{
					this.House.x.ToString(),
					this.House.y.ToString(),
					this.House.z.ToString()
				};
				using (StreamWriter streamWriter = new StreamWriter(folderPath + "\\HousePos.cfg"))
				{
					foreach (string value in array)
					{
						streamWriter.WriteLine(value);
					}
					return;
				}
			}
			if (Type == 0 && File.Exists(folderPath + "\\HousePos.cfg"))
			{
				using (StreamReader streamReader = new StreamReader(folderPath + "\\HousePos.cfg"))
				{
					this.House.x = float.Parse(streamReader.ReadLine());
					this.House.y = float.Parse(streamReader.ReadLine());
					this.House.z = float.Parse(streamReader.ReadLine());
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00003598 File Offset: 0x00001798
		private void ShowBIO()
		{
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < 4; i++)
			{
				if (i == 0)
				{
					vector = this.BioPos;
				}
				if (i == 1)
				{
					vector = this.BioPos1;
				}
				if (i == 2)
				{
					vector = this.BioPos2;
				}
				if (i == 3)
				{
					vector = this.BioPos3;
				}
				float num = Vector3.Distance(this.pLocalPlayer.transform.position, vector);
				int num2 = (int)num;
				Utils.DrawLabel(vector, this.BiosName[i] + "\n[" + num2.ToString() + "]", Color.white, 15);
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00003628 File Offset: 0x00001828
		private void ESP()
		{
			if (this.pLocalPlayer == null)
			{
				return;
			}
			if (this.HK4)
			{
				foreach (NetworkEntityManagerPlayerProxy networkEntityManagerPlayerProxy in this.pPlayers)
				{
					DisplayProxyName component = networkEntityManagerPlayerProxy.GetComponent<DisplayProxyName>();
					string name = component.Name;
					float num = Vector3.Distance(this.pLocalPlayer.transform.position, networkEntityManagerPlayerProxy.transform.position);
					Vector3 vector;
					vector..ctor(Camera.main.WorldToScreenPoint(networkEntityManagerPlayerProxy.transform.position).x, Camera.main.WorldToScreenPoint(networkEntityManagerPlayerProxy.transform.position).y, Camera.main.WorldToScreenPoint(networkEntityManagerPlayerProxy.transform.position).z);
					if (num <= this.esplmt)
					{
						int num2 = (int)num;
						int num3 = Utils.DistanceToCross(networkEntityManagerPlayerProxy.transform.position);
						Utils.DrawLabel(networkEntityManagerPlayerProxy.transform.position, string.Concat(new object[]
						{
							"[",
							num2,
							"m]\n ",
							name
						}), Color.green, 15);
						EquippedHandlerProxy component2 = networkEntityManagerPlayerProxy.GetComponent<EquippedHandlerProxy>();
						ItemInstance equippedItem = component2.GetEquippedItem();
						string text = "";
						if (equippedItem != null)
						{
							text = equippedItem.Item.GetIconName();
						}
						if (vector.z > 0.01f)
						{
							GUI.Label(new Rect(vector.x + 20f, (float)Screen.height - vector.y, 400f, 200f), text);
						}
						if (Input.GetKey(325) && num3 < 50)
						{
							this.tPlayer = networkEntityManagerPlayerProxy;
							this.tName = name;
							this.Target = "Player";
						}
					}
				}
			}
			if (this.HK6)
			{
				if (this.ItensProxy == null)
				{
					return;
				}
				foreach (NetworkEntityManagerWorldItemProxy networkEntityManagerWorldItemProxy in this.ItensProxy)
				{
					string name2 = networkEntityManagerWorldItemProxy.gameObject.name;
					float num4 = Vector3.Distance(this.pLocalPlayer.transform.position, networkEntityManagerWorldItemProxy.transform.position);
					if (num4 <= this.esplmt3)
					{
						int num5 = (int)num4;
						string text2;
						if (name2.Contains("WorldItem"))
						{
							text2 = this.Lang("wi");
						}
						else
						{
							text2 = name2;
						}
						Utils.DrawLabel(networkEntityManagerWorldItemProxy.transform.position, string.Concat(new object[]
						{
							"[",
							num5,
							"m]\n",
							text2
						}), Color.yellow, 15);
					}
				}
			}
			if (this.HK12)
			{
				if (this.Grow == null)
				{
					return;
				}
				foreach (GrowingPlantClient growingPlantClient in this.Grow)
				{
					float num6 = Vector3.Distance(this.pLocalPlayer.transform.position, growingPlantClient.transform.position);
					if (num6 <= this.esplmt5)
					{
						int num7 = (int)num6;
						string text3 = "erro";
						if (growingPlantClient.gameObject.name.Contains("Owrong"))
						{
							text3 = this.Lang("ln");
						}
						else if (growingPlantClient.gameObject.name.Contains("Pitcher"))
						{
							text3 = this.Lang("ja");
						}
						Utils.DrawLabel(growingPlantClient.transform.position, string.Concat(new object[]
						{
							text3,
							"\n[",
							num7,
							"m]"
						}), Color.yellow, 15);
					}
				}
			}
			if (this.HK7)
			{
				if (this.Rocks == null)
				{
					return;
				}
				foreach (RockResourceNode rockResourceNode in this.Rocks)
				{
					float num8 = Vector3.Distance(this.pLocalPlayer.transform.position, rockResourceNode.transform.position);
					if (num8 <= this.esplmt4)
					{
						int num9 = (int)num8;
						Color c = Color.white;
						string text4 = rockResourceNode.gameObject.name;
						if (text4.Contains("Iron"))
						{
							text4 = this.Lang("r_m");
							c = Color.gray;
						}
						if (text4.Contains("Sandstone"))
						{
							text4 = this.Lang("r_a");
							c = Color.Lerp(Color.yellow, Color.black, 0.5f);
						}
						if (text4.Contains("Metal2"))
						{
							text4 = "Tritanium";
							c = Color.red;
						}
						if (text4.Contains("Metal3"))
						{
							text4 = "Mondinium";
							c = Color.green;
						}
						if (text4.Contains("Metal4"))
						{
							text4 = "Ultranium";
							c = Color.blue;
						}
						if (text4.Contains("Coal"))
						{
							text4 = this.Lang("r_c");
							c = Color.Lerp(Color.red, Color.black, 0.5f);
						}
						if (text4.Contains("Flint"))
						{
							text4 = this.Lang("r_p");
							c = Color.Lerp(Color.green, Color.blue, 0.3f);
						}
						Utils.DrawLabel(rockResourceNode.transform.position, string.Concat(new object[]
						{
							"[",
							num9,
							"m]\n",
							text4
						}), c, 15);
					}
				}
			}
			if (this.HK7)
			{
				if (this.Minig == null)
				{
					return;
				}
				foreach (ExplodableMiningRock explodableMiningRock in this.Minig)
				{
					float num10 = Vector3.Distance(this.pLocalPlayer.transform.position, explodableMiningRock.transform.position);
					if (num10 <= this.esplmt4)
					{
						int num11 = (int)num10;
						Color c2 = Color.Lerp(Color.gray, Color.green, 0.5f);
						Utils.DrawLabel(explodableMiningRock.transform.position, string.Concat(new object[]
						{
							"[",
							num11,
							"m]\n",
							this.Lang("ex_r")
						}), c2, 15);
					}
				}
			}
			if (this.sp)
			{
				if (this.Sp == null)
				{
					return;
				}
				foreach (LootSpawnPoint lootSpawnPoint in this.Sp)
				{
					float num12 = Vector3.Distance(this.pLocalPlayer.transform.position, lootSpawnPoint.transform.position);
					if (num12 <= this.esplmt6)
					{
						int num13 = (int)num12;
						Color c3 = Color.Lerp(Color.gray, Color.green, 0.5f);
						Utils.DrawLabel(lootSpawnPoint.transform.position, "X\n" + num13 + "m", c3, 15);
					}
				}
			}
			if (this.HK5)
			{
				if (this.Creatures == null)
				{
					return;
				}
				foreach (MeleeCreatureAttackBehavior meleeCreatureAttackBehavior in this.Creatures)
				{
					GUIStyle guistyle = new GUIStyle(GUI.skin.GetStyle("label"));
					guistyle.fontSize = 15;
					float num15 = Vector3.Distance(this.pLocalPlayer.transform.position, meleeCreatureAttackBehavior.transform.position);
					if (num15 <= this.esplmt2)
					{
						int num16 = (int)num15;
						string text5 = meleeCreatureAttackBehavior.name;
						if (text5.Contains("Bor"))
						{
							text5 = this.Lang("a_j");
						}
						if (text5.Contains("ShigiForest"))
						{
							text5 = this.Lang("a_v");
						}
						if (text5.Contains("Yeti"))
						{
							text5 = "Yeti";
						}
						Utils.DrawLabel(meleeCreatureAttackBehavior.transform.position, string.Concat(new object[]
						{
							text5,
							"\n[",
							num16,
							"]"
						}), Color.red, 15);
						int num17 = Utils.DistanceToCross(meleeCreatureAttackBehavior.transform.position);
						if (Input.GetKey(325) && num17 < 50)
						{
							this.tCreature = meleeCreatureAttackBehavior;
							this.tName = text5 + " [" + meleeCreatureAttackBehavior.GetInstanceID().ToString() + "]";
							this.Target = "Creature";
						}
					}
				}
			}
		}

		// Token: 0x04000001 RID: 1
		public ResourceManager mRes;

		// Token: 0x04000002 RID: 2
		public CultureInfo L;

		// Token: 0x04000003 RID: 3
		private bool LoadHouse = true;

		// Token: 0x04000004 RID: 4
		private bool IMsg = true;

		// Token: 0x04000005 RID: 5
		private Vector3 House = Vector3.zero;

		// Token: 0x04000006 RID: 6
		private NetworkEntityManagerPlayerOwner pLocalPlayer;

		// Token: 0x04000007 RID: 7
		private NetworkEntityManagerPlayerProxy[] pPlayers;

		// Token: 0x04000008 RID: 8
		private MeleeCreatureAttackBehavior[] Creatures;

		// Token: 0x04000009 RID: 9
		private RockResourceNode[] Rocks;

		// Token: 0x0400000A RID: 10
		private NetworkEntityManagerWorldItemProxy[] ItensProxy;

		// Token: 0x0400000B RID: 11
		private MeleeCreatureAttackBehavior tCreature;

		// Token: 0x0400000C RID: 12
		private ExplodableMiningRock[] Minig;

		// Token: 0x0400000D RID: 13
		private NetworkEntityManagerPlayerProxy tPlayer;

		// Token: 0x0400000E RID: 14
		private GrowingPlantClient[] Grow;

		// Token: 0x0400000F RID: 15
		private LootSpawnPoint[] Sp;

		// Token: 0x04000010 RID: 16
		private string Target = "NULL";

		// Token: 0x04000011 RID: 17
		private string tName = "NULL";

		// Token: 0x04000012 RID: 18
		private string SpamText = "NULL";

		// Token: 0x04000013 RID: 19
		private string Alvo = "NULL";

		// Token: 0x04000014 RID: 20
		private bool HK1 = true;

		// Token: 0x04000015 RID: 21
		private bool HK2;

		// Token: 0x04000016 RID: 22
		private bool HK3;

		// Token: 0x04000017 RID: 23
		private bool HK4;

		// Token: 0x04000018 RID: 24
		private bool HK5;

		// Token: 0x04000019 RID: 25
		private bool HK6;

		// Token: 0x0400001A RID: 26
		private bool HK7;

		// Token: 0x0400001B RID: 27
		private bool HK8;

		// Token: 0x0400001C RID: 28
		private bool HK9;

		// Token: 0x0400001D RID: 29
		private bool HK10;

		// Token: 0x0400001E RID: 30
		private bool HK11;

		// Token: 0x0400001F RID: 31
		private bool HK12;

		// Token: 0x04000020 RID: 32
		private bool Ale;

		// Token: 0x04000021 RID: 33
		private bool pPlayert;

		// Token: 0x04000022 RID: 34
		private bool sp;

		// Token: 0x04000023 RID: 35
		private float esplmt = 400f;

		// Token: 0x04000024 RID: 36
		private float esplmt2 = 400f;

		// Token: 0x04000025 RID: 37
		private float esplmt3 = 500f;

		// Token: 0x04000026 RID: 38
		private float esplmt4 = 400f;

		// Token: 0x04000027 RID: 39
		private float esplmt5 = 200f;

		// Token: 0x04000028 RID: 40
		private float esplmt6 = 130f;

		// Token: 0x04000029 RID: 41
		private float chatdelay = 100f;

		// Token: 0x0400002A RID: 42
		public Color MainColor = Color.Lerp(Color.green, Color.yellow, 0.5f);

		// Token: 0x0400002B RID: 43
		private int iLang;

		// Token: 0x0400002C RID: 44
		private int svID = 99;

		// Token: 0x0400002D RID: 45
		private int iMenu = 100;

		// Token: 0x0400002E RID: 46
		private int d;

		// Token: 0x0400002F RID: 47
		private int[] dBug = new int[4];

		// Token: 0x04000030 RID: 48
		private string[] sLang = new string[]
		{
			"PT",
			"EN"
		};

		// Token: 0x04000031 RID: 49
		private string[] sMenu = new string[3];

		// Token: 0x04000032 RID: 50
		private string[] BiosName = new string[4];

		// Token: 0x04000033 RID: 51
		private string[] Servers = new string[]
		{
			"Kortal #1111",
			"Kortal #2",
			"Kortal #3",
			"UNITY",
			"Survival uCant",
			"Knights",
			"Local"
		};

		// Token: 0x04000034 RID: 52
		private string[] ServersIP = new string[]
		{
			"hurtworld.kortal.org",
			"hurtworld2.kortal.org",
			"hurtworld3.kortal.org",
			"188.68.238.192",
			"survival.ucantsurvive.com:12872",
			"knightshurtworld.hopto.org:12876",
			"127.0.0.1"
		};

		// Token: 0x04000035 RID: 53
		private Vector3 BioPos;

		// Token: 0x04000036 RID: 54
		private Vector3 BioPos1;

		// Token: 0x04000037 RID: 55
		private Vector3 BioPos2;

		// Token: 0x04000038 RID: 56
		private Vector3 BioPos3 = Vector3.zero;

		// Token: 0x04000039 RID: 57
		private TOD_Sky pLight;

		// Token: 0x0400003A RID: 58
		private ChatManagerClient pChat;

		// Token: 0x0400003B RID: 59
		private ChatManagerServer pChat2;

		// Token: 0x0400003C RID: 60
		private ConsoleManager pConsole;

		// Token: 0x0400003D RID: 61
		private Rect MenuPos;

		// Token: 0x0400003E RID: 62
		private Rect SvMenu;

		// Token: 0x0400003F RID: 63
		private Rect sMenuPos;

		// Token: 0x04000040 RID: 64
		private int sendsmax;

		// Token: 0x04000041 RID: 65
		private int send;

		// Token: 0x04000042 RID: 66
		private List<LootSpawner.LootProbablityPair> T;
	}
}
