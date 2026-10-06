#include "CSRPPlayerState.h"
#include "Net/UnrealNetwork.h"

ACSRPPlayerState::ACSRPPlayerState()
{
    bReplicates = true;
    NetUpdateFrequency = 10.0f;
}

void ACSRPPlayerState::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{
    Super::GetLifetimeReplicatedProps(OutLifetimeProps);
    DOREPLIFETIME(ACSRPPlayerState, Money);
    DOREPLIFETIME(ACSRPPlayerState, BankBalance);
    DOREPLIFETIME(ACSRPPlayerState, CitizenMissionIndex);
    DOREPLIFETIME(ACSRPPlayerState, CurrentJob);
    DOREPLIFETIME(ACSRPPlayerState, bCitizenProgramCompleted);
    DOREPLIFETIME(ACSRPPlayerState, bStarterVehicleUnlocked);
    DOREPLIFETIME(ACSRPPlayerState, bRentalHomeUnlocked);
    DOREPLIFETIME(ACSRPPlayerState, RentalHomeExpiryUnix);
    DOREPLIFETIME(ACSRPPlayerState, bOfficialJobUnlocked);
}
