# Corrupt State RP — Unity 6

Production direction: **Unity 6 + C# + Android**, with the PDF design document as the primary gameplay/world reference.

## Source of truth
The project PDF defines the city layout, districts, Arabic signage, institutions, New Citizen Program, jobs, economy, RP activities and progression. The Unity implementation must follow that document rather than inventing unrelated systems.

## Engine
- Unity 6.0.63f1
- Android target
- URP/mobile-first rendering
- Unity glTFast for GLB/GLTF assets
- Server-authoritative multiplayer architecture planned from the start

## World quality
The target is a realistic open-world RP city:
- Realistic licensed buildings, vehicles, characters and environment assets.
- Arabic-first signage and street naming.
- Downtown/commercial, rich residential, poor residential, gang, government, coast, port, forest, mountain, prison and military areas.
- Airport, municipality, police, hospital, bank, court, university, driving school, dealerships, services and other PDF-defined landmarks.
- No Roblox-style primitive city and no copied GTA/FiveM/commercial-game assets.

## New Citizen Program
The ten PDF onboarding missions are the authoritative progression reference, including identity, bank, driving license, first vehicle, first job, government visits, lawful play, city discovery, social RP and final career selection/rewards.

## Build
The active CI workflow is `.github/workflows/unity-android.yml`. Unity license credentials are supplied through GitHub Actions secrets when building. No license file or secret is committed to the repository.

## Development rule
Before adding major systems or assets, review the current Unity project and the PDF requirements, preserve working code, and validate changes through the real Unity build before claiming an APK is ready.


## Current build
Unity 6 city foundation now includes a visible fallback citizen, expanded city streets, sidewalks, and street lighting.
