using System;
using UnityEngine;

	public  class ModItemManager : FortressCraftMod
	{
	
		
		public static bool AddNewModdedItem(ItemEntry newItem)
		{
			bool found = false;
			if (newItem == null)
			{
				global::GameManager.DoLocalChat ("[MOD] Item Null (By:Pencol)");
				return false;
			}

			ItemEntry[] ModItemEntry;
			ItemEntry[] NormalItemEntry;
			NormalItemEntry = (ItemEntry[]) ItemEntry.mEntries.Clone();
			foreach(ItemEntry entry in NormalItemEntry)
			{
				if(entry!=null)
				if(entry.ItemID == newItem.ItemID)
				{
					found = true;
				}
			}
			if(!found)
				{

				if(newItem.ItemID< NormalItemEntry.Length)
				{
					ModItemEntry =(ItemEntry[]) ItemEntry.mEntries.Clone();
					ModItemEntry[newItem.ItemID] = newItem;

				}else{
					ModItemEntry = new ItemEntry[newItem.ItemID+1];
					for(int i =0;i< NormalItemEntry.Length;i++)
					{
						ModItemEntry[i] = NormalItemEntry[i];
						
					}
					Debug.Log("[MOD]Item added: " + newItem.Name);
					ModItemEntry[newItem.ItemID] = newItem;
				}
				ItemEntry.mEntries = ModItemEntry;
				return true;
			
			}else{
				Debug.Log("[MOD]Item already exists? (By:Pencol)");
				return false;
			}

		}

		public static bool isReady()
		{
			if (ItemEntry.mEntries!=null) return true;
			return false;
		}

		public static string GetKeyfromItemId(int Item)
		{
			foreach (ModItemMap itemMap in ModManager.mModMappings.Items)
			{
				if (itemMap.ItemId == Item) 
				{
					return itemMap.Key;
				}
			}
			return null;
		}

		public static int GetItemIDFromKey(string key)
		{
			foreach (ModItemMap itemMap in ModManager.mModMappings.Items)
			{
				if (itemMap.Key  == key) 
				{
					return itemMap.ItemId;
				}
			}
			return 0;
		}
	}

