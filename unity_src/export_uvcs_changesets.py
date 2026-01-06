import json
import os
import subprocess
from datetime import datetime
from pathlib import Path

STATE_FILE = Path(".uvcs_last_changeset.txt")
OUT_FILE = Path("changesets.json")

# ⚙️ Optionnel : si tu veux forcer un repo précis, mets un selector du style:
# REPO_SELECTOR = "repo@server:8087" (on-prem) ou "repo@cloud"
# Laisse "" pour utiliser le contexte (workspace courant / default server)
REPO_SELECTOR = ""  # ex: "myrepo@cloud"

def run(cmd: list[str]) -> str:
    p = subprocess.run(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, encoding="utf-8")
    if p.returncode != 0:
        raise RuntimeError(f"Command failed:\n{' '.join(cmd)}\n\nSTDERR:\n{p.stderr.strip()}")
    return p.stdout

def get_last_id() -> int:
    if STATE_FILE.exists():
        try:
            return int(STATE_FILE.read_text(encoding="utf-8").strip())
        except Exception:
            return 0
    return 0

def save_last_id(last_id: int) -> None:
    STATE_FILE.write_text(str(last_id), encoding="utf-8")

def fetch_changesets(last_id: int) -> list[dict]:
    # Query expression; needs the "where" keyword for this cm version.
    # Keep it in a single arg and avoid spaces around ">".
    where = f"where id>{last_id}"

    # format "pipe" simple -> parsing robuste
    fmt = "{id}|{date}|{owner}|{branch}|{comment}"

    # Older cm expects --format=<str> (no space), otherwise it parses it as query.
    cmd = ["cm", "find", "changeset", where, f"--format={fmt}", "--nototal"]

    # Si tu veux forcer un repo, tu peux ajouter un paramètre de scope si ton cm le supporte,
    # sinon fais le dans le bon workspace / bon server login.
    # (Le plus fiable reste: exécuter depuis un workspace du repo visé.)
    out = run(cmd).strip()
    if not out:
        return []

    rows = out.splitlines()
    items = []
    for r in rows:
        parts = r.split("|", 4)
        if len(parts) != 5:
            continue
        cs_id, date_str, owner, branch, comment = parts
        items.append({
            "id": int(cs_id),
            "date": date_str,
            "author": owner,
            "branch": branch,
            "comment": comment,
        })
    return items

def main():
    last_id = get_last_id()
    items = fetch_changesets(last_id)

    if items:
        new_last_id = max(i["id"] for i in items)
        save_last_id(new_last_id)

    # ajoute metadata run
    payload = {
        "generated_at": datetime.utcnow().isoformat() + "Z",
        "from_changeset_id_exclusive": last_id,
        "count": len(items),
        "changesets": items,
    }

    OUT_FILE.write_text(json.dumps(payload, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"Wrote {OUT_FILE} ({len(items)} changesets). Last id = {get_last_id()}")

if __name__ == "__main__":
    main()
