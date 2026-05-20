#!/usr/bin/env bash
# ── FacilityApp — PostgreSQL backup script ────────────────────────────────────
#
# Usage (manual):
#   chmod +x backup.sh
#   ./backup.sh
#
# Usage (automated — add to root crontab with `sudo crontab -e`):
#   0 2 * * * /opt/facilityapp/deploy/backup.sh >> /var/log/facilityapp-backup.log 2>&1
#
# Backups are stored in BACKUP_DIR, compressed with gzip.
# Files older than RETENTION_DAYS are deleted automatically.
# ──────────────────────────────────────────────────────────────────────────────

set -euo pipefail

# ── Configuration ─────────────────────────────────────────────────────────────
BACKUP_DIR="${BACKUP_DIR:-/var/backups/facilityapp}"
RETENTION_DAYS="${RETENTION_DAYS:-14}"
COMPOSE_DIR="$(cd "$(dirname "$0")" && pwd)"   # directory this script lives in
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="${BACKUP_DIR}/facilityapp_${TIMESTAMP}.sql.gz"

# ── Ensure backup directory exists ────────────────────────────────────────────
mkdir -p "$BACKUP_DIR"

# ── Run pg_dump inside the running db container ───────────────────────────────
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Starting backup → ${BACKUP_FILE}"

docker compose -f "${COMPOSE_DIR}/docker-compose.yml" exec -T db \
    pg_dump -U facilityapp facilityapp \
  | gzip > "$BACKUP_FILE"

echo "[$(date '+%Y-%m-%d %H:%M:%S')] Backup complete ($(du -sh "$BACKUP_FILE" | cut -f1))"

# ── Remove backups older than RETENTION_DAYS ──────────────────────────────────
find "$BACKUP_DIR" -name "facilityapp_*.sql.gz" -mtime +"$RETENTION_DAYS" -delete
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Pruned backups older than ${RETENTION_DAYS} days"

# ── Optional: copy to off-site storage ────────────────────────────────────────
# Uncomment and configure one of the following if you want off-site copies:
#
# AWS S3:
#   aws s3 cp "$BACKUP_FILE" "s3://your-bucket/facilityapp-backups/"
#
# Rclone (supports S3, GCS, Dropbox, SFTP, etc.):
#   rclone copy "$BACKUP_FILE" remote:facilityapp-backups/
