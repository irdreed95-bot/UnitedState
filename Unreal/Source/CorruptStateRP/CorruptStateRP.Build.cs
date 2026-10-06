using UnrealBuildTool;

public class CorruptStateRP : ModuleRules
{
    public CorruptStateRP(ReadOnlyTargetRules Target) : base(Target)
    {
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

        PublicDependencyModuleNames.AddRange(new string[]
        {
            "Core",
            "CoreUObject",
            "Engine",
            "InputCore",
            "NetCore",
            "EnhancedInput"
        });
    }
}
