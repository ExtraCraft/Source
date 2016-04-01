using System;
using System.IO;
using UnityEngine;

public class Soil : global::MachineEntity, BuildByItemInterface
{

	private bool mbLinkedToGO;

	private float mrUnityActiveTime;

	public float mrMaxFerment;

	public float mrDelayTimer=0;

	public float mrCurrentFerment;

	public Soil(global::Segment segment, long x, long y, long z, ushort cube, byte flags, ushort lValue, bool loadFromDisk) : base(global::eSegmentEntity.ResearchAssembler,SpawnableObjectEnum.PowerStorageBlock, x, y, z, cube, flags, lValue, Vector3.zero, segment)
	{
		mrCurrentFerment = 100;
		mrMaxFerment = 100;
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
		if (!this.mbLinkedToGO)
		{
			if (this.mWrapper == null || !this.mWrapper.mbHasGameObject)
			{
				return;
			}
			if (this.mWrapper.mGameObjectList == null)
			{
				Debug.LogError("TS missing game object #0?");
			}
			if (this.mWrapper.mGameObjectList[0].gameObject == null)
			{
				Debug.LogError("TS missing game object #0 (GO)?");
			}
			this.mbLinkedToGO = true;
			this.mrUnityActiveTime = 0f;
		}
		this.mrUnityActiveTime += Time.deltaTime;
	}

	public override void LowFrequencyUpdate()
	{
		if (mrCurrentFerment <= 0) {
			WorldScript.instance.localPlayerInstance.mBuilder.BuildOrientation (this.mnX, this.mnY, this.mnZ, 4, 0, 65);
		} else {
			mrDelayTimer += LowFrequencyThread.mrPreviousUpdateTimeStep;
		}
	}

	public override bool ShouldNetworkUpdate()
	{
		return true;
	}

	public override void ReadNetworkUpdate(BinaryReader reader)
	{
		base.ReadNetworkUpdate(reader);
	}

	public override void WriteNetworkUpdate(BinaryWriter writer)
	{
		base.WriteNetworkUpdate(writer);
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
		string percent = ( mrCurrentFerment/ mrMaxFerment).ToString("0.00%");
		return "The soil is " + percent + " fermented\n" +mrCurrentFerment.ToString() + "/" +  mrMaxFerment+"\n\n\n\n\n\n\n\n";
	}

	public override bool ShouldSave()
	{
		return true;
	}


	public void ApplyBuildItem(ItemBase item)
	{
		if (item.mType == ItemType.ItemCharge)
		{
			this.mrCurrentFerment = (item as ItemCharge).mChargeLevel;
			this.MarkDirtyDelayed();
			this.RequestImmediateNetworkUpdate();
		}
	}

	public override void OnDelete()
	{
		base.OnDelete();
		//ItemBase itemBase = ItemManager.SpawnItem(ModItemManager.GetItemIDFromKey("Pencol.SoilI"));
		//(itemBase as ItemCharge).mChargeLevel = this.mrCurrentFerment;
		//ItemManager.instance.DropItem(itemBase, this.mnX, this.mnY, this.mnZ, Vector3.zero);

	}
	public override void Write(BinaryWriter writer)
	{
		writer.Write(this.mrCurrentFerment);
		writer.Write(0);
		float value = 999f;
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
	}

	public override void Read(BinaryReader reader, int entityVersion)
	{
		this.mrCurrentFerment = reader.ReadSingle();
		reader.ReadSingle();
		reader.ReadSingle();
		reader.ReadSingle();
		reader.ReadSingle();
		reader.ReadSingle();
		reader.ReadSingle();
		reader.ReadSingle();
	}
}
