using System;
using System.IO;
using UnityEngine;

public class Fermenter : global::MachineEntity, PowerConsumerInterface
{

	private bool mbLinkedToGO;

	private float mrUnityActiveTime;

	public bool mbAttackActive;

	public float mrCurrentThreat;

	public float mrNextAttackTime;

	public string PopUpString;

	public float mrCurrentPower;

	public float mrMaxPower = 4096f;

	public float mrMaxTransferRate = 1024f;


	private StorageHopper[] maAttachedHoppers;

	public int mnNumValidAttachedHoppers;

	public int mnNumInvalidAttachedHoppers;


	public Fermenter(global::Segment segment, long x, long y, long z, ushort cube, byte flags, ushort lValue, bool loadFromDisk) : base(global::eSegmentEntity.ResearchAssembler,SpawnableObjectEnum.GeologicalSurveyor, x, y, z, cube, flags, lValue, Vector3.zero, segment)
	{
		this.maAttachedHoppers = new StorageHopper[6];
		PopUpString= "Starting";
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
		int xMin = 0, xMax = 0, zMin = 0, zMax = 0;
		string whatsbroken = string.Empty;
		int harvestMaxSize = 6;
		bool foundBroken = false;
		bool foundSecondHarvester = false;
		bool fermentedDirt = false;

		if (ModTerrainManager.GetCube (this.mnX, this.mnY - 1, this.mnZ) == 124) {


		if (Chevrons.CheckSquareSize (this.mnX, this.mnY, this.mnZ, harvestMaxSize, out xMin, out  xMax, out  zMin, out  zMax)) {
			#region Check Size

			if (Math.Abs (xMax - xMin) > harvestMaxSize || Math.Abs (zMax - zMin) > harvestMaxSize) {
				foundBroken = true;
				whatsbroken = "The square is too big!";
			}
			if (Math.Abs (xMax - xMin) < 3 || Math.Abs (zMax - zMin) < 3) {
				foundBroken = true;
				whatsbroken = "The square is too small!";
			}
			#endregion

			#region Check If Full
			//Lets chcek if full
			if (!foundBroken)
				for (int z2 = zMin; z2 < zMax; z2++) {
					if (ModTerrainManager.GetCube (this.mnX + xMin, this.mnY - 1, this.mnZ + z2) != 124) {
						foundBroken = true;
						whatsbroken = "On xMin " + z2.ToString ();
						break;
					}
					if (xMin == 0 && z2 == 0)
						continue;
					if (ModTerrainManager.GetCube (this.mnX + xMin, this.mnY, this.mnZ + z2) == ModTerrainManager.GetCubeFromKey ("Pencol.Fermenter")) {
						foundSecondHarvester = true;
						break;
					}
				}
			if (!foundBroken)
				for (int z2 = zMin; z2 < zMax; z2++) {
					if (ModTerrainManager.GetCube (this.mnX + xMax, this.mnY - 1, this.mnZ + z2) != 124) {
						foundBroken = true;
						whatsbroken = "On xMax " + z2.ToString ();
						break;
					}
					if (xMax == 0 && z2 == 0)
						continue;
					if (ModTerrainManager.GetCube (this.mnX + xMax, this.mnY, this.mnZ + z2) == ModTerrainManager.GetCubeFromKey ("Pencol.Fermenter")) {
						foundSecondHarvester = true;
						break;
					}
				}

			if (!foundBroken)
				for (int x2 = xMin; x2 < xMax; x2++) {

					if (ModTerrainManager.GetCube (this.mnX + x2, this.mnY - 1, this.mnZ + zMax) != 124) {
						foundBroken = true;
						whatsbroken = "On zMax " + x2.ToString ();
						break;
					}
					if (zMax == 0 && x2 == 0)
						continue;
					if (ModTerrainManager.GetCube (this.mnX + x2, this.mnY, this.mnZ + zMax) == ModTerrainManager.GetCubeFromKey ("Pencol.Fermenter")) {
						foundSecondHarvester = true;
						break;
					}
				}
			if (!foundBroken)
				for (int x2 = xMin; x2 < xMax; x2++) {
					if (ModTerrainManager.GetCube (this.mnX + x2, this.mnY - 1, this.mnZ + zMin) != 124) {
						foundBroken = true;
						whatsbroken = "On xMax " + x2.ToString ();
						break;
					}
					if (zMin == 0 && x2 == 0)
						continue;
					if (ModTerrainManager.GetCube (this.mnX + x2, this.mnY, this.mnZ + zMin) == ModTerrainManager.GetCubeFromKey ("Pencol.Fermenter")) {
						foundSecondHarvester = true;
						break;
					}
				}
			#endregion
			if (foundSecondHarvester) {
				PopUpString = "Found a second fermenter!\n\n\n\n\n\n\n";
			} else {
				UpdateAttachedHoppers (false);
				int sap = this.CountItem (ModItemManager.GetItemIDFromKey ("Pencol.Compost"));
					PopUpString = "Waiting for hopper!!\n\n\n\n\n\n\n\n";
				if ((long)sap >= 1) {
					if (mrCurrentPower >= 20) {
						PopUpString = string.Format ("Waiting for a spot to ferment \nPower : {0:N0} / {1:N0}\n\n\n\n\n\n\n\n", (int)mrCurrentPower, mrMaxPower);
						for (int x = xMin; x <= xMax; x++) {
							if (fermentedDirt)
								continue;
							for (int z = zMin; z <= zMax; z++) {
								if (fermentedDirt)
									continue;
								if (ModTerrainManager.GetCube (this.mnX + x, this.mnY - 1, this.mnZ + z) == 4) {
									if (ModTerrainManager.GetCube (this.mnX + x, this.mnY, this.mnZ + z) == 1) {
										//plant
										//take a sampling
										UpdateAttachedHoppers (false);
										if ((long)this.CountItem (ModItemManager.GetItemIDFromKey ("Pencol.Compost")) >= 1) {
											Segment s = WorldScript.instance.GetSegment (this.mnX + x, this.mnY - 1, this.mnZ + z);
											this.CollectFromAttachedHoppers (ModItemManager.GetItemIDFromKey ("Pencol.Compost"), 1);
											WorldScript.instance.BuildOrientationFromEntity (s, this.mnX + x, this.mnY - 1, this.mnZ + z, ModTerrainManager.GetCubeFromKey ("Pencol.Soil"), 0, 1);
											mrCurrentPower -= 20;
											fermentedDirt = true;
											break;
										}	

									}
								}
							}

						}
					} else {
						PopUpString = string.Format ("Need more power!\nPower : {0:N0} / {1:N0}\n\n\n\n\n\n\n\n", (int)mrCurrentPower, mrMaxPower);
					}
				}
			}
		} else {
			PopUpString= "The cevron around is not full\n" + xMin.ToString () + "/" + xMax.ToString () +
				"\n" + zMin.ToString () + "/" + zMax.ToString () + "\n" + whatsbroken + "\n\n\n\n\n\n\n\n";
		}
		}else {
			PopUpString = "there is no chevron below the machine\n\n\n\n\n\n\n";
		}
	}	
	private int CountItem(int lnItemID)
	{
		int num = 0;
		for (int i = 0; i < this.mnNumValidAttachedHoppers; i++)
		{
			if (this.maAttachedHoppers[i].mPermissions != StorageHopper.ePermissions.Locked)
			{
				if (this.maAttachedHoppers[i].mPermissions != StorageHopper.ePermissions.AddOnly)
				{
					num += this.maAttachedHoppers[i].CountHowManyOfItem(lnItemID);
				}
			}
		}
		return num;
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
		return PopUpString;
	}

