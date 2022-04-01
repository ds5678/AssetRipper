﻿using AssetRipper.Core.Classes.Misc;
using AssetRipper.Core.Classes.Misc.KeyframeTpl;
using AssetRipper.Core.Classes.Misc.Serializable.AnimationCurveTpl;
using AssetRipper.Core.Interfaces;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Parser.Files;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;
using System.Collections.Generic;

namespace AssetRipper.Core.Classes.AudioReverbFilter
{
	public sealed class AudioReverbFilter : AudioBehaviour
	{
		public AudioReverbFilter(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			ReverbPreset = (AudioReverbPreset)reader.ReadInt32();
			DryLevel = reader.ReadSingle();
			Room = reader.ReadSingle();
			RoomHF = reader.ReadSingle();
			DecayTime = reader.ReadSingle();
			DecayHFRatio = reader.ReadSingle();
			ReflectionsLevel = reader.ReadSingle();
			ReflectionsDelay = reader.ReadSingle();
			ReverbLevel = reader.ReadSingle();
			ReverbDelay = reader.ReadSingle();
			Diffusion = reader.ReadSingle();
			Density = reader.ReadSingle();
			HfReference = reader.ReadSingle();
			RoomLF = reader.ReadSingle();
			LfReference = reader.ReadSingle();
			if (HasroomRolloffFactor(reader.Version))
			{
				roomRolloffFactor = reader.ReadSingle();
			}
		}

		public static bool HasroomRolloffFactor(UnityVersion version) => version.IsLessEqual(5, 4);

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.AddSerializedVersion(1);
			node.Add("reverbPreset", (int)ReverbPreset);
			node.Add("DryLevel", DryLevel);
			node.Add("Room", Room);
			node.Add("RoomHF", RoomHF);
			node.Add("DecayTime", DecayTime);
			node.Add("DecayHFRatio", DecayHFRatio);
			node.Add("ReflectionsLevel", ReflectionsLevel);
			node.Add("ReflectionsDelay", ReflectionsDelay);
			node.Add("ReverbLevel", ReverbLevel);
			node.Add("ReverbDelay", ReverbDelay);
			node.Add("Diffusion", Diffusion);
			node.Add("Density", Density);
			node.Add("HfReference", HfReference);
			node.Add("RoomLF", RoomLF);
			node.Add("LfReference", LfReference);
			return node;
		}
		public AudioReverbPreset ReverbPreset { get; set; }
		public float DryLevel { get; set; }
		public float Room { get; set; }
		public float RoomHF { get; set; }
		public float DecayTime { get; set; }
		public float DecayHFRatio { get; set; }
		public float ReflectionsLevel { get; set; }
		public float ReflectionsDelay { get; set; }
		public float roomRolloffFactor { get; set; }
		public float ReverbLevel { get; set; }
		public float ReverbDelay { get; set; }
		public float Diffusion { get; set; }
		public float Density { get; set; }
		public float HfReference { get; set; }
		public float RoomLF { get; set; }
		public float LfReference { get; set; }
	}
}