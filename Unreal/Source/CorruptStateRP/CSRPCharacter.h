#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "CSRPCharacter.generated.h"

UCLASS()
class CORRUPTSTATERP_API ACSRPCharacter : public ACharacter
{
    GENERATED_BODY()

public:
    ACSRPCharacter();
    virtual void SetupPlayerInputComponent(UInputComponent* PlayerInputComponent) override;

private:
    void MoveForward(float Value);
    void MoveRight(float Value);

    UFUNCTION(Server, Reliable)
    void ServerInteract();

    void Interact();
};
