# PDF traceability and verification status
Statuses are evidence-based. A source edit or a green compile alone is not runtime proof. Do not mark an item Implemented until its stated test has run successfully.

| # | Requirement | Status on 2026-10-08 | Evidence / test |
|---|---|---|---|
| 1 | Lobby font uses LegacyRuntime.ttf; no Arial.ttf use in game code | Source confirmed; CI pending | Unity/Assets/Scripts/Lobby/LobbyScreen.cs:8; repository code search for Arial.ttf returned no code hits |
| 2 | InputField Text is child object; Button Text is child object | Source confirmed; CI pending | Unity/Assets/Scripts/Onboarding/OnboardingScreen.cs:10-11 |
| 3 | Editable, persisted server URL; Configure before requests; no silent Guest fallback | Source confirmed; runtime pending | Unity/Assets/Scripts/Onboarding/OnboardingScreen.cs:10-18; Unity/Assets/Scripts/Auth/AuthClient.cs:5-8 |
| 4 | Refresh endpoint, separate access/refresh secrets, expiry and automatic refresh | Source confirmed; backend CI passed on earlier SHA; latest source not yet reverified green | Server/src/server.ts; Unity/Assets/Scripts/Auth/AuthClient.cs |
| 5 | pg typings, scrypt+salt registration, transaction rollback and audit RBAC | Source confirmed; database integration not tested | Server/package.json includes @types/pg; Server/src/server.ts; Server/sql/001_initial.sql |
| 6 | No client-supplied economy transaction; server-only mission/salary rewards | Source confirmed; integration pending | Server/src/server.ts; no /economy/transaction route |
| 7 | /me Authorization, server mission completion and authenticated WebSocket positions | Source changed to match test server paths; live multi-client test pending | Unity/Assets/Scripts/Auth/AuthClient.cs; Unity/Assets/Scripts/PlayerRealtimeClient.cs; Server/src/server.ts |
| 8 | Runtime loading of faction/jobs/street JSON; medical 14; streets 22 | Source/data confirmed; Unity build pending | Unity/Assets/Scripts/GameDataCatalog.cs; Unity/Assets/Data/*.json; BuildProject.SyncDataResources |
| 9 | Disable player movement while driving; InputRouter and MobileActionHUD wired | Source present; in-game test pending | Unity/Assets/Scripts/GameManager.cs; Unity/Assets/Scripts/InputRouter.cs; Unity/Assets/Scripts/MobileActionHUD.cs; Unity/Assets/Scripts/Mobile/MobileHUD.cs |
| 10 | Project settings, URP, release AAB signed from GitHub Secrets | Not verified; latest AAB build failed | Unity/Assets/Scripts/Editor/BuildProject.cs; .github/workflows/unity-android.yml; latest run failed before AAB |
| 11 | This traceability document reflects actual evidence | Source updated; verify after CI | This file |

## Current CI evidence
Latest observed Android AAB run: failed — https://github.com/irdreed95-bot/UnitedState/actions/runs/37760467193
Literal error: `Android signing environment is missing.`
No signed AAB artifact was produced by that run.

## Not yet runtime-tested
- Login/register/guest/refresh against the actual Node test server.
- PostgreSQL schema migration and duplicate/expired refresh token behavior.
- Server-side mission idempotency and salary cooldown against a live database.
- Two-client WebSocket position broadcast.
- Android launch, Arabic glyph rendering, touch controls, vehicle controls and landscape orientation.
- Real release signature with configured Android signing Secrets.
