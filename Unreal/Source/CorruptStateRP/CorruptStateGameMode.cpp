#include "CorruptStateGameMode.h"
#include "CSRPCharacter.h"
#include "CSRPPlayerState.h"
#include "CSRPWorldBootstrap.h"
#include "GameFramework/PlayerController.h"
#include "Engine/World.h"

ACorruptStateGameMode::ACorruptStateGameMode()
{
    DefaultPawnClass = ACSRPCharacter::StaticClass();
    PlayerStateClass = ACSRPPlayerState::StaticClass();
}

void ACorruptStateGameMode::PostLogin(APlayerController* NewPlayer)
{
    Super::PostLogin(NewPlayer);

    if (NewPlayer)
    {
        UE_LOG(LogTemp, Log, TEXT("CSRP: authoritative player joined: %s"), *NewPlayer->GetName());
    }
}

void ACorruptStateGameMode::StartPlay()
{
    Super::StartPlay();

    if (!GetWorld())
        return;

    if (GetWorld()->GetAuthGameMode() == this)
    {
        FActorSpawnParameters Params;
        Params.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;

        const FVector BootstrapLocation = FVector::ZeroVector;
        GetWorld()->SpawnActor<ACSRPWorldBootstrap>(
            ACSRPWorldBootstrap::StaticClass(),
            BootstrapLocation,
            FRotator::ZeroRotator,
            Params);
    }
}
