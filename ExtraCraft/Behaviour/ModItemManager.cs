using System;
using UnityEngine;

	public  class ModItemManager : FortressCraftMod
	{
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

