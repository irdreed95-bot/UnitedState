# Corrupt State RP — No-PC Build Strategy

## Constraint
The project owner does not have a PC. Do not make the project depend on the owner installing Unreal Engine locally.

## What we can do from the repository
- Keep all gameplay, replication, world-layout, asset manifests, and build automation in GitHub.
- Run repository-level validation in GitHub Actions.
- Prepare the project so a Windows/Linux Unreal build runner can compile the UE5 project when an Unreal Engine runtime is supplied legally.
- Keep Godot preserved as a rollback/reference copy.

## What GitHub Actions cannot legally provide by itself
The standard GitHub runner does not include the proprietary Unreal Engine Editor/build toolchain. We must not download or redistribute an unofficial UE binary or a ripped engine image.

Therefore this workflow intentionally does NOT pretend to build UE5. It validates the project and reports exactly what remains blocked.

## Remote UE5 execution target
The required final cloud step is a remote Windows/Linux environment with:
1. Unreal Engine 5.x installed or otherwise legitimately available to the runner.
2. The repository checked out.
3. C++ toolchain available.
4. Android SDK/NDK configured when Android packaging is enabled.
5. Enough CPU/RAM/storage for UE5 and the project's realistic assets.

The owner should be able to operate the process from a phone; the heavy editor/build work happens remotely.

## Current milestone
Until a legitimate UE5 runtime is attached to a cloud runner, the repository is **not** marked compiled, playable, packaged, or production-ready.

The PDF remains the source of truth for world layout and gameplay locations. Production visuals must use realistic licensed assets; placeholder cube-city geometry is not an acceptable final result.
