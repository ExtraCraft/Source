using System;
using System.IO;
using UnityEngine;

public class FirePit : global::MachineEntity
{

	private bool mbLinkedToGO;

	private float mrUnityActiveTime;

	public bool mbAttackActive;

	public float mrCurrentThreat;

	public float mrNextAttackTime;


	public FirePit(global::Segment segment, long x, long y, long z, ushort cube, byte flags, ushort lValue, bool loadFromDisk) : base(global::eSegmentEntity.ResearchAssembler,global::SpawnableObjectEnum.Alien_Plant_Toxic_6, x, y, z, cube, flags, lValue, Vector3.zero, segment)
	{
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
		return "Testing!";
	}
}
