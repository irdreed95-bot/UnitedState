# PDF traceability and verification status
Statuses are evidence-based. A green compile proves compilation only, not runtime behavior. **Implemented and tested** requires a passing automated or repeatable runtime test. No feature is labelled tested without that evidence.

| # | Requirement | Status | File evidence |
|---|---|---|---|
| 1 | Lobby font no Arial.ttf | Code changed; CI pending | Unity/Assets/Scripts/Lobby/LobbyScreen.cs — Awake() uses LegacyRuntime.ttf |
| 2 | InputField Text is child, not on Image object | Code changed; CI pending | Unity/Assets/Scripts/Onboarding/OnboardingScreen.cs — Field() creates Field/Image then child Text |
| 3 | Server URL input + persistence; no silent Guest fallback | Code changed; runtime pending | Unity/Assets/Scripts/Onboarding/OnboardingScreen.cs; Unity/Assets/Scripts/Auth/AuthClient.cs |
| 4 | Refresh endpoint and expiresAtUnix | Code changed; CI/runtime pending | Server/src/server.ts; Unity/Assets/Scripts/Auth/AuthClient.cs |
| 5 | @types/pg, scrypt+salt registration, separated tokens, rollback, RBAC | Partial; CI pending | Server/package.json; Server/src/server.ts; Server/sql/001_initial.sql |
| 6 | Remove client-set amount endpoint | Code changed; runtime pending | Server/src/server.ts — only server-defined mission/salary values may alter balances |
| 7 | /me and mission API from client; server WebSocket | Partial | AuthClient calls /me and mission endpoint; server WebSocket exists, Unity WebSocket client still not integrated |
| 8 | Read Assets/Data JSON at runtime; medical 14; streets 22 | Partial | faction/streets JSON counts corrected; runtime loader not yet wired |
| 9 | Vehicle movement lock, InputRouter, MobileActionHUD | Partial | Unity/Assets/Scripts/GameManager.cs; InputRouter.cs; MobileActionHUD.cs |
| 10 | Full ProjectSettings + URP + Release signed AAB via Secrets | Not implemented/verified | .github/workflows/unity-android.yml still needs AAB and Android signing secrets |
| 11 | Traceability honesty | Implemented in documentation | This table marks incomplete and untested work explicitly |
