using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Hurtworld.Libraries;
using Oxide.Game.Hurtworld.Libraries.Covalence;
using Steamworks;
using uLink;
using UnityEngine;

namespace Oxide.Game.Hurtworld
{
	// Token: 0x02000003 RID: 3
	public class HurtworldCore : CSPlugin
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002178 File Offset: 0x00000378
		[HookMethod("GrantCommand")]
		private void GrantCommand(IPlayer player, string command, string[] args)
		{
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if (args.Length < 3)
			{
				player.Reply(this.lang.GetMessage("CommandUsageGrant", this, player.Id));
				return;
			}
			string text = args[0];
			string text2 = args[1].Sanitize();
			string text3 = args[2];
			if (!this.permission.PermissionExists(text3, null))
			{
				player.Reply(string.Format(this.lang.GetMessage("PermissionNotFound", this, player.Id), text3));
				return;
			}
			if (text.Equals("group"))
			{
				if (!this.permission.GroupExists(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), text2));
					return;
				}
				if (this.permission.GroupHasPermission(text2, text3))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupAlreadyHasPermission", this, player.Id), text2, text3));
					return;
				}
				this.permission.GrantGroupPermission(text2, text3, null);
				player.Reply(string.Format(this.lang.GetMessage("GroupPermissionGranted", this, player.Id), text2, text3));
				return;
			}
			else
			{
				if (!text.Equals("user"))
				{
					player.Reply(this.lang.GetMessage("CommandUsageGrant", this, player.Id));
					return;
				}
				IPlayer[] array = HurtworldCore.Covalence.PlayerManager.FindPlayers(text2).ToArray<IPlayer>();
				if (array.Length > 1)
				{
					player.Reply(string.Format(this.lang.GetMessage("PlayersFound", this, player.Id), string.Join(", ", (from p in array
					select p.Name).ToArray<string>())));
					return;
				}
				IPlayer player2 = (array.Length == 1) ? array[0] : null;
				if (player2 == null && !this.permission.UserIdValid(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("PlayerNotFound", this, player.Id), text2));
					return;
				}
				string text4 = text2;
				if (player2 != null)
				{
					text4 = player2.Id;
					text2 = player2.Name;
					this.permission.UpdateNickname(text4, text2);
				}
				if (this.permission.UserHasPermission(text2, text3))
				{
					player.Reply(string.Format(this.lang.GetMessage("PlayerAlreadyHasPermission", this, player.Id), text4, text3));
					return;
				}
				this.permission.GrantUserPermission(text4, text3, null);
				player.Reply(string.Format(this.lang.GetMessage("PlayerPermissionGranted", this, player.Id), text2 + " (" + text4 + ")", text3));
				return;
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002424 File Offset: 0x00000624
		[HookMethod("GroupCommand")]
		private void GroupCommand(IPlayer player, string command, string[] args)
		{
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if (args.Length < 2)
			{
				player.Reply(this.lang.GetMessage("CommandUsageGroup", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageGroupParent", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageGroupRemove", this, player.Id));
				return;
			}
			string text = args[0];
			string text2 = args[1];
			string title = (args.Length >= 3) ? args[2] : "";
			int rank = (args.Length == 4) ? int.Parse(args[3]) : 0;
			if (text.Equals("add"))
			{
				if (this.permission.GroupExists(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupAlreadyExists", this, player.Id), text2));
					return;
				}
				this.permission.CreateGroup(text2, title, rank);
				player.Reply(string.Format(this.lang.GetMessage("GroupCreated", this, player.Id), text2));
				return;
			}
			else if (text.Equals("remove"))
			{
				if (!this.permission.GroupExists(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), text2));
					return;
				}
				this.permission.RemoveGroup(text2);
				player.Reply(string.Format(this.lang.GetMessage("GroupDeleted", this, player.Id), text2));
				return;
			}
			else if (text.Equals("set"))
			{
				if (!this.permission.GroupExists(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), text2));
					return;
				}
				this.permission.SetGroupTitle(text2, title);
				this.permission.SetGroupRank(text2, rank);
				player.Reply(string.Format(this.lang.GetMessage("GroupChanged", this, player.Id), text2));
				return;
			}
			else
			{
				if (!text.Equals("parent"))
				{
					player.Reply(this.lang.GetMessage("CommandUsageGroup", this, player.Id));
					player.Reply(this.lang.GetMessage("CommandUsageGroupParent", this, player.Id));
					player.Reply(this.lang.GetMessage("CommandUsageGroupRemove", this, player.Id));
					return;
				}
				if (args.Length <= 2)
				{
					player.Reply(this.lang.GetMessage("CommandUsageGroupParent", this, player.Id));
					return;
				}
				if (!this.permission.GroupExists(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), text2));
					return;
				}
				string text3 = args[2];
				if (!string.IsNullOrEmpty(text3) && !this.permission.GroupExists(text3))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupParentNotFound", this, player.Id), text3));
					return;
				}
				if (this.permission.SetGroupParent(text2, text3))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupParentChanged", this, player.Id), text2, text3));
					return;
				}
				player.Reply(string.Format(this.lang.GetMessage("GroupParentNotChanged", this, player.Id), text2));
				return;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002780 File Offset: 0x00000980
		[HookMethod("LangCommand")]
		private void LangCommand(IPlayer player, string command, string[] args)
		{
			if (args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageLang", this, player.Id));
				return;
			}
			if (player.IsServer)
			{
				this.lang.SetServerLanguage(args[0]);
				player.Reply(string.Format(this.lang.GetMessage("ServerLanguage", this, player.Id), this.lang.GetServerLanguage()));
				return;
			}
			if (this.lang.GetLanguages(null).Contains(args[0]))
			{
				this.lang.SetLanguage(args[0], player.Id);
			}
			player.Reply(string.Format(this.lang.GetMessage("PlayerLanguage", this, player.Id), args[0]));
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002844 File Offset: 0x00000A44
		[HookMethod("LoadCommand")]
		private void LoadCommand(IPlayer player, string command, string[] args)
		{
			if (args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageLoad", this, player.Id));
				return;
			}
			if (args[0].Equals("*") || args[0].Equals("all"))
			{
				Interface.Oxide.LoadAllPlugins(false);
				return;
			}
			foreach (string text in args)
			{
				if (!string.IsNullOrEmpty(text))
				{
					Interface.Oxide.LoadPlugin(text);
					this.pluginManager.GetPlugin(text);
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000028D4 File Offset: 0x00000AD4
		[HookMethod("PluginsCommand")]
		private void PluginsCommand(IPlayer player, string command, string[] args)
		{
			Plugin[] array = (from pl in this.pluginManager.GetPlugins()
			where !pl.IsCorePlugin
			select pl).ToArray<Plugin>();
			HashSet<string> second = new HashSet<string>(from pl in array
			select pl.Name);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (PluginLoader pluginLoader in Interface.Oxide.GetPluginLoaders())
			{
				foreach (string key in pluginLoader.ScanDirectory(Interface.Oxide.PluginDirectory).Except(second))
				{
					string text;
					dictionary[key] = (pluginLoader.PluginErrors.TryGetValue(key, out text) ? text : "Unloaded");
				}
			}
			if (array.Length + dictionary.Count < 1)
			{
				player.Reply(this.lang.GetMessage("NoPluginsFound", this, player.Id));
				return;
			}
			string text2 = string.Format("Listing {0} plugins:", array.Length + dictionary.Count);
			int num = 1;
			foreach (Plugin plugin in from p in array
			where p.Filename != null
			select p)
			{
				text2 += string.Format("\n  {0:00} \"{1}\" ({2}) by {3} ({4:0.00}s) - {5}", new object[]
				{
					num++,
					plugin.Title,
					plugin.Version,
					plugin.Author,
					plugin.TotalHookTime,
					plugin.Filename.Basename(null)
				});
			}
			foreach (string text3 in dictionary.Keys)
			{
				text2 += string.Format("\n  {0:00} {1} - {2}", num++, text3, dictionary[text3]);
			}
			player.Reply(text2);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002B70 File Offset: 0x00000D70
		[HookMethod("ReloadCommand")]
		private void ReloadCommand(IPlayer player, string command, string[] args)
		{
			if (args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageReload", this, player.Id));
				return;
			}
			if (args[0].Equals("*") || args[0].Equals("all"))
			{
				Interface.Oxide.ReloadAllPlugins(null);
				return;
			}
			foreach (string text in args)
			{
				if (!string.IsNullOrEmpty(text))
				{
					Interface.Oxide.ReloadPlugin(text);
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002BF4 File Offset: 0x00000DF4
		[HookMethod("RevokeCommand")]
		private void RevokeCommand(IPlayer player, string command, string[] args)
		{
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if (args.Length < 3)
			{
				player.Reply(this.lang.GetMessage("CommandUsageRevoke", this, player.Id));
				return;
			}
			string text = args[0];
			string text2 = args[1].Sanitize();
			string text3 = args[2];
			if (text.Equals("group"))
			{
				if (!this.permission.GroupExists(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), text2));
					return;
				}
				if (!this.permission.GroupHasPermission(text2, text3))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupDoesNotHavePermission", this, player.Id), text2, text3));
					return;
				}
				this.permission.RevokeGroupPermission(text2, text3);
				player.Reply(string.Format(this.lang.GetMessage("GroupPermissionRevoked", this, player.Id), text2, text3));
				return;
			}
			else
			{
				if (!text.Equals("user"))
				{
					player.Reply(this.lang.GetMessage("CommandUsageRevoke", this, player.Id));
					return;
				}
				IPlayer[] array = HurtworldCore.Covalence.PlayerManager.FindPlayers(text2).ToArray<IPlayer>();
				if (array.Length > 1)
				{
					player.Reply(string.Format(this.lang.GetMessage("PlayersFound", this, player.Id), string.Join(", ", (from p in array
					select p.Name).ToArray<string>())));
					return;
				}
				IPlayer player2 = (array.Length == 1) ? array[0] : null;
				if (player2 == null && !this.permission.UserIdValid(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("PlayerNotFound", this, player.Id), text2));
					return;
				}
				string text4 = text2;
				if (player2 != null)
				{
					text4 = player2.Id;
					text2 = player2.Name;
					this.permission.UpdateNickname(text4, text2);
				}
				if (!this.permission.UserHasPermission(text4, text3))
				{
					player.Reply(string.Format(this.lang.GetMessage("PlayerDoesNotHavePermission", this, player.Id), text2, text3));
					return;
				}
				this.permission.RevokeUserPermission(text4, text3);
				player.Reply(string.Format(this.lang.GetMessage("PlayerPermissionRevoked", this, player.Id), text2 + " (" + text4 + ")", text3));
				return;
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002E6C File Offset: 0x0000106C
		[HookMethod("ShowCommand")]
		private void ShowCommand(IPlayer player, string command, string[] args)
		{
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if (args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
				return;
			}
			string text = args[0];
			string text2 = (args.Length == 2) ? args[1].Sanitize() : string.Empty;
			if (text.Equals("perms"))
			{
				player.Reply(string.Format(this.lang.GetMessage("Permissions", this, player.Id) + ":\n" + string.Join(", ", this.permission.GetPermissions()), new object[0]));
				return;
			}
			if (text.Equals("perm"))
			{
				if (args.Length < 2 || string.IsNullOrEmpty(text2))
				{
					player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
					player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
					return;
				}
				string[] permissionUsers = this.permission.GetPermissionUsers(text2);
				string[] permissionGroups = this.permission.GetPermissionGroups(text2);
				string text3 = string.Format(this.lang.GetMessage("PermissionPlayers", this, player.Id), text2) + ":\n";
				text3 += ((permissionUsers.Length != 0) ? string.Join(", ", permissionUsers) : this.lang.GetMessage("NoPermissionPlayers", this, player.Id));
				text3 = text3 + "\n\n" + string.Format(this.lang.GetMessage("PermissionGroups", this, player.Id), text2) + ":\n";
				text3 += ((permissionGroups.Length != 0) ? string.Join(", ", permissionGroups) : this.lang.GetMessage("NoPermissionGroups", this, player.Id));
				player.Reply(text3);
				return;
			}
			else if (text.Equals("user"))
			{
				if (args.Length < 2 || string.IsNullOrEmpty(text2))
				{
					player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
					player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
					return;
				}
				IPlayer[] array = HurtworldCore.Covalence.PlayerManager.FindPlayers(text2).ToArray<IPlayer>();
				if (array.Length > 1)
				{
					player.Reply(string.Format(this.lang.GetMessage("PlayersFound", this, player.Id), string.Join(", ", (from p in array
					select p.Name).ToArray<string>())));
					return;
				}
				IPlayer player2 = (array.Length == 1) ? array[0] : null;
				if (player2 == null && !this.permission.UserIdValid(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("PlayerNotFound", this, player.Id), text2));
					return;
				}
				string text4 = text2;
				if (player2 != null)
				{
					text4 = player2.Id;
					text2 = player2.Name;
					this.permission.UpdateNickname(text4, text2);
					text2 = text2 + " (" + text4 + ")";
				}
				string[] userPermissions = this.permission.GetUserPermissions(text4);
				string[] userGroups = this.permission.GetUserGroups(text4);
				string text5 = string.Format(this.lang.GetMessage("PlayerPermissions", this, player.Id), text2) + ":\n";
				text5 += ((userPermissions.Length != 0) ? string.Join(", ", userPermissions) : this.lang.GetMessage("NoPlayerPermissions", this, player.Id));
				text5 = text5 + "\n\n" + string.Format(this.lang.GetMessage("PlayerGroups", this, player.Id), text2) + ":\n";
				text5 += ((userGroups.Length != 0) ? string.Join(", ", userGroups) : this.lang.GetMessage("NoPlayerGroups", this, player.Id));
				player.Reply(text5);
				return;
			}
			else if (text.Equals("group"))
			{
				if (args.Length < 2 || string.IsNullOrEmpty(text2))
				{
					player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
					player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
					return;
				}
				if (!this.permission.GroupExists(text2))
				{
					player.Reply(string.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), text2));
					return;
				}
				string[] usersInGroup = this.permission.GetUsersInGroup(text2);
				string[] groupPermissions = this.permission.GetGroupPermissions(text2, false);
				string text6 = string.Format(this.lang.GetMessage("GroupPlayers", this, player.Id), text2) + ":\n";
				text6 += ((usersInGroup.Length != 0) ? string.Join(", ", usersInGroup) : this.lang.GetMessage("NoPlayersInGroup", this, player.Id));
				text6 = text6 + "\n\n" + string.Format(this.lang.GetMessage("GroupPermissions", this, player.Id), text2) + ":\n";
				text6 += ((groupPermissions.Length != 0) ? string.Join(", ", groupPermissions) : this.lang.GetMessage("NoGroupPermissions", this, player.Id));
				string groupParent = this.permission.GetGroupParent(text2);
				while (this.permission.GroupExists(groupParent))
				{
					text6 = text6 + "\n" + string.Format(this.lang.GetMessage("ParentGroupPermissions", this, player.Id), groupParent) + ":\n";
					text6 += string.Join(", ", this.permission.GetGroupPermissions(groupParent, false));
					groupParent = this.permission.GetGroupParent(groupParent);
				}
				player.Reply(text6);
				return;
			}
			else
			{
				if (text.Equals("groups"))
				{
					player.Reply(string.Format(this.lang.GetMessage("Groups", this, player.Id) + ":\n" + string.Join(", ", this.permission.GetGroups()), new object[0]));
					return;
				}
				player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
				return;
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00003520 File Offset: 0x00001720
		[HookMethod("UnloadCommand")]
		private void UnloadCommand(IPlayer player, string command, string[] args)
		{
			if (args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageUnload", this, player.Id));
				return;
			}
			if (args[0].Equals("*") || args[0].Equals("all"))
			{
				Interface.Oxide.UnloadAllPlugins(null);
				return;
			}
			foreach (string text in args)
			{
				if (!string.IsNullOrEmpty(text))
				{
					Interface.Oxide.UnloadPlugin(text);
				}
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000035A4 File Offset: 0x000017A4
		[HookMethod("UserGroupCommand")]
		private void UserGroupCommand(IPlayer player, string command, string[] args)
		{
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if (args.Length < 3)
			{
				player.Reply(this.lang.GetMessage("CommandUsageUserGroup", this, player.Id));
				return;
			}
			string text = args[0];
			string text2 = args[1].Sanitize();
			string text3 = args[2];
			IPlayer[] array = HurtworldCore.Covalence.PlayerManager.FindPlayers(text2).ToArray<IPlayer>();
			if (array.Length > 1)
			{
				player.Reply(string.Format(this.lang.GetMessage("PlayersFound", this, player.Id), string.Join(", ", (from p in array
				select p.Name).ToArray<string>())));
				return;
			}
			IPlayer player2 = (array.Length == 1) ? array[0] : null;
			if (player2 == null && !this.permission.UserIdValid(text2))
			{
				player.Reply(string.Format(this.lang.GetMessage("PlayerNotFound", this, player.Id), text2));
				return;
			}
			string text4 = text2;
			if (player2 != null)
			{
				text4 = player2.Id;
				text2 = player2.Name;
				this.permission.UpdateNickname(text4, text2);
				text2 = text2 + "(" + text4 + ")";
			}
			if (!this.permission.GroupExists(text3))
			{
				player.Reply(string.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), text3));
				return;
			}
			if (text.Equals("add"))
			{
				this.permission.AddUserGroup(text4, text3);
				player.Reply(string.Format(this.lang.GetMessage("PlayerAddedToGroup", this, player.Id), text2, text3));
				return;
			}
			if (text.Equals("remove"))
			{
				this.permission.RemoveUserGroup(text4, text3);
				player.Reply(string.Format(this.lang.GetMessage("PlayerRemovedFromGroup", this, player.Id), text2, text3));
				return;
			}
			player.Reply(this.lang.GetMessage("CommandUsageUserGroup", this, player.Id));
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000037B0 File Offset: 0x000019B0
		[HookMethod("VersionCommand")]
		private void VersionCommand(IPlayer player, string command, string[] args)
		{
			if (!player.IsServer)
			{
				string format = HurtworldCore.Covalence.FormatText(this.lang.GetMessage("Version", this, player.Id));
				player.Reply(string.Format(format, new object[]
				{
					OxideMod.Version,
					HurtworldCore.Covalence.GameName,
					this.Server.Version,
					this.Server.Protocol
				}));
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00003830 File Offset: 0x00001A30
		[HookMethod("SaveCommand")]
		private void SaveCommand(IPlayer player, string command, string[] args)
		{
			if (this.PermissionsLoaded(player) && player.IsAdmin)
			{
				Interface.Oxide.OnSave();
				HurtworldCore.Covalence.PlayerManager.SavePlayerData();
				player.Reply(this.lang.GetMessage("DataSaved", this, player.Id));
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00003884 File Offset: 0x00001A84
		public HurtworldCore()
		{
			base.Title = "Hurtworld";
			base.Author = HurtworldExtension.AssemblyAuthors;
			base.Version = HurtworldExtension.AssemblyVersion;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00003927 File Offset: 0x00001B27
		internal static IEnumerable<string> RestrictedCommands
		{
			get
			{
				return new string[]
				{
					"bindip",
					"host",
					"queryport"
				};
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00003948 File Offset: 0x00001B48
		[HookMethod("Init")]
		private void Init()
		{
			RemoteLogger.SetTag("game", base.Title.ToLower());
			RemoteLogger.SetTag("game version", this.Server.Version);
			base.AddCovalenceCommand(new string[]
			{
				"oxide.plugins",
				"o.plugins",
				"plugins"
			}, "PluginsCommand", "oxide.plugins");
			base.AddCovalenceCommand(new string[]
			{
				"oxide.load",
				"o.load",
				"plugin.load"
			}, "LoadCommand", "oxide.load");
			base.AddCovalenceCommand(new string[]
			{
				"oxide.reload",
				"o.reload",
				"plugin.reload"
			}, "ReloadCommand", "oxide.reload");
			base.AddCovalenceCommand(new string[]
			{
				"oxide.unload",
				"o.unload",
				"plugin.unload"
			}, "UnloadCommand", "oxide.unload");
			base.AddCovalenceCommand(new string[]
			{
				"oxide.grant",
				"o.grant",
				"perm.grant"
			}, "GrantCommand", "oxide.grant");
			base.AddCovalenceCommand(new string[]
			{
				"oxide.group",
				"o.group",
				"perm.group"
			}, "GroupCommand", "oxide.group");
			base.AddCovalenceCommand(new string[]
			{
				"oxide.revoke",
				"o.revoke",
				"perm.revoke"
			}, "RevokeCommand", "oxide.revoke");
			base.AddCovalenceCommand(new string[]
			{
				"oxide.show",
				"o.show",
				"perm.show"
			}, "ShowCommand", "oxide.show");
			base.AddCovalenceCommand(new string[]
			{
				"oxide.usergroup",
				"o.usergroup",
				"perm.usergroup"
			}, "UserGroupCommand", "oxide.usergroup");
			base.AddCovalenceCommand(new string[]
			{
				"oxide.lang",
				"o.lang"
			}, "LangCommand", null);
			base.AddCovalenceCommand(new string[]
			{
				"oxide.save",
				"o.save"
			}, "SaveCommand", null);
			base.AddCovalenceCommand(new string[]
			{
				"oxide.version",
				"o.version"
			}, "VersionCommand", null);
			foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in Localization.languages)
			{
				this.lang.RegisterMessages(keyValuePair.Value, this, keyValuePair.Key);
			}
			if (this.permission.IsLoaded)
			{
				int num = 0;
				foreach (string text in Interface.Oxide.Config.Options.DefaultGroups)
				{
					if (!this.permission.GroupExists(text))
					{
						this.permission.CreateGroup(text, text, num++);
					}
				}
				this.permission.RegisterValidate(delegate(string s)
				{
					ulong num2;
					return ulong.TryParse(s, out num2) && ((num2 == 0UL) ? 1 : ((int)Math.Floor(Math.Log10(num2) + 1.0))) >= 17;
				});
				this.permission.CleanUp();
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00003C8C File Offset: 0x00001E8C
		[HookMethod("OnPluginLoaded")]
		private void OnPluginLoaded(Plugin plugin)
		{
			if (this.serverInitialized)
			{
				plugin.CallHook("OnServerInitialized", new object[]
				{
					false
				});
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00003CB4 File Offset: 0x00001EB4
		[HookMethod("IOnServerInitialized")]
		private void IOnServerInitialized()
		{
			if (!this.serverInitialized)
			{
				Analytics.Collect();
				HurtworldExtension.ServerConsole();
				SteamGameServer.SetGameTags("oxide,modded");
				this.serverInitialized = true;
				Interface.CallHook("OnServerInitialized", this.serverInitialized);
				Interface.Oxide.LogInfo("Server version is: " + this.Server.Version, new object[0]);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00003D1F File Offset: 0x00001F1F
		[HookMethod("OnServerSave")]
		private void OnServerSave()
		{
			Interface.Oxide.OnSave();
			HurtworldCore.Covalence.PlayerManager.SavePlayerData();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00003D3A File Offset: 0x00001F3A
		[HookMethod("OnServerShutdown")]
		private void OnServerShutdown()
		{
			Interface.Oxide.OnShutdown();
			HurtworldCore.Covalence.PlayerManager.SavePlayerData();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00003D58 File Offset: 0x00001F58
		[HookMethod("IOnServerCommand")]
		private object IOnServerCommand(string arg)
		{
			if (arg == null || arg.Trim().Length == 0)
			{
				return null;
			}
			string text = arg.Split(new char[]
			{
				' '
			})[0] ?? "";
			string[] array = arg.Split(new char[]
			{
				' '
			}).Skip(1).ToArray<string>();
			if (Interface.Call("OnServerCommand", new object[]
			{
				text,
				array
			}) != null)
			{
				return true;
			}
			if (HurtworldCore.Covalence.CommandSystem.HandleConsoleMessage(HurtworldCore.Covalence.CommandSystem.consolePlayer, arg))
			{
				return true;
			}
			return this.cmdlib.HandleConsoleCommand(text, array);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00003E08 File Offset: 0x00002008
		private void ParseCommand(string argstr, out string cmd, out string[] args)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in argstr)
			{
				if (c == '"')
				{
					if (flag)
					{
						string text = stringBuilder.ToString().Trim();
						if (!string.IsNullOrEmpty(text))
						{
							list.Add(text);
						}
						stringBuilder = new StringBuilder();
						flag = false;
					}
					else
					{
						flag = true;
					}
				}
				else if (char.IsWhiteSpace(c) && !flag)
				{
					string text2 = stringBuilder.ToString().Trim();
					if (!string.IsNullOrEmpty(text2))
					{
						list.Add(text2);
					}
					stringBuilder = new StringBuilder();
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if (stringBuilder.Length > 0)
			{
				string text3 = stringBuilder.ToString().Trim();
				if (!string.IsNullOrEmpty(text3))
				{
					list.Add(text3);
				}
			}
			if (list.Count == 0)
			{
				cmd = null;
				args = null;
				return;
			}
			cmd = list[0];
			list.RemoveAt(0);
			args = list.ToArray();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00003F08 File Offset: 0x00002108
		private bool PermissionsLoaded(IPlayer player)
		{
			if (this.permission.IsLoaded)
			{
				return true;
			}
			player.Reply(string.Format(this.lang.GetMessage("PermissionsNotLoaded", this, player.Id), this.permission.LastException.Message));
			return false;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003F58 File Offset: 0x00002158
		public PlayerSession FindSession(string nameOrIdOrIp)
		{
			Dictionary<NetworkPlayer, PlayerSession> sessions = Singleton<GameManager>.Instance.GetSessions();
			PlayerSession result = null;
			foreach (KeyValuePair<NetworkPlayer, PlayerSession> keyValuePair in sessions)
			{
				if (nameOrIdOrIp.Equals(keyValuePair.Value.Identity.Name, StringComparison.OrdinalIgnoreCase) || nameOrIdOrIp.Equals(keyValuePair.Value.SteamId.ToString()) || nameOrIdOrIp.Equals(keyValuePair.Key.ipAddress))
				{
					result = keyValuePair.Value;
					break;
				}
			}
			return result;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00004008 File Offset: 0x00002208
		public PlayerSession FindSessionByNetPlayer(NetworkPlayer player)
		{
			return Singleton<GameManager>.Instance.GetSession(player);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00004018 File Offset: 0x00002218
		public PlayerSession FindSessionByGo(GameObject go)
		{
			return Singleton<GameManager>.Instance.GetSessions().Where(delegate(KeyValuePair<NetworkPlayer, PlayerSession> i)
			{
				object go2 = go;
				KeyValuePair<NetworkPlayer, PlayerSession> keyValuePair = i;
				return go2.Equals(keyValuePair.Value.WorldPlayerEntity);
			}).Select(delegate(KeyValuePair<NetworkPlayer, PlayerSession> i)
			{
				KeyValuePair<NetworkPlayer, PlayerSession> keyValuePair = i;
				return keyValuePair.Value;
			}).FirstOrDefault<PlayerSession>();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00004078 File Offset: 0x00002278
		[HookMethod("IOnUserApprove")]
		private object IOnUserApprove(PlayerSession session)
		{
			session.Identity.Name = (session.Identity.Name ?? "Unnamed");
			session.Name = session.Identity.Name;
			string obj = session.SteamId.ToString();
			string ipAddress = session.Player.ipAddress;
			HurtworldCore.Covalence.PlayerManager.PlayerJoin(session);
			object obj2 = Interface.CallHook("CanClientLogin", session);
			object obj3 = Interface.CallHook("CanUserLogin", session.Identity.Name, obj, ipAddress);
			object obj4 = obj2 ?? obj3;
			if (obj4 is string || (obj4 is bool && !(bool)obj4))
			{
				Singleton<GameManager>.Instance.StartCoroutine(Singleton<GameManager>.Instance.DisconnectPlayerSync(session.Player, (obj4 is string) ? obj4.ToString() : "Connection was rejected"));
				if (Singleton<GameManager>.Instance._playerSessions.ContainsKey(session.Player))
				{
					Singleton<GameManager>.Instance._playerSessions.Remove(session.Player);
				}
				if (Singleton<GameManager>.Instance._steamIdSession.ContainsKey(session.SteamId))
				{
					Singleton<GameManager>.Instance._steamIdSession.Remove(session.SteamId);
				}
				return true;
			}
			Singleton<GameManager>.Instance._playerSessions[session.Player] = session;
			object obj5 = Interface.CallHook("OnUserApprove", session);
			object obj6 = Interface.CallHook("OnUserApproved", session.Identity.Name, obj, ipAddress);
			return obj5 ?? obj6;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000041FC File Offset: 0x000023FC
		[HookMethod("IOnPlayerChat")]
		private object IOnPlayerChat(PlayerSession session, string message)
		{
			if (message.Trim().Length <= 1)
			{
				return true;
			}
			if (!message.Substring(0, 1).Equals("/"))
			{
				object obj = Interface.CallHook("OnPlayerChat", session, message);
				object obj2 = Interface.CallHook("OnUserChat", session.IPlayer, message);
				return obj ?? obj2;
			}
			if (HurtworldCore.Covalence.CommandSystem.HandleChatMessage(session.IPlayer, message))
			{
				return true;
			}
			string text = message.Substring(1);
			string text2;
			string[] args;
			this.ParseCommand(text, out text2, out args);
			if (text2 == null)
			{
				return null;
			}
			if (!this.cmdlib.HandleChatCommand(session, text2, args))
			{
				session.IPlayer.Reply(string.Format(this.lang.GetMessage("UnknownCommand", this, session.SteamId.ToString()), text2));
				return true;
			}
			Interface.CallHook("OnChatCommand", session, text);
			return true;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000042EC File Offset: 0x000024EC
		[HookMethod("IOnPlayerConnected")]
		private void IOnPlayerConnected(string name)
		{
			PlayerSession playerSession = this.Player.Find(name);
			if (playerSession == null)
			{
				return;
			}
			if (this.permission.IsLoaded)
			{
				string id = playerSession.SteamId.ToString();
				this.permission.UpdateNickname(id, playerSession.Identity.Name);
				OxideConfig.DefaultGroups defaultGroups = Interface.Oxide.Config.Options.DefaultGroups;
				if (!this.permission.UserHasGroup(id, defaultGroups.Players))
				{
					this.permission.AddUserGroup(id, defaultGroups.Players);
				}
				if (playerSession.IsAdmin && !this.permission.UserHasGroup(id, defaultGroups.Administrators))
				{
					this.permission.AddUserGroup(id, defaultGroups.Administrators);
				}
			}
			Interface.CallHook("OnPlayerConnected", playerSession);
			HurtworldCore.Covalence.PlayerManager.PlayerConnected(playerSession);
			IPlayer player = HurtworldCore.Covalence.PlayerManager.FindPlayerById(playerSession.SteamId.ToString());
			if (player != null)
			{
				playerSession.IPlayer = player;
				Interface.CallHook("OnUserConnected", playerSession.IPlayer);
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00004404 File Offset: 0x00002604
		[HookMethod("IOnPlayerDisconnected")]
		private void IOnPlayerDisconnected(PlayerSession session)
		{
			if (!session.IsLoaded)
			{
				return;
			}
			Interface.CallHook("OnPlayerDisconnected", session);
			Interface.CallHook("OnUserDisconnected", session.IPlayer, "Unknown");
			HurtworldCore.Covalence.PlayerManager.PlayerDisconnected(session);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00004444 File Offset: 0x00002644
		[HookMethod("IOnPlayerInput")]
		private void IOnPlayerInput(NetworkPlayer player, InputControls input)
		{
			PlayerSession playerSession = this.Player.Find(player);
			if (playerSession != null)
			{
				Interface.CallHook("OnPlayerInput", playerSession, input);
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00004474 File Offset: 0x00002674
		[HookMethod("IOnPlayerSuicide")]
		private object IOnPlayerSuicide(NetworkPlayer player)
		{
			PlayerSession playerSession = this.Player.Find(player);
			if (playerSession == null)
			{
				return null;
			}
			return Interface.CallHook("OnPlayerSuicide", playerSession);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000044A0 File Offset: 0x000026A0
		[HookMethod("IOnPlayerVoice")]
		private object IOnPlayerVoice(NetworkPlayer player)
		{
			PlayerSession playerSession = this.Player.Find(player);
			if (playerSession == null)
			{
				return null;
			}
			return Interface.CallHook("OnPlayerVoice", playerSession);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000044CC File Offset: 0x000026CC
		[HookMethod("IOnTakeDamage")]
		private void IOnTakeDamage(EntityEffectFluid effect, EntityStats target, EntityEffectSourceData source)
		{
			if (effect == null || target == null || source == null || source.Value.Equals(0f))
			{
				return;
			}
			AIEntity component = target.GetComponent<AIEntity>();
			if (component != null)
			{
				Interface.CallHook("OnEntityTakeDamage", component, source);
				return;
			}
			NetworkView networkView = uLinkExtensions.uLinkNetworkView(target);
			if (networkView != null)
			{
				PlayerSession session = Singleton<GameManager>.Instance.GetSession(networkView.owner);
				if (session != null)
				{
					Interface.CallHook("OnPlayerTakeDamage", session, source);
				}
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000454C File Offset: 0x0000274C
		[HookMethod("IOnSingleDoorUsed")]
		private void IOnSingleDoorUsed(DoorSingleServer door)
		{
			NetworkPlayer? lastUsedBy = door.LastUsedBy;
			if (lastUsedBy == null)
			{
				return;
			}
			PlayerSession playerSession = this.Player.Find(lastUsedBy.Value);
			if (playerSession != null)
			{
				Interface.CallHook("OnSingleDoorUsed", door, playerSession);
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00004590 File Offset: 0x00002790
		[HookMethod("IOnDoubleDoorUsed")]
		private void IOnDoubleDoorUsed(DoubleDoorServer door)
		{
			NetworkPlayer? lastUsedBy = door.LastUsedBy;
			if (lastUsedBy == null)
			{
				return;
			}
			PlayerSession playerSession = this.Player.Find(lastUsedBy.Value);
			if (playerSession != null)
			{
				Interface.CallHook("OnDoubleDoorUsed", door, playerSession);
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000045D4 File Offset: 0x000027D4
		[HookMethod("IOnGarageDoorUsed")]
		private void IOnGarageDoorUsed(GarageDoorServer door)
		{
			NetworkPlayer? lastUsedBy = door.LastUsedBy;
			if (lastUsedBy == null)
			{
				return;
			}
			PlayerSession playerSession = this.Player.Find(lastUsedBy.Value);
			if (playerSession != null)
			{
				Interface.CallHook("OnGarageDoorUsed", door, playerSession);
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00004615 File Offset: 0x00002815
		[HookMethod("ICanEnterVehicle")]
		private object ICanEnterVehicle(PlayerSession session, GameObject go)
		{
			return Interface.CallHook("CanEnterVehicle", session, go.GetComponent<VehiclePassenger>());
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00004628 File Offset: 0x00002828
		[HookMethod("ICanExitVehicle")]
		private object ICanExitVehicle(VehiclePassenger vehicle)
		{
			PlayerSession playerSession = this.Player.Find(vehicle.networkView.owner);
			if (playerSession == null)
			{
				return null;
			}
			return Interface.CallHook("CanExitVehicle", playerSession, vehicle);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00004660 File Offset: 0x00002860
		[HookMethod("IOnEnterVehicle")]
		private void IOnEnterVehicle(NetworkPlayer player, VehiclePassenger vehicle)
		{
			PlayerSession obj = this.Player.Find(player);
			Interface.CallHook("OnEnterVehicle", obj, vehicle);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00004688 File Offset: 0x00002888
		[HookMethod("IOnExitVehicle")]
		private void IOnExitVehicle(NetworkPlayer player, VehiclePassenger vehicle)
		{
			PlayerSession obj = this.Player.Find(player);
			Interface.CallHook("OnExitVehicle", obj, vehicle);
		}

		// Token: 0x04000006 RID: 6
		internal readonly Command cmdlib = Interface.Oxide.GetLibrary<Command>(null);

		// Token: 0x04000007 RID: 7
		internal readonly Lang lang = Interface.Oxide.GetLibrary<Lang>(null);

		// Token: 0x04000008 RID: 8
		internal readonly Permission permission = Interface.Oxide.GetLibrary<Permission>(null);

		// Token: 0x04000009 RID: 9
		internal readonly Player Player = Interface.Oxide.GetLibrary<Player>(null);

		// Token: 0x0400000A RID: 10
		internal static readonly HurtworldCovalenceProvider Covalence = HurtworldCovalenceProvider.Instance;

		// Token: 0x0400000B RID: 11
		internal readonly PluginManager pluginManager = Interface.Oxide.RootPluginManager;

		// Token: 0x0400000C RID: 12
		internal readonly IServer Server = HurtworldCore.Covalence.CreateServer();

		// Token: 0x0400000D RID: 13
		private bool serverInitialized;

		// Token: 0x0400000E RID: 14
		private readonly List<string> loadingPlugins = new List<string>();
	}
}
