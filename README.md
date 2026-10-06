# Corrupt State RP

Godot 4.5.2 mobile-first open-world RP project.

## Real 3D foundation now implemented
- CharacterBody3D player controller with acceleration, gravity, turning and third-person camera.
- Procedural humanoid placeholder with AnimationPlayer + AnimationTree state-machine structure (Idle/Walk), ready for licensed GLB/GLTF character replacement.
- VehicleBody3D with four VehicleWheel3D wheels, steering, traction, suspension and braking.
- Modular city generated through GridMap + MeshLibrary rather than hundreds of unrelated building nodes.
- WorldEnvironment baseline with filmic tonemapping, glow and SSAO. SDFGI remains disabled for the Android/mobile renderer so the mobile build stays practical; it can be enabled in a separate high-end desktop profile.
- Arabic mobile HUD, missions, economy, factions, inventory and local save remain integrated.

## Asset policy
Use only original or properly licensed GLB/GLTF assets. Do not copy One State RP, GTA, or other commercial game assets.

## Production roadmap
1. Import licensed GLB/GLTF character + vehicle packs and connect real animations.
2. Expand the city into districts with interiors, traffic, pedestrians and navigation.
3. Add LOD/visibility/streaming budgets for a large mobile world.
4. Build the authoritative online backend: accounts, persistent economy, jobs, factions, chat/voice and admin systems.
5. Add advanced police/EMS/criminal gameplay, housing, businesses, vehicles, missions and events.


## Batch 1 — Real-world test foundation
- Expanded the playable city footprint to 600m x 600m.
- Added a street-detail pass with street lights.
- Added a traffic manager foundation for moving civilian vehicles.
- Added a reusable interaction base for future doors, shops, NPCs, vehicles and services.
- Vehicle foundation now supports engine state, headlights, horn state, repair state and runtime condition data.
- The first test build is still a development build; the final city will be expanded far beyond this footprint using streamed districts.

## Legal 3D asset sources under evaluation
- Quaternius Downtown City MegaKit — CC0, glTF/FBX/OBJ, Godot-compatible source versions.
- Quaternius Universal Base Characters — CC0, rigged characters.
- Quaternius Universal Animation Library — CC0, 120+ retargetable animations.
- Poly Haven — CC0 models, materials and HDRIs.
- Kenney Car Kit — CC0 vehicle models available in GLB/FBX/OBJ.

Asset sources are evaluated individually before inclusion. Commercial game assets from GTA, One State RP or other copyrighted games will not be copied.

## Batch 2 — PDF-aligned world regions
- Added a first visual region pass based directly on the project PDF: coastal beach/sea, forest settlement, mountain route, isolated desert prison, mountain military base, and a working port layout.
- Added Arabic district/location signage for the commercial center, rich district, poor district, gang district, coast, forest, military base and prison.
- Reused the licensed build-time GLB city and vehicle assets instead of relying only on primitive fallback geometry.
- Kept the existing Godot/mobile pipeline intact; this batch is focused on making the world visibly resemble the RP design before deeper gameplay systems are migrated.

### Batch 2 — Playable city core
- Built the central city grid around the PDF's citizen journey instead of a generic test scene.
- Added named Arabic streets and visible landmarks for Civil Affairs/Municipality, Central Bank, Court, Government, Police, Hospital, RP University, Driving School, Car Dealership, Mechanic Garage, Phone Shop, Restaurant and Media/Training buildings.
- Added commercial towers, residential districts (Rich/Poor/Gang), dealership vehicles and a first connected street network using the licensed road/building assets.
- Repositioned the forest/mountain layer so the regions remain inside the playable 600×600 world bounds.
- Updated the initial player/vehicle locations and landmark interaction coordinates to match the new city layout.
