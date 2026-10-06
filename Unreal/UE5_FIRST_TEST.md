# Corrupt State RP — First Unreal Test

## Goal
Get the current Unreal foundation running before adding production assets.

## Required
- Unreal Engine 5.x installed locally.
- Open the repository project: `Unreal/CorruptStateRP.uproject`.
- Let Unreal compile the C++ module.

## Editor setup
1. Run `Unreal/Tools/BuildCSRPWorld.py` from the Unreal Editor Python environment.
2. Confirm a level exists at `/Game/Maps/CSRP_Main`.
3. Confirm the level contains **CSRP World Bootstrap**.
4. Set the editor Game Mode to `CorruptStateGameMode` if the project did not apply it automatically.
5. Press Play In Editor.

## Expected current behavior
- A replicated CSRP character is spawned by the game mode.
- Movement uses W/S/A/D.
- E sends the server-authoritative interaction request.
- The server creates the 10 Citizen Program interaction points from the PDF-aligned city registry.
- PlayerState values replicate to the client.
- Completing all 10 interactions grants the New Citizen rewards already implemented in PlayerState.

## Important
This is the first technical play test, not the final visual build. The realistic licensed environment pass comes next. The PDF remains the source of truth for districts and gameplay locations.

Do not mark the Unreal build production-ready until it has been compiled and tested in the installed UE5 version.
