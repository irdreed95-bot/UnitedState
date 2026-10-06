#pragma once

#include "CoreMinimal.h"

struct FCSRPCityDistrict
{
    FName Id;
    const TCHAR* Name;
    FVector Center;
    FVector Extent;
};

struct FCSRPCitizenMissionLocation
{
    const TCHAR* Id;
    const TCHAR* Name;
    FVector Location;
    int32 RequiredMissionIndex;
};

class FCSRPCityLayout
{
public:
    static const TArray<FCSRPCityDistrict>& GetDistricts();
    static const TArray<FCSRPCitizenMissionLocation>& GetCitizenMissionLocations();
};
