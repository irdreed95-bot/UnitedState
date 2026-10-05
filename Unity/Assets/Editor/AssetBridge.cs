#if UNITY_EDITOR
using UnityEditor;
namespace CorruptStateRP.Editor {
public static class AssetBridge {
 [UnityEditor.InitializeOnLoadMethod] static void Configure(){EditorApplication.delayCall+=()=>AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);}
}
}
#endif
