using System;
using System.IO;
using UnityEngine;
public class Sapling : global::MachineEntity
{

	private bool mbLinkedToGO;

	public System.Random rand;

	public string PopUpText;

	public float mrDelayTimer=0;

	public bool mbConfiguredValue;

	public Sapling(global::Segment segment, long x, long y, long z, ushort cube, byte flags, ushort lValue, bool loadFromDisk) : base(global::eSegmentEntity.ResearchAssembler,global::SpawnableObjectEnum.Alien_Plant_Toxic_6, x, y, z, cube, flags, lValue, Vector3.zero, segment)
	{
		mbConfiguredValue = false;
		rand = new System.Random ();
		PopUpText = "Waiting to growww";
		this.mbNeedsLowFrequencyUpdate = true;
		this.mbNeedsUnityUpdate = true;
	}

	public override void DropGameObject()
	{
		base.DropGameObject();
		this.mbLinkedToGO = false;
	}

	public override void UnityUpdate()
	{
		if (!this.mbLinkedToGO) {
			if (this.mWrapper == null || !this.mWrapper.mbHasGameObject) {
				return;
			}
			if (this.mWrapper.mGameObjectList == null) {
				UnityEngine.Debug.LogError ("TS missing game object #0?");
			}
			if (this.mWrapper.mGameObjectList [0].gameObject == null) {
				UnityEngine.Debug.LogError ("TS missing game object #0 (GO)?");
			}
				
			mWrapper.mGameObjectList [0].gameObject.transform.Rotate (-90f, 0, 0); 
			mWrapper.mGameObjectList [0].gameObject.transform.localPosition =
				new Vector3(
					mWrapper.mGameObjectList [0].gameObject.transform.localPosition .x,
					mWrapper.mGameObjectList [0].gameObject.transform.localPosition .y-0.5f,
					mWrapper.mGameObjectList [0].gameObject.transform.localPosition .z);
			this.mbLinkedToGO = true;
		}
	}
	public override void LowFrequencyUpdate()
	{

		Segment s = WorldScript.instance.GetSegment (this.mnX, this.mnY - 1, this.mnZ);
		if (ModTerrainManager.GetCube (this.mnX, this.mnY - 1, this.mnZ) == ModTerrainManager.GetCubeFromKey("Pencol.Soil")) {
			SegmentEntity crafter = s.SearchEntity (this.mnX, this.mnY - 1, this.mnZ);
			if (crafter is Soil) {

				Soil fermentSoil = (Soil)crafter;
				PopUpText = "Waiting to grow" + mrDelayTimer ;
				if (mrDelayTimer > 10) {
						if (fermentSoil.mrCurrentFerment >= 20) {
						if (rand.Next (1, 10) == 5) 
							if (Trees.CanSpawnTree (1, this.mnX, this.mnY, this.mnZ, false, true)) {
									Trees.SpawnTree (1, this.mnX, this.mnY, this.mnZ);
									fermentSoil.mrCurrentFerment -= 20;
								}
						} else {
							PopUpText = "Not enought minerals on soil";
						}
					mrDelayTimer = 0;
				}
			}
		} else {
			//break sapling
			s = WorldScript.instance.GetSegment (this.mnX, this.mnY, this.mnZ);
			WorldScript.instance.BuildFromEntity (s, this.mnX, this.mnY, this.mnZ, 1, 0);
			DroppedItemData droppedItemData = ItemManager.DropNewCubeStack (ModTerrainManager.GetCubeFromKey("Pencol.Sapling"), 0, 1, this.mnX, this.mnY, this.mnZ, Vector3.zero);
			if (droppedItemData != null) {
				droppedItemData.mrLifeRemaining *= 10f;
			}
	
		}
		mrDelayTimer +=LowFrequencyThread.mrPreviousUpdateTimeStep;
	}

	public override global::HoloMachineEntity CreateHolobaseEntity(global::Holobase holobase)
	{
		global::HolobaseEntityCreationParameters holobaseEntityCreationParameters = new global::HolobaseEntityCreationParameters(this);
		global::HolobaseVisualisationParameters holobaseVisualisationParameters = holobaseEntityCreationParameters.AddVisualisation(holobase.mPreviewCube);
		holobaseVisualisationParameters.Color = Color.green;
		return holobase.CreateHolobaseEntity(holobaseEntityCreationParameters);
	}
	public override string GetPopupText() 
	{
		return PopUpText;
	}
}
