#include "CSRPInteractableActor.h"
#include "CSRPPlayerState.h"
#include "GameFramework/Controller.h"
#include "Net/UnrealNetwork.h"

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

    PlayerState->CitizenMissionIndex = FMath::Clamp(PlayerState->CitizenMissionIndex + 1, 0, 10);
    PlayerState->bCitizenProgramCompleted = (PlayerState->CitizenMissionIndex >= 10);

    if (PlayerState->bCitizenProgramCompleted)
        PlayerState->Money += 10000;

    return true;
}

void ACSRPInteractableActor::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeReplicatedProps) const
{
    Super::GetLifetimeReplicatedProps(OutLifetimeReplicatedProps);
    DOREPLIFETIME(ACSRPInteractableActor, InteractionId);
    DOREPLIFETIME(ACSRPInteractableActor, DisplayName);
    DOREPLIFETIME(ACSRPInteractableActor, RequiredMissionIndex);
}
