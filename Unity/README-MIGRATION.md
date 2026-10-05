# Corrupt State RP — Unity migration

The Godot project is retained as a rollback/reference copy. The active game runtime is being ported to Unity 6.

## Ported now
- 600x600 city foundation and road grid.
- Real 3D building/vehicle/citizen asset bridge using the same verified GLB sources.
- Police, hospital, bank, government, court, prison, garage, market, gang area and port landmarks.
- Trees, parks, street lights and parked vehicles.
- Third-person citizen controller.
- Vehicle controller and traffic manager.
- Arabic HUD and core money/bank/XP/health/hunger/thirst/wanted state.
- Save-state foundation.
- Jobs/factions foundation.
- Touch-control foundation.

## Build
Unity 6.0.63f1 is selected. Android APK is the only Unity target. Unity glTFast 6.14.1 is pinned for GLB support.

The first Unity CI attempt successfully downloaded all verified 3D assets, then stopped before opening/building the Unity project because the repository has no Unity license secret configured. No APK is claimed yet.

## Next
- Complete mobile controls and UI.
- Port the full interaction/job/faction/phone systems from Godot.
- Add building interiors, vehicles/garages/maintenance and stronger NPC AI.
- Add world streaming/LOD/occlusion and Android performance pass.
- Configure Unity Personal license for CI, then run the APK build and fix compile/runtime errors found by the real Unity build.
