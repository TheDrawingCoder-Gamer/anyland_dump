using BepInEx;
using System.IO;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System.Linq;
namespace DumpInv {
	[BepInPlugin("net.bulbyvr.anyland.DumpInv", "Dump Inventory", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		void Awake() {
			Harmony.CreateAndPatchAll(typeof(Patches));
		}

		void Update() {
			
		}
	}
	public class Patches {
		[HarmonyPatch(typeof(InventoryDialog), "Open")]
		[HarmonyPostfix]
		static void OnInvOpen(InventoryDialog __instance) 
		{	
			loadInv(__instance);	
		}
		static void loadInv(InventoryDialog __instance) {
			string path = Paths.BepInExRootPath + "/invdump";
			if (Directory.Exists(path))
				return;
			Directory.CreateDirectory(path);
			ServerManager manager = Managers.serverManager;
			for (int i = 0; i < 100; i++)
			{
				int cur = i;
				__instance.StartCoroutine(manager.LoadInventory(i, delegate(LoadInventory_Response response) {
							Debug.Log($"Page {cur}");
							Directory.CreateDirectory(path + $"/page{cur}");
							foreach (InventoryItemData item in response.inventoryItems.inventoryItems)
							{
								string id = item.thingId;
								dumpInator(__instance, id, $"{path}/page{cur}");
							}
						}));
			}
		}
		static void dumpInator(InventoryDialog __instance, string id, string root) {
			ServerManager manager = Managers.serverManager;
			__instance.StartCoroutine(manager.GetThingDefinition(id, delegate(GetThingDefinition_Response response)
						{
							File.WriteAllText($"{root}/{id}.json", response.thingDefinitionJSON);
							JSONObject res = JSON.Parse(response.thingDefinitionJSON).AsObject;
							if (Enumerable.Contains(res.Keys, "inc"))
							{
								
								Directory.CreateDirectory($"{root}/{id}/inc");
								foreach (JSONNode item in res["inc"].AsArray) 
								{
									string incId = item.AsArray[1].Value;
									dumpInator(__instance, incId, $"{root}/{id}/inc");
								}
							}
							foreach (JSONNode part in res["p"].AsArray) 
							{
								
								if (Enumerable.Contains(part.Keys, "i")) 
								{
									Directory.CreateDirectory($"{root}/{id}/incsub");
									foreach (JSONNode item in part["i"].AsArray) 
									{
										string subId = item.AsObject["t"].Value;
										dumpInator(__instance, subId, $"{root}/{id}/incsub");
									}
								}
								if (Enumerable.Contains(part.Keys, "su"))
								{
									Directory.CreateDirectory($"{root}/{id}/psub");
									foreach (JSONNode item in part["su"].AsArray)
									{
										string subId = item.AsObject["t"].Value;
										dumpInator(__instance, subId, $"{root}/{id}/psub");
									}
								}
							}
						}));
		}
	}
}
