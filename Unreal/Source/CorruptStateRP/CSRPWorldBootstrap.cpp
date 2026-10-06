#include "CSRPWorldBootstrap.h"
#include "CSRPInteractableActor.h"
#include "CSRPCityLayout.h"
#include "Engine/World.h"

ACSRPWorldBootstrap::ACSRPWorldBootstrap()
{
    PrimaryActorTick.bCanEverTick = false;
}

void ACSRPWorldBootstrap::BeginPlay()
{
    Super::BeginPlay();

    if (HasAuthority())
        SpawnCitizenProgramInteractions();
}

void ACSRPWorldBootstrap::SpawnCitizenProgramInteractions()
{
    UWorld* World = GetWorld();
    if (!World)
        return;

    const TArray<FCSRPCitizenMissionLocation>& Missions = FCSRPCityLayout::GetCitizenMissionLocations();

    for (const FCSRPCitizenMissionLocation& Mission : Missions)
    {
        FActorSpawnParameters Params;
        Params.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;

        const FVector Location = GetActorLocation() + Mission.Location;
        ACSRPInteractableActor* Interaction = World->SpawnActor<ACSRPInteractableActor>(
            ACSRPInteractableActor::StaticClass(),
            Location,
            FRotator::ZeroRotator,
            Params);

        if (!Interaction)
            continue;

        Interaction->InteractionId = FName(Mission.Id);
        Interaction->DisplayName = FText::FromString(Mission.Name);
        Interaction->RequiredMissionIndex = Mission.RequiredMissionIndex;
        Interaction->InteractionRadius = 250.0f;
    }
}
