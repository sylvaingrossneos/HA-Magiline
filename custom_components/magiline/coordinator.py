"""Data coordinator for the Magiline integration."""

from __future__ import annotations

import logging
from typing import Any

from homeassistant.core import HomeAssistant
from homeassistant.helpers.update_coordinator import DataUpdateCoordinator, UpdateFailed

from .api import MagilineApiClient, MagilineApiError
from .const import DEFAULT_SCAN_INTERVAL, DOMAIN

_LOGGER = logging.getLogger(__name__)


class MagilineDataUpdateCoordinator(DataUpdateCoordinator[dict[str, Any]]):
    """Coordinate reads of the complete local pool state."""

    def __init__(self, hass: HomeAssistant, client: MagilineApiClient) -> None:
        """Initialize the coordinator."""
        super().__init__(
            hass,
            _LOGGER,
            name=DOMAIN,
            update_interval=DEFAULT_SCAN_INTERVAL,
        )
        self.client = client

    async def _async_update_data(self) -> dict[str, Any]:
        """Fetch the latest local pool state."""
        try:
            return await self.client.async_get_state()
        except MagilineApiError as err:
            raise UpdateFailed(str(err)) from err
