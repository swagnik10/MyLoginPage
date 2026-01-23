async function login(username, password) {
    const result = await apiRequest("/auth/login", {
        method: "POST",
        body: JSON.stringify({ userName: username, password })
    });

    // TEMP: backend returns userId
    localStorage.setItem("userId", result.userId);
    return result;
}

async function register(data) {
    return apiRequest("/auth/register", {
        method: "POST",
        body: JSON.stringify(data)
    });
}

function logout() {
    localStorage.removeItem("userId");
    window.location.href = "index.html";
}
