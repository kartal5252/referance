using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	// Token: 0x02000010 RID: 16
	public static class Localization
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00003F4C File Offset: 0x0000214C
		// Note: this type is marked as 'beforefieldinit'.
		static Localization()
		{
			Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
			string key = "en";
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2["CommandUsageExtLoad"] = "Usage: oxide.ext.load <extname>+";
			dictionary2["CommandUsageExtUnload"] = "Usage: oxide.ext.unload <extname>+";
			dictionary2["CommandUsageExtReload"] = "Usage: oxide.ext.reload <extname>+";
			dictionary2["CommandUsageGrant"] = "Usage: oxide.grant <group|user> <name|id> <permission>";
			dictionary2["CommandUsageGroup"] = "Usage: oxide.group <add|set> <name> [title] [rank]";
			dictionary2["CommandUsageGroupParent"] = "Usage: oxide.group <parent> <name> <parentName>";
			dictionary2["CommandUsageGroupRemove"] = "Usage: oxide.group <remove> <name>";
			dictionary2["CommandUsageLang"] = "Usage: oxide.lang <two-digit language code>";
			dictionary2["CommandUsageLoad"] = "Usage: oxide.load *|<pluginname>+";
			dictionary2["CommandUsageReload"] = "Usage: oxide.reload *|<pluginname>+";
			dictionary2["CommandUsageRevoke"] = "Usage: oxide.revoke <group|user> <name|id> <permission>";
			dictionary2["CommandUsageShow"] = "Usage: oxide.show <groups|perms>";
			dictionary2["CommandUsageShowName"] = "Usage: oxide.show <group|user> <name>";
			dictionary2["CommandUsageUnload"] = "Usage: oxide.unload *|<pluginname>+";
			dictionary2["CommandUsageUserGroup"] = "Usage: oxide.usergroup <add|remove> <username> <groupname>";
			dictionary2["ConnectionRejected"] = "Connection was rejected";
			dictionary2["DataSaved"] = "Saving Oxide data...";
			dictionary2["GroupAlreadyExists"] = "Group '{0}' already exists";
			dictionary2["GroupAlreadyHasPermission"] = "Group '{0}' already has permission '{1}'";
			dictionary2["GroupDoesNotHavePermission"] = "Group '{0}' does not have permission '{1}'";
			dictionary2["GroupChanged"] = "Group '{0}' changed";
			dictionary2["GroupCreated"] = "Group '{0}' created";
			dictionary2["GroupDeleted"] = "Group '{0}' deleted";
			dictionary2["GroupNotFound"] = "Group '{0}' doesn't exist";
			dictionary2["GroupParentChanged"] = "Group '{0}' parent changed to '{1}'";
			dictionary2["GroupParentNotChanged"] = "Group '{0}' parent was not changed";
			dictionary2["GroupParentNotFound"] = "Group parent '{0}' doesn't exist";
			dictionary2["GroupPermissionGranted"] = "Group '{0}' granted permission '{1}'";
			dictionary2["GroupPermissionRevoked"] = "Group '{0}' revoked permission '{1}'";
			dictionary2["GroupPermissions"] = "Group '{0}' permissions";
			dictionary2["GroupPlayers"] = "Group '{0}' players";
			dictionary2["Groups"] = "Groups";
			dictionary2["NoGroupPermissions"] = "No permissions currently granted";
			dictionary2["NoPermissionGroups"] = "No groups with this permission";
			dictionary2["NoPermissionPlayers"] = "No players with this permission";
			dictionary2["NoPluginsFound"] = "No plugins are currently available";
			dictionary2["NoPlayerGroups"] = "Player is not assigned to any groups";
			dictionary2["NoPlayerPermissions"] = "No permissions currently granted";
			dictionary2["NoPlayersInGroup"] = "No players currently in group";
			dictionary2["NotAllowed"] = "You are not allowed to use the '{0}' command";
			dictionary2["ParentGroupPermissions"] = "Parent group '{0}' permissions";
			dictionary2["PermissionGroups"] = "Permission '{0}' Groups";
			dictionary2["PermissionPlayers"] = "Permission '{0}' Players";
			dictionary2["PermissionNotFound"] = "Permission '{0}' doesn't exist";
			dictionary2["Permissions"] = "Permissions";
			dictionary2["PermissionsNotLoaded"] = "Unable to load permission files! Permissions will not work until resolved.\n => {0}";
			dictionary2["PlayerLanguage"] = "Player language set to {0}";
			dictionary2["PluginNotLoaded"] = "Plugin '{0}' not loaded.";
			dictionary2["PluginReloaded"] = "Reloaded plugin {0} v{1} by {2}";
			dictionary2["PluginUnloaded"] = "Unloaded plugin {0} v{1} by {2}";
			dictionary2["ServerLanguage"] = "Server language set to {0}";
			dictionary2["Unknown"] = "Unknown";
			dictionary2["UnknownCommand"] = "Unknown command: {0}";
			dictionary2["PlayerAddedToGroup"] = "Player '{0}' added to group: {1}";
			dictionary2["PlayerAlreadyHasPermission"] = "Player '{0}' already has permission '{1}'";
			dictionary2["PlayerDoesNotHavePermission"] = "Player '{0}' does not have permission '{1}'";
			dictionary2["PlayerNotFound"] = "Player '{0}' not found";
			dictionary2["PlayerGroups"] = "Player '{0}' groups";
			dictionary2["PlayerPermissions"] = "Player '{0}' permissions";
			dictionary2["PlayerPermissionGranted"] = "Player '{0}' granted permission '{1}'";
			dictionary2["PlayerPermissionRevoked"] = "Player '{0}' revoked permission '{1}'";
			dictionary2["PlayerRemovedFromGroup"] = "Player '{0}' removed from group '{1}'";
			dictionary2["PlayersFound"] = "Multiple players were found, please specify: {0}";
			dictionary2["Version"] = "Server is running [#ffb658]Oxide {0}[/#] and [#ee715c]{1} {2} ({3})[/#]";
			dictionary2["YouAreNotAdmin"] = "You are not an admin";
			dictionary[key] = dictionary2;
			Localization.languages = dictionary;
		}

		// Token: 0x04000031 RID: 49
		public static readonly Dictionary<string, Dictionary<string, string>> languages;
	}
}
