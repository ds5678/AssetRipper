﻿using AssetRipper.Assets.Metadata;
using AssetRipper.Checksum;
using AssetRipper.Import.Structure.Assembly;
using AssetRipper.Import.Structure.Assembly.Managers;
using AssetRipper.Import.Structure.Assembly.Serializable;
using AssetRipper.SourceGenerated.Classes.ClassID_1;
using AssetRipper.SourceGenerated.Classes.ClassID_111;
using AssetRipper.SourceGenerated.Classes.ClassID_115;
using AssetRipper.SourceGenerated.Classes.ClassID_4;
using AssetRipper.SourceGenerated.Classes.ClassID_90;
using AssetRipper.SourceGenerated.Classes.ClassID_95;
using AssetRipper.SourceGenerated.Extensions;

namespace AssetRipper.Processing.AnimationClips;

/// <summary>
/// Attempts to recover field paths from <see cref="uint"/> hash values.
/// </summary>
/// <remarks>
/// Replicates Unity CRC32 checksum usage for field names and paths.
/// </remarks>
public readonly struct PathChecksumCache
{
	public PathChecksumCache(IAssemblyManager assemblyManager)
	{
		this.assemblyManager = assemblyManager;
	}

	private readonly Dictionary<string, uint> cachedPropertyNames = new();
	private readonly Dictionary<uint, string> cachedChecksums = new() { { 0, string.Empty } };
	private readonly HashSet<AssetInfo> processedAssets = new();
	private readonly IAssemblyManager assemblyManager;
	
	private void AddAnimatorPathsToCache(IAnimator animator)
	{
		IAvatar? avatar = animator.Avatar_C95P;
		if (avatar != null)
		{
			 AddAvatarTOS(avatar);
			 return;
		}
		
		if (animator.Has_HasTransformHierarchy_C95() && !animator.HasTransformHierarchy_C95)
		{
			return;
		}

		IGameObject gameObject = animator.GameObject_C95.GetAsset(animator.Collection);
		AddGameObjectPathsToCacheRecursive(gameObject, string.Empty);
	}

	private void AddGameObjectPathsToCacheRecursive(IGameObject parent, string parentPath)
	{
		ITransform transform = parent.GetTransform();

		foreach (ITransform? childTransform in transform.Children_C4P)
		{
			IGameObject child = childTransform?.GameObject_C4P ?? throw new NullReferenceException();

			string path = string.IsNullOrEmpty(parentPath)
				? child.NameString
				: $"{parentPath}/{child.NameString}";

			uint pathHash = Crc32Algorithm.HashUTF8(path);
			AddKeys(pathHash, path);

			AddGameObjectPathsToCacheRecursive(child, path);
		}
	}
	
	private void AddAnimationPathsToCache(IAnimation animation)
	{
		IGameObject go = animation.GameObject_C8.GetAsset(animation.Collection);
		AddGameObjectPathsToCacheRecursive(go, string.Empty);
	}
	
	private void AddAvatarTOS(IAvatar avatar)
	{
		foreach (uint key in avatar.TOS_C90.Keys)
		{
			AddKeys(key, avatar.TOS_C90[key]);
		}
	}
	
	public void BuildPathsCache(AnimationCache cache)
	{
		foreach (IAvatar avatar in cache.CachedAvatars)
		{
			AddAvatarTOS(avatar);
		}

		foreach (IAnimator animator in cache.CachedAnimators)
		{
			AddAnimatorPathsToCache(animator);
		}

		foreach (IAnimation animation in cache.CachedAnimations)
		{
			AddAnimationPathsToCache(animation);
		}
	}
	
	public uint Add(string path)
	{
		if (cachedPropertyNames.TryGetValue(path, out uint value))
		{
			return value;
		}

		uint output = Crc32Algorithm.HashUTF8(path);

		AddKeys(output, path);
		return output;
	}

	public void Add(IMonoScript script)
	{
		if (!processedAssets.Add(script.AssetInfo))
		{
			return;
		}

		SerializableType? behaviour = script.GetBehaviourType(assemblyManager);

		if (behaviour is null)
		{
			return;
		}

		for (int f = 0; f < behaviour.Fields.Count; f++)
		{
			SerializableType.Field field = behaviour.Fields[f];

			Add(field.Name);
		}
	}

	public bool TryGetPath(uint identifier, [NotNullWhen(true)] out string? path)
	{
		return cachedChecksums.TryGetValue(identifier, out path);
	}

	public void Reset()
	{
		cachedPropertyNames.Clear();
		cachedChecksums.Clear();
		processedAssets.Clear();
	}

	private void AddKeys(uint checksum, string propertyName)
	{
		cachedPropertyNames[propertyName] = checksum;
		cachedChecksums[checksum] = propertyName;
	}
}
