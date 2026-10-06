#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "CorruptStateGameMode.generated.h"

UCLASS()
class CORRUPTSTATERP_API ACorruptStateGameMode : public AGameModeBase
{
    GENERATED_BODY()

public:
    ACorruptStateGameMode();
    virtual void PostLogin(APlayerController* NewPlayer) override;
    virtual void StartPlay() override;
};
