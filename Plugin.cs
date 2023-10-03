using BepInEx;
using System.IO;
using HarmonyLib;
using UnityEngine;
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
								__instance.StartCoroutine(manager.GetThingDefinition(id, delegate(GetThingDefinition_Response response1) 
										{
											File.WriteAllText(path + $"/page{cur}/{id}.json", response1.thingDefinitionJSON);
										}));
							}
						}));
			}
		}
	}
}
