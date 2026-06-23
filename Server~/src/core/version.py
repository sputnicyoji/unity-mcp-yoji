"""Package version helpers for the bundled Yoji Unity MCP server."""

from importlib import metadata
from pathlib import Path

try:
    import tomllib
except ModuleNotFoundError:  # pragma: no cover - Python <3.11
    import tomli as tomllib  # type: ignore

PACKAGE_NAME = "yoji-unity-mcp-server"


def _version_from_local_pyproject() -> str:
    current = Path(__file__).resolve()
    for parent in current.parents:
        candidate = parent / "pyproject.toml"
        if not candidate.exists():
            continue
        try:
            data = tomllib.loads(candidate.read_text(encoding="utf-8"))
        except Exception:
            continue
        project = data.get("project") or {}
        version = project.get("version")
        if version:
            return str(version)
    return "unknown"


def get_package_version() -> str:
    try:
        return metadata.version(PACKAGE_NAME)
    except Exception:
        return _version_from_local_pyproject()
