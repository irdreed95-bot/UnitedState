# Corrupt State RP — Unity migration

The existing Godot project remains as a rollback/reference copy. This Unity project ports the working foundation instead of starting from zero.

Ported foundation:
- 600x600 city foundation and road grid.
- Existing real 3D building/vehicle/citizen asset paths.
- Police, hospital, bank, government, court, prison, garage, market, gang area and port landmarks.
- Trees, parks, street lights and parked vehicles.
- Third-person citizen controller and vehicle controller.
- Traffic manager.
- Arabic HUD and core money/bank/XP/health/hunger/thirst/wanted state.

Unity glTFast 6.14.1 is used for the existing GLB assets. Unity 6.0.63f1 supports Android builds; the build pipeline will remain APK-first.
