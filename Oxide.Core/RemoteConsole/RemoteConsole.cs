using System;
using System.Linq;
using System.Net;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries.Covalence;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace Oxide.Core.RemoteConsole
{
	// Token: 0x0200001D RID: 29
	public class RemoteConsole
	{
		// Token: 0x06000134 RID: 308 RVA: 0x000076C8 File Offset: 0x000058C8
		public void Initalize()
		{
			if (!this.config.Enabled || this.listener != null || this.server != null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.config.Password))
			{
				Interface.Oxide.LogWarning("[Rcon] Remote console password is not set, disabling", new object[0]);
				return;
			}
			try
			{
				this.server = new WebSocketServer(this.config.Port)
				{
					WaitTime = TimeSpan.FromSeconds(5.0),
					ReuseAddress = true
				};
				this.server.AddWebSocketService<RemoteConsole.RconListener>("/" + this.config.Password, () => this.listener = new RemoteConsole.RconListener(this));
				this.server.Start();
				Interface.Oxide.LogInfo(string.Format("[Rcon] Server started successfully on port {0}", this.server.Port), new object[0]);
			}
			catch (Exception ex)
			{
				OxideMod oxide = Interface.Oxide;
				string format = "[Rcon] Failed to start server on port {0}";
				WebSocketServer webSocketServer = this.server;
				oxide.LogException(string.Format(format, (webSocketServer != null) ? new int?(webSocketServer.Port) : null), ex);
				string format2 = "Failed to start RCON server on port {0}";
				WebSocketServer webSocketServer2 = this.server;
				RemoteLogger.Exception(string.Format(format2, (webSocketServer2 != null) ? new int?(webSocketServer2.Port) : null), ex);
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00007830 File Offset: 0x00005A30
		public void Shutdown(string reason = "Server shutting down", CloseStatusCode code = CloseStatusCode.Normal)
		{
			if (this.server != null)
			{
				this.server.Stop(code, reason);
				this.server = null;
				this.listener = null;
				Interface.Oxide.LogInfo(string.Format("[Rcon] Service has stopped: {0} ({1})", reason, code), new object[0]);
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007881 File Offset: 0x00005A81
		public void SendMessage(RemoteMessage message)
		{
			if (message != null && this.server != null && this.server.IsListening && this.listener != null)
			{
				this.listener.SendMessage(message);
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x000078B0 File Offset: 0x00005AB0
		public void SendMessage(string message, int identifier)
		{
			if (!string.IsNullOrEmpty(message) && this.server != null && this.server.IsListening && this.listener != null)
			{
				this.listener.SendMessage(RemoteMessage.CreateMessage(message, identifier, "Generic", ""));
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00007900 File Offset: 0x00005B00
		public void SendMessage(WebSocketContext connection, string message, int identifier)
		{
			if (!string.IsNullOrEmpty(message) && this.server != null && this.server.IsListening && this.listener != null && connection != null)
			{
				WebSocket webSocket = connection.WebSocket;
				if (webSocket == null)
				{
					return;
				}
				webSocket.Send(RemoteMessage.CreateMessage(message, identifier, "Generic", "").ToJSON());
			}
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000795C File Offset: 0x00005B5C
		private void OnMessage(MessageEventArgs e, WebSocketContext connection)
		{
			if (this.covalence == null)
			{
				Interface.Oxide.LogError("[Rcon] Failed to process command, Covalence is null", new object[0]);
				return;
			}
			RemoteMessage message = RemoteMessage.GetMessage(e.Data);
			if (message == null)
			{
				Interface.Oxide.LogError("[Rcon] Failed to process command, RemoteMessage is null", new object[0]);
				return;
			}
			if (string.IsNullOrEmpty(message.Message))
			{
				Interface.Oxide.LogError("[Rcon] Failed to process command, RemoteMessage.Text is not set", new object[0]);
				return;
			}
			string[] array = CommandLine.Split(message.Message);
			string text = array[0].ToLower();
			string[] array2 = array.Skip(1).ToArray<string>();
			if (Interface.CallHook("OnRconCommand", connection.UserEndPoint, text, array2) != null)
			{
				return;
			}
			IServer server = this.covalence.Server;
			string command = text;
			object[] args = array2;
			server.Command(command, args);
		}

		// Token: 0x0400007F RID: 127
		private readonly Covalence covalence = Interface.Oxide.GetLibrary<Covalence>(null);

		// Token: 0x04000080 RID: 128
		private readonly OxideConfig.OxideRcon config = Interface.Oxide.Config.Rcon;

		// Token: 0x04000081 RID: 129
		private RemoteConsole.RconListener listener;

		// Token: 0x04000082 RID: 130
		private WebSocketServer server;

		// Token: 0x02000077 RID: 119
		private struct RconPlayer
		{
			// Token: 0x1700009C RID: 156
			// (get) Token: 0x060003DB RID: 987 RVA: 0x0000FACD File Offset: 0x0000DCCD
			private string SteamID { get; }

			// Token: 0x1700009D RID: 157
			// (get) Token: 0x060003DC RID: 988 RVA: 0x0000FAD5 File Offset: 0x0000DCD5
			private string OwnerSteamID { get; }

			// Token: 0x1700009E RID: 158
			// (get) Token: 0x060003DD RID: 989 RVA: 0x0000FADD File Offset: 0x0000DCDD
			private string DisplayName { get; }

			// Token: 0x1700009F RID: 159
			// (get) Token: 0x060003DE RID: 990 RVA: 0x0000FAE5 File Offset: 0x0000DCE5
			private string Address { get; }

			// Token: 0x170000A0 RID: 160
			// (get) Token: 0x060003DF RID: 991 RVA: 0x0000FAED File Offset: 0x0000DCED
			private int Ping { get; }

			// Token: 0x170000A1 RID: 161
			// (get) Token: 0x060003E0 RID: 992 RVA: 0x0000FAF5 File Offset: 0x0000DCF5
			private int ConnectedSeconds { get; }

			// Token: 0x170000A2 RID: 162
			// (get) Token: 0x060003E1 RID: 993 RVA: 0x0000FAFD File Offset: 0x0000DCFD
			private float VoiationLevel { get; }

			// Token: 0x170000A3 RID: 163
			// (get) Token: 0x060003E2 RID: 994 RVA: 0x0000FB05 File Offset: 0x0000DD05
			private float CurrentLevel { get; }

			// Token: 0x170000A4 RID: 164
			// (get) Token: 0x060003E3 RID: 995 RVA: 0x0000FB0D File Offset: 0x0000DD0D
			private float UnspentXp { get; }

			// Token: 0x170000A5 RID: 165
			// (get) Token: 0x060003E4 RID: 996 RVA: 0x0000FB15 File Offset: 0x0000DD15
			private float Health { get; }

			// Token: 0x060003E5 RID: 997 RVA: 0x0000FB20 File Offset: 0x0000DD20
			public RconPlayer(IPlayer player)
			{
				this.SteamID = player.Id;
				this.OwnerSteamID = "0";
				this.DisplayName = player.Name;
				this.Address = player.Address;
				this.Ping = player.Ping;
				this.ConnectedSeconds = 0;
				this.VoiationLevel = 0f;
				this.CurrentLevel = 0f;
				this.UnspentXp = 0f;
				this.Health = player.Health;
			}
		}

		// Token: 0x02000078 RID: 120
		public class RconListener : WebSocketBehavior
		{
			// Token: 0x060003E6 RID: 998 RVA: 0x0000FB9C File Offset: 0x0000DD9C
			public RconListener(RemoteConsole parent)
			{
				base.IgnoreExtensions = true;
				this.Parent = parent;
			}

			// Token: 0x060003E7 RID: 999 RVA: 0x0000FBB2 File Offset: 0x0000DDB2
			public void SendMessage(RemoteMessage message)
			{
				base.Sessions.Broadcast(message.ToJSON());
			}

			// Token: 0x060003E8 RID: 1000 RVA: 0x0000FBC8 File Offset: 0x0000DDC8
			protected override void OnClose(CloseEventArgs e)
			{
				string arg = string.IsNullOrEmpty(e.Reason) ? "Unknown" : e.Reason;
				Interface.Oxide.LogInfo(string.Format("[Rcon] Connection from {0} closed: {1} ({2})", this.Address, arg, e.Code), new object[0]);
			}

			// Token: 0x060003E9 RID: 1001 RVA: 0x0000FC1C File Offset: 0x0000DE1C
			protected override void OnError(ErrorEventArgs e)
			{
				Interface.Oxide.LogException(e.Message, e.Exception);
			}

			// Token: 0x060003EA RID: 1002 RVA: 0x0000FC34 File Offset: 0x0000DE34
			protected override void OnMessage(MessageEventArgs e)
			{
				RemoteConsole parent = this.Parent;
				if (parent == null)
				{
					return;
				}
				parent.OnMessage(e, base.Context);
			}

			// Token: 0x060003EB RID: 1003 RVA: 0x0000FC4D File Offset: 0x0000DE4D
			protected override void OnOpen()
			{
				this.Address = base.Context.UserEndPoint.Address;
				Interface.Oxide.LogInfo(string.Format("[Rcon] New connection from {0}", this.Address), new object[0]);
			}

			// Token: 0x040001A0 RID: 416
			private readonly RemoteConsole Parent;

			// Token: 0x040001A1 RID: 417
			private IPAddress Address;
		}
	}
}
