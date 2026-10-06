#include "CSRPWorldBootstrap.h"
#include "CSRPInteractableActor.h"
#include "Engine/World.h"

ACSRPWorldBootstrap::ACSRPWorldBootstrap()
{
    PrimaryActorTick.bCanEverTick = false;
}

void ACSRPWorldBootstrap::BeginPlay()
{
    Super::BeginPlay();

    if (HasAuthority())
        SpawnCitizenProgramInteractions();
}

void ACSRPWorldBootstrap::SpawnCitizenProgramInteractions()
{
    UWorld* World = GetWorld();
    if (!World)
        return;

    struct FCitizenStep
    {
        const TCHAR* Id;
        const TCHAR* Name;
        FVector Offset;
    };

    static const FCitizenStep Steps[] =
    {
        {TEXT("national_id"), TEXT("استخراج الهوية الوطنية"), FVector(0, 0, 0)},
        {TEXT("bank_account"), TEXT("فتح الحساب البنكي"), FVector(600, 0, 0)},
        {TEXT("driving_license"), TEXT("استخراج رخصة القيادة"), FVector(1200, 0, 0)},
        {TEXT("first_vehicle"), TEXT("شراء أول مركبة"), FVector(1800, 0, 0)},
        {TEXT("first_job"), TEXT("الحصول على أول وظيفة"), FVector(2400, 0, 0)},
        {TEXT("government_visit"), TEXT("زيارة المؤسسات الحكومية"), FVector(3000, 0, 0)},
        {TEXT("lawful_citizen"), TEXT("الالتزام بالقانون"), FVector(3600, 0, 0)},
        {TEXT("city_tour"), TEXT("التعرف على المدينة"), FVector(4200, 0, 0)},
        {TEXT("community"), TEXT("التفاعل مع المجتمع"), FVector(4800, 0, 0)},
        {TEXT("future"), TEXT("اختيار المستقبل"), FVector(5400, 0, 0)}
    };

    for (int32 Index = 0; Index < UE_ARRAY_COUNT(Steps); ++Index)
    {
        FActorSpawnParameters Params;
        Params.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;

        const FVector Location = GetActorLocation() + Steps[Index].Offset;
        ACSRPInteractableActor* Interaction = World->SpawnActor<ACSRPInteractableActor>(
            ACSRPInteractableActor::StaticClass(),
            Location,
            FRotator::ZeroRotator,
            Params);

        if (!Interaction)
            continue;

        Interaction->InteractionId = FName(Steps[Index].Id);
        Interaction->DisplayName = FText::FromString(Steps[Index].Name);
        Interaction->RequiredMissionIndex = Index;
        Interaction->InteractionRadius = 250.0f;
    }
}
