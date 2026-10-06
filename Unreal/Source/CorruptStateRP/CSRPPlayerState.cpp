#include "CSRPPlayerState.h"
#include "Net/UnrealNetwork.h"

ACSRPPlayerState::ACSRPPlayerState()
{
    bReplicates = true;
    NetUpdateFrequency = 10.0f;
}

void ACSRPPlayerState::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{
    Super::GetLifetimeReplicatedProps(OutLifetimeProperty);
    DOREPLIFETIME(ACSRPPlayerState, Money);
    DOREPLIFETIME(ACSRPPlayerState, BankBalance);
    DOREPLIFETIME(ACSRPPlayerState, CitizenMissionIndex);
    DOREPLIFETIME(ACSRPPlayerState, CurrentJob);
    DOREPLIFETIME(ACSRPPlayerState, bCitizenProgramCompleted);
}
