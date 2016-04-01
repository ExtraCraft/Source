using System;
using System.IO;
using UnityEngine;

public class Harvester : global::MachineEntity, PowerConsumerInterface
{

	private bool mbLinkedToGO;

	private float mrUnityActiveTime;

	public string PopUpString;

	public float mrDelayTimer=0;

	public System.Random rand;

	public float mrCurrentPower;

	public float mrMaxPower = 4096f;

	public float mrMaxTransferRate = 1024f;


	private StorageHopper[] maAttachedHoppers;

	public int mnNumValidAttachedHoppers;

	public int mnNumInvalidAttachedHoppers;


	//animation stuff

	private GameObject Turret;

	private GameObject ShootEffect;

	private GameObject RangeEffect;

	private GameObject StandModel;

	private GameObject GunModel;

	private bool mbDoShootEffect;

	private float mDistanceToTarget;

	private float ShootEffectTimer=0;

	private int mnUnityUpdates;

	private float mrAimingTime;

	private float mrSpinTimer;

	private float mrSpinRate;

	private float SwingTimer;
	private float mrLowPowerTime =0;

	private Vector3 mVectorToTarget;

	public Harvester(global::Segment segment, long x, long y, long z, ushort cube, byte flags, ushort lValue, bool loadFromDisk) : base(global::eSegmentEntity.ResearchAssembler,SpawnableObjectEnum.PopUpTurret_T2, x, y, z, cube, flags, lValue, Vector3.zero, segment)
	{
		rand = new System.Random ();
		this.maAttachedHoppers = new StorageHopper[6];
		PopUpString = "";
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
			if (this.mWrapper == null) {
				return;
			}
			if (this.mWrapper.mGameObjectList == null) {
				return;
			}
			if (this.mWrapper.mGameObjectList.Count == 0) {
				return;
			}
			if (this.mWrapper.mGameObjectList [0].gameObject == null) {
				UnityEngine.Debug.LogError ("Sentry Gun missing game object #0 (GO)?");
			}
			this.Turret = this.mWrapper.mGameObjectList [0].gameObject.transform.Search ("SentryGun3").gameObject;
			this.ShootEffect = this.mWrapper.mGameObjectList [0].gameObject.transform.Search ("ShootEffect").gameObject;
			this.RangeEffect = this.mWrapper.mGameObjectList [0].gameObject.transform.Search ("RangeEffect").gameObject;
			this.StandModel = this.mWrapper.mGameObjectList [0].gameObject.transform.Search ("SentryGunStand1").gameObject;
			this.GunModel = this.mWrapper.mGameObjectList [0].gameObject.transform.Search ("SentryGun3").gameObject;
			this.mbLinkedToGO = true;
			this.mbDoShootEffect = false;
			this.ShootEffect.SetActive (false);
			this.RangeEffect.SetActive (false);
			return;
		} else {
			bool flag = false;
			if (this.mSegment.mbOutOfView)
			{
				flag = true;
			}
			if (this.mDotWithPlayerForwards < -8f)
			{
				flag = true;
			}
			if (flag)
			{
				this.StandModel.GetComponent<Renderer>().enabled = false;
				this.GunModel.GetComponent<Renderer>().enabled = false;
				if (this.mValue == 2)
				{
					this.GunModel.transform.Search("Muzzle").GetComponent<Renderer>().enabled = false;
				}
				return;
			}
			this.StandModel.GetComponent<Renderer>().enabled = true;
			this.GunModel.GetComponent<Renderer>().enabled = true;
			if (this.mValue == 2)
			{
				this.GunModel.transform.Search("Muzzle").GetComponent<Renderer>().enabled = true;
			}
			if (this.mrLowPowerTime > 1f && this.ShootEffectTimer <= 0f)
			{
				this.mrAimingTime = 0f;
				this.Turret.transform.forward += (Vector3.forward + Vector3.down - this.Turret.transform.forward) * Time.deltaTime;
				UnityEngine.Debug.DrawRay(this.Turret.transform.position, this.Turret.transform.forward * 32f, Color.cyan, 0.5f);
			}
			else if (this.ShootEffectTimer <= 0f && !this.mbDoShootEffect)
			{
				if (this.mDistanceToPlayer < 64f)
				{
					this.mrSpinTimer -= Time.deltaTime;
					if (this.mrSpinTimer > 0f)
					{
						this.Turret.transform.Rotate(0f, this.mrSpinRate * Time.deltaTime, 0f);
					}
					Vector3 forward = this.Turret.transform.forward;
					forward.y *= 0.95f;
					this.Turret.transform.forward = forward;
					if (this.mrSpinTimer < -3f)
					{
						this.mrSpinRate = (float)UnityEngine.Random.Range(-48, 48);
						this.mrSpinTimer = (float)UnityEngine.Random.Range(1, 10);
					}
				}
				this.mrAimingTime = 0f;
			}
			else
			{
				float d = 0.6f;
				if (this.mrAimingTime > 0.75f)
				{
					d = 0.5f;
					Debug.DrawRay(this.Turret.transform.position, this.Turret.transform.forward * 32f, Color.green, 0.2f);
				}
				else
				{
					Debug.DrawRay(this.Turret.transform.position, this.Turret.transform.forward * 32f, Color.red, 2f);
				}
				this.Turret.transform.forward += (this.mVectorToTarget - this.Turret.transform.forward) * Time.deltaTime * d;
				this.mrAimingTime += Time.deltaTime;
			}
			if (this.mbDoShootEffect && UnityEngine.Random.Range(0, 10) == 0 && this.mrAimingTime > 0.75f)
			{
				if (this.mDistanceToPlayer < 32f)
				{
					this.mWrapper.mGameObjectList[0].GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.8f, 1.2f);
					this.mWrapper.mGameObjectList[0].GetComponent<AudioSource>().volume = 0.3f;
					this.mWrapper.mGameObjectList[0].GetComponent<AudioSource>().Play();
				}
				this.mbDoShootEffect = false;
				this.ShootEffect.SetActive(true);
				float num = (this.mDistanceToTarget - 0.5f) / 20f;
				this.ShootEffect.transform.localScale = new Vector3(1f, 1f, this.mDistanceToTarget / 10f);
				this.ShootEffectTimer = 0.1f;
				Debug.DrawRay(this.Turret.transform.position, this.Turret.transform.forward * 32f, Color.red, 2f);
				Debug.DrawRay(this.Turret.transform.position, this.mVectorToTarget * 32f, Color.green, 2f);
			}
			if (this.ShootEffectTimer > 0f)
			{
				this.ShootEffectTimer -= Time.deltaTime;
				if (this.ShootEffectTimer <= 0f)
				{
					this.ShootEffect.SetActive(false);
					this.mrAimingTime = 0f;
				}
			}
			if (this.ShootEffectTimer <= 0f && this.ShootEffect.activeSelf)
			{
				Debug.LogWarning("Error, turret had shoot timer of <0 but the gameobject was active?");
				this.ShootEffect.SetActive(false);
			}
		}

	}

	public override void LowFrequencyUpdate()
	{
		int xMin = 0, xMax = 0, zMin = 0, zMax = 0;
		string whatsbroken = string.Empty;
		int harvestMaxSize = 6;
		bool foundBroken = false;
		bool foundSecondHarvester = false;
		bool brokeBlock = false;
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
						if (ModTerrainManager.GetCube (this.mnX + xMin, this.mnY, this.mnZ + z2) == ModTerrainManager.GetCubeFromKey ("Pencol.Harvester")) {
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
						if (ModTerrainManager.GetCube (this.mnX + xMax, this.mnY, this.mnZ + z2) == ModTerrainManager.GetCubeFromKey ("Pencol.Harvester")) {
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
						if (ModTerrainManager.GetCube (this.mnX + x2, this.mnY, this.mnZ + zMax) == ModTerrainManager.GetCubeFromKey ("Pencol.Harvester")) {
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
						if (ModTerrainManager.GetCube (this.mnX + x2, this.mnY, this.mnZ + zMin) == ModTerrainManager.GetCubeFromKey ("Pencol.Harvester")) {
							foundSecondHarvester = true;
							break;
						}
					}
				#endregion
				if (foundSecondHarvester) {
					PopUpString = "Found a second harvester!\n\n\n\n\n\n\n";
				} else {
					if (this.mrCurrentPower >= 20) {
						PopUpString = string.Format ("Waiting for trees!\nPower : {0:N0} / {1:N0}\n" + mrDelayTimer, (int)this.mrCurrentPower, this.mrMaxPower);


						if (mrDelayTimer > 0.5) {
							mrDelayTimer = 0;
							for (int y = 10; y >= 0; y--) {
								if (brokeBlock)
									continue;
								for (int x = xMin; x <= xMax; x++) {
									if (brokeBlock)
										continue;
									for (int z = zMin; z <= zMax; z++) {
										if (brokeBlock)
											continue;
										
										Vector3 zero2 = Vector3.zero;
										zero2.x = x;
										zero2.y = y ;
										zero2.z = z ;
										this.mVectorToTarget = zero2;
				
										if (ModTerrainManager.GetCube (this.mnX + x, this.mnY + y, this.mnZ + z) == 16) {
											//break the leaves
											this.mbDoShootEffect = true;
											this.UpdateAttachedHoppers (true);
											if (this.mnNumValidAttachedHoppers > 0) {
												for (int i = 0; i < mnNumValidAttachedHoppers; i++) {
													ItemCubeStack Leaf = new ItemCubeStack (16, 0, 1);
													if (rand.Next (0, 20) > 9) {
														if (!maAttachedHoppers [i].AddItem (Leaf)) {
															PopUpString = "No place to storage!\n\n\n\n\n\n\n";
														}
													}
													if (rand.Next (0, 22) == 10) {
														ItemCubeStack Sapling = new ItemCubeStack (ModTerrainManager.GetCubeFromKey ("Pencol.Sapling"), 0, 1);
														if (!maAttachedHoppers [i].AddItem (Sapling)) {
															PopUpString = "No place to storage!\n\n\n\n\n\n\n";
														}

													}
													Segment s = WorldScript.instance.GetSegment (this.mnX + x, this.mnY + y, this.mnZ + z);
													WorldScript.instance.BuildFromEntity (s, this.mnX + x, this.mnY + y, this.mnZ + z, 1, 0);


													this.mrCurrentPower -= 20;
													brokeBlock = true;
													break;


												}
											} else {
												PopUpString = "No place to storage!\n\n\n\n\n\n\n";
											}
										}
										if (brokeBlock)
											continue;
										if (ModTerrainManager.GetCube (this.mnX + x, this.mnY + y, this.mnZ + z) == 14) {
											//break the logs!

											//break the leaves
											this.mbDoShootEffect = true;
											this.UpdateAttachedHoppers (true);
											if (mnNumValidAttachedHoppers > 0) {
												for (int i = 0; i < mnNumValidAttachedHoppers; i++) {
													ItemCubeStack Log = new ItemCubeStack (14, 0, 1);
													if (maAttachedHoppers [i].AddItem (Log)) {
														//	global::GameManager.DoLocalChat("[MOD] adding log should have worked");
														Segment s = WorldScript.instance.GetSegment (this.mnX + x, this.mnY + y, this.mnZ + z);
														WorldScript.instance.BuildFromEntity (s, this.mnX + x, this.mnY + y, this.mnZ + z, 1, 0);
														this.mrCurrentPower -= 20;
														brokeBlock = true;
														break;
													} else {
														PopUpString = "No place to storage!\n\n\n\n\n\n\n";
													}
												}
											} else {
												PopUpString = "No place to storage!\n\n\n\n\n\n\n";
											}
										}
									}
								}

							}
						}
						mrDelayTimer += LowFrequencyThread.mrPreviousUpdateTimeStep;
					} else {
						PopUpString = string.Format ("Need more power!\nPower : {0:N0} / {1:N0}\n\n\n\n\n\n\n\n", (int)this.mrCurrentPower, this.mrMaxPower);
					}
				}
			} else {
				PopUpString= "The cevron around is not full\n" + xMin.ToString () + "/" + xMax.ToString () +
					"\n" + zMin.ToString () + "/" + zMax.ToString () + "\n" + whatsbroken + "\n\n\n\n\n\n\n\n";
			}

			} else {
				PopUpString = "there is no chevron below the machine\n\n\n\n\n\n\n";
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
		return PopUpString;
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
