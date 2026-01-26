async function apiRequest(url, options = {}) {
    const headers = options.headers || {};

    const token = localStorage.getItem("accessToken");
    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    headers["Content-Type"] = "application/json";

    const response = await fetch(`${API_BASE_URL}${url}`, {
        ...options,
        headers
    });

    const contentType = response.headers.get("content-type");

    const result = contentType?.includes("application/json")
        ? await response.json()
        : await response.text();

    if (!response.ok) {
        //const errorText = await response.text();
        if (response.status === 401) {
            handleUnauthorized();
        }
        throw new Error(result.message || "Request failed");
    }

    // if (contentType && contentType.includes("application/json")) {
    //     return await response.json();
    // }

    // if (contentType && contentType.includes("text/")) {
    //     return await response.text();
    // }

    return result;
}
