using UnrealBuildTool;
using System.Collections.Generic;

public class CorruptStateRPEditorTarget : TargetRules
{
    public CorruptStateRPEditorTarget(TargetInfo Target) : base(Target)
    {
        Type = TargetType.Editor;
        DefaultBuildSettings = BuildSettingsVersion.V5;
        IncludeOrderVersion = EngineIncludeOrderVersion.Latest;
        ExtraModuleNames.Add("CorruptStateRP");
    }
}
