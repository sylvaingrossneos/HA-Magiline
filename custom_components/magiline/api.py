"""Local HTTP client for Magiline iMAGI-X controllers.

This module intentionally implements only endpoints confirmed experimentally.
"""

from __future__ import annotations

from dataclasses import dataclass
from typing import Any

import aiohttp

from .const import (
    API_CONFIGURE_FILTRATION,
    API_POOL_INFO,
    API_POOL_LOCAL,
    API_SPOTLIGHT,
    DEFAULT_PORT,
)


class MagilineApiError(Exception):
    """Base error raised by the Magiline API client."""


class MagilineConnectionError(MagilineApiError):
    """Raised when the controller cannot be reached."""


class MagilineResponseError(MagilineApiError):
    """Raised when the controller returns an invalid response."""


@dataclass(slots=True)
class MagilineApiClient:
    """Asynchronous client for the confirmed local Magiline API."""

    host: str
    session: aiohttp.ClientSession
    port: int = DEFAULT_PORT

    @property
    def base_url(self) -> str:
        """Return the local controller base URL."""
        return f"http://{self.host}:{self.port}"

    async def _request(
        self,
        method: str,
        path: str,
        *,
        json: dict[str, Any] | None = None,
    ) -> Any:
        """Execute one HTTP request and decode its JSON response when present."""
        url = f"{self.base_url}{path}"
        try:
            async with self.session.request(
                method,
                url,
                json=json,
                timeout=aiohttp.ClientTimeout(total=10),
            ) as response:
                response.raise_for_status()
                if response.content_length == 0:
                    return None
                try:
                    return await response.json(content_type=None)
                except (aiohttp.ContentTypeError, ValueError) as err:
                    raise MagilineResponseError(
                        f"Controller returned non-JSON content for {path}"
                    ) from err
        except (aiohttp.ClientConnectionError, TimeoutError) as err:
            raise MagilineConnectionError(
                f"Unable to reach Magiline controller at {self.base_url}"
            ) from err
        except aiohttp.ClientResponseError as err:
            raise MagilineResponseError(
                f"Magiline controller returned HTTP {err.status} for {path}"
            ) from err

    async def async_get_info(self) -> dict[str, Any]:
        """Read general pool/controller information."""
        result = await self._request("GET", API_POOL_INFO)
        if not isinstance(result, dict):
            raise MagilineResponseError("Unexpected /pool/info response")
        return result

    async def async_get_state(self) -> dict[str, Any]:
        """Read the complete local pool state."""
        result = await self._request("GET", API_POOL_LOCAL)
        if not isinstance(result, dict):
            raise MagilineResponseError("Unexpected /pool/local response")
        return result

    async def async_set_spotlight(self, wanted: int) -> Any:
        """Set the confirmed spotlight wanted mode."""
        return await self._request(
            "POST",
            API_SPOTLIGHT,
            json={"mode": {"wanted": wanted}},
        )

    async def async_set_filtration_mode(self, wanted: int) -> Any:
        """Set the confirmed filtration wanted mode."""
        return await self._request(
            "POST",
            API_CONFIGURE_FILTRATION,
            json={"mode": {"wanted": wanted}},
        )
