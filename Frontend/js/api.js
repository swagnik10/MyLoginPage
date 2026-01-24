async function apiRequest(url, options = {}) {
    const headers = options.headers || {};

    const userId = localStorage.getItem("userId");
    if (userId) {
        headers["X-User-Id"] = userId;
    }

    headers["Content-Type"] = "application/json";

    const response = await fetch(`${API_BASE_URL}${url}`, {
        ...options,
        headers
    });

    const contentType = response.headers.get("content-type");

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || "Request failed");
    }

    if (contentType && contentType.includes("application/json")) {
        return await response.json();
    }

    if (contentType && contentType.includes("text/")) {
        return await response.text();
    }

    return null;
}
