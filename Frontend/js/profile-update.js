document.addEventListener("DOMContentLoaded", () => {
    getUserProfile();

    document.getElementById("submitBtn")
        .addEventListener("click", updateProfile);

    document.getElementById("credentialBtn")
        .addEventListener("click", () => window.location.href = "edit-credential.html");

    document.getElementById("cancelBtn")
        .addEventListener("click", () => window.location.href = "profile-view.html");
});

function getUserProfile() {
    apiRequest("/profile", {
        method: "GET"
    })
    .then(result => {
        document.getElementById("firstName").value = result.data.firstName;
        document.getElementById("lastName").value = result.data.lastName;
        document.getElementById("address").value = result.data.address;
        document.getElementById("phoneNumber").value = result.data.phoneNumber;
    })
    .catch(error => {
        alert("Failed to determine profile data");
    });
}

function updateProfile() {
    const firstName = document.getElementById("firstName").value.trim();
    const lastName = document.getElementById("lastName").value.trim();
    const address = document.getElementById("address").value.trim();
    const phoneNumber = document.getElementById("phoneNumber").value.trim();

    if (!firstName || !lastName || !address || !phoneNumber) {
        alert("All fields are required");
        return;
    }

    apiRequest("/profile", {
        method: "PUT",
        body: JSON.stringify({
            firstName,
            lastName,
            address,
            phoneNumber
        })
    })
    .then(() => {
        window.location.href = "profile-view.html";
    })
    .catch(error => {
        alert(error.message || error);
    });
}
