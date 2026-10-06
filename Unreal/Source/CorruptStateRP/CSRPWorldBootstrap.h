#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CSRPWorldBootstrap.generated.h"

UCLASS()
class CORRUPTSTATERP_API ACSRPWorldBootstrap : public AActor
{
    GENERATED_BODY()

public:
    ACSRPWorldBootstrap();

protected:
    virtual void BeginPlay() override;

private:
    void SpawnCitizenProgramInteractions();
};
