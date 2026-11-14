from pathlib import Path
import sys

from PIL import Image


def main() -> int:
    if len(sys.argv) != 3:
        raise SystemExit("Usage: create_collection_studio_icon.py <source-png> <destination-ico>")

    source = Path(sys.argv[1])
    destination = Path(sys.argv[2])
    if not source.is_file():
        raise SystemExit(f"Source image does not exist: {source}")

    destination.parent.mkdir(parents=True, exist_ok=True)
    with Image.open(source) as image:
        rgba = image.convert("RGBA")
        rgba.save(destination, format="ICO", sizes=[(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)])

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
