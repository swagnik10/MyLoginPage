document.addEventListener("DOMContentLoaded", async () => {
    const userCredential = await viewUserCredentials();

    document.getElementById("submitBtn")
        .addEventListener("click", () => updateUserCredentials(userCredential));

    document.getElementById("cancelBtn")
        .addEventListener("click", () => window.location.href = "profile-view.html");

    document.getElementById("eye").addEventListener("click", () => {
        const passwordField = document.getElementById("password");
        const type = passwordField.getAttribute("type") === "password" ? "text" : "password";
        passwordField.setAttribute("type", type);
    });
});

/*function viewUserCredentials() {

    let userCredential = null;
    apiRequest("/auth/get-credentials", {
        method: "GET"
    })
    .then(result => {
        if (!result.success) {
            alert(result.message || "Fetch failed");
            return;
        }
        userCredential = existingUserCredentials(result.data);
        document.getElementById("userName").value = userCredential.userName;
        document.getElementById("password").value = userCredential.password;
    })
    .catch(error => {
        alert("Failed to determine credentials data");
    });
    return userCredential;
}*/

async function viewUserCredentials()
{
    const result = await apiRequest("/auth/get-credentials", {
        method: "GET"
    });

    if (!result.success) {
            alert(result.message || "Fetch failed");
            return;
        }

    const userCredential = existingUserCredentials(result.data);
    document.getElementById("userName").value = userCredential.userName;
    document.getElementById("password").value = userCredential.password;
    return userCredential;
}

function updateUserCredentials(userCredential) {
    const updateDetails = {
        userName: document.getElementById("userName").value.trim(),
        password: document.getElementById("password").value.trim()
    }
    // const userName = document.getElementById("userName").value.trim();
    // const password = document.getElementById("password").value.trim();

    if (!updateDetails.userName || !updateDetails.password) {
        alert("All fields are required");
        return;
    }

    if (updateDetails.userName === userCredential.userName && updateDetails.password === userCredential.password) {
        alert("No changes detected");
        return;
    }

    apiRequest("/auth/update-credentials", {
        method: "PUT",
        body: JSON.stringify({
            userName: updateDetails.userName,
            password: updateDetails.password
        })
    })
    .then((res) => {
        if (!res.success) {
            alert(res.message || "Update failed");
            return;
        }
        window.location.href = "index.html";
    })
    .catch(error => {
        alert(error.message || error);
    });
}

function existingUserCredentials(data) {
    return {
        userName : data.userName,
        password : data.password
    };
}
