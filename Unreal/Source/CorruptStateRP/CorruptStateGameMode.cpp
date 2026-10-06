#include "CorruptStateGameMode.h"
#include "CSRPCharacter.h"
#include "CSRPPlayerState.h"
#include "GameFramework/PlayerController.h"

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
