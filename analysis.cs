/* 
  This code is only for function call analysis
  Don't include this code in project. XD
*/

// method: LoadPermanentBundles
string[] array = new string[]
			{
				"gui",
				"bg",
				"cg",
				"monika",
				"yuri",
				"natsuki",
				"sayori",
				"bgm-coarse"
			};
// Check bundles contains key
if (!this.m_ActiveAssetBundles.ContainsKey(text))
{
    this.LoadBundleSync(text);
    this.m_ActiveBundles.ForceAdd(text);
}

-> method: LoadBundleSync 
// Execute bundle load from "load" string
AssetBundle assetBundle = ActiveAssetBundles.PerformLoadBundle(load);

-> method: PerformLoadBundle
string platformForAssetBundles = PathHelpers.GetPlatformForAssetBundles(Application.platform);
// Platform could be one of: OSX, Windows, iOS, Android, PS4, XBoxOne, Switch
string text = Path.Combine(streamingAssetsPath, string.Concat(new string[]
{
    "AssetBundles/",
    platformForAssetBundles,
    "/",
    label,
    ".cy"
}));
// Combine path example: "AssetBundles/Windows/cg.cy"
AssetBundle assetBundle = AssetBundle.LoadFromStream(new XorFileStream(text, FileMode.Open, FileAccess.Read, 40));
return assetBundle;

// Load bundle from stream in AssetBundle class
// The XorFileStream method indicates the stream load method
// Auguments are: path, FileMode, FileAccess, privateKey=40
// using a XOR read stream like:
private void PerformXor(ref byte[] array, int offset, int count)
{
    for (int i = offset; i < offset + count; i++)
    {
        array[i] ^= this.m_Key;
    }
}

// Load from stream calls LoadFromStreamInternal
public static AssetBundle LoadFromStream(Stream stream)
{
    AssetBundle.ValidateLoadFromStream(stream);
    return AssetBundle.LoadFromStreamInternal(stream, 0U, 0U);
}
// This method is from ".\Managed\UnityEngine.AssetBundleModule.dll"
internal static extern AssetBundle LoadFromStreamInternal(Stream stream, uint crc, uint managedReadBufferSize);

// Then using Load method to load data from bundle
public T Load<T>(string bundleName, string path) where T : Object
{
	this.ValidateLoad(bundleName, path);
	return this.m_ActiveAssetBundles[bundleName].LoadAsset<T>(path);
}

[NativeThrows]
[NativeMethod("LoadAsset_Internal")]
[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
[MethodImpl(MethodImplOptions.InternalCall)]
private extern Object LoadAsset_Internal(string name, Type type);
