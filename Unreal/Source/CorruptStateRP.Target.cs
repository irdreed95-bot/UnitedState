using UnrealBuildTool;
using System.Collections.Generic;

public class CorruptStateRPTarget : TargetRules
{
    public CorruptStateRPTarget(TargetInfo Target) : base(Target)
    {
        Type = TargetType.Game;
        DefaultBuildSettings = BuildSettingsVersion.V5;
        IncludeOrderVersion = EngineIncludeOrderVersion.Latest;
        ExtraModuleNames.Add("CorruptStateRP");
    }
}
