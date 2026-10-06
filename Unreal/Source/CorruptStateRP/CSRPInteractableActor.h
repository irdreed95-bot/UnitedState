#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CSRPInteractableActor.generated.h"

UCLASS()
class CORRUPTSTATERP_API ACSRPInteractableActor : public AActor
{
    GENERATED_BODY()

public:
    ACSRPInteractableActor();

    UPROPERTY(EditAnywhere, BlueprintReadOnly, Replicated, Category="Interaction")
    FName InteractionId = NAME_None;

    UPROPERTY(EditAnywhere, BlueprintReadOnly, Replicated, Category="Interaction")
    FText DisplayName;

    UPROPERTY(EditAnywhere, BlueprintReadOnly, Replicated, Category="Interaction")
    int32 RequiredMissionIndex = 0;

    UPROPERTY(EditAnywhere, BlueprintReadOnly, Category="Interaction", meta=(ClampMin="50.0"))
    float InteractionRadius = 250.0f;

    bool CanInteract(const AActor* Interactor) const;
    bool ExecuteInteraction(AController* InstigatingController);

    virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeReplicatedProps) const override;
};
