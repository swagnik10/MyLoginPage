document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("submitBtn")
        .addEventListener("click", submitCredentials);
});

function submitCredentials() {
    const userName = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();
    const confirmPassword = document.getElementById("confirmpassword").value.trim();

    if (!userName || !password || !confirmPassword) {
        alert("All fields are required");
        return;
    }

    if (password !== confirmPassword) {
        alert("Passwords do not match");
        return;
    }

    apiRequest("/auth/register", {
        method: "POST",
        body: JSON.stringify({
            userName,
            password,
            confirmPassword
        })
    })
    .then((res) => {
        if (!res.success) {
            alert(res.message || "Credentials Create failed");
            return;
        }
        window.location.href = "index.html";
    })
    .catch(error => {
        alert(error.message || error);
    });
}