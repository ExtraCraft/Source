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

		public override void LowFrequencyUpdate ()
		{
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
	//		global::GameManager.DoLocalChat("[MOD] Tried to use" + ModItemManager.GetKeyfromItemId(parameters.ItemToUse.mnItemID) );
		if (ModItemManager.GetKeyfromItemId (parameters.ItemToUse.mnItemID).Equals ("Pencol.Compost"))
		{
			
			result.Consume =UseCompost ();
		}
		
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

		#region Handbooks
/*
		public void AddAutoHarvesterHandBook()
		{
			string text = "<MaterialEntry>\n\t<Name>Auto Harvester</Name>\n\t<Pages>\n\t\t<Page>\n\t\t\t<Paragraphs>\n\t\t\t\t<Title>\n\t\t\t\t\t<Text>Auto Harvester</Text>\n\t\t\t\t</Title>\n\t\t\t\t<Icon>\n\t\t\t\t\t<Name>Bound Metal Crate</Name>\n\t\t\t\t\t<Alignment>Center</Alignment>\n\t\t\t\t</Icon>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>Make a square of Chevron Signs and put the machine on top.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>It will automaticly harvest wood and leaves for you.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>The machine decreases the amount of leaves you get but gives a small chance to get a sapling instead!.</Text>\n\t\t\t\t</Paragraph>\t\t\t\t\n\t\t\t\n\t\t\t</Paragraphs>\n\t\t</Page>\t\t\t\t\t\t\t\n\t</Pages>\n</MaterialEntry>\n\n";
			Handbook.MaterialEntry materialEntry = (Handbook.MaterialEntry)XMLParser.DeserializeObject(text, typeof(Handbook.MaterialEntry));
			materialEntry.CubeType = gac;
			materialEntry.CubeValue = harvesterEntityId;
			ModHandbookManager.AddNewMaterialEntry(materialEntry);
		}

		public void AddAutoPlanterHandbook()
		{
			string text = "<MaterialEntry>\n\t<Name>Auto Planter</Name>\n\t<Pages>\n\t\t<Page>\n\t\t\t<Paragraphs>\n\t\t\t\t<Title>\n\t\t\t\t\t<Text>Auto Planter</Text>\n\t\t\t\t</Title>\n\t\t\t\t<Icon>\n\t\t\t\t\t<Name>MK1 Robot Arm</Name>\n\t\t\t\t\t<Alignment>Center</Alignment>\n\t\t\t\t</Icon>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>Make a square of Chevron Signs and put the machine on top.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>It will automaticly plant saplings on top of fermented soil for you.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t</Paragraphs>\n\t\t</Page>\t\t\t\t\t\t\t\n\t</Pages>\n</MaterialEntry>\n\n";
			Handbook.MaterialEntry materialEntry = (Handbook.MaterialEntry)XMLParser.DeserializeObject(text, typeof(Handbook.MaterialEntry));
			materialEntry.CubeType = gac;
			materialEntry.CubeValue = planterEntityId;
			ModHandbookManager.AddNewMaterialEntry(materialEntry);
		}	

		public void AddSaplingHandBook ()
		{
			string text = "<MaterialEntry>\n\t<Name>Sapling</Name>\n\t<Pages>\n\t\t<Page>\n\t\t\t<Paragraphs>\n\t\t\t\t<Title>\n\t\t\t\t\t<Text>Sapling</Text>\n\t\t\t\t</Title>\n\t\t\t\t<Icon>\n\t\t\t\t\t<Name>Toxic Cavern Flora 6</Name>\n\t\t\t\t\t<Alignment>Center</Alignment>\n\t\t\t\t</Icon>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>The sapling will grow trees once they planted on fermented soil.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>For it to grow it need minerals that can be found only on fermented soil.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>The planting is manually at the beggining but it can be automated using the Auto Planter.</Text>\n\t\t\t\t</Paragraph>\t\t\t\t\n\t\t\t\n\t\t\t</Paragraphs>\n\t\t</Page>\t\t\t\t\t\t\t\n\t</Pages>\n</MaterialEntry>\n\n";
			Handbook.MaterialEntry materialEntry = (Handbook.MaterialEntry)XMLParser.DeserializeObject(text, typeof(Handbook.MaterialEntry));
			materialEntry.CubeType = gac;
			materialEntry.CubeValue = saplingEntityId;
			ModHandbookManager.AddNewMaterialEntry(materialEntry);
		}

		public void AddFermentedSoilHandbook()
		{
			string text = "<MaterialEntry>\n\t<Name>Fermented Soil</Name>\n\t<Pages>\n\t\t<Page>\n\t\t\t<Paragraphs>\n\t\t\t\t<Title>\n\t\t\t\t\t<Text>Fermented Soil</Text>\n\t\t\t\t</Title>\n\t\t\t\t<Icon>\n\t\t\t\t\t<Name>Dirt</Name>\n\t\t\t\t\t<Alignment>Center</Alignment>\n\t\t\t\t</Icon>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>Lets you plant saplings on top of them!.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>The minerals they contain to grow trees is limited, but can be replenished using an Auto Fermenter.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t</Paragraphs>\n\t\t</Page>\t\t\t\t\t\t\t\n\t</Pages>\n</MaterialEntry>\n\n";
			Handbook.MaterialEntry materialEntry = (Handbook.MaterialEntry)XMLParser.DeserializeObject(text, typeof(Handbook.MaterialEntry));
			materialEntry.CubeType = gac;
			materialEntry.CubeValue = fermentedSoilId;
			ModHandbookManager.AddNewMaterialEntry(materialEntry);
		}

		public void AddAutoFermenterHandbook()
		{
			string text = "<MaterialEntry>\n\t<Name>Auto Fermenter</Name>\n\t<Pages>\n\t\t<Page>\n\t\t\t<Paragraphs>\n\t\t\t\t<Title>\n\t\t\t\t\t<Text>Auto Fermenter</Text>\n\t\t\t\t</Title>\n\t\t\t\t<Icon>\n\t\t\t\t\t<Name>Geological Surveyor</Name>\n\t\t\t\t\t<Alignment>Center</Alignment>\n\t\t\t\t</Icon>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>Lets you replenish minerals of Fermented Soil around it.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>Uses compost to replenish minerals around.</Text>\n\t\t\t\t</Paragraph>\n\t\t\t\t<Spacer/>\n\t\t\t</Paragraphs>\n\t\t</Page>\t\t\t\t\t\t\t\n\t</Pages>\n</MaterialEntry>\n\n";
			Handbook.MaterialEntry materialEntry = (Handbook.MaterialEntry)XMLParser.DeserializeObject(text, typeof(Handbook.MaterialEntry));
			materialEntry.CubeType = gac;
			materialEntry.CubeValue = autoFermenterId;
			ModHandbookManager.AddNewMaterialEntry(materialEntry);
		}

		public void AddComposterHandbook()
		{
			string text = "<MaterialEntry>\n\t<Name>Composter</Name>\n\t<Pages>\n\t\t<Page>\n\t\t\t<Paragraphs>\n\t\t\t\t<Title>\n\t\t\t\t\t<Text>Composter</Text>\n\t\t\t\t</Title>\n\t\t\t\t<Icon>\n\t\t\t\t\t<Name>Organic Reassembler</Name>\n\t\t\t\t\t<Alignment>Center</Alignment>\n\t\t\t\t</Icon>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>Lets you turn leaves into Compost</Text>\n\t\t\t\t</Paragraph>\n\t\t\t</Paragraphs>\n\t\t</Page>\t\t\t\t\t\t\t\n\t</Pages>\n</MaterialEntry>\n\n";
			Handbook.MaterialEntry materialEntry = (Handbook.MaterialEntry)XMLParser.DeserializeObject(text, typeof(Handbook.MaterialEntry));
			materialEntry.CubeType = gac;
			materialEntry.CubeValue = compostEntityId;
			ModHandbookManager.AddNewMaterialEntry(materialEntry);
		}

		public void AddCompostHandbook()
		{
			string text = "<MaterialEntry>\n\t<Name>Compost</Name>\n\t<Pages>\n\t\t<Page>\n\t\t\t<Paragraphs>\n\t\t\t\t<Title>\n\t\t\t\t\t<Text>Compost</Text>\n\t\t\t\t</Title>\n\t\t\t\t<Icon>\n\t\t\t\t\t<Name>Biomass Growth Ping</Name>\n\t\t\t\t\t<Alignment>Center</Alignment>\n\t\t\t\t</Icon>\n\t\t\t\t<Paragraph>\n\t\t\t\t\t<Text>Created with Composter to use in an Auto Fermenter</Text>\n\t\t\t\t</Paragraph>\n\t\t\t</Paragraphs>\n\t\t</Page>\t\t\t\t\t\t\t\n\t</Pages>\n</MaterialEntry>\n\n";
			Handbook.MaterialEntry materialEntry = (Handbook.MaterialEntry)XMLParser.DeserializeObject(text, typeof(Handbook.MaterialEntry));
			materialEntry.ItemType = compostItemID;
			ModHandbookManager.AddNewMaterialEntry(materialEntry);
		}

	*/
		#endregion

		public void AddMoreTreesCraftCategory()
		{
			CraftingCategory spawnItemsCategory = new CraftingCategory("moretrees","Toxic Cavern Flora 6","More Trees");
			ModCraftManager.AddNewCraftingCategory(spawnItemsCategory);
		}





}
