#include "CSRPInteractableActor.h"
#include "CSRPPlayerState.h"
#include "GameFramework/Controller.h"
#include "Net/UnrealNetwork.h"
#include "Misc/DateTime.h"

ACSRPInteractableActor::ACSRPInteractableActor()
{
    bReplicates = true;
    NetUpdateFrequency = 5.0f;
}

bool ACSRPInteractableActor::CanInteract(const AActor* Interactor) const
{
    if (!HasAuthority() || !IsValid(Interactor))
        return false;

    return FVector::DistSquared(GetActorLocation(), Interactor->GetActorLocation())
        <= FMath::Square(InteractionRadius);
}

bool ACSRPInteractableActor::ExecuteInteraction(AController* InstigatingController)
{
    if (!HasAuthority() || !IsValid(InstigatingController))
        return false;

    APawn* Pawn = InstigatingController->GetPawn();
    ACSRPPlayerState* PlayerState = InstigatingController->GetPlayerState<ACSRPPlayerState>();

    if (!CanInteract(Pawn) || !PlayerState || PlayerState->bCitizenProgramCompleted)
        return false;

    if (PlayerState->CitizenMissionIndex != RequiredMissionIndex)
        return false;

    // Mission-specific progression remains server-authoritative.
    const int32 CompletedMission = PlayerState->CitizenMissionIndex;
    PlayerState->CitizenMissionIndex = FMath::Clamp(CompletedMission + 1, 0, 10);

    // PDF-aligned New Citizen Program rewards.
    if (CompletedMission == 3)
        PlayerState->bStarterVehicleUnlocked = true;

    if (CompletedMission == 4)
        PlayerState->CurrentJob = FName(TEXT("StarterCivilianJob"));

    if (CompletedMission == 9)
    {
        PlayerState->bCitizenProgramCompleted = true;
        PlayerState->Money += 10000;
        PlayerState->bStarterVehicleUnlocked = true;
        PlayerState->bRentalHomeUnlocked = true;
        PlayerState->RentalHomeExpiryUnix = (FDateTime::UtcNow() + FTimespan::FromDays(7)).ToUnixTimestamp();
        PlayerState->bOfficialJobUnlocked = true;
    }

    return true;
}

void ACSRPInteractableActor::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeReplicatedProps) const
{
    Super::GetLifetimeReplicatedProps(OutLifetimeReplicatedProps);
    DOREPLIFETIME(ACSRPInteractableActor, InteractionId);
    DOREPLIFETIME(ACSRPInteractableActor, DisplayName);
    DOREPLIFETIME(ACSRPInteractableActor, RequiredMissionIndex);
}
