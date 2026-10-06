#include "CSRPCharacter.h"
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
        return;

    ServerInteract();
}

void ACSRPCharacter::ServerInteract_Implementation()
{
    // Server-only interaction entry point.
    // Batch 6 will bind this to validated world interaction actors.
}
