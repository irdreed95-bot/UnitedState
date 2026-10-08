# PDF traceability and verification status
A green compile proves compilation only, not runtime behavior. "Tested" requires automated or repeatable runtime evidence. Incomplete features remain Partial/Not tested.

| # | Requirement | Status | File evidence |
|---|---|---|---|
| 1 | Lobby font is LegacyRuntime.ttf; no Arial.ttf use in LobbyScreen | Code changed; CI pending | Unity/Assets/Scripts/Lobby/LobbyScreen.cs — Awake |
| 2 | InputField Text is child of Field/Image | Code changed; CI pending | Unity/Assets/Scripts/Onboarding/OnboardingScreen.cs — Field |
| 3 | Server URL input persists; Login does not silently become Guest | Code changed; runtime pending | Unity/Assets/Scripts/Onboarding/OnboardingScreen.cs; Unity/Assets/Scripts/Auth/AuthClient.cs |
| 4 | Refresh endpoint, separate token secrets, expiresAtUnix | Code changed; CI/runtime pending | Server/src/server.ts; Unity/Assets/Scripts/Auth/AuthClient.cs |
| 5 | @types/pg, salted scrypt registration, rollback, RBAC | Code changed; CI/database runtime pending | Server/package.json; Server/src/server.ts; Server/sql/001_initial.sql |
| 6 | Client cannot choose money amount | Server route removed; CI pending | Server/src/server.ts — rewards/salary are server constants |
| 7 | /me and mission endpoints, authenticated WebSocket | Partial: Unity WebSocket client exists; live multi-client test pending | Unity/Assets/Scripts/Auth/AuthClient.cs; Unity/Assets/Scripts/PlayerRealtimeClient.cs; Server/src/server.ts |
| 8 | Read Assets/Data JSON at runtime; medical ranks 14; streets 22 | Code/data changed; CI pending | Unity/Assets/Scripts/GameDataCatalog.cs; Unity/Assets/Resources/Data/*.json |
| 9 | Player locked while driving; unified input; MobileActionHUD | Code changed; CI pending | Unity/Assets/Scripts/GameManager.cs; Unity/Assets/Scripts/InputRouter.cs; Unity/Assets/Scripts/MobileActionHUD.cs |
| 10 | Complete ProjectSettings + URP + Release signed AAB from Secrets | Not implemented/verified | .github/workflows/unity-android.yml; requires Android signing secrets and URP project asset configuration |
| 11 | Traceability states evidence honestly | Updated; pending CI evidence | docs/PDF_TRACEABILITY.md |
