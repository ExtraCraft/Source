using System;
using UnityEngine;
using System.Collections.Generic;


	public class MoreTreesMod:FortressCraftMod
	{
		public System.Random rand;

		public MoreTreesMod()
		{
			rand = new System.Random ();
		}

		public override ModRegistrationData Register ()
		{
			ModRegistrationData modRegistrationData = new ModRegistrationData ();
			modRegistrationData.RegisterEntityHandler ("Pencol.Sapling");
			modRegistrationData.RegisterEntityHandler ("Pencol.Fermenter");
			modRegistrationData.RegisterEntityHandler ("Pencol.Harvester");
			modRegistrationData.RegisterEntityHandler ("Pencol.Planter");
			modRegistrationData.RegisterEntityHandler ("Pencol.Soil");

			return modRegistrationData;
		}

		public override ModItemActionResults PerformItemAction(ModItemActionParameters parameters)
		{
		ModItemActionResults result = new ModItemActionResults();
		if (ModItemManager.GetKeyfromItemId (parameters.ItemToUse.mnItemID).Equals ("Pencol.Compost"))
			result.Consume =UseCompost ();

		if (ModItemManager.GetKeyfromItemId (parameters.ItemToUse.mnItemID).Equals ("Pencol.Pruner"))
			UsePruner();	
			return result;
		}

		public void UsePruner()
		{
			long selectBlockX = WorldScript.instance.localPlayerInstance.mPlayerBlockPicker.selectBlockX;
			long selectBlockY = WorldScript.instance.localPlayerInstance.mPlayerBlockPicker.selectBlockY;
			long selectBlockZ = WorldScript.instance.localPlayerInstance.mPlayerBlockPicker.selectBlockZ;

		if (ModTerrainManager.GetCube (selectBlockX,selectBlockY, selectBlockZ) == 16) 
		{
			WorldScript.instance.localPlayerInstance.mBuilder.BuildOrientation (selectBlockX, selectBlockY, selectBlockZ, 1, 0, 65);
			if (rand.Next (1, 10) == 4) {

				Segment s = WorldScript.instance.GetSegment (selectBlockX, selectBlockY, selectBlockZ);
				WorldScript.instance.BuildFromEntity (s, selectBlockX, selectBlockY, selectBlockZ, 1, 0);
				DroppedItemData droppedItemData = ItemManager.DropNewCubeStack (ModTerrainManager.GetCubeFromKey ("Pencol.Sapling"), 0, 1, selectBlockX, selectBlockY, selectBlockZ, Vector3.zero);
				if (droppedItemData != null) {
					droppedItemData.mrLifeRemaining *= 10f;
				}
			}
		}
		}
		
		public bool UseCompost()
		{
			long selectBlockX = WorldScript.instance.localPlayerInstance.mPlayerBlockPicker.selectBlockX;
			long selectBlockY = WorldScript.instance.localPlayerInstance.mPlayerBlockPicker.selectBlockY;
			long selectBlockZ = WorldScript.instance.localPlayerInstance.mPlayerBlockPicker.selectBlockZ;
			
		if (ModTerrainManager.GetCube (selectBlockX, selectBlockY, selectBlockZ) == 4 ||
		     ModTerrainManager.GetCube (selectBlockX, selectBlockY, selectBlockZ) == 21) {
			WorldScript.instance.localPlayerInstance.mBuilder.BuildOrientation (selectBlockX, selectBlockY, selectBlockZ, ModTerrainManager.GetCubeFromKey ("Pencol.Soil"), 0, 65);
			Segment s = WorldScript.instance.GetSegment (selectBlockX, selectBlockY, selectBlockZ);
			return true;
		}
		return false;
		}

		public override ModCreateSegmentEntityResults CreateSegmentEntity( ModCreateSegmentEntityParameters parameters)
		{
			ModCreateSegmentEntityResults result = new ModCreateSegmentEntityResults ();

			foreach (ModCubeMap cubeMap in ModManager.mModMappings.CubeTypes)
			{
				if (cubeMap.CubeType == parameters.Cube) {
					if (cubeMap.Key.Equals ("Pencol.Sapling"))
						result.Entity = new Sapling (parameters.Segment, parameters.X, parameters.Y, parameters.Z, parameters.Cube, parameters.Flags, parameters.Value, parameters.LoadFromDisk);
					if (cubeMap.Key.Equals ("Pencol.Fermenter"))
						result.Entity = new Fermenter (parameters.Segment, parameters.X, parameters.Y, parameters.Z, parameters.Cube, parameters.Flags, parameters.Value, parameters.LoadFromDisk);
					if (cubeMap.Key.Equals ("Pencol.Harvester"))
						result.Entity = new Harvester (parameters.Segment, parameters.X, parameters.Y, parameters.Z, parameters.Cube, parameters.Flags, parameters.Value, parameters.LoadFromDisk);
					if (cubeMap.Key.Equals ("Pencol.Planter"))
						result.Entity = new Planter (parameters.Segment, parameters.X, parameters.Y, parameters.Z, parameters.Cube, parameters.Flags, parameters.Value, parameters.LoadFromDisk);
					if (cubeMap.Key.Equals ("Pencol.Sapling"))
						result.Entity = new Sapling (parameters.Segment, parameters.X, parameters.Y, parameters.Z, parameters.Cube, parameters.Flags, parameters.Value, parameters.LoadFromDisk);
					if (cubeMap.Key.Equals ("Pencol.Soil"))
						result.Entity = new Soil (parameters.Segment, parameters.X, parameters.Y, parameters.Z, parameters.Cube, parameters.Flags, parameters.Value, parameters.LoadFromDisk);
				}
			}
			return result;
		}

}