	private int CountCubes(ushort lCube, ushort lValue)
	{
		int num = 0;
		for (int i = 0; i < this.mnNumValidAttachedHoppers; i++)
		{
			if (this.maAttachedHoppers[i].mPermissions != StorageHopper.ePermissions.Locked)
			{
				if (this.maAttachedHoppers[i].mPermissions != StorageHopper.ePermissions.AddOnly)
				{
					num += this.maAttachedHoppers[i].CountHowManyOfType(lCube, lValue);
				}
			}
		}
		return num;
	}



	private void CollectFromAttachedHoppers(int lnItemID, int lnMinCount)
	{
		for (int i = 0; i < this.mnNumValidAttachedHoppers; i++)
		{
			if (this.maAttachedHoppers[i].mPermissions != StorageHopper.ePermissions.Locked)
			{
				if (this.maAttachedHoppers[i].mPermissions != StorageHopper.ePermissions.AddOnly)
				{
					int num = this.maAttachedHoppers[i].CountHowManyOfItem(lnItemID);
					if (num != 0)
					{
						int num2 = num;
						if (num2 > lnMinCount)
						{
							num2 = lnMinCount;
						}
						for (int j = 0; j < num2; j++)
						{
							this.maAttachedHoppers[i].RemoveInventoryItem(lnItemID, 1);
						}
						lnMinCount -= num2;
						if (lnMinCount == 0)
						{
							return;
						}
						if (lnMinCount < 0)
						{
							Debug.LogError("Error, GAC removed too many " + ItemEntry.GetNameFromID(lnItemID) + "!");
						}
					}
				}
			}
		}
	}


