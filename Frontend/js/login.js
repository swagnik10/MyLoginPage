document.addEventListener("DOMContentLoaded", () => {
    const loginBtn = document.getElementById("loginBtn");

    loginBtn.addEventListener("click", login);
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
    body: JSON.stringify({
        userName: username,
        password: password
    })
    })
    .then(response => {
        localStorage.setItem("userId", response.userId);
        handlePostLogin();
    })
    .catch(error => {
        alert(error.message || error);
    });

}

function handlePostLogin() {
    apiRequest("/profile/status", {
        method: "GET"
    })
    .then(result => {
        if (result.hasProfile) {
            window.location.href = "profile-view.html";
        } else {
            window.location.href = "profile-create.html";
        }
    })
    .catch(error => {
        alert("Failed to determine profile status");
    });
}

