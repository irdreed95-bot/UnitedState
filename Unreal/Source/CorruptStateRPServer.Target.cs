using UnrealBuildTool;
using System.Collections.Generic;

public class CorruptStateRPServerTarget : TargetRules
{
    public CorruptStateRPServerTarget(TargetInfo Target) : base(Target)
    {
        Type = TargetType.Server;
        DefaultBuildSettings = BuildSettingsVersion.V5;
        IncludeOrderVersion = EngineIncludeOrderVersion.Latest;
        ExtraModuleNames.Add("CorruptStateRP");
    }
}
