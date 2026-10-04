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
