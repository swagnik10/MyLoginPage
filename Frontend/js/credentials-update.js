document.addEventListener("DOMContentLoaded", () => {
    viewUserCredentials();

    document.getElementById("submitBtn")
        .addEventListener("click", updateUserCredentials);

    document.getElementById("cancelBtn")
        .addEventListener("click", () => window.location.href = "profile-view.html");

    document.getElementById("eye").addEventListener("click", () => {
        const passwordField = document.getElementById("password");
        const type = passwordField.getAttribute("type") === "password" ? "text" : "password";
        passwordField.setAttribute("type", type);
    });
});

function viewUserCredentials() {
    apiRequest("/auth/get-credentials", {
        method: "GET"
    })
    .then(result => {
        document.getElementById("userName").value = result.data.userName;
        document.getElementById("password").value = result.data.password;
    })
    .catch(error => {
        alert("Failed to determine credentials data");
    });
}

function updateUserCredentials() {
    const userName = document.getElementById("userName").value.trim();
    const password = document.getElementById("password").value.trim();

    if (!userName || !password) {
        alert("All fields are required");
        return;
    }
    apiRequest("/auth/update-credentials", {
        method: "PUT",
        body: JSON.stringify({
            userName,
            password
        })
    })
    .then(() => {
        window.location.href = "index.html";
    })
    .catch(error => {
        alert(error.message || error);
    });
}
