async function apiRequest(url, options = {}) {
    const headers = options.headers || {};

    // DEV ONLY – temporary user context
    const userId = localStorage.getItem("userId");
    if (userId) {
        headers["X-User-Id"] = userId;
    }

    headers["Content-Type"] = "application/json";

    const response = await fetch(`${API_BASE_URL}${url}`, {
        ...options,
        headers
    });

    const text = await response.text();

    if (!response.ok) {
        throw new Error(text || "Request failed");
    }

    return text ? JSON.parse(text) : null;
}
