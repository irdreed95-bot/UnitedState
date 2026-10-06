#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerState.h"
#include "CSRPPlayerState.generated.h"

UCLASS()
class CORRUPTSTATERP_API ACSRPPlayerState : public APlayerState
{
    GENERATED_BODY()

public:
    ACSRPPlayerState();

    UPROPERTY(Replicated, BlueprintReadOnly, Category="Citizen")
    int32 Money = 2500;

    UPROPERTY(Replicated, BlueprintReadOnly, Category="Citizen")
    int32 BankBalance = 10000;

    UPROPERTY(Replicated, BlueprintReadOnly, Category="Citizen")
    int32 CitizenMissionIndex = 0;

    UPROPERTY(Replicated, BlueprintReadOnly, Category="Citizen")
    FName CurrentJob = TEXT("Unemployed");

    UPROPERTY(Replicated, BlueprintReadOnly, Category="Citizen")
    bool bCitizenProgramCompleted = false;

    virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;
};
