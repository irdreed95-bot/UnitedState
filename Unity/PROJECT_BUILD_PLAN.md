# Corrupt State RP — Unity 6 build plan

The supplied PDF is the primary design reference. The repository is now Unity 6 only; Unreal and Godot are removed from the active tree.

## Phase order
1. Runtime foundation: URP, Android, FPS 20–120, safe bootstrap, input abstraction.
2. Presentation: 4K splash, Arabic-first login, lobby, profile/XP, daily/weekly missions, shop and faction cards.
3. World: realistic licensed assets and PDF-aligned districts/landmarks; no primitive placeholder city in the production scene.
4. Character: third-person mobile controls, run, jump, crouch/cover, interaction, inventory, emotes.
5. Vehicles: enter/exit, doors, engine, throttle, brake, steering, lights, indicators, horn, seatbelt and wipers.
6. Factions: police, military, health, civil defense, justice, government, national security and gangs with server-authorized rank actions.
7. Admin: permission-gated search, jail, mute, kick, ban, teleport, free camera and macros.
8. Online backend: authentication, persistent economy, jobs, factions, chat/voice, anti-cheat and authoritative state.
9. Optimization: LOD, occlusion, streamed content, mobile quality tiers and FPS profiles.
10. Android release: IL2CPP, ARM64/ARMv7 as supported, signing, alignment and real-device testing.

## Asset policy
Mixamo, Sketchfab, CGTrader, TurboSquid and Unity Asset Store are sources to evaluate individually. A listing alone is not proof that a specific asset may be redistributed in the final game. Use only assets with suitable redistribution rights. No ripped GTA/FiveM/Roblox/commercial-game assets.

## Security rule
UI methods are presentation hooks only. Security-sensitive actions must later call a server-authoritative backend; client buttons never decide permissions, money, bans or rank changes.
