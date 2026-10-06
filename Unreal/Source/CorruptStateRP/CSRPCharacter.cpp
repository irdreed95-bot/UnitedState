#include "CSRPCharacter.h"
#include "CSRPInteractableActor.h"
#include "EngineUtils.h"
#include "GameFramework/CharacterMovementComponent.h"

ACSRPCharacter::ACSRPCharacter()
{
    bReplicates = true;
    SetReplicateMovement(true);
    NetUpdateFrequency = 30.0f;

    GetCharacterMovement()->MaxWalkSpeed = 520.0f;
    GetCharacterMovement()->MaxAcceleration = 1800.0f;
    GetCharacterMovement()->BrakingDecelerationWalking = 1600.0f;
}

void ACSRPCharacter::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
    Super::SetupPlayerInputComponent(PlayerInputComponent);
    PlayerInputComponent->BindAxis(TEXT("MoveForward"), this, &ACSRPCharacter::MoveForward);
    PlayerInputComponent->BindAxis(TEXT("MoveRight"), this, &ACSRPCharacter::MoveRight);
    PlayerInputComponent->BindAction(TEXT("Interact"), IE_Pressed, this, &ACSRPCharacter::Interact);
}

void ACSRPCharacter::MoveForward(float Value)
{
    if (!FMath::IsNearlyZero(Value))
        AddMovementInput(GetActorForwardVector(), Value);
}

void ACSRPCharacter::MoveRight(float Value)
{
    if (!FMath::IsNearlyZero(Value))
        AddMovementInput(GetActorRightVector(), Value);
}

void ACSRPCharacter::Interact()
{
    if (HasAuthority())
        ServerInteract_Implementation();
    else
        ServerInteract();
}

void ACSRPCharacter::ServerInteract_Implementation()
{
    if (!HasAuthority())
        return;

    ACSRPInteractableActor* BestTarget = nullptr;
    float BestDistanceSq = TNumericLimits<float>::Max();

    for (TActorIterator<ACSRPInteractableActor> It(GetWorld()); It; ++It)
    {
        ACSRPInteractableActor* Candidate = *It;
        if (!IsValid(Candidate) || !Candidate->CanInteract(this))
            continue;

        const float DistanceSq = FVector::DistSquared(GetActorLocation(), Candidate->GetActorLocation());
        if (DistanceSq < BestDistanceSq)
        {
            BestDistanceSq = DistanceSq;
            BestTarget = Candidate;
        }
    }

    if (BestTarget)
        BestTarget->ExecuteInteraction(GetController());
}
