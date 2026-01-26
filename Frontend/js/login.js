document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("loginBtn")
        .addEventListener("click", login);
});

function login() {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();

    if (!username || !password) {
        alert("Username and password are required");
        return;
    }

    apiRequest("/auth/login", {
        method: "POST",
        body: JSON.stringify({ userName: username, password })
    })
    .then(res => {
        if (!res.success) {
            alert(res.message || "Login failed");
            return;
        }

        if (!res.data || !res.data.userId) {
            alert("Invalid server response");
            return;
        }

        localStorage.setItem("userId", res.data.userId);
        navigateAfterLogin(res.data);
    })
    .catch(err => {
        alert(err.message || "Network error");
    });
}

function navigateAfterLogin(data) {
    window.location.href = data.hasProfile
        ? "profile-view.html"
        : "profile-create.html";
}
