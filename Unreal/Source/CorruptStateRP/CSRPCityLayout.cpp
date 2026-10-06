#include "CSRPCityLayout.h"

const TArray<FCSRPCityDistrict>& FCSRPCityLayout::GetDistricts()
{
    static const TArray<FCSRPCityDistrict> Districts =
    {
        {FName(TEXT("downtown")), TEXT("وسط المدينة والمركز التجاري"), FVector(0, 0, 0), FVector(4500, 3500, 800)},
        {FName(TEXT("government")), TEXT("الحكومة والمؤسسات المدنية"), FVector(0, 5000, 0), FVector(2600, 1800, 700)},
        {FName(TEXT("banking")), TEXT("القطاع المالي والبنوك"), FVector(3500, 2600, 0), FVector(1800, 1400, 700)},
        {FName(TEXT("rich_residential")), TEXT("الحي الراقي"), FVector(6500, 2500, 0), FVector(3000, 2200, 700)},
        {FName(TEXT("poor_residential")), TEXT("الحي الشعبي"), FVector(-5200, 1800, 0), FVector(3000, 2200, 700)},
        {FName(TEXT("gang")), TEXT("منطقة العصابات"), FVector(-6500, -2800, 0), FVector(2800, 2400, 700)},
        {FName(TEXT("coast")), TEXT("الواجهة الساحلية"), FVector(1000, -6500, 0), FVector(5200, 1700, 700)},
        {FName(TEXT("port")), TEXT("الميناء"), FVector(-4200, -6200, 0), FVector(2400, 1700, 700)},
        {FName(TEXT("forest")), TEXT("الغابة"), FVector(8500, -5000, 0), FVector(4200, 3000, 900)},
        {FName(TEXT("mountains")), TEXT("الجبال"), FVector(10500, 3500, 0), FVector(3800, 4200, 1800)},
        {FName(TEXT("military")), TEXT("القاعدة العسكرية"), FVector(-10500, 6200, 0), FVector(3600, 2600, 900)},
        {FName(TEXT("prison")), TEXT("سجن الصحراء"), FVector(-10500, -7000, 0), FVector(3000, 2200, 900)}
    };

    return Districts;
}

const TArray<FCSRPCitizenMissionLocation>& FCSRPCityLayout::GetCitizenMissionLocations()
{
    static const TArray<FCSRPCitizenMissionLocation> Missions =
    {
        {TEXT("national_id"), TEXT("استخراج الهوية الوطنية"), FVector(0, 5000, 120), 0},
        {TEXT("bank_account"), TEXT("فتح الحساب البنكي"), FVector(3500, 2600, 120), 1},
        {TEXT("driving_license"), TEXT("استخراج رخصة القيادة"), FVector(2800, 900, 120), 2},
        {TEXT("first_vehicle"), TEXT("شراء أول مركبة"), FVector(4800, 900, 120), 3},
        {TEXT("first_job"), TEXT("الحصول على أول وظيفة"), FVector(-1200, 1400, 120), 4},
        {TEXT("government_visit"), TEXT("زيارة المؤسسات الحكومية"), FVector(1400, 6100, 120), 5},
        {TEXT("lawful_citizen"), TEXT("الالتزام بالقانون"), FVector(-900, 3600, 120), 6},
        {TEXT("city_tour"), TEXT("التعرف على المدينة"), FVector(0, -3000, 120), 7},
        {TEXT("community"), TEXT("التفاعل مع المجتمع"), FVector(-5200, 1800, 120), 8},
        {TEXT("future"), TEXT("اختيار المستقبل"), FVector(6500, 2500, 120), 9}
    };

    return Missions;
}