	private void UpdateAttachedHoppers(bool lbInput)
	{
		int num = 0;
		this.mnNumInvalidAttachedHoppers = 0;
		this.mnNumValidAttachedHoppers = 0;
		for (int i = 0; i < 6; i++)
		{
			long num2 = this.mnX;
			long num3 = this.mnY;
			long num4 = this.mnZ;
			if (i == 0)
			{
				num2 -= 1L;
			}
			if (i == 1)
			{
				num2 += 1L;
			}
			if (i == 2)
			{
				num3 -= 1L;
			}
			if (i == 3)
			{
				num3 += 1L;
			}
			if (i == 4)
			{
				num4 -= 1L;
			}
			if (i == 5)
			{
				num4 += 1L;
			}
			Segment segment = base.AttemptGetSegment(num2, num3, num4);
			if (segment != null)
			{
				ushort cube = segment.GetCube(num2, num3, num4);
				if (cube == 505)
				{
					this.mnNumInvalidAttachedHoppers++;
					StorageHopper storageHopper = segment.FetchEntity(eSegmentEntity.StorageHopper, num2, num3, num4) as StorageHopper;
					if (storageHopper != null)
					{
						if (storageHopper.mPermissions != StorageHopper.ePermissions.Locked)
						{
							if (lbInput || storageHopper.mPermissions != StorageHopper.ePermissions.AddOnly)
							{
								if (!lbInput || storageHopper.mPermissions != StorageHopper.ePermissions.RemoveOnly)
								{
									if (!lbInput || storageHopper.mnStorageFree > 0)
									{
										if (lbInput || storageHopper.mnStorageUsed != 0)
										{
											this.maAttachedHoppers[num] = storageHopper;
											this.mnNumInvalidAttachedHoppers--;
											num++;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		this.mnNumValidAttachedHoppers = num;
	}

	public override bool ShouldSave()
	{
		return true;
	}

	public override void Write(BinaryWriter writer)
	{
		int value = -1;
		writer.Write(this.mrCurrentPower);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
		writer.Write(value);
	}

	public override void Read(BinaryReader reader, int entityVersion)
	{
		this.mrCurrentPower = reader.ReadSingle();
		int num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
		num = reader.ReadInt32();
	}

	public float GetRemainingPowerCapacity()
	{
		return this.mrMaxPower - this.mrCurrentPower;
	}

	public float GetMaximumDeliveryRate()
	{
		return this.mrMaxTransferRate;
	}

	public float GetMaxPower()
	{
		return this.mrMaxPower;
	}

	public bool DeliverPower(float amount)
	{
		if (amount > this.GetRemainingPowerCapacity())
		{
			return false;
		}
		this.mrCurrentPower += amount;
		this.MarkDirtyDelayed();
		return true;
	}

	public bool WantsPowerFromEntity(SegmentEntity entity)
	{
		return true;
	}
}
