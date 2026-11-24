# CORRUPT STATE RP — Implementation roadmap

## Approved decisions
- Hosting: Cloudflare for the administration web application and edge/API components.
- Player sign-in providers: email/password, Google Play Games, Google, Discord, Facebook, Apple.
- Owner: full application permissions, with immutable audit events and explicit confirmation for destructive actions.
- Existing PostgreSQL database, current player UUIDs, and game server are preserved.

## Architecture
- Unity Android client remains the game client.
- Existing Fastify/Node.js game API and PostgreSQL remain authoritative for player state.
- Cloudflare Workers + Static Assets host the new admin interface and edge endpoints.
- The current PostgreSQL instance is not directly exposed to the public Internet. A private, authenticated connector (for example Cloudflare Tunnel plus a narrowly scoped origin API, or a properly secured hosted origin) is required before remote administration.
- Cloudflare cannot make a Termux phone a reliable 24/7 game host. Production game hosting is a separate deployment decision.
- OAuth providers are integrated only after each provider's developer application, callback URLs, client IDs/secrets, and platform configuration are supplied. Google Play Games is distinct from Google Sign-In.

## Phases 1–6
1. Additive PostgreSQL schema for linked identities, separate admin accounts/sessions, security logs, recovery, promo codes/redemptions.
2. Provider authentication adapters and safe account-link/unlink flows. Never link solely by matching an unverified email. Require recent re-authentication; prevent removing the final usable sign-in method.
3. Replace the current minimal Unity login screen with a polished Arabic RTL sign-in flow; keep email/password and guest compatibility during migration.
4. Build the Cloudflare-hosted Arabic RTL admin web application with secure owner login and role-aware navigation.
5. Provision the first owner account through a one-time server-side bootstrap secret; never commit credentials or make a public owner-registration endpoint.
6. Implement audited player/account management, promo-code issuance/redemption, and restricted server lifecycle controls. Server actions must call an allowlisted supervisor API; never expose arbitrary shell execution or direct public database access.

## Safety invariants
- No destructive migration; never drop or rewrite existing player rows.
- Keep the existing guest player UUID intact.
- Passwords are hashed with a modern password KDF; OAuth access/refresh tokens are not stored as plaintext.
- Identity uniqueness is enforced by provider + provider subject.
- Admin accounts are separate from game players.
- Every sensitive action is authorized server-side and written to audit logs.
- Promo redemption and reward grants are atomic, idempotent, and server-authoritative.
- No production deployment or database migration is considered complete until tested against a backup and verified on the target environment.

## Current implementation status
- Approved branch created for this work.
- Additive schema and design documentation are being prepared for review.
- No database migration has been applied. No server process has been started or stopped by this change.
- External OAuth credentials, Cloudflare account binding/domain, and a persistent game-server host are not available in the repository and must be configured before production activation.
