# Unreal Asset Acquisition Policy

## Visual target

Corrupt State RP is an open-world mobile RP game. The environment must look like a real modern city, not a primitive/arcade blockout.

The uploaded Corrupt State RP design PDF remains the gameplay/world-layout source of truth.

## Large-city source assessment

### Manhattan Fully Textured (Free3D)

The supplied Manhattan Fully Textured model is useful as a visual reference for city density and scale, but it is **not approved as a production asset yet**.

Reason:
- The Free3D listing currently labels the download **Personal Use License**.
- The listing describes a 1:1 Manhattan scene with buildings and textures and provides Blender/FBX files.
- Its description says textures originate from various sites, including textures.com and CGTrader.

Therefore we must not put this asset into the shipped Corrupt State RP project unless a license permitting our intended game use and redistribution is verified for the complete asset and its textures.

### Manhattan Sketchfab mirror

A Sketchfab upload of the same Manhattan model is listed as CC Attribution and reports about 259.8k triangles. It is a useful candidate for visual comparison, but its description also says the textures come from third-party sources. Treat it as **review required**, not automatically cleared.

### Epic City Sample

The preferred large-city reference/source for Unreal is Epic Games' free **City Sample / City Sample Buildings** ecosystem. It provides realistic modular city buildings, vehicles and crowds, and is explicitly licensed for Unreal Engine-based products. The City Sample is built around World Partition, Nanite, Mass AI and procedural city techniques.

We should use selected City Sample assets/modules where compatible with our project and target device, rather than copying the entire heavy showcase city wholesale.

## Source priority

Preferred order:
1. Official Epic/Fab assets with a license compatible with Unreal projects
2. CC0 / public-domain models
3. CC BY models with attribution recorded in the project
4. Other licenses only after manual license review

Never copy models from GTA, FiveM, Roblox, ripped games, or unknown sources without explicit redistribution rights.

## Realistic asset categories

The production asset pass must cover the PDF requirements with actual meshes/materials, not placeholder cubes:

- roads and sidewalks
- street furniture, lamps, signs and traffic props
- central government / civil services
- government headquarters
- bank
- court
- police station
- hospital / EMS
- university
- driving school
- vehicle dealership
- mechanic / garage
- apartments and residential buildings
- commercial downtown
- rich residential district
- poor residential district
- gang district
- coast / beach
- forest
- mountains
- desert prison
- military base
- port
- vehicles
- playable character
- civilian/NPC characters
- Arabic street and location signage

## Performance rules

For mobile:
- Prefer modular buildings and reusable materials.
- Prefer reasonable triangle counts and texture sizes.
- Use World Partition and HLOD for the large world.
- Use Nanite only where appropriate for the actual target platforms; do not assume Nanite solves mobile performance.
- Generate appropriate LODs and collision.
- Reuse materials and meshes aggressively.
- Do not import a giant monolithic city mesh as the gameplay map.
- Use the large city only as a structural/reference source; assemble the playable world from streamed districts and modular assets.

## Acceptance rule

A district is not considered visually complete until it contains believable 3D buildings/props matching its PDF role and can be viewed in Unreal without placeholder cube geometry.

The final city should feel like a real modern RP city with distinct government, commercial, residential, gang, coast, port, forest, mountain, military and prison areas.
