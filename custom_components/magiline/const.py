"""Constants for the Magiline integration."""

from datetime import timedelta

DOMAIN = "magiline"
DEFAULT_PORT = 11000
DEFAULT_SCAN_INTERVAL = timedelta(seconds=30)

API_POOL_INFO = "/api/v1/pool/info"
API_POOL_LOCAL = "/api/v1/pool/local"
API_SPOTLIGHT = "/api/v1/pool/local/spotlight"
API_CONFIGURE_FILTRATION = "/api/v1/pool/local/configure-filtration"

FILTRATION_MODE_AUTO = 0
FILTRATION_MODE_FORCE_ON = 1
FILTRATION_MODE_OFF = 2

SPOTLIGHT_MODE_OFF = 1
SPOTLIGHT_MODE_ON = 2
