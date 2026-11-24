# Database migration 002 — account and administration foundation

This migration is additive and depends on `001_initial.sql`.
It creates new tables and indexes only. It does not alter existing rows in
`players`, `ledger`, `audit_log`, or `refresh_sessions`.

## Objects created
- `account_identities`: external provider identities linked to existing player UUIDs.
- `admin_accounts` and `admin_sessions`: separate owner/admin identities and sessions.
- `account_security_logs` and `account_recovery`: security events and hashed recovery tokens.
- `promo_codes` and `promo_redemptions`: promotional reward definitions and redemption history.

## Required application safeguards before enabling features
- Normalize admin emails before insert/login; the unique lower(email) index prevents case-variant duplicates.
- Use asynchronous `crypto.scrypt` (or a reviewed Argon2id implementation) for admin password hashing; never use synchronous password hashing in request handlers.
- OAuth identity linking must require provider-verified identity and recent re-authentication. Never link accounts solely because email strings match. Do not allow removal of the final usable sign-in method.
- Google Play Games is a distinct provider from Google Sign-In and needs its own verified server-side token validation.
- Promo redemption MUST run in one database transaction: lock the promo_codes row with `SELECT ... FOR UPDATE`, validate active/time/max_redemptions/per_player_limit, insert redemption, and grant the reward atomically. The schema intentionally permits multiple redemptions; the API enforces the configured per-player limit.
- Reward grants must be idempotent and server-authoritative. Validate reward_payload by reward_type; never trust client-supplied reward values.
- Owner full access is enforced by server-side authorization policy and audit logging, not by trusting UI visibility.
- Admin sessions must use random high-entropy tokens stored only as hashes, short idle/absolute expiry, rotation and revocation. If the Cloudflare UI uses a BFF, set Secure, HttpOnly, SameSite cookies and implement CSRF/origin checks.
- Bootstrap the first owner only through a one-time server-side secret. Do not add a public owner-registration endpoint.

## Existing data and safety
- The current guest player remains in `players`; this migration neither inserts an admin into `players` nor changes player UUIDs.
- Existing `admin_roles` remains untouched for compatibility with the current game API. The new `admin_accounts` is a separate administrative identity system.
- PostgreSQL must remain private; Cloudflare must call a narrowly scoped authenticated origin API, not connect directly to the database from a browser or public Worker.
- Never commit owner credentials, OAuth secrets, database URLs, or JWT secrets.

## Status
- This file is a review of the migration, not authorization to apply it.
- The migration has NOT been applied to the live database.
- Before applying: take a fresh verified backup, test the SQL against a restored copy, inspect created objects, and obtain explicit approval for the final SQL.
