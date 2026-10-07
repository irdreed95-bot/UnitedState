# Backend deployment
Required configuration (never commit values): DATABASE_URL, REDIS_URL, JWT_SECRET, CORS_ORIGIN, OAuth credentials, and voice-provider credentials.
Local: docker compose -f Server/docker-compose.yml up --build; then GET /health.
Production must use HTTPS and secret storage. Voice requires a licensed provider (Vivox/LiveKit/Agora) and is intentionally gated until credentials exist.